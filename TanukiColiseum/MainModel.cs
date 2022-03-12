using Reactive.Bindings;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace TanukiColiseum
{
    /// <summary>
    /// 設定項目を保持するモデルクラス
    /// </summary>
    [DataContract]
    public class MainModel : INotifyPropertyChanged
    {
        [DataMember]
        public ReactiveProperty<string> Engine1FilePath { get; set; } = new ReactiveProperty<string>("");

        [DataMember]
        public ReactiveProperty<string> Engine2FilePath { get; set; } = new ReactiveProperty<string>("");

        [DataMember]
        public ReactiveProperty<string> Eval1FolderPath { get; set; } = new ReactiveProperty<string>("eval");

        [DataMember]
        public ReactiveProperty<string> Eval2FolderPath { get; set; } = new ReactiveProperty<string>("eval");

        [DataMember]
        public ReactiveProperty<int> NumConcurrentGames { get; set; } = new ReactiveProperty<int>(1);

        [DataMember]
        public ReactiveProperty<int> NumGames { get; set; } = new ReactiveProperty<int>(1000);

        [DataMember]
        public ReactiveProperty<int> HashMb { get; set; } = new ReactiveProperty<int>(16);

        [DataMember]
        public ReactiveProperty<int> NumBookMoves1 { get; set; } = new ReactiveProperty<int>(16);

        [DataMember]
        public ReactiveProperty<int> NumBookMoves2 { get; set; } = new ReactiveProperty<int>(16);

        [DataMember]
        public ReactiveProperty<string> BookFileName1 { get; set; } = new ReactiveProperty<string>("standard_book.db");

        [DataMember]
        public ReactiveProperty<string> BookFileName2 { get; set; } = new ReactiveProperty<string>("standard_book.db");

        [DataMember]
        public ReactiveProperty<int> NumBookMoves { get; set; } = new ReactiveProperty<int>(24);

        [DataMember]
        public ReactiveProperty<int> MaxMovesToDraw { get; set; } = new ReactiveProperty<int>(320);

        [DataMember]
        public ReactiveProperty<string> SfenFilePath { get; set; } = new ReactiveProperty<string>("records2016_10818.sfen");

        /// <summary>
        /// 思考エンジン1に渡す思考ノード数。
        /// <para>0が渡された場合、ノード数を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Nodes1 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン2に渡す思考ノード数。
        /// <para>0が渡された場合、ノード数を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Nodes2 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考ノード数に加える乱数(%)。
        /// <para>0が渡されたとき、思考ノード数に乱数を加えない</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> NodesRandomPercent1 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考ノード数に加える乱数(%)。
        /// <para>0が渡されたとき、思考ノード数に乱数を加えない</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> NodesRandomPercent2 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考ノード数の乱数を1手毎に変化させる。
        /// <para>falseの場合、1局を通して同じ値を加える</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<bool> NodesRandomEveryMove1 { get; set; } = new ReactiveProperty<bool>(true);

        /// <summary>
        /// 思考ノード数の乱数を1手毎に変化させる。
        /// <para>falseの場合、1局を通して同じ値を加える</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<bool> NodesRandomEveryMove2 { get; set; } = new ReactiveProperty<bool>(true);

        /// <summary>
        /// 思考エンジン1に渡す持ち時間。
        /// <para>0が渡された場合、持ち時間を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Time1 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン2に渡す持ち時間。
        /// <para>0が渡された場合、持ち時間を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Time2 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン1に渡す秒読み時間。
        /// <para>0が渡された場合、秒読み時間を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Byoyomi1 { get; set; } = new ReactiveProperty<int>(1000);

        /// <summary>
        /// 思考エンジン2に渡す秒読み時間。
        /// <para>0が渡された場合、秒読み時間を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Byoyomi2 { get; set; } = new ReactiveProperty<int>(1000);

        /// <summary>
        /// 思考エンジン1に渡す加算時間。
        /// <para>0が渡された場合、加算時間を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Inc1 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン2に渡す加算時間。
        /// <para>0が渡された場合、加算時間を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Inc2 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン1に渡す乱数付き思考時間。
        /// <para>0が渡された場合、乱数付き思考時間を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Rtime1 { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// 思考エンジン2に渡す乱数付き思考時間。
        /// <para>0が渡された場合、乱数付き思考時間を指定しない。</para>
        /// </summary>
        [DataMember]
        public ReactiveProperty<int> Rtime2 { get; set; } = new ReactiveProperty<int>(0);
        [DataMember]
        public ReactiveProperty<int> NumNumaNodes { get; set; } = new ReactiveProperty<int>(1);

        [DataMember]
        public ReactiveProperty<int> ProgressIntervalMs { get; set; } = new ReactiveProperty<int>(1);

        [DataMember]
        public ReactiveProperty<int> NumThreads1 { get; set; } = new ReactiveProperty<int>(1);

        [DataMember]
        public ReactiveProperty<int> NumThreads2 { get; set; } = new ReactiveProperty<int>(1);

        [DataMember]
        public ReactiveProperty<int> BookEvalDiff1 { get; set; } = new ReactiveProperty<int>(30);

        [DataMember]
        public ReactiveProperty<int> BookEvalDiff2 { get; set; } = new ReactiveProperty<int>(30);

        [DataMember]
        public ReactiveProperty<string> ConsiderBookMoveCount1 { get; set; } = new ReactiveProperty<string>("false");

        [DataMember]
        public ReactiveProperty<string> ConsiderBookMoveCount2 { get; set; } = new ReactiveProperty<string>("false");

        [DataMember]
        public ReactiveProperty<string> IgnoreBookPly1 { get; set; } = new ReactiveProperty<string>("false");

        [DataMember]
        public ReactiveProperty<string> IgnoreBookPly2 { get; set; } = new ReactiveProperty<string>("false");

        [DataMember]
        public ReactiveProperty<int> SlowMover1 { get; set; } = new ReactiveProperty<int>(100);

        [DataMember]
        public ReactiveProperty<int> SlowMover2 { get; set; } = new ReactiveProperty<int>(100);
        [DataMember]
        public ReactiveProperty<int> DrawValue1 { get; set; } = new ReactiveProperty<int>(-2);

        [DataMember]
        public ReactiveProperty<int> DrawValue2 { get; set; } = new ReactiveProperty<int>(-2);
        [DataMember]
        public ReactiveProperty<int> BookEvalBlackLimit1 { get; set; } = new ReactiveProperty<int>(0);

        [DataMember]
        public ReactiveProperty<int> BookEvalBlackLimit2 { get; set; } = new ReactiveProperty<int>(0);
        [DataMember]
        public ReactiveProperty<int> BookEvalWhiteLimit1 { get; set; } = new ReactiveProperty<int>(-140);

        [DataMember]
        public ReactiveProperty<int> BookEvalWhiteLimit2 { get; set; } = new ReactiveProperty<int>(-140);

        public event PropertyChangedEventHandler PropertyChanged;

        public void Save(string filePath)
        {
            string folderPath = Path.GetDirectoryName(filePath);
            // ファイル名だけが渡された場合、FolderPathが空になる。
            // GUI画面から測定開始できない · Issue #11 · nodchip/TanukiColiseum https://github.com/nodchip/TanukiColiseum/issues/11
            if (folderPath.Length != 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            var serializer = new DataContractSerializer(typeof(MainModel));
            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                serializer.WriteObject(writer, this);
            }
        }

        public void Load(string filePath)
        {
            MainModel model;
            var serializer = new DataContractSerializer(typeof(MainModel));
            using (var reader = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    model = (MainModel)serializer.ReadObject(reader);
                }
                catch (XmlException)
                {
                    // 設定ファイルがxmlとして読み込めなかった場合は、デフォルトの値をロードする。
                    // 自動保存の設定ファイルが壊れているとGUIが起動できない · Issue #12 · nodchip/TanukiColiseum https://github.com/nodchip/TanukiColiseum/issues/12
                    model = new MainModel();
                }
            }

            // 過去のデータ形式において保存されなかった値の担保
            if (model.Byoyomi1 == null)
            {
                model.Byoyomi1 = new ReactiveProperty<int>(1000);
            }

            if (model.Byoyomi2 == null)
            {
                model.Byoyomi2 = new ReactiveProperty<int>(1000);
            }

            if (model.Inc1 == null)
            {
                model.Inc1 = new ReactiveProperty<int>(0);
            }

            if (model.Inc2 == null)
            {
                model.Inc2 = new ReactiveProperty<int>(0);
            }

            if (model.Rtime1 == null)
            {
                model.Rtime1 = new ReactiveProperty<int>(0);
            }

            if (model.Rtime2 == null)
            {
                model.Rtime2 = new ReactiveProperty<int>(0);
            }

            if (model.NodesRandomPercent1 == null)
            {
                model.NodesRandomPercent1 = new ReactiveProperty<int>(0);
            }

            if (model.NodesRandomPercent2 == null)
            {
                model.NodesRandomPercent2 = new ReactiveProperty<int>(0);
            }

            if (model.NodesRandomEveryMove1 == null)
            {
                model.NodesRandomEveryMove1 = new ReactiveProperty<bool>(true);
            }

            if (model.NodesRandomEveryMove2 == null)
            {
                model.NodesRandomEveryMove2 = new ReactiveProperty<bool>(true);
            }

            if (model.MaxMovesToDraw == null)
            {
                model.MaxMovesToDraw = new ReactiveProperty<int>(320);
            }

            if (model.SlowMover1 == null)
            {
                model.SlowMover1 = new ReactiveProperty<int>(100);
            }

            if (model.SlowMover2 == null)
            {
                model.SlowMover2 = new ReactiveProperty<int>(100);
            }

            if (model.DrawValue1 == null)
            {
                model.DrawValue1 = new ReactiveProperty<int>(-2);
            }

            if (model.DrawValue2 == null)
            {
                model.DrawValue2 = new ReactiveProperty<int>(-2);
            }

            if (model.BookEvalBlackLimit1 == null)
            {
                model.BookEvalBlackLimit1 = new ReactiveProperty<int>(0);
            }

            if (model.BookEvalBlackLimit2 == null)
            {
                model.BookEvalBlackLimit2 = new ReactiveProperty<int>(0);
            }

            if (model.BookEvalWhiteLimit1 == null)
            {
                model.BookEvalWhiteLimit1 = new ReactiveProperty<int>(-140);
            }

            if (model.BookEvalWhiteLimit2 == null)
            {
                model.BookEvalWhiteLimit2 = new ReactiveProperty<int>(-140);
            }

            CopyFrom(model);
        }

        public void CopyFrom(MainModel model)
        {
            Engine1FilePath.Value = model.Engine1FilePath.Value;
            Engine2FilePath.Value = model.Engine2FilePath.Value;
            Eval1FolderPath.Value = model.Eval1FolderPath.Value;
            Eval2FolderPath.Value = model.Eval2FolderPath.Value;
            NumConcurrentGames.Value = model.NumConcurrentGames.Value;
            NumGames.Value = model.NumGames.Value;
            HashMb.Value = model.HashMb.Value;
            NumBookMoves1.Value = model.NumBookMoves1.Value;
            NumBookMoves2.Value = model.NumBookMoves2.Value;
            BookFileName1.Value = model.BookFileName1.Value;
            BookFileName2.Value = model.BookFileName2.Value;
            NumBookMoves.Value = model.NumBookMoves.Value;
            MaxMovesToDraw.Value = model.MaxMovesToDraw.Value;
            SfenFilePath.Value = model.SfenFilePath.Value;
            Nodes1.Value = model.Nodes1.Value;
            Nodes2.Value = model.Nodes2.Value;
            NodesRandomPercent1.Value = model.NodesRandomPercent1.Value;
            NodesRandomPercent2.Value = model.NodesRandomPercent2.Value;
            NodesRandomEveryMove1.Value = model.NodesRandomEveryMove1.Value;
            NodesRandomEveryMove2.Value = model.NodesRandomEveryMove2.Value;
            Time1.Value = model.Time1.Value;
            Time2.Value = model.Time2.Value;
            Byoyomi1.Value = model.Byoyomi1.Value;
            Byoyomi2.Value = model.Byoyomi2.Value;
            Inc1.Value = model.Inc1.Value;
            Inc2.Value = model.Inc2.Value;
            Rtime1.Value = model.Rtime1.Value;
            Rtime2.Value = model.Rtime2.Value;
            NumNumaNodes.Value = model.NumNumaNodes.Value;
            ProgressIntervalMs.Value = model.ProgressIntervalMs.Value;
            NumThreads1.Value = model.NumThreads1.Value;
            NumThreads2.Value = model.NumThreads2.Value;
            BookEvalDiff1.Value = model.BookEvalDiff1.Value;
            BookEvalDiff2.Value = model.BookEvalDiff2.Value;
            ConsiderBookMoveCount1.Value = model.ConsiderBookMoveCount1.Value;
            ConsiderBookMoveCount2.Value = model.ConsiderBookMoveCount2.Value;
            IgnoreBookPly1.Value = model.IgnoreBookPly1.Value;
            IgnoreBookPly2.Value = model.IgnoreBookPly2.Value;
            SlowMover1.Value = model.SlowMover1.Value;
            SlowMover2.Value = model.SlowMover2.Value;
            DrawValue1.Value = model.DrawValue1.Value;
            DrawValue2.Value = model.DrawValue2.Value;
            BookEvalBlackLimit1.Value = model.BookEvalBlackLimit1.Value;
            BookEvalBlackLimit2.Value = model.BookEvalBlackLimit2.Value;
            BookEvalWhiteLimit1.Value = model.BookEvalWhiteLimit1.Value;
            BookEvalWhiteLimit2.Value = model.BookEvalWhiteLimit2.Value;
        }
    }
}
