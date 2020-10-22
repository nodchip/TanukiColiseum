using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TanukiColiseum
{
    public class MainViewModel
    {
        public ReactiveProperty<string> Engine1FilePath { get; private set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Engine2FilePath { get; private set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Eval1FolderPath { get; private set; } = new ReactiveProperty<string>("eval");

        public ReactiveProperty<string> Eval2FolderPath { get; private set; } = new ReactiveProperty<string>("eval");

        [RangeAttribute(1, int.MaxValue)]
        public ReactiveProperty<int> NumConcurrentGames { get; private set; } = new ReactiveProperty<int>(1);

        [RangeAttribute(1, int.MaxValue)]
        public ReactiveProperty<int> NumGames { get; private set; } = new ReactiveProperty<int>(1000);

        [RangeAttribute(1, 1048576)]
        public ReactiveProperty<int> HashMb { get; private set; } = new ReactiveProperty<int>(16);

        [RangeAttribute(0, 10000)]
        public ReactiveProperty<int> NumBookMoves1 { get; private set; } = new ReactiveProperty<int>(16);

        [RangeAttribute(0, 10000)]
        public ReactiveProperty<int> NumBookMoves2 { get; private set; } = new ReactiveProperty<int>(16);

        public ReactiveProperty<string> BookFileName1 { get; private set; } = new ReactiveProperty<string>("standard_book.db");

        public ReactiveProperty<string> BookFileName2 { get; private set; } = new ReactiveProperty<string>("standard_book.db");

        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> NumBookMoves { get; private set; } = new ReactiveProperty<int>(24);

        public ReactiveProperty<string> SfenFilePath { get; private set; } = new ReactiveProperty<string>("records2016_10818.sfen");

        /// <summary>
        /// 思考エンジン1に渡す思考ノード数。
        /// <para>0が渡された場合、ノード数を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Nodes1 { get; private set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン2に渡す思考ノード数。
        /// <para>0が渡された場合、ノード数を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Nodes2 { get; private set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン1に渡す思考時間。
        /// <para>0が渡された場合、思考時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Time1 { get; private set; } = new ReactiveProperty<int>(1000);

        /// <summary>
        /// 思考エンジン2に渡す思考時間。
        /// <para>0が渡された場合、思考時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Time2 { get; private set; } = new ReactiveProperty<int>(1000);

        [RangeAttribute(1, int.MaxValue)]
        public ReactiveProperty<int> NumNumaNodes { get; private set; } = new ReactiveProperty<int>(1);

        [RangeAttribute(1, int.MaxValue)]
        public ReactiveProperty<int> ProgressIntervalMs { get; private set; } = new ReactiveProperty<int>(1);

        [RangeAttribute(1, 512)]
        public ReactiveProperty<int> NumThreads1 { get; private set; } = new ReactiveProperty<int>(1);

        [RangeAttribute(1, 512)]
        public ReactiveProperty<int> NumThreads2 { get; private set; } = new ReactiveProperty<int>(1);

        [RangeAttribute(1, 99999)]
        public ReactiveProperty<int> BookEvalDiff1 { get; private set; } = new ReactiveProperty<int>(30);

        [RangeAttribute(1, 99999)]
        public ReactiveProperty<int> BookEvalDiff2 { get; private set; } = new ReactiveProperty<int>(30);

        public ReactiveProperty<string> ConsiderBookMoveCount1 { get; private set; } = new ReactiveProperty<string>("false");

        public ReactiveProperty<string> ConsiderBookMoveCount2 { get; private set; } = new ReactiveProperty<string>("false");

        public ReactiveProperty<string> IgnoreBookPly1 { get; private set; } = new ReactiveProperty<string>("false");

        public ReactiveProperty<string> IgnoreBookPly2 { get; private set; } = new ReactiveProperty<string>("false");

        public ReactiveProperty<string> State { get; private set; } = new ReactiveProperty<string>();

        public ReactiveCommand OnSfenFilePathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnEngine1FilePathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnEngine2FilePathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnEval1FolderPathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnEval2FolderPathButton { get; private set; } = new ReactiveCommand();

        public MainViewModel()
        {
            OnSfenFilePathButton.Subscribe(() => SelectFilePath(SfenFilePath));
            OnEngine1FilePathButton.Subscribe(() => SelectFilePath(Engine1FilePath));
            OnEngine2FilePathButton.Subscribe(() => SelectFilePath(Engine2FilePath));
            OnEval1FolderPathButton.Subscribe(() => SelectFolderPath(Eval1FolderPath));
            OnEval2FolderPathButton.Subscribe(() => SelectFolderPath(Eval2FolderPath));
        }

        private static void SelectFilePath(ReactiveProperty<string> property)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            property.Value = dialog.FileName;
        }

        private static void SelectFolderPath(ReactiveProperty<string> property)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            property.Value = dialog.SelectedPath;
        }
    }
}
