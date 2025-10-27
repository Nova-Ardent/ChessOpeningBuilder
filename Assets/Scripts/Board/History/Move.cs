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
        public PieceTypes? Promotion;

        public PieceTypes PieceType;
        
        public File FromFile;
        public Rank FromRank;

        public File ToFile;
        public Rank ToRank;

        public File? FileDisambiguation;
        public Rank? RankDisambiguation;

        public string resultingFen;

        public override string ToString()
        {
            if (IsCastle)
            {
                if (ToFile == File.G)
                {
                    return "O-O";
                }
                else
                {
                    return "O-O-O";
                }
            }

            string notation = "";

            switch (PieceType)
            {
                case PieceTypes.Pawn:
                    if (IsCapture)
                    {
                        notation += FromFile.ToString().ToLower();
                    }
                    break;
                case PieceTypes.Knight:
                    notation += "N";
                    break;
                case PieceTypes.Bishop:
                    notation += "B";
                    break;
                case PieceTypes.Rook:
                    notation += "R";
                    break;
                case PieceTypes.Queen:
                    notation += "Q";
                    break;
                case PieceTypes.King:
                    notation += "K";
                    break;
            }

            if (FileDisambiguation.HasValue)
            {
                notation += FileDisambiguation.Value.ToString().ToLower();
            }

            if (RankDisambiguation.HasValue)
            {
                notation += ((int)RankDisambiguation.Value + 1).ToString();
            }

            if (IsCapture)
            {
                notation += "x";
            }

            notation += $"{ToFile.ToString().ToLower()}{(int)ToRank + 1}";

            switch (Promotion)
            {
                case PieceTypes.Knight:
                    notation += "=N";
                    break;
                case PieceTypes.Bishop:
                    notation += "=B";
                    break;
                case PieceTypes.Rook:
                    notation += "=R";
                    break;
                case PieceTypes.Queen:
                    notation += "=Q";
                    break;
                default:
                    break;
            }

            if (IsCheck)
            {
                notation += "+";
            }

            return notation;
        }
    }
}
