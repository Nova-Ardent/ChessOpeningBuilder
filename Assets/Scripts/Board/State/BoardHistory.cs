using Board.Moves;
using System.Collections.Generic;
using UnityEngine;

namespace Board.State
{
    public class BoardHistory
    {
        public bool IsViewingLatestMove => ViewingMoveIndex == MoveList.Count - 1;
        public int ViewingMoveIndex { get; set; } = -1;
        public List<MoveInformation> MoveList { get; private set; } = new List<MoveInformation>();

        public void AddMove(MoveInformation move)
        {
            ViewingMoveIndex = MoveList.Count;
            MoveList.Add(move);
        }

        public MoveInformation ViewMove(int move)
        {
            ViewingMoveIndex = Mathf.Clamp(move, 0, MoveList.Count - 1);
            return MoveList[move];
        }

        public MoveInformation GotToLast()
        {
            return ViewMove(MoveList.Count - 1);
        }

        public MoveInformation RemoveLast()
        {
            MoveInformation lastMove = GotToLast();

            if (MoveList.Count == 1)
            {
                ClearMoveHistory();
                return lastMove;
            }
            else
            {
                MoveList.RemoveAt(MoveList.Count - 1);
                ViewingMoveIndex = MoveList.Count - 1;
                return lastMove;
            }
        }

        public void ClearMoveHistory()
        {
            MoveList.Clear();
            ViewingMoveIndex = -1;
        }
    }
}
