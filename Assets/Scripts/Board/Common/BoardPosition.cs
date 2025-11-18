using UnityEngine;

namespace Board.Common
{
    public struct BoardPosition
    {
        public Files File;
        public Ranks Rank;

        public BoardPosition(Files file, Ranks rank)
        {
            File = file;
            Rank = rank;
        }
    }
}
