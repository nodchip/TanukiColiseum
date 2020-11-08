using System;
using System.Collections.Generic;
using System.IO;
using static TanukiColiseum.Coliseum;

namespace TanukiColiseum
{
    public class Game
    {
        private int NumBookMoves;
        public List<string> Moves { get; } = new List<string>();
        public int Turn { get; private set; }
        public int InitialTurn { get; private set; }
        public bool Running { get; set; } = false;
        public List<Engine> Engines { get; } = new List<Engine>();
        private Random Random = new Random();
        private int[] Nodes;
        private int[] Times;
        private string[] Openings;
        private bool ChangeOpening = true;
        private int OpeningIndex = 0;
        private string sfenFilePath;
        private event ErrorHandler ShowErrorMessage;
        private static object SfenFileLock = new object();

        public Game(int initialTurn, int nodes1, int nodes2, int time1, int time2, Engine engine1,
            Engine engine2, int numBookMoves, string[] openings, string sfenFilePath, ErrorHandler ShowErrorMessage)
        {
            this.Nodes = new int[] { nodes1, nodes2 };
            this.Times = new int[] { time1, time2 };
            this.Engines.Add(engine1);
            this.Engines.Add(engine2);
            this.NumBookMoves = numBookMoves;
            this.Openings = openings;
            this.sfenFilePath = sfenFilePath;
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
                if (move == "startpos" || move == "moves")
                {
                    continue;
                }
                Moves.Add(move);
                if (Moves.Count >= NumBookMoves)
                {
                    break;
                }
            }

            Turn = InitialTurn;

            Running = true;

            foreach (var engine in Engines)
            {
                if (!engine.Isready() || !engine.Usinewgame())
                {
                    ShowErrorMessage($"エンジン({engine})が異常終了またはタイムアウトしました");
                }
            }
        }

        public void Go()
        {
            if (!Engines[Turn].Position(Moves))
            {
                ShowErrorMessage($"エンジン({Engines[Turn]})が異常終了またはタイムアウトしました");
            }

            if (Nodes[Turn] != 0)
            {
                if (!Engines[Turn].Go("nodes", Nodes[Turn]))
                {
                    ShowErrorMessage($"エンジン({Engines[Turn]})が異常終了またはタイムアウトしました");
                }
            }
            else if (Times[Turn] != 0)
            {
                if (!Engines[Turn].Go("byoyomi", Times[Turn]))
                {
                    ShowErrorMessage($"エンジン({Engines[Turn]})が異常終了またはタイムアウトしました");
                }
            }
            else
            {
                ShowErrorMessage("nodesかtimeのいずれかを指定してください。");
            }
        }
        public void OnMove(string move)
        {
            Moves.Add(move);
            Turn ^= 1;
        }

        public void OnGameFinished()
        {
            var sfen = "startpos moves " + string.Join(" ", Moves) + "\n";
            lock (SfenFileLock)
            {
                File.AppendAllText(sfenFilePath, sfen);
            }

            InitialTurn ^= 1;
            Running = false;
        }
    }
}
