﻿using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
        /// 思考エンジン1に渡す思考時間。
        /// <para>0が渡された場合、思考時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Time1 { get; }

        /// <summary>
        /// 思考エンジン2に渡す思考時間。
        /// <para>0が渡された場合、思考時間を指定しない。</para>
        /// </summary>
        [RangeAttribute(0, int.MaxValue)]
        public ReactiveProperty<int> Time2 { get; }

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

        public ReactiveProperty<bool> StartButtonEnabled { get; } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<string> State { get; } = new ReactiveProperty<string>("");

        public ReactiveProperty<int> ProgressBarValue { get; } = new ReactiveProperty<int>();

        public ReactiveProperty<int> ProgressBarMinimum { get; } = new ReactiveProperty<int>();

        public ReactiveProperty<int> ProgressBarMaximum { get; } = new ReactiveProperty<int>();

        public ReactiveCommand OnSfenFilePathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnEngine1FilePathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnEngine2FilePathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnEval1FolderPathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnEval2FolderPathButton { get; } = new ReactiveCommand();

        public ReactiveCommand OnStartButton { get; } = new ReactiveCommand();

        public MainModel model { get; } = new MainModel();

        public MainViewModel()
        {
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