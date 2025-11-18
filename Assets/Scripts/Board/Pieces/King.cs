using Board.Common;
using Board.Display.Moves;
using Board.Pieces.Types;
using Board.State;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Board.State.ColorInfo;

namespace Board.Pieces
{
    public class King : Piece
    {
        public static readonly Vector2[] MoveDirections = new Vector2[]
        {
            new Vector2Int(1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
        };

        public const char BlackCharacter = 'k';
        public const char WhiteCharacter = 'K';
        public const char MoveCharacter = 'K';

        public const string CastleKingSideNotation = "O-O";
        public const string CastleQueenSideNotation = "O-O-O";

        public override char PieceCharacter => Color == PieceColor.White ? WhiteCharacter : BlackCharacter;

        public readonly static PieceTypes PieceType = PieceTypes.King;

        public override PieceTypes Type => PieceType;

        public override IEnumerable<PossibleMoveInfo> GetPossibleMoves(BoardPieces boardPieces = null, bool ignoreSpecialMoves = false)
        {
            if (boardPieces == null)
            {
                boardPieces = BoardPieces;
            }

            foreach (var direction in MoveDirections)
            {
                int targetFile = (int)File + (int)direction.x;
                int targetRank = (int)Rank + (int)direction.y;
                if (targetFile < 0 || targetFile > 7 || targetRank < 0 || targetRank > 7)
                {
                    continue;
                }

                if (boardPieces[(Files)targetFile, (Ranks)targetRank] == null)
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = (Files)targetFile,
                        Rank = (Ranks)targetRank,
                        IsCapture = false,
                    };
                }
                else if (boardPieces[(Files)targetFile, (Ranks)targetRank].Color != Color)
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = (Files)targetFile,
                        Rank = (Ranks)targetRank,
                        IsCapture = true,
                    };
                }
            }

            if (!ignoreSpecialMoves && CanCastleKingSide(boardPieces))
            {
                if (Color == PieceColor.White)
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = Files.G,
                        Rank = Ranks._1,
                        IsCapture = false
                    };
                }
                else
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = Files.G,
                        Rank = Ranks._8,
                        IsCapture = false
                    };
                }
            }

            if (!ignoreSpecialMoves && CanCastleQueenSide(boardPieces))
            {
                if (Color == PieceColor.White)
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = Files.C,
                        Rank = Ranks._1,
                        IsCapture = false
                    };
                }
                else
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = Files.C,
                        Rank = Ranks._8,
                        IsCapture = false
                    };
                }
            }

            bool CanCastleKingSide(BoardPieces boardPieces)
            {
                if (Color == PieceColor.White)
                {
                    if (!boardPieces.GetCastlingRights(Color).HasFlag(CastlingRights.KingSide))
                    {
                        return false;
                    }

                    if (boardPieces[Files.F, Ranks._1] != null ||
                        boardPieces[Files.G, Ranks._1] != null)
                    {
                        return false;
                    }

                    return !boardPieces.IsPositionAttacked(Files.E, Ranks._1, PieceColor.Black) &&
                           !boardPieces.IsPositionAttacked(Files.F, Ranks._1, PieceColor.Black) &&
                           !boardPieces.IsPositionAttacked(Files.G, Ranks._1, PieceColor.Black);
                }
                else
                {
                    if (!boardPieces.GetCastlingRights(Color).HasFlag(CastlingRights.KingSide))
                    {
                        return false;
                    }

                    if (boardPieces[Files.F, Ranks._8] != null ||
                        boardPieces[Files.G, Ranks._8] != null)
                    {
                        return false;
                    }

                    return !boardPieces.IsPositionAttacked(Files.E, Ranks._8, PieceColor.White) &&
                           !boardPieces.IsPositionAttacked(Files.F, Ranks._8, PieceColor.White) &&
                           !boardPieces.IsPositionAttacked(Files.G, Ranks._8, PieceColor.White);
                }
            }

            bool CanCastleQueenSide(BoardPieces boardPieces)
            {
                if (Color == PieceColor.White)
                {
                    if (!boardPieces.GetCastlingRights(Color).HasFlag(CastlingRights.QueenSide))
                    {
                        return false;
                    }

                    if (boardPieces[Files.B, Ranks._1] != null ||
                        boardPieces[Files.C, Ranks._1] != null ||
                        boardPieces[Files.D, Ranks._1] != null)
                    {
                        return false;
                    }

                    return !boardPieces.IsPositionAttacked(Files.E, Ranks._1, PieceColor.Black) &&
                           !boardPieces.IsPositionAttacked(Files.D, Ranks._1, PieceColor.Black) &&
                           !boardPieces.IsPositionAttacked(Files.C, Ranks._1, PieceColor.Black);
                }
                else
                {
                    if (!boardPieces.GetCastlingRights(Color).HasFlag(CastlingRights.QueenSide))
                    {
                        return false;
                    }

                    if (boardPieces[Files.B, Ranks._8] != null ||
                        boardPieces[Files.C, Ranks._8] != null ||
                        boardPieces[Files.D, Ranks._8] != null)
                    {
                        return false;
                    }

                    return !boardPieces.IsPositionAttacked(Files.E, Ranks._8, PieceColor.White) &&
                           !boardPieces.IsPositionAttacked(Files.D, Ranks._8, PieceColor.White) &&
                           !boardPieces.IsPositionAttacked(Files.C, Ranks._8, PieceColor.White);
                }
            }
        }
    }
}