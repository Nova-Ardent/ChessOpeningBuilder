using Board.Display.Arrows;
using UnityEngine;
using Board.Common;
using Board.Display.Moves;
using Board.Pieces;
using Board.State;

namespace Board.Display.Highlight
{
    [System.Serializable]
    public class HighlightManager
    {
        [SerializeField] Highlighting _RightClickSquareHighlighting;
        [SerializeField] ArrowManager _RightClickArrowManager;

        [SerializeField] Highlighting _pieceHighlighting;
        [SerializeField] MoveDisplayManager _moveDisplayManager;

        BoardPosition? _from;
        BoardPosition? _to;

        public void ToggleHighlightSquare(Files file, Ranks rank)
        {
            _pieceHighlighting.ClearAll();
            _moveDisplayManager.ClearMarkers();
            _RightClickSquareHighlighting.ToggleHighlight(file, rank);

            UpdateLastMove();
        }

        public void HighlightPiece(Piece piece, BoardPieces pieces)
        {
            _RightClickSquareHighlighting.ClearAll();
            _pieceHighlighting.ClearAll();
            _moveDisplayManager.ClearMarkers();

            _pieceHighlighting.ToggleHighlight(piece.File, piece.Rank);
            _moveDisplayManager.SpawnMoves(piece.GetLegalMoves(pieces));

            UpdateLastMove();
        }

        public void ClearAllHighlights()
        {
            _RightClickSquareHighlighting.ClearAll();
            _RightClickArrowManager.ClearAll();
            _pieceHighlighting.ClearAll();
            _moveDisplayManager.ClearMarkers();
        }

        public void SetLastMove(BoardPosition from, BoardPosition to)
        {
            _from = from;
            _to = to;

            UpdateLastMove();
        }

        public void ClearLastMove()
        {
            _from = null;
            _to = null;
        }

        void UpdateLastMove()
        {
            if(_from == null || _to == null)
            {
                return;
            }

            if (!_RightClickSquareHighlighting.IsHighlighted(_from.Value.File, _from.Value.Rank))
                _pieceHighlighting.SetHighlight(_from.Value.File, _from.Value.Rank, true);

            if (!_RightClickSquareHighlighting.IsHighlighted(_to.Value.File, _to.Value.Rank))
                _pieceHighlighting.SetHighlight(_to.Value.File, _to.Value.Rank, true);
        }

        public void UpdateHighlightArrow(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            _RightClickArrowManager.UpdateArrow(fromFile, fromRank, toFile, toRank);
        }

        public void AddArrow(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            _RightClickArrowManager.AddArrow(fromFile, fromRank, toFile, toRank);
        }

        public void RemoveArrowIfExists(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            _RightClickArrowManager.RemoveArrowIfExists(fromFile, fromRank, toFile, toRank);
        }
    }
}