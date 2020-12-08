using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TanukiColiseum
{
    public class MainViewModel
    {
        private const string SettingFileName = "TanukiColiseum.setting.xml";
        private const string LogFolderName = "log";

        public ReactiveProperty<string> Engine1FilePath { get; }

        public ReactiveProperty<string> Engine2FilePath { get; }

        public ReactiveProperty<string> Eval1FolderPath { get; }

        public ReactiveProperty<string> Eval2FolderPath { get; }

        [RangeAttribute(1, int.MaxValue)]
        public ReactiveProperty<int> NumConcurrentGames { get; }

        [RangeAttribute(1, int.MaxValue)]
        public ReactiveProperty<int> NumGames { get; }

        [RangeAttribute(1, 1048576)]
        public ReactiveProperty<int> HashMb { get; }

        [RangeAttribute(0, 10000)]
        public ReactiveProperty<int> NumBookMoves1 { get; }

        [RangeAttribute(0, 10000)]
        public ReactiveProperty<int> NumBookMoves2 { get; }

        public ReactiveProperty<string> BookFileName1 { get; }

        public ReactiveProperty<string> BookFileName2 { get; }

        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> NumBookMoves { get; }

        public ReactiveProperty<string> SfenFilePath { get; }

        /// <summary>
        /// 思考エンジン1に渡す思考ノード数。
        /// <para>0が渡された場合、ノード数を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Nodes1 { get; }

        /// <summary>
        /// 思考エンジン2に渡す思考ノード数。
        /// <para>0が渡された場合、ノード数を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Nodes2 { get; }

        /// <summary>
        /// 思考エンジン1に渡す持ち時間。
        /// <para>0が渡された場合、持ち時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Time1 { get; }

        /// <summary>
        /// 思考エンジン2に渡す持ち時間。
        /// <para>0が渡された場合、持ち時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Time2 { get; }

        /// <summary>
        /// 思考エンジン1に渡す秒読み時間。
        /// <para>0が渡された場合、秒読み時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Byoyomi1 { get; }

        /// <summary>
        /// 思考エンジン2に渡す秒読み時間。
        /// <para>0が渡された場合、秒読み時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Byoyomi2 { get; }

        /// <summary>
        /// 思考エンジン1に渡す加算時間。
        /// <para>0が渡された場合、加算時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Inc1 { get; }

        /// <summary>
        /// 思考エンジン2に渡す加算時間。
        /// <para>0が渡された場合、加算時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Inc2 { get; }

        /// <summary>
        /// 思考エンジン1に渡す乱数付き思考時間。
        /// <para>0が渡された場合、乱数付き思考時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Rtime1 { get; }

        /// <summary>
        /// 思考エンジン2に渡す乱数付き思考時間。
        /// <para>0が渡された場合、乱数付き思考時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Rtime2 { get; }

        [RangeAttribute(1, int.MaxValue)]
        public ReactiveProperty<int> NumNumaNodes { get; }

        [RangeAttribute(1, int.MaxValue)]
        public ReactiveProperty<int> ProgressIntervalMs { get; }

        [RangeAttribute(1, 512)]
        public ReactiveProperty<int> NumThreads1 { get; }

        [RangeAttribute(1, 512)]
        public ReactiveProperty<int> NumThreads2 { get; }

        [RangeAttribute(1, 99999)]
        public ReactiveProperty<int> BookEvalDiff1 { get; }

        [RangeAttribute(1, 99999)]
        public ReactiveProperty<int> BookEvalDiff2 { get; }

        public ReactiveProperty<string> ConsiderBookMoveCount1 { get; }

        public ReactiveProperty<string> ConsiderBookMoveCount2 { get; }

        public ReactiveProperty<string> IgnoreBookPly1 { get; }

        public ReactiveProperty<string> IgnoreBookPly2 { get; }

        public ReactiveProperty<bool> StartMenuItemEnabled { get; } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<string> State { get; } = new ReactiveProperty<string>("");

        public ReactiveProperty<int> ProgressBarValue { get; } = new ReactiveProperty<int>();

        public ReactiveProperty<int> ProgressBarMinimum { get; } = new ReactiveProperty<int>();

        public ReactiveProperty<int> ProgressBarMaximum { get; } = new ReactiveProperty<int>();

        public ReactiveCommand OnSfenFilePathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnEngine1FilePathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnEngine2FilePathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnEval1FolderPathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnEval2FolderPathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnNewMenuItem { get; } = new ReactiveCommand();

        public ReactiveCommand OnOpenMenuItem { get; } = new ReactiveCommand();

        public ReactiveCommand OnSaveMenuItem { get; } = new ReactiveCommand();

        public ReactiveCommand OnSaveAsMenuItem { get; } = new ReactiveCommand();

        public ReactiveCommand OnExitMenuItem { get; } = new ReactiveCommand();

        public ReactiveCommand OnStartMenuItem { get; } = new ReactiveCommand();

        public Action CloseAction { get; set; }

        public MainModel model { get; } = new MainModel();

        private string filePath;

        public MainViewModel()
        {
            if (File.Exists(SettingFileName))
            {
                model.Load(SettingFileName);
            }

            Engine1FilePath = model.Engine1FilePath.ToReactivePropertyAsSynchronized(x => x.Value);
            Engine2FilePath = model.Engine2FilePath.ToReactivePropertyAsSynchronized(x => x.Value);
            Eval1FolderPath = model.Eval1FolderPath.ToReactivePropertyAsSynchronized(x => x.Value);
            Eval2FolderPath = model.Eval2FolderPath.ToReactivePropertyAsSynchronized(x => x.Value);
            NumConcurrentGames = model.NumConcurrentGames.ToReactivePropertyAsSynchronized(x => x.Value);
            NumGames = model.NumGames.ToReactivePropertyAsSynchronized(x => x.Value);
            HashMb = model.HashMb.ToReactivePropertyAsSynchronized(x => x.Value);
            NumBookMoves1 = model.NumBookMoves1.ToReactivePropertyAsSynchronized(x => x.Value);
            NumBookMoves2 = model.NumBookMoves2.ToReactivePropertyAsSynchronized(x => x.Value);
            BookFileName1 = model.BookFileName1.ToReactivePropertyAsSynchronized(x => x.Value);
            BookFileName2 = model.BookFileName2.ToReactivePropertyAsSynchronized(x => x.Value);
            NumBookMoves = model.NumBookMoves.ToReactivePropertyAsSynchronized(x => x.Value);
            SfenFilePath = model.SfenFilePath.ToReactivePropertyAsSynchronized(x => x.Value);
            Nodes1 = model.Nodes1.ToReactivePropertyAsSynchronized(x => x.Value);
            Nodes2 = model.Nodes2.ToReactivePropertyAsSynchronized(x => x.Value);
            Time1 = model.Time1.ToReactivePropertyAsSynchronized(x => x.Value);
            Time2 = model.Time2.ToReactivePropertyAsSynchronized(x => x.Value);
            Byoyomi1 = model.Byoyomi1.ToReactivePropertyAsSynchronized(x => x.Value);
            Byoyomi2 = model.Byoyomi2.ToReactivePropertyAsSynchronized(x => x.Value);
            Inc1 = model.Inc1.ToReactivePropertyAsSynchronized(x => x.Value);
            Inc2 = model.Inc2.ToReactivePropertyAsSynchronized(x => x.Value);
            Rtime1 = model.Rtime1.ToReactivePropertyAsSynchronized(x => x.Value);
            Rtime2 = model.Rtime2.ToReactivePropertyAsSynchronized(x => x.Value);
            NumNumaNodes = model.NumNumaNodes.ToReactivePropertyAsSynchronized(x => x.Value);
            ProgressIntervalMs = model.ProgressIntervalMs.ToReactivePropertyAsSynchronized(x => x.Value);
            NumThreads1 = model.NumThreads1.ToReactivePropertyAsSynchronized(x => x.Value);
            NumThreads2 = model.NumThreads2.ToReactivePropertyAsSynchronized(x => x.Value);
            BookEvalDiff1 = model.BookEvalDiff1.ToReactivePropertyAsSynchronized(x => x.Value);
            BookEvalDiff2 = model.BookEvalDiff2.ToReactivePropertyAsSynchronized(x => x.Value);
            ConsiderBookMoveCount1 = model.ConsiderBookMoveCount1.ToReactivePropertyAsSynchronized(x => x.Value);
            ConsiderBookMoveCount2 = model.ConsiderBookMoveCount2.ToReactivePropertyAsSynchronized(x => x.Value);
            IgnoreBookPly1 = model.IgnoreBookPly1.ToReactivePropertyAsSynchronized(x => x.Value);
            IgnoreBookPly2 = model.IgnoreBookPly2.ToReactivePropertyAsSynchronized(x => x.Value);

            OnSfenFilePathButton.Subscribe(() => SelectFilePath(SfenFilePath));
            OnEngine1FilePathButton.Subscribe(() => SelectFilePath(Engine1FilePath));
            OnEngine2FilePathButton.Subscribe(() => SelectFilePath(Engine2FilePath));
            OnEval1FolderPathButton.Subscribe(() => SelectFolderPath(Eval1FolderPath));
            OnEval2FolderPathButton.Subscribe(() => SelectFolderPath(Eval2FolderPath));
            OnNewMenuItem.Subscribe(OnNew);
            OnOpenMenuItem.Subscribe(OnOpen);
            OnSaveMenuItem.Subscribe(OnSave);
            OnSaveAsMenuItem.Subscribe(OnSaveAs);
            OnExitMenuItem.Subscribe(OnExit);
            OnStartMenuItem.Subscribe(OnStart);
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

        public void OnNew()
        {
            model.CopyFrom(new MainModel());
        }

        public void OnOpen()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            model.Load(dialog.FileName);
            filePath = dialog.FileName;
        }

        public void OnSave()
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                model.Save(filePath);
                return;
            }
            OnSaveAs();
        }

        public void OnSaveAs()
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            model.Save(dialog.FileName);
            filePath = dialog.FileName;
        }

        public void OnExit()
        {
            CloseAction();
        }

        private void OnStart()
        {
            // 測定開始前に設定を保存しておく
            model.Save(SettingFileName);

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
                Byoyomi1 = Byoyomi1.Value,
                Byoyomi2 = Byoyomi2.Value,
                Inc1 = Inc1.Value,
                Inc2 = Inc2.Value,
                Rtime1 = Rtime1.Value,
                Rtime2 = Rtime2.Value,
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
            coliseum.ShowStatus += ShowStatus;
            coliseum.ShowErrorMessage += ShowErrorMessage;

            // logフォルダを作成する。
            string logFolderPath = Path.Combine(LogFolderName, Util.GetDateString());
            Directory.CreateDirectory(logFolderPath);

            // ログフォルダに設定ファイルを保存する。
            model.Save(Path.Combine(logFolderPath, "setting.xml"));

            Task.Run(() =>
            {
                StartMenuItemEnabled.Value = false;
                coliseum.Run(options, logFolderPath);
                StartMenuItemEnabled.Value = true;
            });
        }

        private void ShowStatus(Options options, Status status, Engine engine1, Engine engine2)
        {
            State.Value = Coliseum.CreateStatusMessage(options, status, engine1, engine2);
            ProgressBarValue.Value = status.NumFinishedGames;
        }

        public void ShowErrorMessage(string errorMessage)
        {
            State.Value = errorMessage;
        }

        public void SaveSettingFile()
        {
            model.Save(SettingFileName);
        }
    }
}
