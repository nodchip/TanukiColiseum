using System;

namespace TanukiColiseum
{
    public class Status
    {
        public int NumGames { get; set; }
        public int NumThreads { get; set; }
        public int[] Nodes { get; set; }
        public int[] Times { get; set; }
        // [エンジン][先手・後手]
        public int[,] Win { get; } = { { 0, 0 }, { 0, 0 }, };
        // [エンジン][先手・後手]
        public int[,] DeclarationWin { get; } = { { 0, 0 }, { 0, 0 } };
        // NumDraw[i] = エンジンiが先手の時の引き分けの回数
        public int[] NumDraw { get; } = { 0, 0 };

        public int NumFinishedGames
        {
            get
            {
                int engine1Win = Win[0, 0] + Win[0, 1];
                int engine2Win = Win[1, 0] + Win[1, 1];
                int engine1DrawBlack = NumDraw[0];
                int engine2DrawBlack = NumDraw[1];
                int numDraw = engine1DrawBlack + engine2DrawBlack;
                int numFinishedGames = engine1Win + engine2Win + numDraw;
                return numFinishedGames;
            }
        }

        public Status() { }

        public Status(Status status)
        {
            NumGames = status.NumGames;
            NumThreads = status.NumThreads;
            Nodes = status.Nodes;
            Times = status.Times;
            for (int engine = 0; engine < 2; ++engine)
            {
                for (int blackWhite = 0; blackWhite < 2; ++blackWhite)
                {
                    Win[engine, blackWhite] = status.Win[engine, blackWhite];
                    DeclarationWin[engine, blackWhite] = status.DeclarationWin[engine, blackWhite];
                }
                NumDraw[engine] = status.NumDraw[engine];
            }
        }

        public string ToHumanReadableString()
        {
            string engine1 = "engine1";
            string engine2 = "engine2";
            int numGames = NumGames;
            int blackWin = Win[0, 0] + Win[1, 0];
            int whiteWin = Win[0, 1] + Win[1, 1];
            double blackWinRatio = 100.0 * blackWin / (blackWin + whiteWin);
            double whiteWinRatio = 100.0 * whiteWin / (blackWin + whiteWin);
            int engine1DrawBlack = NumDraw[0];
            int engine2DrawBlack = NumDraw[1];
            int numDraw = engine1DrawBlack + engine2DrawBlack;
            int engine1Win = Win[0, 0] + Win[0, 1];
            int engine1BlackWin = Win[0, 0];
            int engine1WhiteWin = Win[0, 1];
            int engine2Win = Win[1, 0] + Win[1, 1];
            int engine2BlackWin = Win[1, 0];
            int engine2WhiteWin = Win[1, 1];
            double engine1WinRatio = 100.0 * engine1Win / (engine1Win + engine2Win);
            double engine2WinRatio = 100.0 * engine2Win / (engine1Win + engine2Win);
            double engine1BlackWinRatio = 100.0 * engine1BlackWin / (engine1Win + engine2Win);
            double engine1WhiteWinRatio = 100.0 * engine1WhiteWin / (engine1Win + engine2Win);
            double engine2BlackWinRatio = 100.0 * engine2BlackWin / (engine1Win + engine2Win);
            double engine2WhiteWinRatio = 100.0 * engine2WhiteWin / (engine1Win + engine2Win);
            int numFinishedGames = engine1Win + engine2Win + numDraw;
            int engine1DeclarationWinBlack = DeclarationWin[0, 0];
            int engine1DeclarationWinWhite = DeclarationWin[0, 1];
            int engine2DeclarationWinBlack = DeclarationWin[1, 0];
            int engine2DeclarationWinWhite = DeclarationWin[1, 1];
            int engine1DeclarationWin = engine1DeclarationWinBlack + engine1DeclarationWinWhite;
            int engine2DeclarationWin = engine2DeclarationWinBlack + engine2DeclarationWinWhite;

            double winRate = (engine1Win + numDraw * 0.5) / numFinishedGames;
            double rating = 0.0;
            double confidenceInterval = 0.0;
            if (1e-8 < winRate && winRate < 1.0 - 1e-8)
            {
                rating = -400.0 * Math.Log10((1.0 - winRate) / winRate);
                // 自己対戦及び連続対戦の誤差論　１：標準誤差と信頼区間 : コンピュータ将棋基礎情報研究所 http://lfics81.techblog.jp/archives/2982884.html
                double s = Math.Sqrt(numFinishedGames / (numFinishedGames - 1.5) * winRate * (1.0 - winRate));
                double S = s / Math.Sqrt(numFinishedGames);
                confidenceInterval = 1.96 * 400.0 / Math.Log(10.0) * S / (winRate * (1.0 - winRate));
            }

            return $@"numFinishedGames={numFinishedGames} blackWin={blackWin}({blackWinRatio:0.0}%) whiteWin={whiteWin}({whiteWinRatio:0.0}%) numDraw={numDraw}
{engine1}
engine1Win={engine1Win}({engine1WinRatio:0.0}% R{rating:0.0} +-{confidenceInterval:0.0}) engine1BlackWin={engine1BlackWin}({engine1BlackWinRatio:0.0}%) engine1WhiteWin={engine1WhiteWin}({engine1WhiteWinRatio:0.0}%)
engine1DeclarationWin={engine1DeclarationWin} engine1DeclarationWinBlack={engine1DeclarationWinBlack} engine1DeclarationWinWhite={engine1DeclarationWinWhite} engine1DrawBlack={engine1DrawBlack} engine2DrawBlack={engine2DrawBlack}
{engine2}
engine2Win={engine2Win}({engine2WinRatio:0.0}%) engine2BlackWin={engine2BlackWin}({engine2BlackWinRatio:0.0}%) engine2WhiteWin={engine2WhiteWin}({engine2WhiteWinRatio:0.0}%)
engine2DeclarationWin={engine2DeclarationWin} engine2DeclarationWinBlack={engine2DeclarationWinBlack} engine2DeclarationWinWhite={engine2DeclarationWinWhite} engine2DrawBlack={engine2DrawBlack} engine1DrawBlack={engine1DrawBlack}
{engine1Win},{numDraw},{engine2Win}
";
        }
    }
}
