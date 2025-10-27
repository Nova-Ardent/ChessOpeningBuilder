using Board.History.Pairs;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Board.History
{
    public class BoardHistory : MonoBehaviour
    {
        [System.Serializable]
        public struct MovePositionData
        {
            public float indexX;

            public float x;
            public float y;
            public float dx;
            public float dy;
        }

        public struct MoveView
        {
            public int Index;
            public bool IsWhite;
        }

        public MovePositionData movePositionData;

        MoveView _latestMove = new MoveView() { Index = 0, IsWhite = true };
        MoveView _viewingMove = new MoveView() { Index = 0, IsWhite = true };

        List<MoveLabelPair> _moveLabels = new List<MoveLabelPair>();
        List<MovePair> _moves = new List<MovePair>();
        public BoardState _boardState;

        public MoveIndexLabel moveIndexLabelPrefab;
        public MoveLabel moveLabelPrefab;
        public GameObject MoveListObject;

        public string StartingFen;

        private void Awake()
        {
            AddMovePair();
        }

        public void SetBoardState(BoardState boardState)
        {
            _boardState = boardState;
        }

        public void GoToMove(int index, bool isWhite)
        {
            _viewingMove = new MoveView() { Index = index, IsWhite = isWhite  };

            if (_boardState == null)
            {
                Debug.LogError("BoardState is not set in BoardHistory.");
            }
            _boardState.SetFEN(isWhite ? _moves[index].White.resultingFen : _moves[index].Black.resultingFen);
        }

        public void AddMove(Move move)
        {
            MoveLabel moveLabel = Instantiate<MoveLabel>(moveLabelPrefab, MoveListObject.transform);
            moveLabel.SetMove(move);
            moveLabel.transform.localPosition = new Vector3
                ( movePositionData.x + movePositionData.dx * (move.IsWhite ? 0 : 1)
                , movePositionData.y - movePositionData.dy * _moveLabels.Count
                , 0
                );

            if (!move.IsWhite)
            {
                _moves.Last().Black = move;
                _moveLabels.Last().Black = moveLabel;

                int moveCount = _moves.Count;
                moveLabel.SetCallback(() => GoToMove(moveCount - 1, false));
                
                AddMovePair();
            }
            else
            {
                _moves.Last().White = move;
                _moveLabels.Last().White = moveLabel;

                int moveCount = _moves.Count;
                moveLabel.SetCallback(() => GoToMove(moveCount - 1, true));
            }
        }

        void AddMovePair()
        {
            _moveLabels.Add(new MoveLabelPair());
            _moves.Add(new MovePair());

            MoveIndexLabel indexLabel = Instantiate<MoveIndexLabel>(moveIndexLabelPrefab, MoveListObject.transform);
            indexLabel.SetMove(_moves.Count);
            indexLabel.transform.localPosition = new Vector3
                ( movePositionData.indexX
                , movePositionData.y - movePositionData.dy * _moveLabels.Count
                , 0
                );
            _moveLabels.Last().Index = indexLabel;
        }
    }
}
