using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RocketTanuki
{
    public enum Color
    {
        Black,
        White,
        NumColors,
    }

    public enum Piece
    {
        NoPiece,
        BlackPawn,
        BlackLance,
        BlackKnight,
        BlackSilver,
        BlackGold,
        BlackBishop,
        BlackRook,
        BlackKing,
        BlackPromotedPawn,
        BlackPromotedLance,
        BlackPromotedKnight,
        BlackPromotedSilver,
        BlackHorse,
        BlackDragon,
        WhitePawn,
        WhiteLance,
        WhiteKnight,
        WhiteSilver,
        WhiteGold,
        WhiteBishop,
        WhiteRook,
        WhiteKing,
        WhitePromotedPawn,
        WhitePromotedLance,
        WhitePromotedKnight,
        WhitePromotedSilver,
        WhiteHorse,
        WhiteDragon,
        NumPieces,
    }

    public class Direction
    {
        public int DeltaFile { get; set; }
        public int DeltaRank { get; set; }
    }

    public class MoveDirection
    {
        public Direction Direction { get; set; }
        public bool Long { get; set; } = false;
    }

    /// <summary>
    /// 各種型のユーティリティ関数
    /// </summary>
    public static class Types
    {
        private static Direction UpLeft = new Direction { DeltaFile = +1, DeltaRank = -1 };
        private static Direction Up = new Direction { DeltaFile = 0, DeltaRank = -1 };
        private static Direction UpRight = new Direction { DeltaFile = -1, DeltaRank = -1 };
        private static Direction Left = new Direction { DeltaFile = +1, DeltaRank = 0 };
        private static Direction Right = new Direction { DeltaFile = -1, DeltaRank = 0 };
        private static Direction DownLeft = new Direction { DeltaFile = +1, DeltaRank = +1 };
        private static Direction Down = new Direction { DeltaFile = 0, DeltaRank = +1 };
        private static Direction DownRight = new Direction { DeltaFile = -1, DeltaRank = +1 };

        public static List<MoveDirection>[] MoveDirections = {
            // NoPiece
            null,
            // BlackPawn
            new List<MoveDirection>{
                new MoveDirection{Direction=Up},
            },
            // BlackLance
            new List<MoveDirection>{
                new MoveDirection{Direction=Up,Long=true},
            },
            // BlackKnight
            new List<MoveDirection>{
                new MoveDirection{Direction=new Direction{DeltaRank=-2,DeltaFile=+1}},
                new MoveDirection{Direction=new Direction{DeltaRank=-2,DeltaFile=-1}},
            },
            // BlackSilver
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=DownRight},
            },
            // BlackGold
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=Down},
            },
            // BlackBishop
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft,Long=true},
                new MoveDirection{Direction=UpRight,Long=true},
                new MoveDirection{Direction=DownLeft,Long=true},
                new MoveDirection{Direction=DownRight,Long=true},
            },
            // BlackRook
            new List<MoveDirection>{
                new MoveDirection{Direction=Up,Long=true},
                new MoveDirection{Direction=Left,Long=true},
                new MoveDirection{Direction=Right,Long=true},
                new MoveDirection{Direction=Down,Long=true},
            },
            // BlackKing
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight},
            },
            // BlackPromotedPawn
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=Down},
            },
            // BlackPromotedLance
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=Down},
            },
            // BlackPromotedKnight
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=Down},
            },
            // BlackPromotedSilver
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=Down},
            },
            // BlackHorse
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft,Long=true},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight,Long=true},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft,Long=true},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight,Long=true},
            },
            // BlackDragon
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up,Long=true},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left,Long=true},
                new MoveDirection{Direction=Right,Long=true},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down,Long=true},
                new MoveDirection{Direction=DownRight},
            },
            // WhitePawn
            new List<MoveDirection>{
                new MoveDirection{Direction=Down},
            },
            // WhiteLance
            new List<MoveDirection>{
                new MoveDirection{Direction=Down,Long=true},
            },
            // WhiteKnight
            new List<MoveDirection>{
                new MoveDirection{Direction=new Direction{DeltaRank=2,DeltaFile=+1}},
                new MoveDirection{Direction=new Direction{DeltaRank=2,DeltaFile=-1}},
            },
            // WhiteSilver
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight},
            },
            // WhiteGold
            new List<MoveDirection>{
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight},
            },
            // WhiteBishop
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft,Long=true},
                new MoveDirection{Direction=UpRight,Long=true},
                new MoveDirection{Direction=DownLeft,Long=true},
                new MoveDirection{Direction=DownRight,Long=true},
            },
            // WhiteRook
            new List<MoveDirection>{
                new MoveDirection{Direction=Up,Long=true},
                new MoveDirection{Direction=Left,Long=true},
                new MoveDirection{Direction=Right,Long=true},
                new MoveDirection{Direction=Down,Long=true},
            },
            // WhiteKing
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight},
            },
            // WhitePromotedPawn
            new List<MoveDirection>{
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight},
            },
            // WhitePromotedLance
            new List<MoveDirection>{
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight},
            },
            // WhitePromotedKnight
            new List<MoveDirection>{
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight},
            },
            // WhitePromotedSilver
            new List<MoveDirection>{
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight},
            },
            // WhiteHorse
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft,Long=true},
                new MoveDirection{Direction=Up},
                new MoveDirection{Direction=UpRight,Long=true},
                new MoveDirection{Direction=Left},
                new MoveDirection{Direction=Right},
                new MoveDirection{Direction=DownLeft,Long=true},
                new MoveDirection{Direction=Down},
                new MoveDirection{Direction=DownRight,Long=true},
            },
            // WhiteDragon
            new List<MoveDirection>{
                new MoveDirection{Direction=UpLeft},
                new MoveDirection{Direction=Up,Long=true},
                new MoveDirection{Direction=UpRight},
                new MoveDirection{Direction=Left,Long=true},
                new MoveDirection{Direction=Right,Long=true},
                new MoveDirection{Direction=DownLeft},
                new MoveDirection{Direction=Down,Long=true},
                new MoveDirection{Direction=DownRight},
            },
            // NumPieces
            null,
        };

        public static void Initialize()
        {
            NonPromotedToPromoted[(int)Piece.BlackPawn] = Piece.BlackPromotedPawn;
            NonPromotedToPromoted[(int)Piece.BlackLance] = Piece.BlackPromotedLance;
            NonPromotedToPromoted[(int)Piece.BlackKnight] = Piece.BlackPromotedKnight;
            NonPromotedToPromoted[(int)Piece.BlackSilver] = Piece.BlackPromotedSilver;
            NonPromotedToPromoted[(int)Piece.BlackBishop] = Piece.BlackHorse;
            NonPromotedToPromoted[(int)Piece.BlackRook] = Piece.BlackDragon;
            NonPromotedToPromoted[(int)Piece.WhitePawn] = Piece.WhitePromotedPawn;
            NonPromotedToPromoted[(int)Piece.WhiteLance] = Piece.WhitePromotedLance;
            NonPromotedToPromoted[(int)Piece.WhiteKnight] = Piece.WhitePromotedKnight;
            NonPromotedToPromoted[(int)Piece.WhiteSilver] = Piece.WhitePromotedSilver;
            NonPromotedToPromoted[(int)Piece.WhiteBishop] = Piece.WhiteHorse;
            NonPromotedToPromoted[(int)Piece.WhiteRook] = Piece.WhiteDragon;

            CharToPiece['K'] = Piece.BlackKing;
            CharToPiece['k'] = Piece.WhiteKing;
            CharToPiece['R'] = Piece.BlackRook;
            CharToPiece['r'] = Piece.WhiteRook;
            CharToPiece['B'] = Piece.BlackBishop;
            CharToPiece['b'] = Piece.WhiteBishop;
            CharToPiece['G'] = Piece.BlackGold;
            CharToPiece['g'] = Piece.WhiteGold;
            CharToPiece['S'] = Piece.BlackSilver;
            CharToPiece['s'] = Piece.WhiteSilver;
            CharToPiece['N'] = Piece.BlackKnight;
            CharToPiece['n'] = Piece.WhiteKnight;
            CharToPiece['L'] = Piece.BlackLance;
            CharToPiece['l'] = Piece.WhiteLance;
            CharToPiece['P'] = Piece.BlackPawn;
            CharToPiece['p'] = Piece.WhitePawn;

            PieceToChar[(int)Piece.BlackKing] = 'K';
            PieceToChar[(int)Piece.WhiteKing] = 'k';
            PieceToChar[(int)Piece.BlackRook] = 'R';
            PieceToChar[(int)Piece.WhiteRook] = 'r';
            PieceToChar[(int)Piece.BlackBishop] = 'B';
            PieceToChar[(int)Piece.WhiteBishop] = 'b';
            PieceToChar[(int)Piece.BlackGold] = 'G';
            PieceToChar[(int)Piece.WhiteGold] = 'g';
            PieceToChar[(int)Piece.BlackSilver] = 'S';
            PieceToChar[(int)Piece.WhiteSilver] = 's';
            PieceToChar[(int)Piece.BlackKnight] = 'N';
            PieceToChar[(int)Piece.WhiteKnight] = 'n';
            PieceToChar[(int)Piece.BlackLance] = 'L';
            PieceToChar[(int)Piece.WhiteLance] = 'l';
            PieceToChar[(int)Piece.BlackPawn] = 'P';
            PieceToChar[(int)Piece.WhitePawn] = 'p';
        }

        /// <summary>
        /// 相手側の手番へと変換する。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToOpponent(this Color color)
        {
            return color == Color.Black ? Color.White : Color.Black;
        }

        /// <summary>
        /// 人間にとって読みやすい文字列へ変換する。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToHumanReadableString(this Color color)
        {
            return color == Color.Black ? "☗" : "☖";
        }

        /// <summary>
        /// 与えられた駒の種類を、成り駒の種類に変換する。
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Piece AsPromoted(this Piece piece)
        {
            Debug.Assert(NonPromotedToPromoted[(int)piece] != Piece.NoPiece);
            return NonPromotedToPromoted[(int)piece];
        }

        /// <summary>
        /// 成ることができる駒かどうかを判定する。
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static bool CanPromote(this Piece piece)
        {
            return NonPromotedToPromoted[(int)piece] != Piece.NoPiece;
        }

        /// <summary>
        /// 与えられた駒の種類を、先手・後手に変換する。
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Color ToColor(this Piece piece)
        {
            Debug.Assert(Piece.BlackPawn <= piece && piece < Piece.NumPieces);
            return piece < Piece.WhitePawn ? Color.Black : Color.White;
        }

        /// <summary>
        /// 相手の持ち駒に加わったときの駒の種類を返す。
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Piece AsOpponentHandPiece(this Piece piece)
        {
            Debug.Assert(PieceToOpponentHandPieces[(int)piece] != Piece.NoPiece);
            return PieceToOpponentHandPieces[(int)piece];
        }

        /// <summary>
        /// 相手の駒に変換する。
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Piece AsOpponentPiece(this Piece piece)
        {
            Debug.Assert(PieceToOpponentHandPieces[(int)piece] != Piece.NoPiece);
            return PieceToOpponentPieces[(int)piece];
        }

        /// <summary>
        /// 不成の状態の駒に変換する。
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Piece AsNonPromotedPiece(this Piece piece)
        {
            Debug.Assert(PieceToNonPromotedPieces[(int)piece] != Piece.NoPiece);
            return PieceToNonPromotedPieces[(int)piece];
        }

        /// <summary>
        /// USIで使用される文字へ変換する。
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static char ToUsiChar(this Piece piece)
        {
            Debug.Assert(PieceToChar[(int)piece] != '\0');
            return PieceToChar[(int)piece];
        }

        /// <summary>
        /// 人間が読める文字列に変換する。USIとの互換性はない。
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static string ToHumanReadableString(this Piece piece)
        {
            Debug.Assert(PieceToString[(int)piece] != null);
            return PieceToString[(int)piece];
        }

        private static Piece[] NonPromotedToPromoted = new Piece[(int)Piece.NumPieces];

        private static Piece[] PieceToOpponentHandPieces = {
            Piece.NoPiece,
            Piece.WhitePawn,
            Piece.WhiteLance,
            Piece.WhiteKnight,
            Piece.WhiteSilver,
            Piece.WhiteGold,
            Piece.WhiteBishop,
            Piece.WhiteRook,
            Piece.NoPiece,
            Piece.WhitePawn,
            Piece.WhiteLance,
            Piece.WhiteKnight,
            Piece.WhiteSilver,
            Piece.WhiteBishop,
            Piece.WhiteRook,
            Piece.BlackPawn,
            Piece.BlackLance,
            Piece.BlackKnight,
            Piece.BlackSilver,
            Piece.BlackGold,
            Piece.BlackBishop,
            Piece.BlackRook,
            Piece.NoPiece,
            Piece.BlackPawn,
            Piece.BlackLance,
            Piece.BlackKnight,
            Piece.BlackSilver,
            Piece.BlackBishop,
            Piece.BlackRook,
            Piece.NumPieces,
        };

        private static Piece[] PieceToOpponentPieces = {
            Piece.NoPiece,
            Piece.WhitePawn,
            Piece.WhiteLance,
            Piece.WhiteKnight,
            Piece.WhiteSilver,
            Piece.WhiteGold,
            Piece.WhiteBishop,
            Piece.WhiteRook,
            Piece.WhiteKing,
            Piece.WhitePromotedPawn,
            Piece.WhitePromotedLance,
            Piece.WhitePromotedKnight,
            Piece.WhitePromotedSilver,
            Piece.WhiteHorse,
            Piece.WhiteDragon,
            Piece.BlackPawn,
            Piece.BlackLance,
            Piece.BlackKnight,
            Piece.BlackSilver,
            Piece.BlackGold,
            Piece.BlackBishop,
            Piece.BlackRook,
            Piece.BlackKing,
            Piece.BlackPromotedPawn,
            Piece.BlackPromotedLance,
            Piece.BlackPromotedKnight,
            Piece.BlackPromotedSilver,
            Piece.BlackHorse,
            Piece.BlackDragon,
            Piece.NumPieces,
        };

        private static Piece[] PieceToNonPromotedPieces = {
            Piece.NoPiece,
            Piece.BlackPawn,
            Piece.BlackLance,
            Piece.BlackKnight,
            Piece.BlackSilver,
            Piece.BlackGold,
            Piece.BlackBishop,
            Piece.BlackRook,
            Piece.BlackKing,
            Piece.BlackPawn,
            Piece.BlackLance,
            Piece.BlackKnight,
            Piece.BlackSilver,
            Piece.BlackBishop,
            Piece.BlackRook,
            Piece.WhitePawn,
            Piece.WhiteLance,
            Piece.WhiteKnight,
            Piece.WhiteSilver,
            Piece.WhiteGold,
            Piece.WhiteBishop,
            Piece.WhiteRook,
            Piece.WhiteKing,
            Piece.WhitePawn,
            Piece.WhiteLance,
            Piece.WhiteKnight,
            Piece.WhiteSilver,
            Piece.WhiteBishop,
            Piece.WhiteRook,
            Piece.NumPieces,
        };

        public static Piece[] CharToPiece { get; } = new Piece[128];

        private static char[] PieceToChar { get; } = new char[(int)Piece.NumPieces];

        private static String[] PieceToString { get; } = {
            "　　",
            " 歩 ",
            " 香 ",
            " 桂 ",
            " 銀 ",
            " 金 ",
            " 角 ",
            " 飛 ",
            " 王 ",
            " と ",
            " 杏 ",
            " 圭 ",
            " 全 ",
            " 馬 ",
            " 龍 ",
            "歩↓",
            "香↓",
            "桂↓",
            "銀↓",
            "金↓",
            "角↓",
            "飛↓",
            "王↓",
            "と↓",
            "杏↓",
            "圭↓",
            "全↓",
            "馬↓",
            "龍↓",
            null,
        };
    }
}
