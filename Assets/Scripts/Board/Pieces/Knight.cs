using Board.Common;
using Board.Display.Moves;
using Board.Pieces.Types;
using Board.State;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Board.Pieces
{
    public class Knight : Piece
    {
        public static readonly Vector2[] MoveDirections = new Vector2[]
        {
            new Vector2(1, 2),
            new Vector2(2, 1),
            new Vector2(2, -1),
            new Vector2(1, -2),
            new Vector2(-1, -2),
            new Vector2(-2, -1),
            new Vector2(-2, 1),
            new Vector2(-1, 2),
        };

        public const char BlackCharacter = 'n';
        public const char WhiteCharacter = 'N';
        public const char MoveCharacter = 'N';

        public override char PieceCharacter => Color == PieceColor.White ? WhiteCharacter : BlackCharacter;

        public readonly static PieceTypes PieceType = PieceTypes.Knight;
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

                if (boardPieces[(Files)targetFile, (Ranks)targetRank] != null)
                {
                    if (boardPieces[(Files)targetFile, (Ranks)targetRank].Color == Color)
                    {
                        continue;
                    }

                    yield return new PossibleMoveInfo()
                    {
                        File = (Files)targetFile,
                        Rank = (Ranks)targetRank,
                        IsCapture = true
                    };
                }
                else
                {
                    yield return new PossibleMoveInfo()
                    {
                        File = (Files)targetFile,
                        Rank = (Ranks)targetRank,
                        IsCapture = false
                    };
                }
            }
        }
    }
}