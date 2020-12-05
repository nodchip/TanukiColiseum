using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static TanukiColiseum.Coliseum;
using System.Linq;

namespace TanukiColiseum
{
    public class Game
    {
        public enum Result : int
        {
            Win,
            Lose,
            Draw,
        }

        public class Move
        {
            public string Best { get; set; }
            public string Next { get; set; }
            public int Value { get; set; }
            public int Depth { get; set; }

            /// <summary>
            /// 局面集の指し手は1、そうでない場合は0
            /// </summary>
            public int Book { get; set; }
        }

        private int NumBookMoves;
        public List<Move> Moves { get; } = new List<Move>();
        public int Turn { get; private set; }
        public int InitialTurn { get; private set; }
        public bool Running { get; set; } = false;
        public List<Engine> Engines { get; } = new List<Engine>();
        private Random Random = new Random();
        private int[] Nodes;
        private int[] Times;
        private int[] Byoyomis;
        private int[] Incs;
        private string[] Openings;
        private bool ChangeOpening = true;
        private int OpeningIndex = 0;
        private string sfenFilePath;
        private string sqlite3FilePath;
        private event ErrorHandler ShowErrorMessage;
        private static object WriteResultLock = new object();
        private static int globalGameId = 0;
        private DateTime goDateTime;

        public Game(int initialTurn, int nodes1, int nodes2, int time1, int time2, int byoyomi1,
            int byoyomi2, int inc1, int inc2, Engine engine1, Engine engine2, int numBookMoves,
            string[] openings, string sfenFilePath, string sqlite3FilePath,
            ErrorHandler ShowErrorMessage)
        {
            // アンコメントアウトする
            //this.InitialTurn = initialTurn;
            this.Nodes = new int[] { nodes1, nodes2 };
            this.Times = new int[] { time1, time2 };
            this.Byoyomis = new int[] { byoyomi1, byoyomi2 };
            this.Incs = new int[] { inc1, inc2 };
            this.Engines.Add(engine1);
            this.Engines.Add(engine2);
            this.NumBookMoves = numBookMoves;
            this.Openings = openings;
            this.sfenFilePath = sfenFilePath;
            this.sqlite3FilePath = sqlite3FilePath;
            this.ShowErrorMessage = ShowErrorMessage;
        }

        public void OnNewGame()
        {
            // 2回に1回開始局面を変更する
            if (ChangeOpening)
            {
                OpeningIndex = Random.Next(Openings.Length);
            }
            ChangeOpening = !ChangeOpening;

            Moves.Clear();
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
            }

            Turn = InitialTurn;

            Running = true;

            foreach (var engine in Engines)
            {
                if (!engine.Isready())
                {
                    ShowErrorMessage($"isreadyコマンドの送信に失敗しました。エンジン({engine})が異常終了またはタイムアウトしました。");
                }

                if (!engine.Usinewgame())
                {
                    ShowErrorMessage($"usinewgameコマンドの送信に失敗しました。エンジン({engine})が異常終了またはタイムアウトしました。");
                }
            }
        }

        public void Go()
        {
            if (!Engines[Turn].Position(Moves.Select(x => x.Best).ToList()))
            {
                ShowErrorMessage($"positionコマンドの送信に失敗しました。エンジン({Engines[Turn]})が異常終了またはタイムアウトしました。");
            }

            var nameAndValues = new Dictionary<string, int> {
                { "btime", Times[InitialTurn] },
                { "wtime", Times[InitialTurn ^ 1] },
                { "byoyomi", Byoyomis[Turn] },
                { "binc", Incs[InitialTurn] },
                { "winc", Incs[InitialTurn ^ 1] },
                { "nodes", Nodes[Turn] },
            };

            Engines[Turn].Go(nameAndValues);

            goDateTime = DateTime.Now;
        }

        /// <summary>
        /// 指し手が指されたときに呼ばれる
        /// </summary>
        /// <param name="move"></param>
        public void OnMove(Move move)
        {
            // 持ち時間から思考時間を引く
            var bestmoveDateTime = DateTime.Now;
            var thinkingTime = bestmoveDateTime - goDateTime;
            Times[Turn] += Incs[Turn];
            Times[Turn] -= (int)thinkingTime.TotalMilliseconds;
            Times[Turn] = Math.Max(Times[Turn], 0);

            Moves.Add(move);
            Turn ^= 1;
        }

        /// <summary>
        /// 宗教した場合に呼ばれる
        /// </summary>
        /// <param name="win"></param>
        public void OnGameFinished(Result result)
        {
            // sfenファイルへの書き込み
            var sfen = "startpos moves " + string.Join(" ", Moves.Select(x => x.Best)) + "\n";
            lock (WriteResultLock)
            {
                File.AppendAllText(sfenFilePath, sfen);

                // sqlite3ファイルへの書き込み
                int gameId = Interlocked.Increment(ref globalGameId);
                int winner;
                switch (result)
                {
                    case Result.Win:
                        winner = Turn;
                        break;
                    case Result.Lose:
                        winner = Turn ^ 1;
                        break;
                    case Result.Draw:
                        winner = 2;
                        break;
                    default:
                        throw new Exception($"Invalid Result type. result={result}");
                }

                var builder = new SQLiteConnectionStringBuilder { DataSource = sqlite3FilePath };
                using (var connection = new SQLiteConnection(builder.ToString()))
                {
                    connection.Open();
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

                    for (int play = 0; play < Moves.Count; ++play)
                    {
                        var move = Moves[play];

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
                }
            }

            InitialTurn ^= 1;
            Running = false;
        }
    }
}
