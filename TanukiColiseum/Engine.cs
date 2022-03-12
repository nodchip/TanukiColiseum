using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TanukiColiseum
{
	public class Engine
	{
		private static readonly TimeSpan ProcessWaitTime = TimeSpan.FromMinutes(1);
		private Process Process = new Process();

		private TaskCompletionSource<bool> UsiTaskCompletionSource;
		private TaskCompletionSource<bool> IsreadyTaskCompletionSource;
		private TaskCompletionSource<Move> GoTaskCompletionSource;
		private int ProcessIndex;
		private int GameIndex;
		private int EngineIndex;
		private Dictionary<string, string> OverriddenOptions;
		public string Name { get; set; }
		public string Author { get; set; }
		private List<string> lastInfoCommand;
		private readonly SemaphoreSlim sendAsyncSemaphore = new SemaphoreSlim(1, 1);

		public Engine(string fileName, int processIndex, int gameIndex, int engineIndex, int numaNode, Dictionary<string, string> overriddenOptions)
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
			this.ProcessIndex = processIndex;
			this.GameIndex = gameIndex;
			this.EngineIndex = engineIndex;
			this.OverriddenOptions = overriddenOptions;

			// 同期的に実行した場合、
			// BeginOutputReadLine()/BeginErrorReadLine()が呼び出されたあと
			// 読み込みスレッドが動かない。
			// これを避けるため、思考エンジンへのコマンド送信時は、非同期で書き込みを行っている。
			Process.Start();
			Process.BeginOutputReadLine();
			Process.BeginErrorReadLine();
		}

		public async Task Finish()
		{
			await SendAsync("quit");
			Process.WaitForExit();
			Process.Dispose();
		}

		/// <summary>
		/// エンジンにコマンドを送信する
		/// </summary>
		/// <param name="command"></param>
		private async Task SendAsync(string command)
		{
			//Debug.WriteLine($"> [{ProcessIndex}] {command}");

			await sendAsyncSemaphore.WaitAsync().ConfigureAwait(false);
			try
			{
				await Process.StandardInput.WriteLineAsync(command);
				await Process.StandardInput.FlushAsync();
			}
			finally
			{
				sendAsyncSemaphore.Release();
			}
		}

		/// <summary>
		/// 思考エンジンの標準出力を処理する
		/// </summary>
		/// <param name="sender">出力を送ってきた思考エンジンのプロセス</param>
		/// <param name="e">思考エンジンの出力</param>
		private async void HandleStdout(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null)
			{
				return;
			}

			//Debug.WriteLine($"< [{ProcessIndex}] {e.Data}");

			List<string> command = Util.Split(e.Data);
			if (command.Contains("id"))
			{
				HandleId(command);
			}
			else if (command.Contains("usiok"))
			{
				HandleUsiok(command);
			}
			else if (command.Contains("option"))
			{
				await HandleOption(command);
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
			UsiTaskCompletionSource.SetResult(true);
		}

		private async Task HandleOption(List<string> command)
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
			await SendAsync($"setoption name {name} value {value}");
		}

		private void HandleReadyok(List<string> command)
		{
			// readyokを待機している関数を進行する
			IsreadyTaskCompletionSource.SetResult(true);
		}

		private void HandleBestmove(List<string> command)
		{
			if (command[1] == "resign")
			{
				GoTaskCompletionSource.SetResult(new Move { Resign = true });
				return;
			}
			else if (command[1] == "win")
			{
				GoTaskCompletionSource.SetResult(new Move { Win = true });
				return;
			}

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

			GoTaskCompletionSource.SetResult(move);
		}

		public void HandleInfo(List<string> command)
		{
			lastInfoCommand = command;
		}

		public async Task<bool> UsiAsync()
		{
			UsiTaskCompletionSource = new TaskCompletionSource<bool>();
			await SendAsync("usi");
			return await Task.WhenAny(UsiTaskCompletionSource.Task, Task.Delay(ProcessWaitTime)) == UsiTaskCompletionSource.Task;
		}

		public async Task<bool> IsreadyAsync()
		{
			IsreadyTaskCompletionSource = new TaskCompletionSource<bool>();
			await SendAsync("isready");
			return await Task.WhenAny(IsreadyTaskCompletionSource.Task, Task.Delay(ProcessWaitTime)) == IsreadyTaskCompletionSource.Task;
		}

		public async Task UsinewgameAsync()
		{
			await SendAsync("usinewgame");
		}

		public async Task PositionAsync(List<string> moves)
		{
			await SendAsync(string.Join(" ", new[] { "position", "startpos", "moves" }.Concat(moves)));
		}

		public async Task<Move> GoAsync(Dictionary<string, int> nameAndValues)
		{
			GoTaskCompletionSource = new TaskCompletionSource<Move>();

			var command = new List<string> { "go" };
			foreach (var nameAndValue in nameAndValues)
			{
				command.Add(nameAndValue.Key);
				command.Add(nameAndValue.Value.ToString());
			}

			var commandString = string.Join(" ", command);
			await SendAsync(commandString);
			//Trace.WriteLine(commandString);
			return await GoTaskCompletionSource.Task;
		}

		public override string ToString()
		{
			return $"ProcessIndex={ProcessIndex} GameIndex={GameIndex} EngineIndex={EngineIndex}";
		}
	}
}
