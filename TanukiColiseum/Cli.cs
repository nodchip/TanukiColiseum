using System;
using System.Threading.Tasks;

namespace TanukiColiseum
{
    public class Cli
    {
        public void Run(Options options)
        {
            var coliseum = new Coliseum();
            coliseum.OnStatusChanged += ShowResult;
            coliseum.OnError += OnError;
            coliseum.Run(options);
        }

        private void ShowResult(Status status)
        {
            string engine1 = "engine1";
            string engine2 = "engine2";
            int numGames = status.NumGames;
            int blackWin = status.Win[0, 0] + status.Win[1, 0];
            int whiteWin = status.Win[0, 1] + status.Win[1, 1];
            int blackWinRatio = 100 * blackWin / (blackWin + whiteWin);
            int whiteWinRatio = 100 * whiteWin / (blackWin + whiteWin);
            int engine1DrawBlack = status.NumDraw[0];
            int engine2DrawBlack = status.NumDraw[1];
            int numDraw = engine1DrawBlack + engine2DrawBlack;
            int engine1Win = status.Win[0, 0] + status.Win[0, 1];
            int engine1BlackWin = status.Win[0, 0];
            int engine1WhiteWin = status.Win[0, 1];
            int engine2Win = status.Win[1, 0] + status.Win[1, 1];
            int engine2BlackWin = status.Win[1, 0];
            int engine2WhiteWin = status.Win[1, 1];
            int engine1WinRatio = 100 * engine1Win / (engine1Win + engine2Win);
            int engine2WinRatio = 100 * engine2Win / (engine1Win + engine2Win);
            int engine1BlackWinRatio = 100 * engine1BlackWin / (engine1Win + engine2Win);
            int engine1WhiteWinRatio = 100 * engine1WhiteWin / (engine1Win + engine2Win);
            int engine2BlackWinRatio = 100 * engine2BlackWin / (engine1Win + engine2Win);
            int engine2WhiteWinRatio = 100 * engine2WhiteWin / (engine1Win + engine2Win);
            int numFinishedGames = engine1Win + engine2Win + numDraw;
            int engine1DeclarationWinBlack = status.DeclarationWin[0, 0];
            int engine1DeclarationWinWhite = status.DeclarationWin[0, 1];
            int engine2DeclarationWinBlack = status.DeclarationWin[1, 0];
            int engine2DeclarationWinWhite = status.DeclarationWin[1, 1];

            double winRate = engine1Win / (double)(engine1Win + engine2Win);
            double rating = 0.0;
            if (1e-8 < winRate && winRate < 1.0 - 1e-8)
            {
                rating = -400.0 * Math.Log10((1.0 - winRate) / winRate);
            }

            Console.WriteLine(
                @"対局数{0} 先手勝ち{1}({2}%) 後手勝ち{3}({4}%) 引き分け{5}
{6}
勝ち{7}({8}% R{22:0.00}) 先手勝ち{9}({10}%) 後手勝ち{11}({12}%)
宣言勝ち{20} 先手宣言勝ち{23} 後手宣言勝ち{24}
先手引き分け{27} 後手引き分け{28}
{13}
勝ち{14}({15}%) 先手勝ち{16}({17}%) 後手勝ち{18}({19}%)
宣言勝ち{21} 先手宣言勝ち{25} 後手宣言勝ち{26}
先手引き分け{29} 後手引き分け{30}
{7},{5},{14}
",
                // 0-5
                numFinishedGames, blackWin, blackWinRatio, whiteWin, whiteWinRatio, numDraw,
                // 6
                engine1,
                // 7-12
                engine1Win, engine1WinRatio, engine1BlackWin, engine1BlackWinRatio, engine1WhiteWin, engine1WhiteWinRatio,
                // 13
                engine2,
                // 14-19
                engine2Win, engine2WinRatio, engine2BlackWin, engine2BlackWinRatio, engine2WhiteWin, engine2WhiteWinRatio,
                // 20-21
                engine1DeclarationWinBlack + engine1DeclarationWinWhite, engine2DeclarationWinBlack + engine2DeclarationWinWhite,
                // 22
                rating,
                // 23-26
                engine1DeclarationWinBlack, engine1DeclarationWinWhite, engine2DeclarationWinBlack, engine2DeclarationWinWhite,
                // 27-30
                engine1DrawBlack, engine2DrawBlack, engine2DrawBlack, engine1DrawBlack
                );
            Console.Out.Flush();
        }

        public void OnError(string errorMessage)
        {
            Console.Out.WriteLine(errorMessage);
            Console.Out.Flush();
        }
    }
}
