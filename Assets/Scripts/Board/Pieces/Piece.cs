using Board.Common;
using Board.Display.Moves;
using Board.Pieces.Types;
using Board.State;
using Board.State.ColorInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Board.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        RectTransform _transform;


        Files _file;
        public Files File
        {
            get => _file;
            set
            {   
                _file = value;
                UpdatePosition();
            }
        }

        Ranks _rank;
        public Ranks Rank
        {
            get => _rank;
            set
            {
                _rank = value;
                UpdatePosition();
            }
        }

        public abstract PieceTypes Type { get; }
        public abstract char PieceCharacter { get; }

        [SerializeField] PieceColor _color;
        public PieceColor Color { get => _color; }

        protected BoardPieces BoardPieces { get; private set; }

        public Vector3 AnimationFrom { get; private set; }
        public Vector3 AnimationTo { get; private set; }
        public float AnimationProgress { get; private set; }
        public bool IsAnimating { get; private set; }
        Action _onAnimationComplete;


        private void Awake()
        {
            if (transform is RectTransform rectTransform)
            {
                _transform = rectTransform;
            }
            else
            {
                Debug.LogError("Piece must be attached to a GameObject with a RectTransform.");
            }
        }
        
        public void RegisterBoardPiecesState(BoardPieces boardState)
        {
            transform.SetParent(boardState.transform, false);
            BoardPieces = boardState;
        }

        public IEnumerable<PossibleMoveInfo> GetLegalMoves(BoardPieces boardPieces = null)
        {
            if (boardPieces == null)
            {
                boardPieces = BoardPieces;
            }

            if (boardPieces.CurrentMove != Color)
            {
                yield break;
            }

            foreach (var move in GetPossibleMoves(boardPieces))
            {
                if (boardPieces.IsMoveValid(this, move.File, move.Rank))
                {
                    yield return move;
                }
            }
        }

        public virtual IEnumerable<PossibleMoveInfo> GetPossibleMoves(BoardPieces boardPieces = null, bool ignoreSpecialMoves = false)
        {
            yield break;
        }

        public void UpdatePosition()
        {
            int x = (int)_file;
            int y = (int)_rank;
            Vector2 p1 = 100 * new Vector2(x, y) - new Vector2(350, 350);

            _transform.localPosition = p1;
        }

        public void StartAnimation(BoardPosition from, BoardPosition to, Action onAnimationComplete = null)
        {
            _onAnimationComplete = onAnimationComplete;

            int fx = (int)from.File;
            int fy = (int)from.Rank;

            int tx = (int)to.File;
            int ty = (int)to.Rank;

            AnimationFrom = 100 * new Vector2(fx, fy) - new Vector2(350, 350);
            AnimationTo = 100 * new Vector2(tx, ty) - new Vector2(350, 350);
            AnimationProgress = 0f;
            IsAnimating = true;
            UpdateAnimation();
        }

        public void ForceFinishAnimation()
        {
            AnimationProgress = 1f;
            UpdateAnimation();
        }

        public bool UpdateAnimation()
        {
            AnimationProgress += Time.deltaTime / 0.1f;
            if (AnimationProgress >= 1f)
            {
                AnimationProgress = 1f;
                IsAnimating = false;
                UpdatePosition();

                _onAnimationComplete?.Invoke();
            }
            else
            {
                _transform.localPosition = Vector3.Lerp(AnimationFrom, AnimationTo, AnimationProgress);
            }
            return !IsAnimating;
        }
    }
}
