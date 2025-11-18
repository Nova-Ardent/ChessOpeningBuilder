using UnityEngine;


namespace Board.Common
{
    public enum Ranks
    {
        _1,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        Count,
    }

    public static class RanksExtensions
    {
        public static Ranks ToRank(this char rank)
        {
            int fileIndex = (rank - '1');
            if (fileIndex < 0 || fileIndex >= (int)Ranks.Count)
            {
                Debug.LogError($"invalid file text {rank}");
                return Ranks.Count;
            }

            return (Ranks)fileIndex;
        }

        public static string AsText(this Ranks rank)
        {
            switch (rank)
            {
                case Ranks._1:
                    return "1";
                case Ranks._2:
                    return "2";
                case Ranks._3:
                    return "3";
                case Ranks._4:
                    return "4";
                case Ranks._5:
                    return "5";
                case Ranks._6:
                    return "6";
                case Ranks._7:
                    return "7";
                case Ranks._8:
                    return "8";
                default:
                    Debug.LogError("Invalid rank value: " + rank);
                    return "";
            }
        }
    }
}
