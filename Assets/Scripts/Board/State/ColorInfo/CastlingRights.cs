using UnityEngine;

namespace Board.State.ColorInfo
{
    public enum CastlingRights
    {
        None = 0,
        KingSide = 1 << 0,
        QueenSide = 1 << 1,
        Both = KingSide | QueenSide,
    }
}