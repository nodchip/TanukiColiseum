using System;
using System.IO;

namespace TanukiColiseum
{
	public class Options
	{
		public string Engine1FilePath { get; set; }
		public string Engine2FilePath { get; set; }
		public string Eval1FolderPath { get; set; }
		public string Eval2FolderPath { get; set; }
		public int NumConcurrentGames { get; set; }
		public int NumGames { get; set; }
		public int HashMb { get; set; }
		public int NumBookMoves1 { get; set; }
		public int NumBookMoves2 { get; set; }
		public string BookFileName1 { get; set; }
		public string BookFileName2 { get; set; }
		public int NumBookMoves { get; set; }

		/// <summary>
		/// 最大手数。この手数を超えて対局が続く場合は引き分けとなる。
		/// </summary>
		public int MaxMovesToDraw { get; set; }

		public string SfenFilePath { get; set; }

		/// <summary>
		/// 思考エンジン1に渡す思考ノード数。
		/// <para>0が渡された場合、ノード数を指定しない。</para>
		/// </summary>
		public int Nodes1 { get; set; }

		/// <summary>
		/// 思考エンジン2に渡す思考ノード数。
		/// <para>0が渡された場合、ノード数を指定しない。</para>
		/// </summary>
		public int Nodes2 { get; set; }

		/// <summary>
		/// 思考ノード数に加える乱数(%)。
		/// <para>0が渡されたとき、思考ノード数に乱数を加えない</para>
		/// </summary>
		public int NodesRandomPercent1 { get; set; }

		/// <summary>
		/// 思考ノード数に加える乱数(%)。
		/// <para>0が渡されたとき、思考ノード数に乱数を加えない</para>
		/// </summary>
		public int NodesRandomPercent2 { get; set; }

		/// <summary>
		/// 思考ノード数の乱数を1手毎に変化させる。
		/// <para>falseの場合、1局を通して同じ値を加える</para>
		/// </summary>
		public bool NodesRandomEveryMove1 { get; set; }

		/// <summary>
		/// 思考ノード数の乱数を1手毎に変化させる。
		/// <para>falseの場合、1局を通して同じ値を加える</para>
		/// </summary>
		public bool NodesRandomEveryMove2 { get; set; }

		/// <summary>
		/// 思考エンジン1に渡す持ち時間。
		/// <para>0が渡された場合、持ち時間を指定しない。</para>
		/// </summary>
		public int Time1 { get; set; }

		/// <summary>
		/// 思考エンジン2に渡す持ち時間。
		/// <para>0が渡された場合、持ち時間を指定しない。</para>
		/// </summary>
		public int Time2 { get; set; }

		/// <summary>
		/// 思考エンジン1に渡す秒読み時間。
		/// <para>0が渡された場合、秒読み時間を指定しない。</para>
		/// </summary>
		public int Byoyomi1 { get; set; }

		/// <summary>
		/// 思考エンジン2に渡す秒読み時間。
		/// <para>0が渡された場合、秒読み時間を指定しない。</para>
		/// </summary>
		public int Byoyomi2 { get; set; }

		/// <summary>
		/// 思考エンジン1に渡す加算時間。
		/// <para>0が渡された場合、加算時間を指定しない。</para>
		/// </summary>
		public int Inc1 { get; set; }

		/// <summary>
		/// 思考エンジン2に渡す加算時間。
		/// <para>0が渡された場合、加算時間を指定しない。</para>
		/// </summary>
		public int Inc2 { get; set; }

		/// <summary>
		/// 思考エンジン1に渡す乱数付き思考時間。
		/// <para>0が渡された場合、乱数付き思考時間を指定しない。</para>
		/// </summary>
		public int Rtime1 { get; set; }

		/// <summary>
		/// 思考エンジン2に渡す乱数付き思考時間。
		/// <para>0が渡された場合、乱数付き思考時間を指定しない。</para>
		/// </summary>
		public int Rtime2 { get; set; }

		public int NumNumaNodes { get; set; }
		public int ProgressIntervalMs { get; set; }
		public int NumThreads1 { get; set; }
		public int NumThreads2 { get; set; }
		public int BookEvalDiff1 { get; set; }
		public int BookEvalDiff2 { get; set; }
		public string ConsiderBookMoveCount1 { get; set; }
		public string ConsiderBookMoveCount2 { get; set; }
		public string IgnoreBookPly1 { get; set; }
		public string IgnoreBookPly2 { get; set; }
		public int SlowMover1 { get; set; }
		public int SlowMover2 { get; set; }
		public int DrawValue1 { get; set; }
		public int DrawValue2 { get; set; }
		public int BookEvalBlackLimit1 { get; set; }
		public int BookEvalBlackLimit2 { get; set; }
		public int BookEvalWhiteLimit1 { get; set; }
		public int BookEvalWhiteLimit2 { get; set; }
		public int FVScale1 { get; set; }
		public int FVScale2 { get; set; }
		public int Depth1 { get; set; }
		public int Depth2 { get; set; }
		public int MinimumThinkingTime1 { get; set; }
		public int MinimumThinkingTime2 { get; set; }
		public bool Gui { get; set; }

		public MainModel ToModel()
		{
			var model = new MainModel();
			model.Engine1FilePath.Value = Engine1FilePath;
			model.Engine2FilePath.Value = Engine2FilePath;
			model.Eval1FolderPath.Value = Eval1FolderPath;
			model.Eval2FolderPath.Value = Eval2FolderPath;
			model.NumConcurrentGames.Value = NumConcurrentGames;
			model.NumGames.Value = NumGames;
			model.HashMb.Value = HashMb;
			model.NumBookMoves1.Value = NumBookMoves1;
			model.NumBookMoves2.Value = NumBookMoves2;
			model.BookFileName1.Value = BookFileName1;
			model.BookFileName2.Value = BookFileName2;
			model.NumBookMoves.Value = NumBookMoves;
			model.MaxMovesToDraw.Value = MaxMovesToDraw;
			model.SfenFilePath.Value = SfenFilePath;
			model.Nodes1.Value = Nodes1;
			model.Nodes2.Value = Nodes2;
			model.NodesRandomPercent1.Value = NodesRandomPercent1;
			model.NodesRandomPercent2.Value = NodesRandomPercent2;
			model.NodesRandomEveryMove1.Value = NodesRandomEveryMove1;
			model.NodesRandomEveryMove2.Value = NodesRandomEveryMove2;
			model.Time1.Value = Time1;
			model.Time2.Value = Time2;
			model.Byoyomi1.Value = Byoyomi1;
			model.Byoyomi2.Value = Byoyomi2;
			model.Inc1.Value = Inc1;
			model.Inc2.Value = Inc2;
			model.Rtime1.Value = Rtime1;
			model.Rtime2.Value = Rtime2;
			model.NumNumaNodes.Value = NumNumaNodes;
			model.ProgressIntervalMs.Value = ProgressIntervalMs;
			model.NumThreads1.Value = NumThreads1;
			model.NumThreads2.Value = NumThreads2;
			model.BookEvalDiff1.Value = BookEvalDiff1;
			model.BookEvalDiff2.Value = BookEvalDiff2;
			model.ConsiderBookMoveCount1.Value = ConsiderBookMoveCount1;
			model.ConsiderBookMoveCount2.Value = ConsiderBookMoveCount2;
			model.IgnoreBookPly1.Value = IgnoreBookPly1;
			model.IgnoreBookPly2.Value = IgnoreBookPly2;
			model.SlowMover1.Value = SlowMover1;
			model.SlowMover2.Value = SlowMover2;
			model.DrawValue1.Value = DrawValue1;
			model.DrawValue2.Value = DrawValue2;
			model.BookEvalBlackLimit1.Value = BookEvalBlackLimit1;
			model.BookEvalBlackLimit2.Value = BookEvalBlackLimit2;
			model.BookEvalWhiteLimit1.Value = BookEvalWhiteLimit1;
			model.BookEvalWhiteLimit2.Value = BookEvalWhiteLimit2;
			model.FVScale1.Value = FVScale1;
			model.FVScale2.Value = FVScale2;
			model.Depth1.Value = Depth1;
			model.Depth2.Value = Depth2;
			model.MinimumThinkingTime1.Value = MinimumThinkingTime1;
			model.MinimumThinkingTime2.Value = MinimumThinkingTime2;
			return model;
		}

		public string ToHumanReadableString(Engine engine1, Engine engine2)
		{
			return $@"対局数={NumGames} 同時対局数={NumConcurrentGames} ハッシュサイズ={HashMb} 開始手数={NumBookMoves} 最大手数={MaxMovesToDraw} 開始局面ファイル={SfenFilePath} NUMAノード数={NumNumaNodes} 表示更新間隔(ms)={ProgressIntervalMs}
思考エンジン1 name={engine1.Name} author={engine1.Author} exeファイル={Engine1FilePath} 評価関数フォルダパス={Eval1FolderPath} 定跡手数={NumBookMoves1} 定跡ファイル名={BookFileName1} 思考ノード数={Nodes1} 思考ノード数に加える乱数(%)={NodesRandomPercent1} 思考ノード数の乱数を1手毎に変化させる={NodesRandomEveryMove1} 持ち時間(ms)={Time1} 秒読み時間(ms)={Byoyomi1} 加算時間(ms)={Inc1} 乱数付き思考時間(ms)={Rtime1} スレッド数={NumThreads1} BookEvalDiff={BookEvalDiff1} 定跡の採択率を考慮する={ConsiderBookMoveCount1} 定跡の手数を無視する={IgnoreBookPly1} SlowMover={SlowMover1} DrawValue={DrawValue1} BookEvalBlackLimit={BookEvalBlackLimit1} BookEvalWhiteLimit={BookEvalWhiteLimit1} FVScale1={FVScale1} Depth1={Depth1} MinimumThinkingTime1={MinimumThinkingTime1}
思考エンジン2 name={engine2.Name} author={engine2.Author} exeファイル={Engine2FilePath} 評価関数フォルダパス={Eval2FolderPath} 定跡手数={NumBookMoves2} 定跡ファイル名={BookFileName2} 思考ノード数={Nodes2} 思考ノード数に加える乱数(%)={NodesRandomPercent2} 思考ノード数の乱数を1手毎に変化させる={NodesRandomEveryMove2} 持ち時間(ms)={Time2} 秒読み時間(ms)={Byoyomi2} 加算時間(ms)={Inc2} 乱数付き思考時間(ms)={Rtime2} スレッド数={NumThreads2} BookEvalDiff={BookEvalDiff2} 定跡の採択率を考慮する={ConsiderBookMoveCount2} 定跡の手数を無視する={IgnoreBookPly2} SlowMover={SlowMover2} DrawValue={DrawValue2} BookEvalBlackLimit={BookEvalBlackLimit2} BookEvalWhiteLimit={BookEvalWhiteLimit2} FVScale2={FVScale2} Depth2={Depth2} MinimumThinkingTime2={MinimumThinkingTime2}
";
		}
	}
}
