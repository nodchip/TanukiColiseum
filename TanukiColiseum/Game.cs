using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TanukiColiseum.Coliseum;

namespace TanukiColiseum
{
	public partial class Game
	{
		private int NumBookMoves;
		private int MaxMovesToDraw;
		public List<Move> Moves { get; } = new List<Move>();
		public int Turn { get; private set; }
		public int InitialTurn { get; private set; }
		public List<Engine> Engines { get; } = new List<Engine>();
		private Random Random = new Random();
		private int[] Nodes;
		private int[] NodesRandomPercent;
		private bool[] NodesRandomEveryMove;
		private int[] NodesForThisGame;
		private int[] InitialTimes;
		private int[] CurrentTimes;
		private int[] Byoyomis;
		private int[] Incs;
		private int[] Rtime;
		private string[] Openings;
		private bool ChangeOpening = true;
		private int OpeningIndex = 0;
		private event ErrorHandler ShowErrorMessage;
		private DateTime goDateTime;
		private Dictionary<long, int> hashToCount = new Dictionary<long, int>();
		private RocketTanuki.Position position = new RocketTanuki.Position();

		public Game(int initialTurn, int nodes1, int nodes2, int nodesRandomPercent1,
			int nodesRandomPercent2, bool nodesRandomEveryMove1, bool nodesRandomEveryMove2,
			int time1, int time2, int byoyomi1, int byoyomi2, int inc1, int inc2, int rtime1,
			int rtime2, Engine engine1, Engine engine2, int numBookMoves, int maxMovesToDraw,
			string[] openings, ErrorHandler ShowErrorMessage)
		{
			// StartAsync()の最初に反転させるため、あらかじめ反転させておく。
			this.InitialTurn = initialTurn ^ 1;
			this.Nodes = new int[] { nodes1, nodes2 };
			this.NodesRandomPercent = new int[] { nodesRandomPercent1, nodesRandomPercent2 };
			this.NodesRandomEveryMove = new bool[] { nodesRandomEveryMove1, nodesRandomEveryMove2 };
			this.NodesForThisGame = new int[2];
			this.InitialTimes = new int[] { time1, time2 };
			this.CurrentTimes = new int[] { time1, time2 };
			this.Byoyomis = new int[] { byoyomi1, byoyomi2 };
			this.Incs = new int[] { inc1, inc2 };
			this.Rtime = new int[] { rtime1, rtime2 };
			this.Engines.Add(engine1);
			this.Engines.Add(engine2);
			this.NumBookMoves = numBookMoves;
			this.MaxMovesToDraw = maxMovesToDraw;
			this.Openings = openings;
			this.ShowErrorMessage = ShowErrorMessage;
		}

		public async Task<Game> StartAsync()
		{
			InitialTurn ^= 1;

			// 2回に1回開始局面を変更する
			if (ChangeOpening)
			{
				OpeningIndex = Random.Next(Openings.Length);
			}
			ChangeOpening = !ChangeOpening;

			Moves.Clear();
			position.Set(RocketTanuki.Position.StartposSfen);
			foreach (var move in Util.Split(Openings[OpeningIndex]))
			{
				if (Moves.Count >= NumBookMoves)
				{
					break;
				}

				if (move == "startpos" || move == "moves")
				{
					continue;
				}

				Moves.Add(new Move
				{
					Best = move,
					Next = "none",
					Book = 1,
				});
				position.DoMove(RocketTanuki.Move.FromUsiString(position, move));
			}

			Turn = InitialTurn;

			foreach (var engine in Engines)
			{
				if (!await engine.IsreadyAsync())
				{
					ShowErrorMessage($"isreadyコマンドの送信に失敗しました。エンジン({engine})が異常終了またはタイムアウトしました。");
				}

				await engine.UsinewgameAsync();
			}

			for (int engineIndex = 0; engineIndex < 2; ++engineIndex)
			{
				CurrentTimes[engineIndex] = InitialTimes[engineIndex];

				NodesForThisGame[engineIndex] = Nodes[engineIndex];
				if (NodesRandomPercent[engineIndex] != 0 && !NodesRandomEveryMove[engineIndex])
				{
					var nodesRandom = NodesRandomPercent[engineIndex] * 0.01;
					var delta = NodesForThisGame[engineIndex] * ((Random.NextDouble() * nodesRandom * 2.0) - nodesRandom);
					NodesForThisGame[engineIndex] += (int)delta;
				}
			}

			// エンジン1とエンジン2の設定が、両方とも1局を通して乱数が同じで、
			// その他の思考ノード数のパラメーターが等しい場合、思考ノード数を同じにする。
			if (!NodesRandomEveryMove[0] &&
				!NodesRandomEveryMove[1] &&
				Nodes[0] == Nodes[1] &&
				NodesRandomPercent[0] == NodesRandomPercent[1])
			{
				NodesForThisGame[1] = NodesForThisGame[0];
			}

			// 千日手判定用のデータをクリアする
			hashToCount.Clear();

			// 対局開始
			while (Moves.Count < MaxMovesToDraw)
			{
				// position
				await Engines[Turn].PositionAsync(Moves.Select(x => x.Best).ToList());

				// 千日手の判定
				var hash = position.Hash;
				if (!hashToCount.ContainsKey(hash))
				{
					hashToCount[hash] = 0;
				}
				if (++hashToCount[hash] == 4)
				{
					Moves.Add(new Move() { Draw = true });
					return this;
				}

				// go
				int nodes = NodesForThisGame[Turn];
				if (NodesRandomPercent[Turn] != 0 && NodesRandomEveryMove[Turn])
				{
					var nodesRandom = NodesRandomPercent[Turn] * 0.01;
					var delta = nodes * ((Random.NextDouble() * nodesRandom * 2.0) - nodesRandom);
					nodes += (int)delta;
				}

				var nameAndValues = new Dictionary<string, int> {
					{ "btime", CurrentTimes[InitialTurn] },
					{ "wtime", CurrentTimes[InitialTurn ^ 1] },
					{ "byoyomi", Byoyomis[Turn] },
					{ "binc", Incs[InitialTurn] },
					{ "winc", Incs[InitialTurn ^ 1] },
					{ "nodes", nodes },
					{ "rtime", Rtime[Turn] },
				};

				goDateTime = DateTime.Now;

				var move = await Engines[Turn].GoAsync(nameAndValues);

				// 持ち時間から思考時間を引く
				var bestmoveDateTime = DateTime.Now;
				var thinkingTime = bestmoveDateTime - goDateTime;
				CurrentTimes[Turn] += Incs[Turn];
				CurrentTimes[Turn] -= (int)thinkingTime.TotalMilliseconds;
				CurrentTimes[Turn] = Math.Max(CurrentTimes[Turn], 0);

				Moves.Add(move);

				if (move.Resign || move.Win)
				{
					return this;
				}

				// 対局終了時に、最後の手を指した側のエンジンを指すようにする。
				Turn ^= 1;

				position.DoMove(RocketTanuki.Move.FromUsiString(position, move.Best));
			}

			Moves.Add(new Move() { Draw = true });
			return this;
		}
	}
}
