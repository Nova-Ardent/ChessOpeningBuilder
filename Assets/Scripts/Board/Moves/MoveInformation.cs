using Board.Common;
using Board.Pieces;
using Board.Pieces.Types;

namespace Board.Moves
{
    public class MoveInformation
    {
        public PieceColor PieceColor { get; set; }
        public bool IsCapture { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCastle { get; set; }
        public PieceTypes PieceType { get; set; }
        public PieceTypes? Promotion { get; set; }

        public BoardPosition From { get; set; }
        public BoardPosition To { get; set; }

        public Files? FileDisambiguation;
        public Ranks? RankDisambiguation;

        public string resultingFen;

        public override string ToString()
        {
            if (IsCastle)
            {
                if (To.File == Files.G)
                {
                    return King.CastleKingSideNotation;
                }
                else
                {
                    return King.CastleQueenSideNotation;
                }
            }

            string notation = "";
            switch (PieceType)
            {
                case PieceTypes.Pawn:
                    if (IsCapture)
                    {
                        notation += From.File.AsText();
                    }
                    break;
                case PieceTypes.Knight:
                    notation += Knight.MoveCharacter;
                    break;
                case PieceTypes.Bishop:
                    notation += Bishop.MoveCharacter;
                    break;
                case PieceTypes.Rook:
                    notation += Rook.MoveCharacter;
                    break;
                case PieceTypes.Queen:
                    notation += Queen.MoveCharacter;
                    break;
                case PieceTypes.King:
                    notation += King.MoveCharacter;
                    break;
            }

            if (FileDisambiguation.HasValue)
            {
                notation += FileDisambiguation.Value.AsText();
            }

            if (RankDisambiguation.HasValue)
            {
                notation += RankDisambiguation.Value.AsText();
            }

            if (IsCapture)
            {
                notation += "x";
            }

            notation += $"{To.File.AsText()}{To.Rank.AsText()}";

            switch (Promotion)
            {
                case PieceTypes.Knight:
                    notation += $"={Knight.MoveCharacter}";
                    break;
                case PieceTypes.Bishop:
                    notation += $"={Bishop.MoveCharacter}";
                    break;
                case PieceTypes.Rook:
                    notation += $"={Rook.MoveCharacter}";
                    break;
                case PieceTypes.Queen:
                    notation += $"={Queen.MoveCharacter}";
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
