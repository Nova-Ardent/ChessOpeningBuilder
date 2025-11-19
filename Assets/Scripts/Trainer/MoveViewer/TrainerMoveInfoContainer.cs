using Board.Common;
using Board.Moves;
using System.Collections.Generic;
using System.Linq;
using Trainer.Data;
using Trainer.Data.Moves;
using UnityEngine;
using static Trainer.Data.TrainerData;

namespace Trainer.MoveViewer
{
    public class TrainerMoveInfoContainer : MonoBehaviour
    {
        [SerializeField] TrainerMoveInformationDisplay _moveInfoDisplayPrefab;

        public TrainerMoveInformationDisplay[] Moves { get; private set; } = new TrainerMoveInformationDisplay[0];

        public void ClearMoves()
        {
            foreach (TrainerMoveInformationDisplay move in Moves)
            {
                Destroy(move.gameObject);
            }

            Moves = new TrainerMoveInformationDisplay[0];
        }

        public void SetMoves(TrainerData.StatsView statsView, params TrainerMoveInformation[] trainerMoveInformations)
        {
            SetMoves(statsView, (IEnumerable<TrainerMoveInformation>)trainerMoveInformations);
        }

        public void SetMoves(TrainerData.StatsView statsView, IEnumerable<TrainerMoveInformation> trainerMoveInformations)
        {
            ClearMoves();

            Moves = trainerMoveInformations.Select(x => MakeMoveDisplay(statsView, x, this.transform))
                .ToArray();

            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i].gameObject.name = $"MoveInfoDisplay_{i}_{Moves[i].MoveNameText.text}";
                Moves[i].transform.localPosition = new Vector3
                    ( 0
                    , ((_moveInfoDisplayPrefab.transform as RectTransform).rect.height + 5) * (0.5f + i - Moves.Length / 2f)
                    , 0
                    );
            }
        }

        TrainerMoveInformationDisplay MakeMoveDisplay(TrainerData.StatsView statsView, TrainerMoveInformation moveInformation, Transform parent)
        {
            var nextMove = Instantiate(_moveInfoDisplayPrefab, parent);
            nextMove.Init(moveInformation);
            nextMove.transform.localPosition = Vector3.zero;

            if (statsView == StatsView.ByMove)
            {
                if (moveInformation.TimesGuessed > 0)
                {
                    nextMove.PercentageBar.Percentage = 1.0f * moveInformation.TimesCorrect / moveInformation.TimesGuessed;
                }
            }
            else if (statsView == StatsView.ByVariation)
            {
                if (moveInformation.VariationTimesGuessed > 0)
                {
                    nextMove.PercentageBar.Percentage = 1.0f * moveInformation.VariationTimesCorrect / moveInformation.VariationTimesGuessed;
                }
            }
            else
            {
                int timesGuessed = 0;
                int timesCorrect = 0;
                GetStatsOfBranch(ref timesGuessed, ref timesCorrect, moveInformation);

                if (timesGuessed > 0)
                {
                    nextMove.PercentageBar.Percentage = 1.0f * timesCorrect / timesGuessed;
                }
            }

            if (moveInformation.Color == PieceColor.White)
            {
                nextMove.SetAsWhiteTile();
            }
            else
            {
                nextMove.SetAsBlackTile();
            }

            return nextMove;
        }

        void GetStatsOfBranch(ref int guessed, ref int correct, TrainerMoveInformation moveInformation)
        {
            guessed += moveInformation.TimesGuessed;
            correct += moveInformation.TimesCorrect;

            foreach (var move in moveInformation.PossibleNextMoves)
            {
                GetStatsOfBranch(ref guessed, ref correct, move);
            }
        }
    }
}
