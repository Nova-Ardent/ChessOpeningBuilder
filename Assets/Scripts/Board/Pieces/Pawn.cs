using Board.Common;
using Board.Display.Moves;
using Board.Pieces.Types;
using Board.State;
using System.Collections.Generic;
using System.ComponentModel;

namespace Board.Pieces
{
    public class Pawn : Piece
    {
        public const char BlackCharacter = 'p';
        public const char WhiteCharacter = 'P';

        public override char PieceCharacter => Color == PieceColor.White ? WhiteCharacter : BlackCharacter;


        public readonly static PieceTypes PieceType = PieceTypes.Pawn;

        public override PieceTypes Type => PieceType;
        public bool CanEnPassant { get; set; } = false;

        public override IEnumerable<PossibleMoveInfo> GetPossibleMoves(BoardPieces boardPieces = null, bool ignoreSpecialMoves = false)
        {
            if (boardPieces == null)
            {
                boardPieces = BoardPieces;
            }

            if (Rank == Ranks._8)
            {
                yield break;
            }

            if (Color == PieceColor.White)
            {
                if (Rank == Ranks._2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (boardPieces[File, Rank + i + 1] != null)
                        {
                            break;
                        }

                        yield return new PossibleMoveInfo()
                        {
                            File = File,
                            Rank = Rank + i + 1,
                            IsCapture = false
                        };
                    }
                }
                else
                {
                    if (boardPieces[File, Rank + 1] == null)
                    {
                        yield return new PossibleMoveInfo()
                        {
                            File = File,
                            Rank = Rank + 1,
                            IsCapture = false
                        };
                    }
                }

                Ranks diagnolRank = Rank + 1;
                if (File != Files.A)
                {
                    Files diagnolFileLeft = File - 1;
                    Piece targetPieceLeft = boardPieces[diagnolFileLeft, diagnolRank];

                    if (targetPieceLeft != null && targetPieceLeft.Color != Color)
                    {
                        yield return new PossibleMoveInfo()
                        {
                            File = diagnolFileLeft,
                            Rank = diagnolRank,
                            IsCapture = true
                        };
                    }
                }

                if (File != Files.H)
                {
                    Files diagnolFileRight = File + 1;
                    Piece targetPieceRight = boardPieces[diagnolFileRight, diagnolRank];

                    if (targetPieceRight != null && targetPieceRight.Color != Color)
                    {
                        yield return new PossibleMoveInfo()
                        {
                            File = diagnolFileRight,
                            Rank = diagnolRank,
                            IsCapture = true
                        };
                    }
                }


                Files enPassantLeft = File - 1;
                if ((int)enPassantLeft >= 0 && boardPieces[enPassantLeft, Rank] is Pawn leftPawn && leftPawn.CanEnPassant)
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = enPassantLeft,
                        Rank = Rank + 1,
                        IsCapture = true
                    };
                }

                Files enPassantRight = File + 1;
                if ((int)enPassantRight <= 7 && boardPieces[enPassantRight, Rank] is Pawn rightPawn && rightPawn.CanEnPassant)
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = enPassantRight,
                        Rank = Rank + 1,
                        IsCapture = true
                    };
                }
            }
            else
            {
                if (Rank == Ranks._7)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (boardPieces[File, Rank - i - 1] != null)
                        {
                            break;
                        }

                        yield return new PossibleMoveInfo() 
                        { 
                            File = File,
                            Rank = Rank - i - 1,
                            IsCapture = false
                        };
                    }
                }
                else
                {
                    if (boardPieces[File, Rank - 1] == null)
                    {
                        yield return new PossibleMoveInfo() 
                        {
                            File = File,
                            Rank = Rank - 1,
                            IsCapture = false
                        };
                    }
                }

                Ranks diagnolRank = Rank - 1;
                if (File != Files.A)
                {
                    Files diagnolFileLeft = File - 1;
                    Piece targetPieceLeft = boardPieces[diagnolFileLeft, diagnolRank];

                    if (targetPieceLeft != null && targetPieceLeft.Color != Color)
                    {
                        yield return new PossibleMoveInfo() 
                        {
                            File = diagnolFileLeft,
                            Rank = diagnolRank,
                            IsCapture = true
                        };
                    }
                }

                if (File != Files.H)
                {
                    Files diagnolFileRight = File + 1;
                    Piece targetPieceRight = boardPieces[diagnolFileRight, diagnolRank];

                    if (targetPieceRight != null && targetPieceRight.Color != Color)
                    {
                        yield return new PossibleMoveInfo() 
                        {
                            File = diagnolFileRight,
                            Rank = diagnolRank,
                            IsCapture = true
                        };
                    }
                }

                Files enPassantLeft = File - 1;
                if ((int)enPassantLeft >= 0 && boardPieces[enPassantLeft, Rank] is Pawn leftPawn && leftPawn.CanEnPassant)
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = enPassantLeft,
                        Rank = Rank - 1,
                        IsCapture = true
                    };
                }

                Files enPassantRight = File + 1;
                if ((int)enPassantRight <= 7 && boardPieces[enPassantRight, Rank] is Pawn rightPawn && rightPawn.CanEnPassant)
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = enPassantRight,
                        Rank = Rank - 1,
                        IsCapture = true
                    };
                }
            }
        }
    }
}