using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TanukiColiseum
{
	public class Coliseum
	{
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
		private int globalGameId;

		public async Task RunAsync(Options options, string logFolderPath)
		{
			this.options = options;

			// 評価関数フォルダと思考エンジンの存在確認を行う。
			// ただし、思考エンジンが batファイルの場合、sshを使っている可能性があるため評価関数フォルダの存在確認を行わない。
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
			else if (Path.GetExtension(options.Engine1FilePath) == ".exe" && !Directory.Exists(options.Eval1FolderPath))
			{
				ShowErrorMessage("評価関数フォルダ1が見つかりませんでした。正しい評価関数フォルダを指定してください");
				return;
			}
			else if (Path.GetExtension(options.Engine2FilePath) == ".exe" && !Directory.Exists(options.Eval2FolderPath))
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
			ValidateOpenings(openings);

			Console.WriteLine("Initializing engines...");
			Console.Out.Flush();

			Directory.CreateDirectory(logFolderPath);
			var sfenFilePath = Path.Combine(logFolderPath, "sfen.txt");
			var sqlite3FilePath = Path.Combine(logFolderPath, "result.sqlite3");

			var computerInfo = new ComputerInfo();
			var previousAvailablePhysicalMemory = computerInfo.AvailablePhysicalMemory;

			// 各エンジンに渡すThreadIdOffsetの値を計算する際に使用するストライド
			// CPUの論理コア数を超えるIDが設定される場合や、
			// スレッド数がエンジン1とエンジン2で異なる場合が考えられるが、
			// とりあえずこの計算式で計算することにする。
			int threadIdStride = Math.Max(options.NumThreads1, options.NumThreads2);
			var games = new List<Game>();
			for (int gameIndex = 0; gameIndex < options.NumConcurrentGames; ++gameIndex)
			{
				// 残り物理メモリサイズを調べ、エンジンの起動に必要なメモリが足りない場合、
				// 警告を表示して終了する。
				// 残り物理メモリ量 · Issue #13 · nodchip/TanukiColiseum https://github.com/nodchip/TanukiColiseum/issues/13
				var currentAvailablePhysicalMemory = computerInfo.AvailablePhysicalMemory;
				var consumedMemoryPerGame = previousAvailablePhysicalMemory - currentAvailablePhysicalMemory;
				if (previousAvailablePhysicalMemory >= currentAvailablePhysicalMemory && consumedMemoryPerGame > currentAvailablePhysicalMemory)
				{
					ShowErrorMessage("利用可能物理メモリが足りません。同時対局数やハッシュサイズを下げてください。");
					await FinishEnginesAsync(games);
					return;
				}
				previousAvailablePhysicalMemory = currentAvailablePhysicalMemory;

				int numaNode = gameIndex * options.NumNumaNodes / options.NumConcurrentGames;

				// エンジン1初期化
				Dictionary<string, string> overriddenOptions1 = new Dictionary<string, string>(){
					{"EvalDir", options.Eval1FolderPath},
					{"USI_Hash", options.HashMb.ToString()},
					{"MinimumThinkingTime", options.MinimumThinkingTime1.ToString()},
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
					{"MaxMovesToDraw", options.MaxMovesToDraw.ToString()},
					{"ThreadIdOffset", (gameIndex * threadIdStride).ToString()},
					{"LargePageEnable", "false"},
					{"BookOnTheFly", "true"},
					{"SlowMover", options.SlowMover1.ToString()},
					{"DrawValueBlack", options.DrawValue1.ToString()},
					{"DrawValueWhite", options.DrawValue1.ToString()},
					{"BookEvalBlackLimit", options.BookEvalBlackLimit1.ToString()},
					{"BookEvalWhiteLimit", options.BookEvalWhiteLimit1.ToString()},
					{"FV_SCALE", options.FVScale1.ToString()},
				};
				Console.WriteLine($"Starting an engine process. gameIndex={gameIndex} engine=1 Engine1FilePath={options.Engine1FilePath}");
				Console.Out.Flush();
				engine1 = new Engine(options.Engine1FilePath, gameIndex * 2, gameIndex, 0, numaNode, overriddenOptions1);
				// Windows 10 May 2019 Updateにおいて、
				// 複数のプロセスが同時に大量のメモリを確保しようとしたときに
				// フリーズする現象を確認した
				// 原因がわかるまでは1プロセスずつメモリを確保するようにする
				// isreadyコマンド受信時にメモリが確保されることを想定する
				if (!await engine1.UsiAsync() || !await engine1.IsreadyAsync())
				{
					ShowErrorMessage($"エンジン1が異常終了またはタイムアウトしました gameIndex={gameIndex}");
					return;
				}

				// エンジン2初期化
				Dictionary<string, string> overriddenOptions2 = new Dictionary<string, string>()
				{
					{"EvalDir", options.Eval2FolderPath},
					{"USI_Hash", options.HashMb.ToString()},
					{"MinimumThinkingTime", options.MinimumThinkingTime2.ToString()},
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
					{"MaxMovesToDraw", options.MaxMovesToDraw.ToString()},
					{"ThreadIdOffset", (gameIndex * threadIdStride).ToString()},
					{"LargePageEnable", "false"},
					{"BookOnTheFly", "true"},
					{"SlowMover", options.SlowMover2.ToString()},
					{"DrawValueBlack", options.DrawValue2.ToString()},
					{"DrawValueWhite", options.DrawValue2.ToString()},
					{"BookEvalBlackLimit", options.BookEvalBlackLimit2.ToString()},
					{"BookEvalWhiteLimit", options.BookEvalWhiteLimit2.ToString()},
					{"FV_SCALE", options.FVScale2.ToString()},
				};
				Console.WriteLine($"Starting an engine process. gameIndex={gameIndex} engine=2 Engine2FilePath={options.Engine2FilePath}");
				Console.Out.Flush();
				engine2 = new Engine(options.Engine2FilePath, gameIndex * 2 + 1, gameIndex, 1, numaNode, overriddenOptions2);
				if (!await engine2.UsiAsync() || !await engine2.IsreadyAsync())
				{
					ShowErrorMessage($"エンジン2が異常終了またはタイムアウトしました gameIndex={gameIndex}");
					return;
				}

				// ゲーム初期化
				// 偶数番目はengine1が先手、奇数番目はengine2が先手
				games.Add(new Game(gameIndex & 1, options.Nodes1, options.Nodes2,
					options.NodesRandomPercent1, options.NodesRandomPercent2,
					options.NodesRandomEveryMove1, options.NodesRandomEveryMove2, options.Time1,
					options.Time2, options.Byoyomi1, options.Byoyomi2, options.Inc1, options.Inc2,
					options.Rtime1, options.Rtime2, options.Depth1, options.Depth2, engine1, engine2,
					options.NumBookMoves, options.MaxMovesToDraw, openings, ShowErrorMessage));
			}

			Console.WriteLine("Initialized engines...");
			Console.WriteLine("Started games...");
			Console.Out.Flush();

			// numConcurrentGames局同時に対局できるようにする
			var tasks = new List<Task<Game>>();
			int numStartedGames = 0;
			for (int i = 0; i < Math.Min(options.NumConcurrentGames, options.NumGames); ++i)
			{
				tasks.Add(games[i].StartAsync());
				++numStartedGames;
			}

			while (tasks.Count > 0)
			{
				// 対局の終了を待機する。
				var finishedTask = await Task.WhenAny(tasks);
				var game = await finishedTask;
				tasks.Remove(finishedTask);

				int engineWin;
				int blackWhiteWin;
				bool draw = false;
				bool declaration = false;
				var lastMove = game.Moves.Last();
				Debug.Assert(lastMove.Resign || lastMove.Win || lastMove.Draw);
				if (lastMove.Resign)
				{
					// 相手側の勝数を上げる
					engineWin = game.Turn ^ 1;
					blackWhiteWin = game.Moves.Count & 1;
				}
				else if (lastMove.Win)
				{
					// 自分側の勝数を上げる
					engineWin = game.Turn;
					blackWhiteWin = (game.Moves.Count + 1) & 1;
					declaration = true;
				}
				else if (lastMove.Draw)
				{
					// 引き分け
					engineWin = 0;
					blackWhiteWin = 0;
					draw = true;
				}
				else
				{
					throw new Exception("Invalid last move.");
				}

				OnGameFinished(engineWin, blackWhiteWin, draw, declaration, game.InitialTurn, sfenFilePath, sqlite3FilePath, game);

				// 次の対局を開始する
				if (numStartedGames < options.NumGames)
				{
					tasks.Add(game.StartAsync());
					++numStartedGames;
				}
			}

			await FinishEnginesAsync(games);

			Debug.Assert(engine1 != null);
			Debug.Assert(engine2 != null);
			ShowStatus(options, Status, engine1, engine2);
			Directory.CreateDirectory(logFolderPath);
			File.WriteAllText(Path.Combine(logFolderPath, "result.txt"), CreateStatusMessage(options, Status, engine1, engine2));
		}

		private async Task FinishEnginesAsync(List<Game> games)
		{
			foreach (var game in games)
			{
				foreach (var engine in game.Engines)
				{
					await engine.Finish();
				}
			}
		}

		/// <summary>
		/// ある対局が終了した際に呼ばれるコールバック
		/// </summary>
		/// <param name="engineWin">勝利した思考エンジン。engin1の場合は0、engine2の場合は1。</param>
		/// <param name="blackWhiteWin">先後どちらが勝利したか。先手の場合は0、後手の場合は1</param>
		/// <param name="draw">引き分けの場合はtrue、そうでない場合はfalse。</param>
		/// <param name="declarationWin">宣言勝ちの場合はtrue、そうでない場合はfalse。</param>
		/// <param name="initialTurn">どちらの思考エンジンが先手だったか。engine1の場合は0、engine2の場合は1。</param>
		public void OnGameFinished(int engineWin, int blackWhiteWin, bool draw, bool declarationWin, int initialTurn, string sfenFilePath, string sqlite3FilePath, Game game)
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

			// sfenファイルへの書き込み
			var sfen = "startpos moves " + string.Join(" ", game.Moves.Select(x => x.Best)) + "\n";
			File.AppendAllText(sfenFilePath, sfen);

			// sqlite3ファイルへの書き込み
			int gameId = Interlocked.Increment(ref globalGameId);
			int winner;
			if (game.Moves.Last().Resign)
			{
				winner = game.Turn ^ 1;
			}
			else if (game.Moves.Last().Win)
			{
				winner = game.Turn;
			}
			else if (game.Moves.Last().Draw)
			{
				winner = 2;
			}
			else
			{
				throw new Exception("Invalid last move.");
			}

			var builder = new SQLiteConnectionStringBuilder { DataSource = sqlite3FilePath };
			using (var connection = new SQLiteConnection(builder.ToString()))
			{
				connection.Open();
				SQLiteTransaction transaction = connection.BeginTransaction();
				using (var command = connection.CreateCommand())
				{
					command.CommandText = @"
CREATE TABLE IF NOT EXISTS game (
    id INTEGER PRIMARY KEY ASC,
    winner INTEGER
);
";
					command.ExecuteNonQuery();
				}

				using (var command = connection.CreateCommand())
				{
					command.CommandText =
@"CREATE TABLE IF NOT EXISTS move (
    game_id INTEGER,
    play INTEGER,
    best TEXT,
    next TEXT,
    value INTEGER,
    depth INTEGER,
    book INTEGER
);
";
					command.ExecuteNonQuery();
				}

				using (var command = connection.CreateCommand())
				{
					command.CommandText =
@"CREATE INDEX IF NOT EXISTS move_game_id_index ON move (
    game_id
);
";
					command.ExecuteNonQuery();
				}

				using (var command = connection.CreateCommand())
				{
					command.CommandText =
@"
    INSERT INTO game (id, winner)
    VALUES ($id, $winner)
";
					command.Parameters.AddWithValue("$id", gameId);
					command.Parameters.AddWithValue("$winner", winner);
					command.ExecuteNonQuery();
				}

				for (int play = 0; play < game.Moves.Count; ++play)
				{
					var move = game.Moves[play];

					using (var command = connection.CreateCommand())
					{
						command.CommandText =
@"
INSERT INTO move (game_id, play, best, next, value, depth, book)
VALUES ($game_id, $play, $best, $next, $value, $depth, $book)
";
						command.Parameters.AddWithValue("$game_id", gameId);
						command.Parameters.AddWithValue("$play", play);
						command.Parameters.AddWithValue("$best", move.Best);
						command.Parameters.AddWithValue("$next", move.Next);
						command.Parameters.AddWithValue("$value", move.Value);
						command.Parameters.AddWithValue("$depth", move.Depth);
						command.Parameters.AddWithValue("$book", move.Book);
						command.ExecuteNonQuery();
					}
				}

				transaction.Commit();
			}
		}

		public static string CreateStatusMessage(Options options, Status status, Engine engine1, Engine engine2)
		{
			return options.ToHumanReadableString(engine1, engine2)
				+ new Status(status).ToHumanReadableString();
		}

		/// <summary>
		/// 開始局面集の書式を検証する。
		/// </summary>
		/// <param name="openings"></param>
		private void ValidateOpenings(string[] openings)
		{
			int lineNumber = 1;
			foreach (var opening in openings)
			{
				RocketTanuki.Position position = new RocketTanuki.Position();
				position.Set(RocketTanuki.Position.StartposSfen);
				foreach (var move in Util.Split(opening))
				{
					if (move == "startpos" || move == "moves")
					{
						continue;
					}

					try
					{
						position.DoMove(RocketTanuki.Move.FromUsiString(position, move));
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw new Exception($"不正な開始局面が見つかりました。 opening={opening} move={move} lineNumber={lineNumber}", e);
					}
				}

				++lineNumber;
			}
		}
	}
}
