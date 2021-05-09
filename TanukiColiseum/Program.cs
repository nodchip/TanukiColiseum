using System.Threading;
using System.Windows;

namespace TanukiColiseum
{
    class Program
    {
        static void Main(
            string engine1FilePath,
            string engine2FilePath,
            string eval1FolderPath,
            string eval2FolderPath,
            int numConcurrentGames,
            int numGames,
            int hashMb,
            int nodes1,
            int nodes2,
            int nodesRandomPercent1,
            int nodesRandomPercent2,
            bool nodesRandomEveryMove1,
            bool nodesRandomEveryMove2,
            int time1,
            int time2,
            int byoyomi1,
            int byoyomi2,
            int inc1,
            int inc2,
            int rtime1,
            int rtime2,
            int numBookMoves1 = 0,
            int numBookMoves2 = 0,
            string bookFileName1 = "no_book",
            string bookFileName2 = "no_book",
            int numBookMoves = 0,
            int maxMovesToDraw = 320,
            string sfenFilePath = "records2016_10818.sfen",
            int numNumaNodes = 1,
            int progressIntervalMs = 60 * 60 * 1000,
            int numThreads1 = 1,
            int numThreads2 = 1,
            int bookEvalDiff1 = 30,
            int bookEvalDiff2 = 30,
            string considerBookMoveCount1 = "false",
            string considerBookMoveCount2 = "false",
            string ignoreBookPly1 = "false",
            string ignoreBookPly2 = "false",
            int evalScalePercentage1 = 100,
            int evalScalePercentage2 = 100,
            bool gui = true)
        {
            var options = new Options
            {
                Engine1FilePath = engine1FilePath,
                Engine2FilePath = engine2FilePath,
                Eval1FolderPath = eval1FolderPath,
                Eval2FolderPath = eval2FolderPath,
                NumConcurrentGames = numConcurrentGames,
                NumGames = numGames,
                HashMb = hashMb,
                NumBookMoves1 = numBookMoves1,
                NumBookMoves2 = numBookMoves2,
                BookFileName1 = bookFileName1,
                BookFileName2 = bookFileName2,
                NumBookMoves = numBookMoves,
                MaxMovesToDraw = maxMovesToDraw,
                SfenFilePath = sfenFilePath,
                Nodes1 = nodes1,
                Nodes2 = nodes2,
                NodesRandomPercent1 = nodesRandomPercent1,
                NodesRandomPercent2 = nodesRandomPercent2,
                NodesRandomEveryMove1 = nodesRandomEveryMove1,
                NodesRandomEveryMove2 = nodesRandomEveryMove2,
                Time1 = time1,
                Time2 = time2,
                Byoyomi1 = byoyomi1,
                Byoyomi2 = byoyomi2,
                Inc1 = inc1,
                Inc2 = inc2,
                Rtime1 = rtime1,
                Rtime2 = rtime2,
                NumNumaNodes = numNumaNodes,
                ProgressIntervalMs = progressIntervalMs,
                NumThreads1 = numThreads1,
                NumThreads2 = numThreads2,
                BookEvalDiff1 = bookEvalDiff1,
                BookEvalDiff2 = bookEvalDiff2,
                ConsiderBookMoveCount1 = considerBookMoveCount1,
                ConsiderBookMoveCount2 = considerBookMoveCount2,
                IgnoreBookPly1 = ignoreBookPly1,
                IgnoreBookPly2 = ignoreBookPly2,
                EvalScalePercentage1 = evalScalePercentage1,
                EvalScalePercentage2 = evalScalePercentage2,
                Gui = gui,
            };

            if (gui)
            {
                var thread = new Thread(() =>
                {
                    var application = new Application();
                    application.Run(new MainView());
                });
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                thread.IsBackground = true; //メインスレッドが終了した場合に、動作中のスレッドも終了させる場合
                thread.Start();
                thread.Join();
            }
            else
            {
                var cli = new Cli();
                cli.Run(options);
            }
        }
    }
}
