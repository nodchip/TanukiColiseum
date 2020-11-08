using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TanukiColiseum
{
    public class Coliseum
    {
        private SemaphoreSlim GameSemaphoreSlim;
        private SemaphoreSlim FinishSemaphoreSlim = new SemaphoreSlim(0);
        public List<Game> Games { get; } = new List<Game>();
        private DateTime LastOutput = DateTime.Now;
        private int ProgressIntervalMs;
        private Status Status { get; } = new Status();
        public delegate void StatusHandler(Options options, Status status, Engine engine1, Engine engine2);
        public delegate void ErrorHandler(string errorMessage);
        public event StatusHandler ShowStatus;
        public event ErrorHandler ShowErrorMessage;
        private Engine engine1;
        private Engine engine2;
        private Options options;

        public void Run(Options options, string logFolderPath)
        {
            this.options = options;

            // 評価関数フォルダと思考エンジンの存在確認を行う
            if (!File.Exists(options.Engine1FilePath))
            {
                ShowErrorMessage("思考エンジン1が見つかりませんでした。正しいexeファイルを指定してください。");
                return;
            }
            else if (!File.Exists(options.Engine2FilePath))
            {
                ShowErrorMessage("思考エンジン2が見つかりませんでした。正しいexeファイルを指定してください。");
                return;
            }
            else if (!Directory.Exists(options.Eval1FolderPath))
            {
                ShowErrorMessage("評価関数フォルダ1が見つかりませんでした。正しい評価関数フォルダを指定してください");
                return;
            }
            else if (!Directory.Exists(options.Eval2FolderPath))
            {
                ShowErrorMessage("評価関数フォルダ2が見つかりませんでした。正しい評価関数フォルダを指定してください");
                return;
            }
            else if (!File.Exists(options.SfenFilePath))
            {
                ShowErrorMessage("開始局面ファイルが見つかりませんでした。正しい開始局面ファイルを指定してください");
                return;
            }

            Status.NumGames = options.NumGames;
            Status.Nodes = new int[] { options.Nodes1, options.Nodes2 };
            ProgressIntervalMs = options.ProgressIntervalMs;

            // 開始局面集を読み込む
            string[] openings = File.ReadAllLines(options.SfenFilePath);

            Console.WriteLine("Initializing engines...");
            Console.Out.Flush();

            Directory.CreateDirectory(logFolderPath);
            var sfenFilePath = Path.Combine(logFolderPath, "sfen.txt");

            // 各エンジンに渡すThreadIdOffsetの値を計算する際に使用するストライド
            // CPUの論理コア数を超えるIDが設定される場合や、
            // スレッド数がエンジン1とエンジン2で異なる場合が考えられるが、
            // とりあえずこの計算式で計算することにする。
            int threadIdStride = Math.Max(options.NumThreads1, options.NumThreads2);
            for (int gameIndex = 0; gameIndex < options.NumConcurrentGames; ++gameIndex)
            {
                int numaNode = gameIndex * options.NumNumaNodes / options.NumConcurrentGames;

                // エンジン1初期化
                Dictionary<string, string> overriddenOptions1 = new Dictionary<string, string>(){
                    {"EvalDir", options.Eval1FolderPath},
                    {"USI_Hash", options.HashMb.ToString()},
                    {"MinimumThinkingTime", "1000"},
                    {"NetworkDelay", "0"},
                    {"NetworkDelay2", "0"},
                    {"EvalShare", "true"},
                    {"BookMoves", options.NumBookMoves1.ToString()},
                    {"BookFile", options.BookFileName1},
                    {"Threads", options.NumThreads1.ToString()},
                    {"BookEvalDiff", options.BookEvalDiff1.ToString()},
                    {"ConsiderBookMoveCount", options.ConsiderBookMoveCount1},
                    {"IgnoreBookPly", options.IgnoreBookPly1},
                    {"BookDepthLimit", "0"},
                    {"MaxMovesToDraw", "256"},
                    {"ThreadIdOffset", (gameIndex * threadIdStride).ToString()}
                };
                Console.WriteLine($"Starting an engine process. gameIndex={gameIndex} engine=1 Engine1FilePath={options.Engine1FilePath}");
                Console.Out.Flush();
                engine1 = new Engine(options.Engine1FilePath, this, gameIndex * 2, gameIndex, 0, numaNode, overriddenOptions1);
                // Windows 10 May 2019 Updateにおいて、
                // 複数のプロセスが同時に大量のメモリを確保しようとしたときに
                // フリーズする現象を確認した
                // 原因がわかるまでは1プロセスずつメモリを確保するようにする
                // isreadyコマンド受信時にメモリが確保されることを想定する
                if (!engine1.Usi() || !engine1.Isready())
                {
                    ShowErrorMessage($"エンジン1が異常終了またはタイムアウトしました gameIndex={gameIndex}");
                    return;
                }

                // エンジン2初期化
                Dictionary<string, string> overriddenOptions2 = new Dictionary<string, string>()
                {
                    {"EvalDir", options.Eval2FolderPath},
                    {"USI_Hash", options.HashMb.ToString()},
                    {"MinimumThinkingTime", "1000"},
                    {"NetworkDelay", "0"},
                    {"NetworkDelay2", "0"},
                    {"EvalShare", "true"},
                    {"BookMoves", options.NumBookMoves2.ToString()},
                    {"BookFile", options.BookFileName2},
                    {"Threads", options.NumThreads2.ToString()},
                    {"BookEvalDiff", options.BookEvalDiff2.ToString()},
                    {"ConsiderBookMoveCount", options.ConsiderBookMoveCount2},
                    {"IgnoreBookPly", options.IgnoreBookPly2},
                    {"BookDepthLimit", "0"},
                    {"MaxMovesToDraw", "256"},
                    {"ThreadIdOffset", (gameIndex * threadIdStride).ToString()}
                };
                Console.WriteLine($"Starting an engine process. gameIndex={gameIndex} engine=2 Engine2FilePath={options.Engine2FilePath}");
                Console.Out.Flush();
                engine2 = new Engine(options.Engine2FilePath, this, gameIndex * 2 + 1, gameIndex, 1, numaNode, overriddenOptions2);
                if (!engine2.Usi() || !engine2.Isready())
                {
                    ShowErrorMessage($"エンジン2が異常終了またはタイムアウトしました gameIndex={gameIndex}");
                    return;
                }

                // ゲーム初期化
                // 偶数番目はengine1が先手、奇数番目はengine2が先手
                Games.Add(new Game(gameIndex & 1, options.Nodes1, options.Nodes2, options.Time1,
                    options.Time2, engine1, engine2, options.NumBookMoves, openings, sfenFilePath, ShowErrorMessage));
            }

            Console.WriteLine("Initialized engines...");
            Console.WriteLine("Started games...");
            Console.Out.Flush();

            // numConcurrentGames局同時に対局できるようにする
            GameSemaphoreSlim = new SemaphoreSlim(options.NumConcurrentGames, options.NumConcurrentGames);
            var random = new Random();
            for (int i = 0; i < options.NumGames; ++i)
            {
                GameSemaphoreSlim.Wait();

                // 空いているゲームインスタンスを探す
                Game game = Games.Find(x => !x.Running);
                game.OnNewGame();
                game.Go();
            }

            while (Games.Count(game => game.Running) > 0)
            {
                FinishSemaphoreSlim.Wait();
            }

            foreach (var game in Games)
            {
                foreach (var engine in game.Engines)
                {
                    engine.Finish();
                }
            }

            Console.WriteLine($"engine1={options.Engine1FilePath} eval1={options.Eval1FolderPath}");
            Console.WriteLine($"engine2={options.Engine2FilePath} eval2={options.Eval2FolderPath}");
            Debug.Assert(engine1 != null);
            Debug.Assert(engine2 != null);
            ShowStatus(options, Status, engine1, engine2);
            File.WriteAllText(Path.Combine(logFolderPath, "result.txt"), CreateStatusMessage(options, Status, engine1, engine2));
        }

        /// <summary>
        /// ある対局が終了した際に呼ばれるコールバック
        /// </summary>
        /// <param name="engineWin">勝利した思考エンジン。engin1の場合は0、engine2の場合は1。</param>
        /// <param name="blackWhiteWin">先後どちらが勝利したか。先手の場合は0、後手の場合は1</param>
        /// <param name="draw">引き分けの場合はtrue、そうでない場合はfalse。</param>
        /// <param name="declarationWin">宣言勝ちの場合はtrue、そうでない場合はfalse。</param>
        /// <param name="initialTurn">どちらの思考エンジンが先手だったか。engine1の場合は0、engine2の場合は1。</param>
        public void OnGameFinished(int engineWin, int blackWhiteWin, bool draw, bool declarationWin, int initialTurn)
        {
            if (!draw)
            {
                Interlocked.Increment(ref Status.Win[engineWin, blackWhiteWin]);
            }
            else
            {
                Interlocked.Increment(ref Status.NumDraw[initialTurn]);
            }

            if (declarationWin)
            {
                Interlocked.Increment(ref Status.DeclarationWin[engineWin, blackWhiteWin]);
            }

            if (LastOutput.AddMilliseconds(ProgressIntervalMs) <= DateTime.Now)
            {
                ShowStatus(options, Status, engine1, engine2);
                LastOutput = DateTime.Now;
            }
            GameSemaphoreSlim.Release();
            FinishSemaphoreSlim.Release();
        }

        public static string CreateStatusMessage(Options options, Status status, Engine engine1, Engine engine2)
        {
            return options.ToHumanReadableString(engine1, engine2)
                + new Status(status).ToHumanReadableString();
        }
    }
}
