using UnityEngine;

namespace Board.History.TopMove
{
    public struct TopMoveData
    {
        public float Evaluation { get; set; }
        public string UCI { get; set; }
        public int MateIn { get; set; }
        public bool HasMate { get; set; }
        public int Depth { get; set; }
    }
}