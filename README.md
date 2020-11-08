# TanukiColiseum
コンピュータ将棋USIエンジン自己対戦ソフトウェア

# 準備
適切なフォルダにTanukiColiseum.exeを配置して下さい。
続いて適切なフォルダに互角の局面集を配置して下さい。
互角の局面集は [自己対局用に互角の局面集を公開しました | やねうら王 公式サイト](http://yaneuraou.yaneu.com/2016/08/24/%E8%87%AA%E5%B7%B1%E5%AF%BE%E5%B1%80%E7%94%A8%E3%81%AB%E4%BA%92%E8%A7%92%E3%81%AE%E5%B1%80%E9%9D%A2%E9%9B%86%E3%82%92%E5%85%AC%E9%96%8B%E3%81%97%E3%81%BE%E3%81%97%E3%81%9F/ "自己対局用に互角の局面集を公開しました | やねうら王 公式サイト") から入手できます。
拙作の互角局面集も利用可能です。 [Release 互角局面集2017-05-19 · nodchip/hakubishin-](https://github.com/nodchip/hakubishin-/releases/tag/even-positions-2017-05-19 "Release 互角局面集2017-05-19 · nodchip/hakubishin-")

# 利用方法
起動するとGUIが表示されます。

# コマンドラインオプション
TanukiColiseumはコマンドラインから起動することもできます。

|オプション|説明|
|------------|-------------|
|--engine1-file-path|思考エンジン1のファイルパス|
|--engine2-file-path|思考エンジン2のファイルパス|
|--eval1-folder-path|思考エンジン1に読み込ませる評価関数が含まれるフォルダのパス|
|--eval2-folder-path|思考エンジン2に読み込ませる評価関数が含まれるフォルダのパス|
|--num-concurrent-games|同時対局数、物理コア数に合わせることをおすすめします|
|--num-games|合計対局数|
|--hash-mb|置換表サイズ、単位はMB|
|--nodes1|思考エンジン1の思考ノード数、0を指定した場合は思考ノード数を指定しない|
|--nodes2|思考エンジン2の思考ノード数、0を指定した場合は思考ノード数を指定しない|
|--time1|思考エンジン1の思考時間、単位はミリ秒|
|--time2|思考エンジン2の思考時間、単位はミリ秒|
|--num-book-moves1|思考エンジン1で何手目まで定跡データベースを使用するか|
|--num-book-moves2|思考エンジン2で何手目まで定跡データベースを使用するか|
|--book-file-name1|思考エンジン1に使用する定跡データベースファイル名|
|--book-file-name2|思考エンジン2に使用する定跡データベースファイル名|
|--num-book-moves|開始局面集の何手目から対局を開始するか|
|--sfen-file-path|開始局面集のファイルパス|
|--num-numa-nodes|NUMAノード数|
|--progress-interval-ms|進捗状況の表示間隔、単位はミリ秒|
|--num-threads1|思考エンジン1のスレッド数|
|--num-threads2|思考エンジン2のスレッド数|
|--book-eval-diff1|思考エンジン1のBookEvalDiffに渡す値|
|--book-eval-diff2|思考エンジン2のBookEvalDiffに渡す値|
|--consider-book-move-count1|思考エンジン1で定跡データベースに含まれる採択回数を考慮する場合はtrue、そうでない場合はfalse|
|--consider-book-move-count2|思考エンジン2で定跡データベースに含まれる採択回数を考慮する場合はtrue、そうでない場合はfalse|
|--ignore-book-ply1|思考エンジン1で定跡データベースに含まれる手数を無視する場合はtrue、そうでない場合はfalse|
|--ignore-book-ply2|思考エンジン2で定跡データベースに含まれる手数を無視する場合はtrue、そうでない場合はfalse|
|--gui|GUIを表示する場合はtrue、そうでない場合はfalse|

# コマンドライン例
    TanukiColiseum.exe --gui false --engine1-file-path YaneuraOu-2017-early.exe --engine2-file-path YaneuraOu-2017-early.exe --eval1-folder-path eval --eval2-folder-path eval --num-concurrent-games 4 --num-games 1000 --hash-mb 256 --time1 1000 --num-numa-nodes 1 --num-book-moves1 0 --num-book-moves2 0 --book-file-name1 book.bin --book-file-name2 yaneura_book1.db --num-book-moves 24

# 出力例
    Initializing engines...
    Starting an engine process. gameIndex=0 engine=1 Engine1FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=0 engine=2 Engine2FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=1 engine=1 Engine1FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=1 engine=2 Engine2FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=2 engine=1 Engine1FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=2 engine=2 Engine2FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=3 engine=1 Engine1FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=3 engine=2 Engine2FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=4 engine=1 Engine1FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=4 engine=2 Engine2FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=5 engine=1 Engine1FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=5 engine=2 Engine2FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=6 engine=1 Engine1FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=6 engine=2 Engine2FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=7 engine=1 Engine1FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Starting an engine process. gameIndex=7 engine=2 Engine2FilePath=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe
    Initialized engines...
    Started games...
    対局数=10 同時対局数=8 ハッシュサイズ=1024 開始手数=24 開始局面ファイル=C:\home\nodchip\TanukiColiseum\records2016_10818.sfen NUMAノード数=1 表示更新間隔(ms)=1
    思考エンジン1 name=YaneuraOu NNUE 5.30 64AVX2BMI2 TOURNAMENT author=by yaneurao exeファイル=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe 評価関数フォルダパス=C:\home\nodchip\hakubishin-private\source\eval 定跡手数=16 定跡ファイル名=standard_book.db 思考ノード数=0 思考時間(ms)=100 スレッド数=1 BookEvalDiff=30 定跡の採択率を考慮する=false 定跡の手数を無視する=false
    思考エンジン2 name=YaneuraOu NNUE 5.30 64AVX2BMI2 TOURNAMENT author=by yaneurao exeファイル=C:\home\nodchip\YaneuraOu\source\YaneuraOu-by-gcc.exe 評価関数フォルダパス=C:\home\nodchip\hakubishin-private\source\eval 定跡手数=16 定跡ファイル名=standard_book.db 思考ノード数=0 思考時間(ms)=100 スレッド数=1 BookEvalDiff=30 定跡の採択率を考慮する=false 定跡の手数を無視する=false
    対局数10 先手勝ち3(30.0%) 後手勝ち7(70.0%) 引き分け0
    engine1
    勝ち5(50.0% R0.0 +-233.6) 先手勝ち3(30.0%) 後手勝ち2(20.0%)
    宣言勝ち0 先手宣言勝ち0 後手宣言勝ち0 先手引き分け0 後手引き分け0
    engine2
    勝ち5(50.0%) 先手勝ち0(0.0%) 後手勝ち5(50.0%)
    宣言勝ち0 先手宣言勝ち0 後手宣言勝ち0 先手引き分け0 後手引き分け0
    5,0,5

# 便利な使い方
- GUIの結果表示部分をクリックし、Ctrl+A Ctrl+Cで、テキストをコピーできます

# サードパーティ製ライブラリ
TanukiColiseumは以下のサードパーティ製ライブラリを用いて開発しています。
- ControlzEx Copyright
  - (c) since 2015 Jan Karger, Bastian Schmidt, James Willock
- MahApps.Metro Copyright
  - (c) .NET Foundation and Contributors. All rights reserved.
- Microsoft.CSharp
  - Copyright (c) .NET Foundation and Contributors
- Microsoft.Xaml.Behaviors.Wpf
  - Copyright (c) 2015 Microsoft
- ReactiveProperty
  - Copyright (c) 2018 neuecc, xin9le, okazuki
- ReactiveProperty.Core
  - Copyright (c) 2018 neuecc, xin9le, okazuki
- System.Buffers
  - Copyright (c) .NET Foundation and Contributors
- System.CommandLine
  - Copyright © .NET Foundation and Contributors
- System.CommandLine.DragonFruit
  - Copyright © .NET Foundation and Contributors
- System.CommandLine.Rendering
  - Copyright © .NET Foundation and Contributors
- System.ComponentModel.Annotations
  - Copyright (c) .NET Foundation and Contributors
- System.Memory
  - Copyright (c) .NET Foundation and Contributors
- System.Numerics.Vectors
  - Copyright (c) .NET Foundation and Contributors
- System.Reactive
  - Copyright (c) .NET Foundation and Contributors
- System.Runtime.CompilerServices.Unsafe
  - Copyright (c) .NET Foundation and Contributors
- System.Threading.Tasks.Extensions
  - Copyright (c) .NET Foundation and Contributors
- System.ValueTuple
  - Copyright (c) .NET Foundation and Contributors
