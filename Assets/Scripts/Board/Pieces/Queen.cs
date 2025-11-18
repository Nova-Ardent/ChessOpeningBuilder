using Board.Common;
using Board.Display.Moves;
using Board.Pieces.Types;
using Board.State;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Board.Pieces
{
    public class Queen : Piece
    {
        static readonly Vector2[] MoveDirections = new Vector2[]
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

        public const char BlackCharacter = 'q';
        public const char WhiteCharacter = 'Q';
        public const char MoveCharacter = 'Q';

        public override char PieceCharacter => Color == PieceColor.White ? WhiteCharacter : BlackCharacter;

        public readonly static PieceTypes PieceType = PieceTypes.Queen;

        public override PieceTypes Type => PieceType;

        public override IEnumerable<PossibleMoveInfo> GetPossibleMoves(BoardPieces boardPieces = null, bool ignoreSpecialMoves = false)
        {
            if (boardPieces == null)
            {
                boardPieces = BoardPieces;
            }

            foreach (var direction in MoveDirections)
            {
                for (int i = 1; i < 8; i++)
                {
                    int targetFile = (int)File + (int)direction.x * i;
                    int targetRank = (int)Rank + (int)direction.y * i;
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
                            IsCapture = false
                        };
                    }
                    else if (boardPieces[(Files)targetFile, (Ranks)targetRank].Color != Color)
                    {
                        yield return new PossibleMoveInfo()
                        {
                            File = (Files)targetFile,
                            Rank = (Ranks)targetRank,
                            IsCapture = true
                        };
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}