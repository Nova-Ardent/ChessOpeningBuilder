using UnityEngine;
using Board.Pieces;
using static Board.Pieces.Piece;

namespace Board.History
{
    public class Move
    {
        public bool IsCapture;
        public bool IsCheck;
        public bool IsCastle;
        public bool IsWhite;

        public PieceTypes Piece;
        
        public File ToFile;
        public Rank ToRank;

        public File? FileDisambiguation;
        public Rank? RankDisambiguation;
    }
}
