using Board.Common;
using Board.Display.Moves;
using Board.FlipBoard;
using Board.Moves;
using Board.Pieces;
using Board.Pieces.Types;
using Board.State.ColorInfo;
using Board.Stockfish;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Board.State
{
    public class BoardPieces : MonoBehaviour
    {
        [SerializeField] StockfishObject _stockfish;

        public PieceColor CurrentMove { get; private set; }
        [SerializeField] Info _whiteInfo;
        [SerializeField] Info _blackInfo;

        FlipBoardHandler _flipBoardHandler;

        BoardPosition? _enPassantPosition;
        Pawn _enPassantPawn;

        Piece[,] _pieces = new Piece[8, 8];
        public Piece this[Files file, Ranks rank]
        {
            get
            {
                if (rank == Ranks.Count || file == Files.Count)
                {
                    Debug.LogError("");
                }
                return _pieces[(int)file, (int)rank];
            }
            set => _pieces[(int)file, (int)rank] = value;
        }

        private void Awake()
        {
            _flipBoardHandler = GetComponent<FlipBoardHandler>();
            _whiteInfo.Initialize();
            _blackInfo.Initialize();
        }

        public void SetFen(string fen)
        {
            ClearBoard();

            string[] split = fen.Split(new[] { ' ' }, 2);
            string boardFen = split.Length > 1 ? split[0] : fen;
            string gameStateFen = split.Length > 1 ? split[1] : "";

            Ranks rank = Ranks._8;
            Files file = Files.A;
            foreach (char c in boardFen)
            {
                if (c == ' ')
                {
                    break;
                }

                if (char.IsLetter(c))
                {
                    Piece piece = null;
                    if (char.IsUpper(c))
                    {
                        piece = _whiteInfo.InstantiatePiece(c);
                    }
                    else
                    {
                        piece = _blackInfo.InstantiatePiece(c);
                    }

                    if (piece != null)
                    {
                        this[file, rank] = piece;
                        piece.RegisterBoardPiecesState(this);
                        piece.Rank = rank;
                        piece.File = file;

                        _flipBoardHandler.AddChild(piece.GetComponent<FlipBoardHandler>());
                    }

                    file++;
                }
                else if (c == '/')
                {
                    rank--;
                    file = Files.A;
                }
                else if (char.IsDigit(c))
                {
                    file += (int)char.GetNumericValue(c);
                }
            }

            foreach (char c in gameStateFen)
            {
                if (c == 'w' || c == 'W')
                {
                    CurrentMove = PieceColor.White;
                }
                else if (c == 'b' || c == 'B')
                {
                    CurrentMove = PieceColor.Black;
                }
                else if (c == 'K' 
                    && this[Files.H, Ranks._1].Type == PieceTypes.Rook
                    && this[Files.H, Ranks._1].Color == PieceColor.White 
                    && _whiteInfo.King?.Rank == Ranks._1 
                    && _whiteInfo.King?.File == Files.E)
                {
                    _whiteInfo.CastlingRights |= CastlingRights.KingSide;
                }
                else if (c == 'Q' 
                    && this[Files.A, Ranks._1].Type == PieceTypes.Rook 
                    && this[Files.A, Ranks._1].Color == PieceColor.White
                    && _whiteInfo.King?.Rank == Ranks._1 
                    && _whiteInfo.King?.File == Files.E)
                {
                    _whiteInfo.CastlingRights |= CastlingRights.QueenSide;
                }
                else if (c == 'k' 
                    && this[Files.H, Ranks._8].Type == PieceTypes.Rook
                    && this[Files.H, Ranks._8].Color == PieceColor.Black 
                    && _blackInfo.King?.Rank == Ranks._8 
                    && _blackInfo.King?.File == Files.E)
                {
                    _blackInfo.CastlingRights |= CastlingRights.KingSide;
                }
                else if (c == 'q' 
                    && this[Files.A, Ranks._8].Type == PieceTypes.Rook 
                    && this[Files.A, Ranks._8].Color == PieceColor.Black
                    && _blackInfo.King?.Rank == Ranks._8 
                    && _blackInfo.King?.File == Files.E)
                {
                    _blackInfo.CastlingRights |= CastlingRights.QueenSide;
                }
            }

            if (_blackInfo.King == null || _whiteInfo.King == null)
            {
                Debug.LogError("invalid position, missing king.");
                return;
            }

            for (int i = 0; i < gameStateFen.Length - 1; i++)
            {
                if (char.IsLetter(gameStateFen[i]) && char.IsNumber(gameStateFen[i + 1]))
                {
                    char enPassantfile = char.ToLower(gameStateFen[i]);
                    char enPassantrank = gameStateFen[i + 1];

                    _enPassantPosition = new BoardPosition((Files)(enPassantfile - 'a'), (Ranks)(enPassantrank - '1'));

                    if (CurrentMove == PieceColor.White)
                    {
                        enPassantrank--;
                    }
                    else
                    {
                        enPassantrank++;
                    }

                    Piece piece = _pieces[enPassantfile - 'a', enPassantrank - '1'];
                    if (piece != null && piece is Pawn pawn)
                    {
                        pawn.CanEnPassant = true;
                        _enPassantPawn = pawn;
                    }
                    else
                    {
                        Debug.LogError($"invalid enpassant piece in FEN position {enPassantfile}{enPassantrank}");
                    }
                }
            }

            _flipBoardHandler.SyncBoardFlipped();
            _stockfish.GoToPosition(fen, CurrentMove == PieceColor.White);
        }

        public string GetFen()
        {
            string fen = "";
            for (int rank = 7; rank >= 0; rank--)
            {
                int skip = 0;
                for (int file = 0; file < 8; file++)
                {
                    Piece piece = _pieces[file, rank];
                    if (piece != null)
                    {
                        if (skip > 0)
                        {
                            fen += skip.ToString();
                            skip = 0;
                        }

                        fen += piece.PieceCharacter;
                    }
                    else
                    {
                        skip++;
                    }
                }

                if (skip > 0)
                {
                    fen += skip.ToString();
                    skip = 0;
                }

                if (rank > 0)
                {
                    fen += '/';
                }
            }

            fen += ' ';
            fen += CurrentMove == PieceColor.White ? 'w' : 'b';
            fen += ' ';


            if (_whiteInfo.CastlingRights.HasFlag(CastlingRights.KingSide))
                fen += 'K';
            if (_whiteInfo.CastlingRights.HasFlag(CastlingRights.QueenSide))
                fen += 'Q';
            if (_blackInfo.CastlingRights.HasFlag(CastlingRights.KingSide))
                fen += 'k';
            if (_blackInfo.CastlingRights.HasFlag(CastlingRights.QueenSide))
                fen += 'q';

            if (_enPassantPawn != null)
            {
                fen += " " + _enPassantPosition.Value.File.AsText() + _enPassantPosition.Value.Rank.AsText();
            }
            else
            {
                fen += " -";
            }

            return fen;
        }

        public bool IsMoveValid(Piece piece, Files toFile, Ranks toRank)
        {
            Files fromFile = piece.File;
            Ranks fromRank = piece.Rank;

            // move piece
            Piece capturedPiece = null;
            if (this[toFile, toRank] != null)
            {
                capturedPiece = this[toFile, toRank];
                this[toFile, toRank] = null;
            }
            else if (_enPassantPawn != null && toFile == _enPassantPosition?.File && toRank == _enPassantPosition?.Rank && piece is Pawn)
            {
                capturedPiece = _enPassantPawn;
                this[_enPassantPawn.File, _enPassantPawn.Rank] = null;
            }

            this[fromFile, fromRank] = null;
            this[toFile, toRank] = piece;
            piece.File = toFile;
            piece.Rank = toRank;

            // is king attacked
            bool isKingAttacked = false;
            if (piece.Color == PieceColor.White)
            {
                Info info = this._whiteInfo;
                isKingAttacked = IsPositionAttacked(info.King.File, info.King.Rank, PieceColor.Black, capturedPiece);
            }
            else
            {
                Info info = this._blackInfo;
                isKingAttacked = IsPositionAttacked(info.King.File, info.King.Rank, PieceColor.White, capturedPiece);
            }

            // unmove piece
            this[toFile, toRank] = null;
            this[fromFile, fromRank] = piece;
            piece.File = fromFile;
            piece.Rank = fromRank;

            if (capturedPiece != null)
            {
                this[capturedPiece.File, capturedPiece.Rank] = capturedPiece;
            }

            return !isKingAttacked;
        }

        public MoveInformation MovePiece(string notation)
        {
            try
            {
                if (notation == "O-O-O")
                {
                    if (CurrentMove == PieceColor.White)
                    {
                        return MovePiece(_whiteInfo.King, Files.C, _whiteInfo.King.Rank, null);
                    }
                    else
                    {
                        return MovePiece(_blackInfo.King, Files.C, _blackInfo.King.Rank, null);
                    }
                }

                if (notation == "O-O")
                {
                    if (CurrentMove == PieceColor.White)
                    {
                        return MovePiece(_whiteInfo.King, Files.G, _whiteInfo.King.Rank, null);
                    }
                    else
                    {
                        return MovePiece(_blackInfo.King, Files.G, _blackInfo.King.Rank, null);
                    }
                }

                if (notation.Last() == '+')
                {
                    notation = notation.Remove(notation.Length - 1);
                }

                PieceTypes? promotion = null;
                if (notation.Contains("=Q"))
                {
                    promotion = PieceTypes.Queen;
                    notation = notation.Replace("=Q", "");
                }
                else if (notation.Contains("=N"))
                {
                    promotion = PieceTypes.Knight;
                    notation = notation.Replace("=N", "");
                }
                else if (notation.Contains("=R"))
                {
                    promotion = PieceTypes.Rook;
                    notation = notation.Replace("=R", "");
                }
                else if (notation.Contains("=B"))
                {
                    promotion = PieceTypes.Bishop;
                    notation = notation.Replace("=B", "");
                }

                PieceTypes pieceType;
                switch (notation[0])
                {
                    case 'K': pieceType = PieceTypes.King; break;
                    case 'Q': pieceType = PieceTypes.Queen; break;
                    case 'R': pieceType = PieceTypes.Rook; break;
                    case 'B': pieceType = PieceTypes.Bishop; break;
                    case 'N': pieceType = PieceTypes.Knight; break;
                    default: pieceType = PieceTypes.Pawn; break;
                }

                if (pieceType != PieceTypes.Pawn)
                {
                    notation = notation.Substring(1, notation.Length - 1);
                }

                Ranks toRank = notation.Last().ToRank();
                notation = notation.Substring(0, notation.Length - 1);

                Files toFile = notation.Last().ToFile();
                notation = notation.Substring(0, notation.Length - 1);

                if (notation.Contains('x'))
                {
                    notation = notation.Replace("x", "");
                }

                Files? fileDisambiguation = null;
                Ranks? rankDisambiguation = null;
                for (int i = 0; i < 2; i++)
                {
                    if (notation.Length > 0)
                    {
                        char disambiguation = notation.Last();
                        if (char.IsDigit(disambiguation))
                        {
                            rankDisambiguation = disambiguation.ToRank();
                        }
                        else if (char.IsLetter(disambiguation))
                        {
                            fileDisambiguation = disambiguation.ToFile();
                        }
                    }
                }

                Info info;
                if (CurrentMove == PieceColor.White)
                {
                    info = _whiteInfo;
                }
                else
                {
                    info = _blackInfo;
                }

                List<Piece> resultingPieces = info.PiecesByType[pieceType].Where(x => {
                    if (x == null)
                    {
                        return false;
                    }
                    else if (fileDisambiguation != null && x.File != fileDisambiguation)
                    {
                        return false;
                    }
                    else if (rankDisambiguation != null && x.Rank != rankDisambiguation)
                    {
                        return false;
                    }

                    return x.GetLegalMoves(this).Any(y => y.File == toFile && y.Rank == toRank);
                }).ToList();

                if (resultingPieces.Count > 1)
                {
                    Debug.LogError("Something went wrong with move calculation, there is still piece ambiguity");
                    return null;
                }
                else if (resultingPieces.Count == 0)
                {
                    Debug.LogError("Something went wrong with the move calculation, couldn't find valid piece.");
                    return null;
                }

                return MovePiece(resultingPieces.First(), toFile, toRank, promotion);
            }
            catch (Exception)
            {
                Debug.LogError("Failed to parse move");
                return null;
            }
        }

        public MoveInformation MovePiece(Piece piece, Files toFile, Ranks toRank, PieceTypes? promotion = null)
        {
            MoveInformation moveInformation = new MoveInformation();
            moveInformation.IsCapture = false;
            moveInformation.PieceColor = piece.Color;
            moveInformation.PieceType = piece.Type;
            moveInformation.From = new BoardPosition(piece.File, piece.Rank);
            moveInformation.To = new BoardPosition(toFile, toRank);

            UpdateAmbiguity(moveInformation, piece);

            if (this[toFile, toRank] != null)
            {
                moveInformation.IsCapture = true;
                RemovePiece(this[toFile, toRank]);
            }
            else if (_enPassantPosition != null && toFile == _enPassantPosition?.File && toRank == _enPassantPosition?.Rank && piece is Pawn)
            {
                moveInformation.IsCapture = true;
                RemovePiece(_enPassantPawn);
                _enPassantPawn = null;
                _enPassantPosition = null;
            }

            UpdateEnpassantPawn(piece, toFile, toRank);
            SetPiecePosition(moveInformation, piece, toFile, toRank);
            SetCastle(moveInformation);
            SetPromotion(moveInformation, piece, promotion);
            UpdateCastlingRights(moveInformation);
            UpdateIsCheck(moveInformation, piece.Color);

            CurrentMove = CurrentMove == PieceColor.White ? PieceColor.Black : PieceColor.White;
            
            
            moveInformation.resultingFen = GetFen();

            _flipBoardHandler.SyncBoardFlipped();
            _stockfish.GoToPosition(moveInformation.resultingFen, CurrentMove == PieceColor.White);

            return moveInformation;
        }

        void UpdateEnpassantPawn(Piece piece, Files toFile, Ranks toRank)
        {
            if (_enPassantPawn != null)
            {
                _enPassantPawn.CanEnPassant = false;
                _enPassantPawn = null;
            }

            if (piece.Type == PieceTypes.Pawn && Mathf.Abs((int)toRank - (int)piece.Rank) == 2)
            {
                _enPassantPawn = piece as Pawn;
                _enPassantPawn.CanEnPassant = true;

                Ranks enPassantrank = piece.Color == PieceColor.White ? toRank - 1 : toRank + 1;
                _enPassantPosition = new BoardPosition(toFile, enPassantrank);
            }
        }

        void SetPiecePosition(MoveInformation moveInformation, Piece piece, Files toFile, Ranks toRank)
        {
            this[piece.File, piece.Rank] = null;

            piece.File = toFile;
            piece.Rank = toRank;

            this[toFile, toRank] = piece;

            piece.UpdatePosition();
        }

        void SetCastle(MoveInformation moveInformation)
        {
            if (moveInformation.PieceType != PieceTypes.King || Mathf.Abs((int)moveInformation.From.File - (int)moveInformation.To.File) != 2)
            {
                moveInformation.IsCastle = false;
                return;
            }

            Ranks rank;
            Files start;
            Files fin;

            if (moveInformation.PieceColor == PieceColor.White)
            {
                if (moveInformation.To.File == Files.C)
                {
                    rank = Ranks._1;
                    start = Files.A;
                    fin = Files.D;
                }
                else
                {
                    rank = Ranks._1;
                    start = Files.H;
                    fin = Files.F;
                }
            }
            else
            {
                if (moveInformation.To.File == Files.C)
                {
                    rank = Ranks._8;
                    start = Files.A;
                    fin = Files.D;
                }
                else
                {
                    rank = Ranks._8;
                    start = Files.H;
                    fin = Files.F;
                }
            }

            this[fin, rank] = this[start, rank];
            this[fin, rank].File = fin;
            this[start, rank] = null;

            moveInformation.IsCastle = true;
        }

        void SetPromotion(MoveInformation moveInformation, Piece piece, PieceTypes? promotion)
        {
            if (promotion == null)
            {
                moveInformation.Promotion = null;
                return;
            }

            Piece newPiece;
            if (piece.Color == PieceColor.White)
            {
                newPiece = _whiteInfo.InstantiatePiece(promotion.Value);
            }
            else
            {
                newPiece = _blackInfo.InstantiatePiece(promotion.Value);
            }

            RemovePiece(piece);

            newPiece.File = moveInformation.To.File;
            newPiece.Rank = moveInformation.To.Rank;
            newPiece.RegisterBoardPiecesState(this);
            this[newPiece.File, newPiece.Rank] = newPiece;
        }

        void UpdateCastlingRights(MoveInformation moveInformation)
        {
            if (moveInformation.PieceType == PieceTypes.King)
            {
                if (moveInformation.PieceColor == PieceColor.White)
                {
                    _whiteInfo.CastlingRights = CastlingRights.None;
                }
                else
                {
                    _blackInfo.CastlingRights = CastlingRights.None;
                }
            }
            else if (moveInformation.PieceType == PieceTypes.Rook)
            {
                if (moveInformation.PieceColor == PieceColor.White)
                {
                    if (moveInformation.From.File == Files.A && moveInformation.From.Rank == Ranks._1)
                    {
                        _whiteInfo.CastlingRights &= ~CastlingRights.QueenSide;
                    }
                    else if (moveInformation.From.File == Files.H && moveInformation.From.Rank == Ranks._1)
                    {
                        _whiteInfo.CastlingRights &= ~CastlingRights.KingSide;
                    }
                }
                else
                {
                    if (moveInformation.From.File == Files.A && moveInformation.From.Rank == Ranks._8)
                    {
                        _blackInfo.CastlingRights &= ~CastlingRights.QueenSide;
                    }
                    else if (moveInformation.From.File == Files.H && moveInformation.From.Rank == Ranks._8)
                    {
                        _blackInfo.CastlingRights &= ~CastlingRights.KingSide;
                    }
                }
            }
        }

        void UpdateIsCheck(MoveInformation moveInformation, PieceColor movedColor)
        {
            if (movedColor == PieceColor.Black)
            {
                Info info = this._whiteInfo;
                moveInformation.IsCheck = IsPositionAttacked(info.King.File, info.King.Rank, PieceColor.Black);
            }
            else
            {
                Info info = this._blackInfo;
                moveInformation.IsCheck = IsPositionAttacked(info.King.File, info.King.Rank, PieceColor.White);
            }
        }

        void UpdateAmbiguity(MoveInformation moveInformation, Piece piece)
        {
            if (piece.Type == PieceTypes.Pawn)
            {
                return;
            }

            Info info;
            if (piece.Color == PieceColor.White)
            {
                info = _whiteInfo;
            }
            else
            {
                info = _blackInfo;
            }

            List<Piece> ambiguousPieces = info.PiecesByType[piece.Type]
                .Where(x => x != piece)
                .Where(x => x.GetPossibleMoves().Any(y => y.File == moveInformation.To.File && y.Rank == moveInformation.To.Rank))
                .ToList();

            if (ambiguousPieces.Count > 0)
            {
                bool isFileAmbiguous = false;
                bool isRankAmbiguous = false;
                foreach (var ambiguousPiece in ambiguousPieces)
                {
                    if (ambiguousPiece.File == moveInformation.From.File)
                    {
                        isFileAmbiguous = true;
                    }
                    if (ambiguousPiece.Rank == moveInformation.From.Rank)
                    {
                        isRankAmbiguous = true;
                    }
                }

                if (isFileAmbiguous)
                {
                    if (isRankAmbiguous)
                    {
                        moveInformation.FileDisambiguation = moveInformation.From.File;
                        moveInformation.RankDisambiguation = moveInformation.From.Rank;
                    }
                    else
                    {
                        moveInformation.RankDisambiguation = moveInformation.From.Rank;
                    }
                }
                else
                {
                    moveInformation.FileDisambiguation = moveInformation.From.File;
                }
            }

        }

        void RemovePiece(Piece piece)
        {
            Info info;
            if (piece.Color == PieceColor.White)
            {
                info = this._whiteInfo;
            }
            else
            {
                info = this._blackInfo;
            }

            info.RemovePiece(piece);

            _flipBoardHandler.RemoveChild(piece.GetComponent<FlipBoardHandler>());
            this[piece.File, piece.Rank] = null;
            Destroy(piece.gameObject);
        }

        public bool IsPositionAttacked(Files file, Ranks rank, PieceColor pieceColor, Piece ignorePiece = null)
        {
            Info info;
            if (pieceColor == PieceColor.White)
            {
                info = this._whiteInfo;
            }
            else
            {
                info = this._blackInfo;
            }

            foreach (var piecesByType in info.PiecesByType)
            {
                foreach (var piece in piecesByType.Value)
                {
                    if (piece == ignorePiece)
                    {
                        continue;
                    }

                    if (piece.Type == PieceTypes.King)
                    {
                        if (piece.GetPossibleMoves(ignoreSpecialMoves: true).Any(x => x.File == file && x.Rank == rank))
                        {
                            return true;
                        }
                    }
                    else if (piece.GetPossibleMoves().Any(x => x.File == file && x.Rank == rank))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public CastlingRights GetCastlingRights(PieceColor color)
        {
            Info info;
            if (color == PieceColor.White)
            {
                info = this._whiteInfo;
            }
            else
            {
                info = this._blackInfo;
            }

            return info.CastlingRights;
        }

        void ClearBoard()
        {
            _whiteInfo.Clear();
            _blackInfo.Clear();
            CurrentMove = PieceColor.White;
            _enPassantPawn = null;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (_pieces[i, j] == null)
                    {
                        continue;
                    }

                    _flipBoardHandler.RemoveChild(_pieces[i, j].GetComponent<FlipBoardHandler>());
                    GameObject.Destroy(_pieces[i, j].gameObject);
                    _pieces[i, j] = null;
                }
            }
        }
    }
}