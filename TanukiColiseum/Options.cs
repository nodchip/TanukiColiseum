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
        /// 思考エンジン1に渡す思考時間。
        /// <para>0が渡された場合、思考時間を指定しない。</para>
        /// </summary>
        public int Time1 { get; set; }

        /// <summary>
        /// 思考エンジン2に渡す思考時間。
        /// <para>0が渡された場合、思考時間を指定しない。</para>
        /// </summary>
        public int Time2 { get; set; }
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
            model.SfenFilePath.Value = SfenFilePath;
            model.Nodes1.Value = Nodes1;
            model.Nodes2.Value = Nodes2;
            model.Time1.Value = Time1;
            model.Time2.Value = Time2;
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
            return model;
        }

        public string ToHumanReadableString(Engine engine1, Engine engine2)
        {
            return $@"対局数={NumGames} 同時対局数={NumConcurrentGames} ハッシュサイズ={HashMb} 開始手数={NumBookMoves} 開始局面ファイル={SfenFilePath} NUMAノード数={NumNumaNodes} 表示更新間隔(ms)={ProgressIntervalMs}
思考エンジン1 name={engine1.Name} author={engine1.Author} exeファイル={Engine1FilePath} 評価関数フォルダパス={Eval1FolderPath} 定跡手数={NumBookMoves1} 定跡ファイル名={BookFileName1} 思考ノード数={Nodes1} 思考時間(ms)={Time1} スレッド数={NumThreads1} BookEvalDiff={BookEvalDiff1} 定跡の採択率を考慮する={ConsiderBookMoveCount1} 定跡の手数を無視する={IgnoreBookPly1}
思考エンジン2 name={engine2.Name} author={engine2.Author} exeファイル={Engine2FilePath} 評価関数フォルダパス={Eval2FolderPath} 定跡手数={NumBookMoves2} 定跡ファイル名={BookFileName2} 思考ノード数={Nodes2} 思考時間(ms)={Time2} スレッド数={NumThreads2} BookEvalDiff={BookEvalDiff2} 定跡の採択率を考慮する={ConsiderBookMoveCount2} 定跡の手数を無視する={IgnoreBookPly2}
";
        }
    }
}
