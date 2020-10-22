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
        public ReactiveProperty<string> Engine1FilePath { get; private set; } = new ReactiveProperty<string>("");

        public ReactiveProperty<string> Engine2FilePath { get; private set; } = new ReactiveProperty<string>("");

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

        public ReactiveProperty<bool> StartButtonEnabled { get; private set; } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<string> State { get; private set; } = new ReactiveProperty<string>("");

        public ReactiveProperty<int> ProgressBarValue { get; private set; } = new ReactiveProperty<int>();

        public ReactiveProperty<int> ProgressBarMinimum { get; private set; } = new ReactiveProperty<int>();

        public ReactiveProperty<int> ProgressBarMaximum { get; private set; } = new ReactiveProperty<int>();

        public ReactiveCommand OnSfenFilePathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnEngine1FilePathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnEngine2FilePathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnEval1FolderPathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnEval2FolderPathButton { get; private set; } = new ReactiveCommand();

        public ReactiveCommand OnStartButton { get; private set; } = new ReactiveCommand();

        public MainViewModel()
        {
            OnSfenFilePathButton.Subscribe(() => SelectFilePath(SfenFilePath));
            OnEngine1FilePathButton.Subscribe(() => SelectFilePath(Engine1FilePath));
            OnEngine2FilePathButton.Subscribe(() => SelectFilePath(Engine2FilePath));
            OnEval1FolderPathButton.Subscribe(() => SelectFolderPath(Eval1FolderPath));
            OnEval2FolderPathButton.Subscribe(() => SelectFolderPath(Eval2FolderPath));
            OnStartButton.Subscribe(() => OnStart());
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

        private void OnStart()
        {
            var options = new Options
            {
                Engine1FilePath = Engine1FilePath.Value,
                Engine2FilePath = Engine2FilePath.Value,
                Eval1FolderPath = Eval1FolderPath.Value,
                Eval2FolderPath = Eval2FolderPath.Value,
                NumConcurrentGames = NumConcurrentGames.Value,
                NumGames = NumGames.Value,
                HashMb = HashMb.Value,
                NumBookMoves1 = NumBookMoves1.Value,
                NumBookMoves2 = NumBookMoves2.Value,
                BookFileName1 = BookFileName1.Value,
                BookFileName2 = BookFileName2.Value,
                NumBookMoves = NumBookMoves.Value,
                SfenFilePath = SfenFilePath.Value,
                Nodes1 = Nodes1.Value,
                Nodes2 = Nodes2.Value,
                Time1 = Time1.Value,
                Time2 = Time2.Value,
                NumNumaNodes = NumNumaNodes.Value,
                ProgressIntervalMs = ProgressIntervalMs.Value,
                NumThreads1 = NumThreads1.Value,
                NumThreads2 = NumThreads2.Value,
                BookEvalDiff1 = BookEvalDiff1.Value,
                BookEvalDiff2 = BookEvalDiff2.Value,
                ConsiderBookMoveCount1 = ConsiderBookMoveCount1.Value,
                ConsiderBookMoveCount2 = ConsiderBookMoveCount2.Value,
                IgnoreBookPly1 = IgnoreBookPly1.Value,
                IgnoreBookPly2 = IgnoreBookPly2.Value,
                Gui = true,
            };

            ProgressBarValue.Value = 0;
            ProgressBarMinimum.Value = 0;
            ProgressBarMaximum.Value = NumGames.Value;

            var coliseum = new Coliseum();
            coliseum.OnStatusChanged += ShowResult;
            coliseum.OnError += OnError;

            Task.Run(() =>
            {
                StartButtonEnabled.Value = false;
                coliseum.Run(options);
                StartButtonEnabled.Value = true;
            });
        }

        private void ShowResult(Status status)
        {
            State.Value = status.ToHumanReadableString();
            ProgressBarValue.Value = status.NumFinishedGames;
        }

        public void OnError(string errorMessage)
        {
            State.Value = errorMessage;
        }
    }
}
