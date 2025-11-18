using System.Collections.Generic;
using UnityEngine;
using Board.Common;

namespace Board.Display.Moves
{
    public class MoveDisplayManager : MonoBehaviour
    {
        public Move MoveMarkerPrefab;
        public Move TakeMarkerPrefab;

        Stack<Move> _movePool = new Stack<Move>();
        Stack<Move> _takePool = new Stack<Move>();

        Stack<Move> _activeMoves = new Stack<Move>();
        Stack<Move> _activeTakes = new Stack<Move>();

        public void SpawnMoves(IEnumerable<PossibleMoveInfo> moveData)
        {
            ClearMarkers();

            foreach (var move in moveData)
            {
                Move marker = GetMoveMarker(move);
                if (move.IsCapture)
                {
                    _activeTakes.Push(marker);
                }
                else
                {
                    _activeMoves.Push(marker);
                }
            }
        }

        Move GetMoveMarker(PossibleMoveInfo move)
        {
            Move marker = null;
            if (move.IsCapture)
            {
                if (_takePool.Count > 0)
                {
                    marker = _takePool.Pop();
                    marker.gameObject.SetActive(true);
                }
                else
                {
                    marker = Instantiate<Move>(TakeMarkerPrefab, this.transform);
                }
            }
            else
            {
                if (_movePool.Count > 0)
                {
                    marker = _movePool.Pop();
                    marker.gameObject.SetActive(true);
                }
                else
                {
                    marker = Instantiate<Move>(MoveMarkerPrefab, this.transform);
                }
            }

            marker.File = move.File;
            marker.Rank = move.Rank;
            return marker;
        }

        public void ClearMarkers()
        {
            while (_activeMoves.Count > 0)
            {
                var marker = _activeMoves.Pop();
                _movePool.Push(marker);
                marker.gameObject.SetActive(false);
            }

            while (_activeTakes.Count > 0)
            {
                var marker = _activeTakes.Pop();
                _takePool.Push(marker);
                marker.gameObject.SetActive(false);
            }
        }
    }
}