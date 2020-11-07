using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TanukiColiseum
{
    /// <summary>
    /// 設定項目を保持するモデルクラス
    /// </summary>
    public class MainModel : INotifyPropertyChanged
    {
        public ReactiveProperty<string> Engine1FilePath { get; } = new ReactiveProperty<string>("");

        public ReactiveProperty<string> Engine2FilePath { get; } = new ReactiveProperty<string>("");

        public ReactiveProperty<string> Eval1FolderPath { get; } = new ReactiveProperty<string>("eval");

        public ReactiveProperty<string> Eval2FolderPath { get; } = new ReactiveProperty<string>("eval");

        public ReactiveProperty<int> NumConcurrentGames { get; } = new ReactiveProperty<int>(1);

        public ReactiveProperty<int> NumGames { get; } = new ReactiveProperty<int>(1000);

        public ReactiveProperty<int> HashMb { get; } = new ReactiveProperty<int>(16);

        public ReactiveProperty<int> NumBookMoves1 { get; } = new ReactiveProperty<int>(16);

        public ReactiveProperty<int> NumBookMoves2 { get; } = new ReactiveProperty<int>(16);

        public ReactiveProperty<string> BookFileName1 { get; } = new ReactiveProperty<string>("standard_book.db");

        public ReactiveProperty<string> BookFileName2 { get; } = new ReactiveProperty<string>("standard_book.db");

        public ReactiveProperty<int> NumBookMoves { get; } = new ReactiveProperty<int>(24);

        public ReactiveProperty<string> SfenFilePath { get; } = new ReactiveProperty<string>("records2016_10818.sfen");

        /// <summary>
        /// 思考エンジン1に渡す思考ノード数。
        /// <para>0が渡された場合、ノード数を指定しない。</para>
        /// </summary>
        public ReactiveProperty<int> Nodes1 { get; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン2に渡す思考ノード数。
        /// <para>0が渡された場合、ノード数を指定しない。</para>
        /// </summary>
        public ReactiveProperty<int> Nodes2 { get; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン1に渡す思考時間。
        /// <para>0が渡された場合、思考時間を指定しない。</para>
        /// </summary>
        public ReactiveProperty<int> Time1 { get; } = new ReactiveProperty<int>(1000);

        /// <summary>
        /// 思考エンジン2に渡す思考時間。
        /// <para>0が渡された場合、思考時間を指定しない。</para>
        /// </summary>
        public ReactiveProperty<int> Time2 { get; } = new ReactiveProperty<int>(1000);

        public ReactiveProperty<int> NumNumaNodes { get; } = new ReactiveProperty<int>(1);

        public ReactiveProperty<int> ProgressIntervalMs { get; } = new ReactiveProperty<int>(1);

        public ReactiveProperty<int> NumThreads1 { get; } = new ReactiveProperty<int>(1);

        public ReactiveProperty<int> NumThreads2 { get; } = new ReactiveProperty<int>(1);

        public ReactiveProperty<int> BookEvalDiff1 { get; } = new ReactiveProperty<int>(30);

        public ReactiveProperty<int> BookEvalDiff2 { get; } = new ReactiveProperty<int>(30);

        public ReactiveProperty<string> ConsiderBookMoveCount1 { get; } = new ReactiveProperty<string>("false");

        public ReactiveProperty<string> ConsiderBookMoveCount2 { get; } = new ReactiveProperty<string>("false");

        public ReactiveProperty<string> IgnoreBookPly1 { get; } = new ReactiveProperty<string>("false");

        public ReactiveProperty<string> IgnoreBookPly2 { get; } = new ReactiveProperty<string>("false");

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
