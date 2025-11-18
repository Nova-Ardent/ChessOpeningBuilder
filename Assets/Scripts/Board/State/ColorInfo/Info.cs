using Board.Pieces;
using Board.Pieces.Types;
using System.Collections.Generic;
using UnityEngine;

namespace Board.State.ColorInfo
{
    [System.Serializable]
    public class Info
    {
        [System.Serializable]
        public struct PiecesPrefabs
        {
            public Pawn Pawn;
            public Rook Rook;
            public Knight Knight;
            public Bishop Bishop;
            public Queen Queen;
            public King King;

            public Dictionary<PieceTypes, Piece> PieceLookup;

            public void SetPrefabLookup()
            {
                PieceLookup = new Dictionary<PieceTypes, Piece>
                {
                    { PieceTypes.Pawn, Pawn },
                    { PieceTypes.Rook, Rook },
                    { PieceTypes.Knight, Knight },
                    { PieceTypes.Bishop, Bishop },
                    { PieceTypes.Queen, Queen },
                    { PieceTypes.King, King },
                };
            }
        }

        [SerializeField] PiecesPrefabs _piecePrefabs;

        public King King { get; private set; }
        public Dictionary<PieceTypes, List<Piece>> PiecesByType { get; set; } = new Dictionary<PieceTypes, List<Piece>>
        {
            { PieceTypes.Pawn, new List<Piece>() },
            { PieceTypes.Rook, new List<Piece>() },
            { PieceTypes.Knight, new List<Piece>() },
            { PieceTypes.Bishop, new List<Piece>() },
            { PieceTypes.Queen, new List<Piece>() },
            { PieceTypes.King, new List<Piece>() },
        };

        public CastlingRights CastlingRights { get; set; } = new CastlingRights();

        public void Initialize()
        {
            _piecePrefabs.SetPrefabLookup();
        }

        public Piece InstantiatePiece(PieceTypes pieceType)
        {
            if (_piecePrefabs.PieceLookup.TryGetValue(pieceType, out Piece prefab))
            {
                if (prefab == null)
                {
                    Debug.LogError($"Prefab for piece type {pieceType} is null.");
                    return null;
                }

                Piece piece = Piece.Instantiate(prefab);
                PiecesByType[pieceType].Add(piece);

                if (pieceType == PieceTypes.King)
                {
                    King = piece as King;
                }

                return piece;
            }

            Debug.LogError($"Prefab for piece type {pieceType} not found.");
            return null;
        }

        public Piece InstantiatePiece(char pieceType)
        {
            foreach (var kvp in _piecePrefabs.PieceLookup)
            {
                Piece prefab = kvp.Value;
                if (prefab == null)
                {
                    Debug.LogError($"Prefab for piece type {pieceType} is null.");
                    return null;
                }

                if (prefab.PieceCharacter == pieceType)
                {
                    Piece piece = Piece.Instantiate(prefab);
                    PiecesByType[kvp.Key].Add(piece);

                    if (kvp.Key == PieceTypes.King)
                    {
                        King = piece as King;
                    }

                    return piece;
                }
            }

            Debug.LogError($"Prefab for piece character {pieceType} not found.");
            return null;
        }

        public void RemovePiece(Piece piece)
        {
            if (PiecesByType.TryGetValue(piece.Type, out List<Piece> pieceList))
            {
                pieceList.Remove(piece);
                Object.Destroy(piece.gameObject);
            }
            else
            {
                Debug.LogError($"Piece type {piece.Type} not found in PiecesByType.");
            }
        }


        public void Clear()
        {
            foreach (var pieceList in PiecesByType.Values)
            {
                pieceList.Clear();
            }
            King = null;
            CastlingRights = CastlingRights.None;
        }
    }
}
