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
            double blackWinRatio = 100.0 * blackWin / (blackWin + whiteWin);
            double whiteWinRatio = 100.0 * whiteWin / (blackWin + whiteWin);
            int engine1DrawBlack = status.NumDraw[0];
            int engine2DrawBlack = status.NumDraw[1];
            int numDraw = engine1DrawBlack + engine2DrawBlack;
            int engine1Win = status.Win[0, 0] + status.Win[0, 1];
            int engine1BlackWin = status.Win[0, 0];
            int engine1WhiteWin = status.Win[0, 1];
            int engine2Win = status.Win[1, 0] + status.Win[1, 1];
            int engine2BlackWin = status.Win[1, 0];
            int engine2WhiteWin = status.Win[1, 1];
            double engine1WinRatio = 100.0 * engine1Win / (engine1Win + engine2Win);
            double engine2WinRatio = 100.0 * engine2Win / (engine1Win + engine2Win);
            double engine1BlackWinRatio = 100.0 * engine1BlackWin / (engine1Win + engine2Win);
            double engine1WhiteWinRatio = 100.0 * engine1WhiteWin / (engine1Win + engine2Win);
            double engine2BlackWinRatio = 100.0 * engine2BlackWin / (engine1Win + engine2Win);
            double engine2WhiteWinRatio = 100.0 * engine2WhiteWin / (engine1Win + engine2Win);
            int numFinishedGames = engine1Win + engine2Win + numDraw;
            int engine1DeclarationWinBlack = status.DeclarationWin[0, 0];
            int engine1DeclarationWinWhite = status.DeclarationWin[0, 1];
            int engine2DeclarationWinBlack = status.DeclarationWin[1, 0];
            int engine2DeclarationWinWhite = status.DeclarationWin[1, 1];

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

            Console.WriteLine(
                $@"対局数{numFinishedGames} 先手勝ち{blackWin}({blackWinRatio:0.0}%) 後手勝ち{whiteWin}({whiteWinRatio:0.0}%) 引き分け{numDraw}
{engine1}
勝ち{engine1Win}({engine1WinRatio:0.0}% R{rating:0.0} +-{confidenceInterval:0.0}) 先手勝ち{engine1BlackWin}({engine1BlackWinRatio:0.0}%) 後手勝ち{engine1WhiteWin}({engine1WhiteWinRatio:0.0}%)
宣言勝ち{engine1DeclarationWinBlack + engine1DeclarationWinWhite} 先手宣言勝ち{engine1DeclarationWinBlack} 後手宣言勝ち{engine1DeclarationWinWhite} 先手引き分け{engine1DrawBlack} 後手引き分け{engine2DrawBlack}
{engine2}
勝ち{engine2Win}({engine2WinRatio:0.0}%) 先手勝ち{engine2BlackWin}({engine2BlackWinRatio:0.0}%) 後手勝ち{engine2WhiteWin}({engine2WhiteWinRatio:0.0}%)
宣言勝ち{engine2DeclarationWinBlack + engine2DeclarationWinWhite} 先手宣言勝ち{engine2DeclarationWinBlack} 後手宣言勝ち{engine2DeclarationWinWhite} 先手引き分け{engine2DrawBlack} 後手引き分け{engine1DrawBlack}
{engine1Win},{numDraw},{engine2Win}
");
            Console.Out.Flush();
        }

        public void OnError(string errorMessage)
        {
            Console.Out.WriteLine(errorMessage);
            Console.Out.Flush();
        }
    }
}
