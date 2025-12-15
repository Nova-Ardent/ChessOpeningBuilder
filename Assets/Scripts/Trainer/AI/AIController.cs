using Board;
using Board.Common;
using Board.FlipBoard;
using Board.Moves;
using History.MoveHistory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Trainer.Data;
using Trainer.Data.Moves;
using UnityEngine;
using Common;
using Trainer.AI.Modes;

namespace Trainer.AI
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] BoardObject _boardObject;
        [SerializeField] FlipBoardHandler _flipBoardHandler;

        [SerializeField] BoardHistoryObject _boardHistoryObject;

        [SerializeField] GameObject _nextVariationButton;
        [SerializeField] TextMeshProUGUI _CurrentVariationText;

        [System.Serializable]
        public struct NonTrainingModule
        {
            public GameObject Target;
            [NonSerialized] public bool WasActive;
        }

        [SerializeField] public NonTrainingModule[] NonTrainingModules;
        [SerializeField] public GameObject[] TrainingModules;

        Mode CurrentMode;

        public bool HasUsedHint = false;
        public bool HasUsedBothHints = false;
        Coroutine _runner;

        private void Start()
        {
            _boardObject.RegisterOnMoveCallback(OnPlayerMoved);
        }

        private void Update()
        {
            if (CurrentMode != null && _nextVariationButton.activeSelf & Input.GetKey(KeyCode.Space))
            {
                LoadNextVariation();
            }
        }

        public void StartTraining(TrainerData trainerData, TrainerMoveInformation startingMove = null)
        {
            TurnOffNonTrainingModules();
            switch (trainerData.DepthType)
            {
                default:
                    Debug.LogError($"failed to find variation mode. {trainerData.DepthType}");
                    break;
                case TrainerData.TrainerType.ByCompleteVariation:
                    CurrentMode = new ByVariationMode();
                    break;
                case TrainerData.TrainerType.ByMoveCount:
                    CurrentMode = new ByMoveCountMode();
                    break;
                case TrainerData.TrainerType.MarathonMode:
                    CurrentMode = new MarathonMode(false);
                    break;
                case TrainerData.TrainerType.MarathonUnique:
                    CurrentMode = new MarathonMode(true);
                    break;
                case TrainerData.TrainerType.EvolutionMode:
                    CurrentMode = new EvolutionMode();
                    break;
            }

            if (!CurrentMode.Initialize(trainerData, startingMove))
            {
                StopTraining();
                return;
            }


            SetVariation();

            _runner = StartCoroutine(Run());
            _flipBoardHandler.SetBoardFlipped(trainerData.Color == PieceColor.Black);
        }

        public void StopTraining()
        {
            TurnOnNonTrainingModules();

            CurrentMode = null;

            if (_runner != null)
            {
                StopCoroutine(_runner);
            }
        }

        void TurnOffNonTrainingModules()
        {
            for (int i = 0; i < NonTrainingModules.Length; i++)
            {
                NonTrainingModules[i].WasActive = NonTrainingModules[i].Target.activeSelf;
                NonTrainingModules[i].Target.SetActive(false);
            }

            foreach (GameObject module in TrainingModules)
            {
                module.SetActive(true);
            }

            _nextVariationButton.SetActive(false);
        }

        void TurnOnNonTrainingModules()
        {
            for (int i = 0; i < NonTrainingModules.Length; i++)
            {
                if (NonTrainingModules[i].WasActive)
                    NonTrainingModules[i].Target.SetActive(true);
            }

            foreach (GameObject module in TrainingModules)
            {
                module.SetActive(false);
            }
        }

        public void LoadNextVariation()
        {
            StopCoroutine(_runner);

            if (!CurrentMode.IncrementVariation())
            {
                StopTraining();
                return;
            }
            SetVariation();

            _nextVariationButton.SetActive(false);
            _runner = StartCoroutine(Run());
        }

        void SetVariation()
        {
            _boardHistoryObject.ClearHistory();
            CurrentMode.SetNextVariation();
            _CurrentVariationText.text = CurrentMode.ToString();
        }

        void OnPlayerMoved()
        {
            HasUsedHint = false;
            HasUsedBothHints = false;

            if (CurrentMode == null)
            {
                return;
            }

            if (_boardObject.GetCurrentColor() == CurrentMode.TrainerData.Color)
            {
                return;
            }

            Mode.Variation variation = CurrentMode.GetCurrentVariation();
            if (variation.CurrentMoveIndex >= variation.MoveList.Count)
            {
                return;
            }

            TrainerMoveInformation currentTrainerMove = variation.MoveList[variation.CurrentMoveIndex];

            MoveInformation moveInformation = _boardObject.GetMoveHistory().Last();
            if (moveInformation.ToString() == currentTrainerMove.MoveNotation)
            {
                currentTrainerMove.TimesGuessed++;
                currentTrainerMove.TimesCorrect++;
                variation.CurrentMoveIndex++;

                if (variation.CurrentMoveIndex >= variation.MoveList.Count)
                {
                    _nextVariationButton.SetActive(true);
                }
            }
            else
            {
                currentTrainerMove.TimesGuessed++;
                CurrentMode.MarkFailure();

                _boardHistoryObject.RemoveLatestMove();
                _boardHistoryObject.MarkLabelAsIncorrect(variation.CurrentMoveIndex);
            }
        }

        IEnumerator Run()
        {
            while (true)
            {
                Mode.Variation variation = CurrentMode.GetCurrentVariation();
                if (variation.CurrentMoveIndex >= variation.MoveList.Count)
                {
                    yield return null;
                    continue;
                }

                if (_boardObject.GetCurrentColor() == CurrentMode.TrainerData.Color)
                {
                    yield return null;
                    continue;
                }

                if (!_boardObject.CanMakeMoves())
                {
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(0.25f);

                if (!_boardObject.CanMakeMoves())
                {
                    yield return null;
                    continue;
                }

                // if color is white, then bot plays black.
                if (_boardObject.GetCurrentColor() != CurrentMode.TrainerData.Color)
                {
                    _boardObject.MovePieceAlgebraic(variation.MoveList[variation.CurrentMoveIndex].MoveNotation);
                    variation.CurrentMoveIndex++;

                    if (variation.CurrentMoveIndex >= variation.MoveList.Count)
                    {
                        _nextVariationButton.SetActive(true);
                    }
                }

                yield return null;
            }
        }


        public void Hint()
        {
            Mode.Variation variation = CurrentMode.GetCurrentVariation();
            if (variation.CurrentMoveIndex >= variation.MoveList.Count)
            {
                return;
            }

            TrainerMoveInformation currentTrainerMove = variation.MoveList[variation.CurrentMoveIndex];

            Files fromFile = currentTrainerMove.HintOne.ToLower()[0].ToFile();
            Ranks fromRank = currentTrainerMove.HintOne.ToLower()[1].ToRank();

            if (!HasUsedHint)
            {
                _boardObject.MarkBoard(fromFile, fromRank, fromFile, fromRank);
                currentTrainerMove.TimesGuessed++;
                HasUsedHint = true;

                CurrentMode.MarkFailure();
            }
            else
            {
                Files toFile = currentTrainerMove.HintTwo.ToLower()[0].ToFile();
                Ranks toRank = currentTrainerMove.HintTwo.ToLower()[1].ToRank();
                _boardObject.MarkBoard(fromFile, fromRank, toFile, toRank);

                if (!HasUsedBothHints)
                {
                    currentTrainerMove.TimesGuessed++;
                }

                HasUsedBothHints = true;
            }
        }
    }
}
