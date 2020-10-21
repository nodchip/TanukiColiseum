using System;

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
    }
}
