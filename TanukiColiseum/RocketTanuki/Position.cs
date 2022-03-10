using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static RocketTanuki.Types;
using static System.Math;

namespace RocketTanuki
{
    /// <summary>
    /// 局面を表すデータ構造
    /// </summary>
    public class Position
    {
        public const string StartposSfen = "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1";
        public const string MatsuriSfen = "l6nl/5+P1gk/2np1S3/p1p4Pp/3P2Sp1/1PPb2P1P/P5GS1/R8/LN4bKL w RGgsn5p 1";
        public const string MaxSfen = "8R/kSS1S1K2/4B4/9/9/9/9/9/3L1L1L1 b RBGSNLP3g3n17p 1";

        public const int BoardSize = 9;
        public Color SideToMove { get; set; }
        public Piece[,] Board { get; } = new Piece[BoardSize, BoardSize];
        public int[] HandPieces { get; } = new int[(int)Piece.NumPieces];
        public int Play { get; set; }
        public long Hash { get; set; }
        public int BlackKingFile { get; set; }
        public int BlackKingRank { get; set; }
        public int WhiteKingFile { get; set; }
        public int WhiteKingRank { get; set; }
        public PositionState State { get; set; }
        public Move LastMove { get; set; }

        public static void Initialize()
        {
        }

        /// <summary>
        /// 与えられた指し手に従い、局面を更新する。
        /// </summary>
        /// <param name="move"></param>
        public void DoMove(Move move)
        {
            Debug.Assert(SideToMove == move.SideToMove);
            Debug.Assert(move.Drop || Board[move.FileFrom, move.RankFrom] == move.PieceFrom);
            Debug.Assert(move.Drop || Board[move.FileTo, move.RankTo] == move.PieceTo);

            // 相手の駒を取る
            if (move.PieceTo != Piece.NoPiece)
            {
                RemovePiece(move.FileTo, move.RankTo);

                Debug.Assert(move.PieceTo.ToColor() != SideToMove);
                Debug.Assert(move.PieceTo.AsOpponentHandPiece().ToColor() == SideToMove);
                PutHandPiece(move.PieceTo.AsOpponentHandPiece());
            }

            if (move.Drop)
            {
                // 駒を打つ指し手
                Debug.Assert(move.PieceFrom.ToColor() == SideToMove);
                RemoveHandPiece(move.PieceFrom);
            }
            else
            {
                // 駒を移動する指し手
                RemovePiece(move.FileFrom, move.RankFrom);
            }

            PutPiece(move.FileTo, move.RankTo,
                move.Promotion
                ? move.PieceFrom.AsPromoted()
                : move.PieceFrom);
            Debug.Assert(Board[move.FileTo, move.RankTo].ToColor() == SideToMove);

            SideToMove = SideToMove.ToOpponent();
            Hash ^= Zobrist.Instance.Side;

            if (move.PieceFrom == Piece.BlackKing)
            {
                BlackKingFile = move.FileTo;
                BlackKingRank = move.RankTo;
            }
            else if (move.PieceFrom == Piece.WhiteKing)
            {
                WhiteKingFile = move.FileTo;
                WhiteKingRank = move.RankTo;
            }

            ++Play;

            LastMove = move;

            var newState = new PositionState();
            newState.Previous = State;
            State = newState;
        }

        /// <summary>
        /// 与えられた指し手に従い、局面を1手戻す。
        /// </summary>
        /// <param name="move"></param>
        public void UndoMove(Move move)
        {
            Debug.Assert(SideToMove != move.SideToMove);

            State = State.Previous;

            LastMove = null;

            --Play;

            Hash ^= Zobrist.Instance.Side;
            SideToMove = SideToMove.ToOpponent();

            if (move.PieceFrom == Piece.BlackKing)
            {
                BlackKingFile = move.FileFrom;
                BlackKingRank = move.RankFrom;
            }
            else if (move.PieceFrom == Piece.WhiteKing)
            {
                WhiteKingFile = move.FileFrom;
                WhiteKingRank = move.RankFrom;
            }

            RemovePiece(move.FileTo, move.RankTo);

            if (move.Drop)
            {
                // 駒を打つ指し手
                Debug.Assert(move.PieceFrom.ToColor() == SideToMove);
                PutHandPiece(move.PieceFrom);
            }
            else
            {
                // 駒を移動する指し手
                Debug.Assert(move.PieceFrom.ToColor() == SideToMove);
                PutPiece(move.FileFrom, move.RankFrom, move.PieceFrom);
            }

            // 相手の駒を取る
            if (move.PieceTo != Piece.NoPiece)
            {
                Debug.Assert(move.PieceTo.ToColor() != SideToMove);
                RemoveHandPiece(move.PieceTo.AsOpponentHandPiece());
                PutPiece(move.FileTo, move.RankTo, move.PieceTo);
            }
        }

        /// <summary>
        /// 盤面に駒を配置する
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        /// <param name="piece"></param>
        private void PutPiece(int file, int rank, Piece piece)
        {
            Debug.Assert(Board[file, rank] == Piece.NoPiece);
            Hash += Zobrist.Instance.PieceSquare[(int)piece, file, rank];
            Board[file, rank] = piece;
        }

        /// <summary>
        /// 盤面から駒を取り除く
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        private void RemovePiece(int file, int rank)
        {
            Debug.Assert(Board[file, rank] != Piece.NoPiece);
            Hash -= Zobrist.Instance.PieceSquare[(int)Board[file, rank], file, rank];
            Board[file, rank] = Piece.NoPiece;
        }

        /// <summary>
        /// 持ち駒に駒を加える
        /// </summary>
        /// <param name="piece"></param>
        private void PutHandPiece(Piece piece)
        {
            Hash += Zobrist.Instance.HandPiece[(int)piece];
            ++HandPieces[(int)piece];
        }

        /// <summary>
        /// 持ちがお魔から駒を取り除く
        /// </summary>
        /// <param name="piece"></param>
        private void RemoveHandPiece(Piece piece)
        {
            Debug.Assert(HandPieces[(int)piece] > 0);
            Hash -= Zobrist.Instance.HandPiece[(int)piece];
            --HandPieces[(int)piece];
        }

        /// <summary>
        /// sfen文字列をセットする
        /// </summary>
        /// <param name="sfen"></param>
        public void Set(string sfen)
        {
            SideToMove = Color.Black;
            Array.Clear(Board, 0, Board.Length);
            Array.Clear(HandPieces, 0, HandPieces.Length);
            Play = 1;
            Hash = 0;
            BlackKingFile = 0;
            BlackKingRank = 0;
            WhiteKingFile = 0;
            WhiteKingRank = 0;
            State = null;
            LastMove = null;

            // 盤面
            int file = BoardSize - 1;
            int rank = 0;
            int index = 0;
            bool promotion = false;
            while (true)
            {
                var ch = sfen[index++];
                if (ch == ' ')
                {
                    break;
                }
                else if (ch == '/')
                {
                    Debug.Assert(file == -1);
                    ++rank;
                    file = BoardSize - 1;
                }
                else if (ch == '+')
                {
                    promotion = true;
                }
                else if (Char.IsDigit(ch))
                {
                    int numNoPieces = ch - '0';
                    do
                    {
                        Board[file, rank] = Piece.NoPiece;
                        --file;
                    } while (--numNoPieces > 0);
                }
                else
                {
                    var piece = CharToPiece[ch];
                    Debug.Assert(piece != Piece.NoPiece);
                    if (promotion)
                    {
                        piece = piece.AsPromoted();
                        promotion = false;
                    }
                    Board[file, rank] = piece;
                    Hash += Zobrist.Instance.PieceSquare[(int)piece, file, rank];

                    if (piece == Piece.BlackKing)
                    {
                        BlackKingFile = file;
                        BlackKingRank = rank;
                    }
                    else if (piece == Piece.WhiteKing)
                    {
                        WhiteKingFile = file;
                        WhiteKingRank = rank;
                    }

                    --file;
                }
            }

            // 手番
            var sideToMove = sfen[index++];
            Debug.Assert(sideToMove == 'b' || sideToMove == 'w');
            if (sideToMove == 'b')
            {
                SideToMove = Color.Black;
            }
            else
            {
                SideToMove = Color.White;
                Hash ^= Zobrist.Instance.Side;
            }
            ++index;

            // 持ち駒
            for (int handPieceIndex = 0; handPieceIndex < (int)Piece.NumPieces; ++handPieceIndex)
            {
                HandPieces[handPieceIndex] = 0;
            }
            int numAddedHandPieces = 0;
            while (true)
            {
                var ch = sfen[index++];
                if (ch == ' ')
                {
                    break;
                }
                else if (ch == '-')
                {
                    continue;
                }
                else if (Char.IsDigit(ch))
                {
                    // 2桁の場合に対応する
                    numAddedHandPieces *= 10;
                    numAddedHandPieces += ch - '0';
                    continue;
                }

                var piece = CharToPiece[ch];
                Debug.Assert(piece != Piece.NoPiece);
                HandPieces[(int)piece] += Max(1, numAddedHandPieces);
                Hash += Zobrist.Instance.HandPiece[(int)piece];
                numAddedHandPieces = 0;
            }

            Play = int.Parse(sfen.Substring(index));

            State = new PositionState();
        }

        public String ToSfenString()
        {
            var writer = new StringWriter();

            // 盤面
            for (int rank = 0; rank < BoardSize; ++rank)
            {
                for (int file = 8; file >= 0; --file)
                {
                    int numNoPieces;
                    for (numNoPieces = 0; file >= 0 && Board[file, rank] == Piece.NoPiece; --file)
                    {
                        ++numNoPieces;
                    }

                    if (numNoPieces > 0)
                    {
                        writer.Write(numNoPieces);
                    }

                    if (file >= 0)
                    {
                        var piece = Board[file, rank];
                        if (piece != piece.AsNonPromotedPiece())
                        {
                            writer.Write('+');
                        }
                        piece = piece.AsNonPromotedPiece();
                        writer.Write(piece.ToUsiChar());
                    }
                }

                if (rank < 8)
                {
                    writer.Write("/");
                }
            }

            // 手番
            writer.Write(SideToMove == Color.Black ? " b " : " w ");

            // 持ち駒
            bool handPieceExists = false;
            foreach (var handPiece in HandPieceTypes)
            {
                if (HandPieces[(int)handPiece] == 0)
                {
                    continue;
                }
                else if (HandPieces[(int)handPiece] > 1)
                {
                    writer.Write(HandPieces[(int)handPiece]);
                }
                writer.Write(handPiece.ToUsiChar());
                handPieceExists = true;
            }
            if (!handPieceExists)
            {
                writer.Write("-");
            }

            //手数
            writer.Write(" ");
            writer.Write(Play);

            return writer.ToString();
        }

        /// <summary>
        /// 局面を文字列化する。sfenではなく独自形式とする。
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            var writer = new StringWriter();
            writer.WriteLine("+----+----+----+----+----+----+----+----+----+");
            for (int rank = 0; rank < BoardSize; ++rank)
            {
                writer.Write("|");
                for (int file = BoardSize - 1; file >= 0; --file)
                {
                    writer.Write(Board[file, rank].ToHumanReadableString());
                    writer.Write("|");
                }
                writer.WriteLine();

                writer.WriteLine("+----+----+----+----+----+----+----+----+----+");
            }

            writer.Write("先手 手駒 : ");
            for (var piece = Piece.BlackPawn; piece < Piece.WhitePawn; ++piece)
            {
                for (int i = 0; i < HandPieces[(int)piece]; ++i)
                {
                    writer.Write(piece.ToHumanReadableString().Trim()[0]);
                }
            }

            writer.Write(" , 後手 手駒 : ");
            for (var piece = Piece.WhitePawn; piece < Piece.NumPieces; ++piece)
            {
                for (int i = 0; i < HandPieces[(int)piece]; ++i)
                {
                    writer.Write(piece.ToHumanReadableString().Trim()[0]);
                }
            }
            writer.WriteLine();

            writer.Write("手番 = ");
            writer.Write(SideToMove == Color.Black ? "先手" : "後手");
            writer.WriteLine();

            return writer.ToString();
        }

        public bool IsChecked(Color color)
        {
            var king = color == Color.Black ? Piece.BlackKing : Piece.WhiteKing;

            // 駒を移動する指し手
            for (int fileFrom = 0; fileFrom < Position.BoardSize; ++fileFrom)
            {
                for (int rankFrom = 0; rankFrom < Position.BoardSize; ++rankFrom)
                {
                    var pieceFrom = Board[fileFrom, rankFrom];
                    if (pieceFrom == Piece.NoPiece)
                    {
                        // 駒が置かれていないので何もしない
                        continue;
                    }

                    if (pieceFrom.ToColor() == color)
                    {
                        // 味方の駒なので何もしない
                        continue;
                    }

                    foreach (var moveDirection in MoveDirections[(int)pieceFrom])
                    {
                        int maxDistance = moveDirection.Long ? 8 : 1;
                        int fileTo = fileFrom;
                        int rankTo = rankFrom;
                        for (int distance = 0; distance < maxDistance; ++distance)
                        {
                            fileTo += moveDirection.Direction.DeltaFile;
                            rankTo += moveDirection.Direction.DeltaRank;

                            if (fileTo < 0 || Position.BoardSize <= fileTo || rankTo < 0 || Position.BoardSize <= rankTo)
                            {
                                // 盤外
                                continue;
                            }

                            var pieceTo = Board[fileTo, rankTo];
                            if (pieceTo == king)
                            {
                                // 味方の玉に利きを持っている
                                return true;
                            }

                            if (pieceTo != Piece.NoPiece)
                            {
                                // 味方の玉以外の駒がある
                                break;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void FindPiece(Piece piece, out int file, out int rank)
        {
            for (int f = 0; f < BoardSize; ++f)
            {
                for (int r = 0; r < BoardSize; ++r)
                {
                    if (Board[f, r] == piece)
                    {
                        file = f;
                        rank = r;
                        return;
                    }
                }
            }

            throw new Exception($"Piece not found. piece={piece}");
        }

        /// <summary>
        /// 与えられた指し手を、現局面で指すことができるかどうか返す。
        /// 置換表の指し手を確認することが主目的の為、細かいチェックはしていない。
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public bool IsValid(Move move)
        {
            if (move.Drop)
            {
                if (HandPieces[(int)move.PieceFrom] == 0)
                {
                    return false;
                }
            }
            else
            {
                if (Board[move.FileFrom, move.RankFrom] != move.PieceFrom)
                {
                    return false;
                }
            }

            if (Board[move.FileTo, move.RankTo] != move.PieceTo)
            {
                return false;
            }

            if (move.SideToMove != SideToMove)
            {
                return false;
            }

            return true;
        }

        private static Piece[] HandPieceTypes = new[] {
                Piece.BlackRook, Piece.BlackBishop, Piece.BlackGold,Piece.BlackSilver,Piece.BlackKnight,Piece.BlackLance,Piece.BlackPawn,
                Piece.WhiteRook, Piece.WhiteBishop, Piece.WhiteGold,Piece.WhiteSilver,Piece.WhiteKnight,Piece.WhiteLance,Piece.WhitePawn,
            };
    }

    public class PositionState
    {
        // 一つ前の局面の局面情報
        // Rootノードの場合はnull
        public PositionState Previous { get; set; }
        public short[] Z1Black { get; set; }
        public short[] Z1White { get; set; }
    }
}
