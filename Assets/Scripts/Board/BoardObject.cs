using Board.Common;
using Board.Display.Highlight;
using Board.Moves;
using Board.Pieces;
using Board.State;
using UnityEngine;
using Board.Audio;
using Board.Pieces.Types;
using System.Collections.Generic;
using System;

namespace Board
{
    public class BoardObject : MonoBehaviour
    {
        [SerializeField] string StartingFen;
        [SerializeField] BoardState _boardState;
        [SerializeField] HighlightManager _highlightManager;
        [SerializeField] AudioManager _audioManager;

        Piece _animatedPiece = null;
        bool _pieceBeingAnimated = false;

        Action _onMoveCallback = () => { };

        public Piece CurrentlySelectedPiece { get; private set; }

        private void Start()
        {
            _boardState.SetFen(StartingFen);
        }

        private void Update()
        {
            _animatedPiece?.UpdateAnimation();
        }

        public void MarkBoard(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            if (fromFile == toFile && fromRank == toRank)
            {
                _highlightManager.ToggleHighlightSquare(fromFile, fromRank);
            }
            else
            {
                _highlightManager.UpdateHighlightArrow(fromFile, fromRank, toFile, toRank);
            }
        }

        public void ClearMarkings()
        {
            _highlightManager.ClearAllHighlights();
            _highlightManager.SetLastMove(_boardState.GetViewedMove().From, _boardState.GetViewedMove().To);
        }

        public void AddArrow(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            _highlightManager.AddArrow(fromFile, fromRank, toFile, toRank);
        }

        public void RemoveArrowIfExists(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            _highlightManager.RemoveArrowIfExists(fromFile, fromRank, toFile, toRank);
        }


        public void SelectSquare(Files file, Ranks rank)
        {
            if (!CanMakeMoves())
            {
                return;
            }

            if (_boardState.Pieces[file, rank] != null)
            {
                CurrentlySelectedPiece = _boardState.Pieces[file, rank];
                CurrentlySelectedPiece.transform.SetAsLastSibling();
                _highlightManager.HighlightPiece(CurrentlySelectedPiece, _boardState.Pieces);
            }
        }

        public Piece GetPiece(Files file, Ranks rank)
        {
            return _boardState.Pieces[file, rank];
        }

        public void MoveHighlightedPiece(Files toFile, Ranks toRank, PieceTypes? promotion = null, bool animate = false)
        {
            if (!CanMakeMoves())
            {
                return;
            }

            if (CurrentlySelectedPiece != null)
            {
                MoveInformation moveInformation = _boardState.MovePiece(CurrentlySelectedPiece, toFile, toRank, promotion);
                _highlightManager.ClearAllHighlights();
                _highlightManager.SetLastMove(moveInformation.From, moveInformation.To);
                
                if (animate)
                {
                    AnimatePiece(moveInformation, false);
                }

                _audioManager.Play(moveInformation);
                CurrentlySelectedPiece = null;

                _onMoveCallback();
            }
            else
            {
                Debug.LogError("Attempted to move null piece");
            }
        }

        public void MovePieceUCI(string uci)
        {
            if (!CanMakeMoves())
            {
                return;
            }

            if (!(uci.Length == 4 || uci.Length == 5))
            {
                Debug.LogError("invalid UCI length");
                return;
            }

            Files fromFile = uci[0].ToFile();
            Ranks fromRank = uci[1].ToRank();

            Files toFile = uci[2].ToFile();
            Ranks toRank = uci[3].ToRank();

            PieceTypes? promotion = null;
            if (uci.Length == 5)
            {
                switch (uci[4])
                {
                    case 'r': promotion = PieceTypes.Rook; break;
                    case 'q': promotion = PieceTypes.Queen; break;
                    case 'b': promotion = PieceTypes.Bishop; break;
                    case 'n': promotion = PieceTypes.Knight; break;
                }
            }
            

            if (fromFile == Files.Count || fromRank == Ranks.Count || toFile == Files.Count || toRank == Ranks.Count)
            {
                Debug.LogError($"invalid uci {uci}");
                return;
            }

            SelectSquare(fromFile, fromRank);
            if (CurrentlySelectedPiece == null || CurrentlySelectedPiece.File != fromFile || CurrentlySelectedPiece.Rank != fromRank)
            {
                Debug.LogError($"failed to find piece with UCI {uci}");
                return;
            }
            MoveHighlightedPiece(toFile, toRank, promotion, true);
        }

        public void MovePieceAlgebraic(string notation, bool playAnimation = true, bool playSound = true)
        {
            if (!CanMakeMoves())
            {
                return;
            }

            MoveInformation moveInformation = _boardState.MovePiece(notation);
            _highlightManager.ClearAllHighlights();
            _highlightManager.SetLastMove(moveInformation.From, moveInformation.To);

            if (playAnimation)
            {
                AnimatePiece(moveInformation, false);
            }
            
            if (playSound)
            {
                _audioManager.Play(moveInformation);
            }

            CurrentlySelectedPiece = null;
            _onMoveCallback();
        }

        public void RegisterOnMoveCallback(Action callback)
        {
            _onMoveCallback += callback;
        }

        public void ViewMove(int index, bool noAnimation = false, bool noAudio = false)
        {
            if (_pieceBeingAnimated)
            {
                return;
            }

            if (_boardState.GetMoveHistoryCount() == 0)
            {
                return;
            }

            index = Mathf.Clamp(index, 0, GetMoveHistoryCount() - 1);

            if (_boardState.GetViewedMoveIndex() == index)
            {
                return;
            }

            _highlightManager.ClearAllHighlights();

            bool shouldAnimatePiece = Mathf.Abs(index - _boardState.GetViewedMoveIndex()) == 1;
            bool shouldAnimateInReverse = _boardState.GetViewedMoveIndex() - index > 0;
            MoveInformation reverseAnimationMove = _boardState.GetViewedMove();

            MoveInformation moveInformation = _boardState.ViewMove(index);
            _highlightManager.SetLastMove(moveInformation.From, moveInformation.To);

            if (!noAudio)
            {
                _audioManager.Play(moveInformation);
            }

            if (shouldAnimatePiece && !noAnimation)
            {
                if (shouldAnimateInReverse)
                {
                    AnimatePiece(reverseAnimationMove, true);
                }
                else
                {
                    AnimatePiece(moveInformation, false);
                }
            }
        }

        public int GetViewedMoveIndex()
        {
            return _boardState.GetViewedMoveIndex();
        }

        public int GetMoveHistoryCount()
        {
            return _boardState.GetMoveHistoryCount();
        }

        public IEnumerable<MoveInformation> GetMoveHistory()
        {
            return _boardState.GetMoveHistory();
        }

        public void ClearHistory()
        {
            _boardState.ClearHistory(StartingFen);
            _highlightManager.ClearLastMove();
            _highlightManager.ClearAllHighlights();
        }

        public void RemoveLatestMove()
        {
            if (_pieceBeingAnimated)
            {
                return;
            }

            if (_boardState.GetMoveHistoryCount() == 0)
            {
                return;
            }

            MoveInformation moveInformation = _boardState.RemoveLatestMove(StartingFen);
            AnimatePiece(moveInformation, true);

            if (_boardState.GetMoveHistoryCount() == 0)
            {
                _audioManager.Play(new MoveInformation());
            }
            else
            {
                _audioManager.Play(_boardState.GetViewedMove());
            }
        }

        public void AnimatePiece(MoveInformation moveInformation, bool inReverse)
        {
            _pieceBeingAnimated = true;

            if (inReverse)
            {
                _animatedPiece = _boardState.Pieces[moveInformation.From.File, moveInformation.From.Rank];
                if (_animatedPiece == null)
                {
                    Debug.LogError("failed to find piece to animate");
                    _pieceBeingAnimated = false;
                    return;
                }

                _animatedPiece.StartAnimation(moveInformation.To, moveInformation.From, OnMoveAnimationComplete);
            }
            else
            {
                _animatedPiece = _boardState.Pieces[moveInformation.To.File, moveInformation.To.Rank];
                if (_animatedPiece == null)
                {
                    Debug.LogError("failed to find piece to animate");
                    _pieceBeingAnimated = false;
                    return;
                }

                _animatedPiece.StartAnimation(moveInformation.From, moveInformation.To, OnMoveAnimationComplete);
            }
        }

        public void OnMoveAnimationComplete()
        {
            _pieceBeingAnimated = false;
            _animatedPiece = null;
        }

        public PieceColor GetCurrentColor()
        {
            return _boardState.Pieces.CurrentMove;
        }

        public bool CanMakeMoves()
        {
            return _boardState.IsViewingLatestMove()
                && !_pieceBeingAnimated;
        }
    }
}