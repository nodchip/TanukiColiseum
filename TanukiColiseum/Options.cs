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
        public int Nodes1 { get; set; }
        public int Nodes2 { get; set; }
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
    }
}
