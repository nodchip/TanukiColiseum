using System;
using System.Collections.Generic;
using System.IO;

namespace TanukiColiseum
{
    class Game
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

        public Game(int initialTurn, int nodes1, int nodes2, int time1, int time2, Engine engine1,
            Engine engine2, int numBookMoves, string[] openings, string sfenFilePath)
        {
            this.Nodes = new int[] { nodes1, nodes2 };
            this.Times = new int[] { time1, time2 };
            this.Engines.Add(engine1);
            this.Engines.Add(engine2);
            this.NumBookMoves = numBookMoves;
            this.Openings = openings;
            this.sfenFilePath = sfenFilePath;
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
                engine.Send("usinewgame");
            }
        }

        public void Go()
        {
            string command = "position startpos moves";
            foreach (var move in Moves)
            {
                command += " ";
                command += move;
            }
            Engines[Turn].Send(command);
            if (Nodes[Turn] != 0)
            {
                Engines[Turn].Send($"go nodes {Nodes[Turn]}");
            }
            else if (Times[Turn] != 0)
            {
                Engines[Turn].Send($"go byoyomi {Times[Turn]}");
            }
            else
            {
                throw new Exception("nodesかtimeのいずれかを指定してください。");
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
            File.AppendAllText(sfenFilePath, sfen);

            InitialTurn ^= 1;
            Running = false;
        }
    }
}
