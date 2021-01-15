using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static TanukiColiseum.Game;

namespace TanukiColiseum
{
    public class Engine
    {
        private static readonly TimeSpan ProcessWaitTime = TimeSpan.FromMinutes(1);
        private Process Process = new Process();
        private Coliseum Coliseum;
        private SemaphoreSlim UsiokSemaphore = new SemaphoreSlim(0);
        private SemaphoreSlim ReadyokSemaphore = new SemaphoreSlim(0);
        private SemaphoreSlim HashSemaphore = new SemaphoreSlim(0);
        private int ProcessIndex;
        private int GameIndex;
        private int EngineIndex;
        private Dictionary<string, string> OverriddenOptions;
        public string Name { get; set; }
        public string Author { get; set; }
        public string Hash { get; set; }
        private List<string> lastInfoCommand;

        public Engine(string fileName, Coliseum coliseum, int processIndex, int gameIndex, int engineIndex, int numaNode, Dictionary<string, string> overriddenOptions)
        {
            this.Process.StartInfo.FileName = "cmd.exe";
            if (Path.IsPathRooted(fileName))
            {
                this.Process.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
            }
            this.Process.StartInfo.Arguments = $"/c start /B /WAIT /NODE {numaNode} {fileName}";
            this.Process.StartInfo.UseShellExecute = false;
            this.Process.StartInfo.RedirectStandardInput = true;
            this.Process.StartInfo.RedirectStandardOutput = true;
            this.Process.StartInfo.RedirectStandardError = true;
            this.Process.OutputDataReceived += HandleStdout;
            this.Process.ErrorDataReceived += HandleStderr;
            this.Coliseum = coliseum;
            this.ProcessIndex = processIndex;
            this.GameIndex = gameIndex;
            this.EngineIndex = engineIndex;
            this.OverriddenOptions = overriddenOptions;

            // 同期的に実行した場合、
            // BeginOutputReadLine()/BeginErrorReadLine()が呼び出されたあと
            // 読み込みスレッドが動かない。
            // これを避けるため、Usi()やIsready()の内部でUsiAsync()やIsreadyAsync()を、
            // 非同期的に呼びだしている。
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
        }

        public void Finish()
        {
            Send("quit");
            Process.WaitForExit();
            Process.Dispose();
        }

        /// <summary>
        /// エンジンにコマンドを送信する
        /// </summary>
        /// <param name="command"></param>
        private void Send(string command)
        {
            //Debug.WriteLine($"> [{ProcessIndex}] {command}");
            Process.StandardInput.WriteLine(command);
            Process.StandardInput.Flush();
        }

        /// <summary>
        /// 思考エンジンの標準出力を処理する
        /// </summary>
        /// <param name="sender">出力を送ってきた思考エンジンのプロセス</param>
        /// <param name="e">思考エンジンの出力</param>
        private void HandleStdout(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }

            //Debug.WriteLine($"< [{ProcessIndex}] {e.Data}");

            List<string> command = Util.Split(e.Data);
            if (Regex.IsMatch(e.Data, "^[0-9a-f]{1,16}$"))
            {
                HandleHash(command);
            }
            else if (command.Contains("id"))
            {
                HandleId(command);
            }
            else if (command.Contains("usiok"))
            {
                HandleUsiok(command);
            }
            else if (command.Contains("option"))
            {
                HandleOption(command);
            }
            else if (command.Contains("readyok"))
            {
                HandleReadyok(command);
            }
            else if (command.Contains("bestmove"))
            {
                HandleBestmove(command);
            }
            else if (command.Contains("info"))
            {
                HandleInfo(command);
            }
        }

        /// <summary>
        /// 思考エンジンの標準エラー出力を処理する
        /// </summary>
        /// <param name="sender">出力を送ってきた思考エンジンのプロセス</param>
        /// <param name="e">思考エンジンの出力</param>
        private void HandleStderr(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine($"    ! [{ProcessIndex}] {e.Data}");
        }

        private void HandleHash(List<string> command)
        {
            Hash = command[0];
            HashSemaphore.Release();
        }

        private void HandleId(List<string> command)
        {
            if (command.Count < 2)
            {
                return;
            }

            switch (command[1])
            {
                case "name":
                    Name = string.Join(" ", command.Skip(2));
                    break;

                case "author":
                    Author = string.Join(" ", command.Skip(2));
                    break;

                default:
                    throw new Exception($"Unknown attribute: command[1]={command[1]}");
            }
        }

        private void HandleUsiok(List<string> command)
        {
            // usiokを待機している関数を進行する
            UsiokSemaphore.Release();
        }

        private void HandleOption(List<string> command)
        {
            int nameIndex = command.IndexOf("name");
            int defaultIndex = command.IndexOf("default");
            if (nameIndex == -1 || nameIndex + 1 >= command.Count ||
                defaultIndex == -1 || defaultIndex + 1 >= command.Count)
            {
                return;
            }

            string name = command[nameIndex + 1];
            string value = command[defaultIndex + 1];
            if (OverriddenOptions.ContainsKey(name))
            {
                value = OverriddenOptions[name];
            }
            Send($"setoption name {name} value {value}");
        }

        private void HandleReadyok(List<string> command)
        {
            // readyokを待機している関数を進行する
            ReadyokSemaphore.Release();
        }

        private void HandleBestmove(List<string> command)
        {
            var game = Coliseum.Games[GameIndex];
            if (command[1] == "resign" || command[1] == "win")
            {
                int engineWin;
                int blackWhiteWin;
                bool draw = false;
                bool declaration = false;
                Game.Result result;
                if (command[1] == "resign")
                {
                    // 相手側の勝数を上げる
                    engineWin = game.Turn ^ 1;
                    blackWhiteWin = (game.Moves.Count + 1) & 1;
                    result = Game.Result.Lose;
                }
                else if (command[1] == "win")
                {
                    // 自分側の勝数を上げる
                    engineWin = game.Turn;
                    blackWhiteWin = game.Moves.Count & 1;
                    declaration = true;
                    result = Game.Result.Win;
                }
                else
                {
                    // 引き分け
                    engineWin = 0;
                    blackWhiteWin = 0;
                    draw = true;
                    result = Game.Result.Draw;
                }

                // 次の対局を開始する
                // 先にGame.OnGameFinished()を読んでゲームの状態を停止状態に移行する
                game.OnGameFinished(result);
                Coliseum.OnGameFinished(engineWin, blackWhiteWin, draw, declaration, game.InitialTurn);
            }
            else
            {
                Debug.Assert(lastInfoCommand != null);
                var move = new Move();

                int index = lastInfoCommand.IndexOf("depth");
                if (index != -1)
                {
                    move.Depth = int.Parse(lastInfoCommand[index + 1]);
                }

                move.Best = command[1];
                if (command.Count == 4)
                {
                    move.Next = command[3];
                }
                else
                {
                    move.Next = "none";
                }

                index = lastInfoCommand.IndexOf("score");
                if (index != -1)
                {
                    if (lastInfoCommand[index + 1] == "cp")
                    {
                        move.Value = int.Parse(lastInfoCommand[index + 2]);
                    }
                    else
                    {
                        // 将棋所：USIプロトコルとは http://shogidokoro.starfree.jp/usi.html
                        // mate時の評価値のパースのバグを修正する · Issue #21 · nodchip/TanukiColiseum https://github.com/nodchip/TanukiColiseum/issues/21
                        Debug.Assert(lastInfoCommand[index + 1] == "mate");
                        var mateString = lastInfoCommand[index + 2];
                        if (mateString == "+")
                        {
                            move.Value = 32000;
                        }
                        else if (mateString == "-")
                        {
                            move.Value = -32000;
                        }
                        else if (mateString.StartsWith("-"))
                        {
                            move.Value = -32000 - int.Parse(mateString);
                        }
                        else
                        {
                            move.Value = 32000 - int.Parse(mateString);
                        }
                    }
                }

                lastInfoCommand = null;

                game.OnMove(move);
            }
        }

        public void HandleInfo(List<string> command)
        {
            lastInfoCommand = command;
        }

        public bool Usi()
        {
            return UsiAsync().Wait(ProcessWaitTime);
        }

        private async Task UsiAsync()
        {
            await SendAndReceive("usi", UsiokSemaphore);
        }

        public bool Isready()
        {
            return IsreadyAsync().Wait(ProcessWaitTime);
        }

        private async Task IsreadyAsync()
        {
            await SendAndReceive("isready", ReadyokSemaphore);
        }

        public bool Stop()
        {
            Send("stop");
            return true;
        }

        private async Task SendAndReceive(string commandToSend, SemaphoreSlim semaphore)
        {
            Send(commandToSend);
            await semaphore.WaitAsync();
        }

        public bool Usinewgame()
        {
            Send("usinewgame");
            return true;
        }

        public bool Position(List<string> moves)
        {
            Send(string.Join(" ", new[] { "position", "startpos", "moves" }.Concat(moves)));
            return true;
        }

        public bool Go(Dictionary<string, int> nameAndValues)
        {
            var command = new List<string> { "go" };
            foreach (var nameAndValue in nameAndValues)
            {
                command.Add(nameAndValue.Key);
                command.Add(nameAndValue.Value.ToString());
            }

            var commandString = string.Join(" ", command);
            Send(commandString);
            //Trace.WriteLine(commandString);
            return true;
        }

        public bool Key()
        {
            return KeyAsync().Wait(ProcessWaitTime);
        }

        private async Task KeyAsync()
        {
            await SendAndReceive("key", HashSemaphore);
        }

        public override string ToString()
        {
            return $"ProcessIndex={ProcessIndex} GameIndex={GameIndex} EngineIndex={EngineIndex}";
        }
    }
}