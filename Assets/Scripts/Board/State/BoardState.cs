using UnityEngine;
using Board.Common;
using Board.Pieces;
using Board.Pieces.Types;
using Board.Moves;
using System.Collections.Generic;

namespace Board.State
{
    [System.Serializable]
    public class BoardState
    {
        [SerializeField] BoardHistory _history = new BoardHistory();
        public BoardHistory History => _history;

        [SerializeField] BoardPieces _pieces;
        public BoardPieces Pieces => _pieces;

        public BoardState()
        {
        }

        public void SetFen(string fen)
        {
            _pieces.SetFen(fen);
        }

        public MoveInformation MovePiece(Piece piece, Files toFile, Ranks toRank, PieceTypes? promotion = null)
        {
            MoveInformation move = _pieces.MovePiece(piece, toFile, toRank, promotion);
            _history.AddMove(move);
            return move;
        }

        public MoveInformation MovePiece(string algebraicNotation)
        {
            MoveInformation move = _pieces.MovePiece(algebraicNotation);
            _history.AddMove(move);
            return move;
        }

        public MoveInformation ViewMove(int move)
        {
            MoveInformation moveInformation = _history.ViewMove(move);
            _pieces.SetFen(moveInformation.resultingFen);
            return moveInformation;
        }

        public int GetViewedMoveIndex()
        {
            return _history.ViewingMoveIndex;
        }

        public MoveInformation GetViewedMove()
        {
            return _history.MoveList[_history.ViewingMoveIndex];
        }

        public bool IsViewingLatestMove()
        {
            return _history.IsViewingLatestMove;
        }

        public int GetMoveHistoryCount()
        {
            return _history.MoveList.Count;
        }

        public void ClearHistory(string startingFen)
        {
            _history.ClearMoveHistory();
            _pieces.SetFen(startingFen);
        }

        public MoveInformation RemoveLatestMove(string startingFen)
        {
            MoveInformation removedMove = _history.RemoveLast();

            if (GetMoveHistoryCount() == 0)
            {
                _pieces.SetFen(startingFen);
                return removedMove;
            }
            else
            {
                MoveInformation newLast = GetViewedMove();
                _pieces.SetFen(newLast.resultingFen);
            }

            return removedMove;
        }

        public IEnumerable<MoveInformation> GetMoveHistory()
        {
            return _history.MoveList;
        }
    }
}