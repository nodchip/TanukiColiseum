using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RocketTanuki.Types;

namespace RocketTanuki
{
    /// <summary>
    /// 指し手を表すデータ構造
    /// </summary>
    public class Move
    {
        public int FileFrom { get; set; }
        public int RankFrom { get; set; }
        public Piece PieceFrom { get; set; }
        public int FileTo { get; set; }
        public int RankTo { get; set; }
        public Piece PieceTo { get; set; }
        public bool Drop { get; set; }
        public bool Promotion { get; set; }
        public Color SideToMove { get; set; }

        public override string ToString()
        {
            return $"{SideToMove.ToHumanReadableString()}{(char)('１' + FileTo)}{RankToKanjiLetters[RankTo]}{PieceFrom.ToHumanReadableString().Trim()[0]}{(Promotion ? "成" : "")}";
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var rh = (Move)obj;
            return FileFrom == rh.FileFrom
                && RankFrom == rh.RankFrom
                && PieceFrom == rh.PieceFrom
                && FileTo == rh.FileTo
                && RankTo == rh.RankTo
                && PieceTo == rh.PieceTo
                && Drop == rh.Drop
                && Promotion == rh.Promotion
                && SideToMove == rh.SideToMove;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // boost/container_hash/hash.hpp - 1.74.0 https://www.boost.org/doc/libs/1_74_0/boost/container_hash/hash.hpp
            int seed = 0;
            seed ^= FileFrom + (seed << 6) + (seed >> 2);
            seed ^= RankFrom + (seed << 6) + (seed >> 2);
            seed ^= (int)PieceFrom + (seed << 6) + (seed >> 2);
            seed ^= FileTo + (seed << 6) + (seed >> 2);
            seed ^= RankTo + (seed << 6) + (seed >> 2);
            seed ^= (int)PieceTo + (seed << 6) + (seed >> 2);
            seed ^= Convert.ToInt32(Drop) + (seed << 6) + (seed >> 2);
            seed ^= Convert.ToInt32(Promotion) + (seed << 6) + (seed >> 2);
            seed ^= (int)SideToMove + (seed << 6) + (seed >> 2);
            return seed;
        }

        public string ToUsiString()
        {
            if (this == Resign)
            {
                return "resign";
            }
            else if (this == Win)
            {
                return "win";
            }

            string usiString = "";
            if (Drop)
            {
                usiString += char.ToUpper(PieceFrom.ToUsiChar());
                usiString += "*";
            }
            else
            {
                usiString += (char)(FileFrom + '1');
                usiString += (char)(RankFrom + 'a');
            }

            usiString += (char)(FileTo + '1');
            usiString += (char)(RankTo + 'a');

            if (Promotion)
            {
                usiString += "+";
            }

            return usiString;
        }

        public static Move FromUsiString(Position position, string moveString)
        {
            if (moveString == "resign")
            {
                return Move.Resign;
            }
            else if (moveString == "win")
            {
                return Move.Win;
            }
            else if (moveString == "none")
            {
                return Move.None;
            }

            var move = new Move();
            if (moveString[1] == '*')
            {
                // 駒打ちの指し手
                move.FileFrom = -1;
                move.RankFrom = -1;
                move.PieceFrom = CharToPiece[moveString[0]];
                if (position.SideToMove == Color.White)
                {
                    move.PieceFrom = move.PieceFrom.AsOpponentHandPiece();
                }
                move.Drop = true;
            }
            else
            {
                // 駒を移動する指し手
                move.FileFrom = moveString[0] - '1';
                move.RankFrom = moveString[1] - 'a';
                move.PieceFrom = position.Board[move.FileFrom, move.RankFrom];
                move.Drop = false;
            }

            move.FileTo = moveString[2] - '1';
            move.RankTo = moveString[3] - 'a';
            move.PieceTo = position.Board[move.FileTo, move.RankTo];

            move.Promotion = moveString.Length == 5;
            move.SideToMove = position.SideToMove;
            return move;
        }

        /// <summary>
        /// 16ビット整数形式に変換する。
        /// </summary>
        /// <returns></returns>
        public ushort ToUshort()
        {
            if (this == Resign)
            {
                return Resign16;
            }
            else if (this == Win)
            {
                return Win16;
            }
            else if (this == None)
            {
                return None16;
            }

            // 0...6ビット目: 移動先のマス
            // 7...13ビット目: 駒を移動する指し手の場合は移動元のマス、駒を打つ指し手の場合はPiece
            // 14ビット目: 駒を移動する指し手の場合は0、駒を打つ指し手の場合は1
            // 15ビット目: 駒を成る指し手の場合は1、それ以外は0
            int to = FileTo + RankTo * Position.BoardSize;
            int from = Drop ? (int)PieceFrom : FileFrom + RankFrom * Position.BoardSize;
            int drop = Drop ? 1 : 0;
            int promotion = Promotion ? 1 : 0;
            return (ushort)(to | (from << 7) | (drop << 14) | (promotion << 15));
        }

        /// <summary>
        /// 16ビット整数形式から復元する。
        /// </summary>
        /// <param name="position"></param>
        /// <param name="move16"></param>
        /// <returns></returns>
        public static Move FromUshort(Position position, ushort move16)
        {
            if (move16 == Resign16)
            {
                return Resign;
            }
            else if (move16 == Win16)
            {
                return Win;
            }
            else if (move16 == None16)
            {
                return None;
            }

            int to = move16 & ((1 << 7) - 1);
            int from = (move16 >> 7) & ((1 << 7) - 1);
            int drop = (move16 >> 14) & 1;
            int promotion = (move16 >> 15) & 1;
            return new Move
            {
                FileFrom = drop == 1 ? -1 : from % 9,
                RankFrom = drop == 1 ? -1 : from / 9,
                PieceFrom = drop == 1 ? (Piece)from : position.Board[from % 9, from / 9],
                FileTo = to % 9,
                RankTo = to / 9,
                PieceTo = position.Board[to % 9, to / 9],
                Drop = drop == 1,
                Promotion = promotion == 1,
                SideToMove = position.SideToMove,
            };
        }

        public static Move Resign = new Move
        {
            FileFrom = 2,
            FileTo = 2,
        };

        public static Move Win = new Move
        {
            FileFrom = 3,
            FileTo = 3,
        };

        public static Move None = new Move
        {
            FileFrom = 4,
            FileTo = 4,
        };

        private const ushort Null16 = (1 << 7) + 1;
        private const ushort Resign16 = (2 << 7) + 2;
        private const ushort Win16 = (3 << 7) + 3;
        private const ushort None16 = (4 << 7) + 4;

        private static string[] RankToKanjiLetters = { "一", "二", "三", "四", "五", "六", "七", "八", "九" };
    }
}
