using Board.Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Board.Display.Moves
{
    public struct PossibleMoveInfo
    {
        public Files File;
        public Ranks Rank;
        public bool IsCapture;
    }
}
