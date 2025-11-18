using Board.Common;

namespace Board.Controller
{
    public struct MouseData
    {
        public const int FromPositionShiftX = 1 << 0;
        public const int FromPositionShiftY = 1 << 4;
        public const int ToPositionShiftX = 1 << 8;
        public const int ToPositionShiftY = 1 << 12;
        public const int MouseDownShift = 1 << 20;


        public bool IsMouseDown;
        public BoardPosition FromPosition;
        public BoardPosition ToPosition;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash =
                + ((int)FromPosition.File * FromPositionShiftX)
                + ((int)FromPosition.Rank * FromPositionShiftY)
                + ((int)ToPosition.File * ToPositionShiftX)
                + ((int)ToPosition.Rank * ToPositionShiftY)
                + (IsMouseDown ? MouseDownShift : 0);
            return hash;
        }
    }
}