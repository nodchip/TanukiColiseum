namespace TanukiColiseum
{
    public class Status
    {
        public int NumGames { get; set; }
        public int NumThreads { get; set; }
        public int[] Nodes { get; set; }
        // [エンジン][先手・後手]
        public int[,] Win { get; } = { { 0, 0 }, { 0, 0 }, };
        // [エンジン][先手・後手]
        public int[,] DeclarationWin { get; } = { { 0, 0 }, { 0, 0 } };
        // NumDraw[i] = エンジンiが先手の時の引き分けの回数
        public int[] NumDraw { get; } = { 0, 0 };

        public Status() { }

        public Status(Status status)
        {
            NumGames = status.NumGames;
            NumThreads = status.NumThreads;
            Nodes = status.Nodes;
            for (int engine = 0; engine < 2; ++engine)
            {
                for (int blackWhite = 0; blackWhite < 2; ++blackWhite)
                {
                    Win[engine, blackWhite] = status.Win[engine, blackWhite];
                    DeclarationWin[engine, blackWhite] = status.DeclarationWin[engine, blackWhite];
                }
                NumDraw[engine] = status.NumDraw[engine];
            }
        }
    }
}
