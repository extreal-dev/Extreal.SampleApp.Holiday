# 性能テスト(P2Pマルチプレイ)
## 性能テスト用のビルド
### 1. 性能テストシーンを追加
性能テストでは全プレイヤーを自動操作させる。  
`Build Settings` - `Scenes in Build`で`Holiday.PerformanceTest/PerformanceTest`をチェックを入れ、先頭に配置する。
### 2. ホスト・クライアントの選択
P2Pマルチプレイにおいてホストとクライアントで自動操作の内容が異なる。  
どちらで動作させるかは、`Assets/Holiday.PerformanceTest/PerformanceTestConfig`の`Role`で選択する
### 3. シグナリングサーバのアドレス設定
P2P接続のシグナリングサーバのアドレスをP2PConfig > Signaling Urlに設定する。
### 4. 利用状況可視化サーバのアドレス設定
利用状況可視化サーバのアドレスをAppUsageConfig > Push Urlに設定する。
### 5. Build Settings
- WebGL
    - `Development Build`はオフ
    - `Code Optimization`は`Shorter Build Time`
- Windows Dedicated Server(負荷クライアント)
    - `Development Build`は**オン**。オンにしないと起動時エラーが発生する。

## 性能テスト実施
#### 負荷クライアント開始バッチファイルのパラメータ修正
負荷条件に応じて[負荷クライアント開始バッチファイル](Assets/Holiday.PerformanceTest/.Client/StartPerformanceTest.bat)を修正する。
|パラメータ|機能|
|--|--|
|exec_time|性能テストの起動時間。この時間を経過（＝完了）するとS3に結果をアップロードする|
|client_num|起動する負荷クライアントの数|
### 負荷クライアントのセットアップ
1. 負荷クライアントPCに[.Client](Assets/Holiday.PerformanceTest/.Client/)フォルダをコピーする。
1. `.Client`フォルダに性能テスト用にビルドしたWindowsアプリのファイル郡を直下に全てコピーする。
    - ※Holiday.exeとStartPerformanceTest.batが同じ階層に存在する状態
### 負荷クライアントの実行
1. `StartPerformanceTest.bat`を実行する
### 性能テスト結果整理
#### 負荷クライアントのメモリ使用量結果計測ツール
- [DataAnalysis.py](/Assets/Holiday.PerformanceTest/.Server/DataAnalysis.py)
    - メモリ使用量は起動したクライアントごとのファイルに出力される。複数クライアント全体でメモリのmaxとminを集計するツール。
    - 使い方
        - 例）下記フォルダの状態の時。
      ```
      ├─ DataAnalysis.py
      └──1
         ├─ dev-stress-server1_CpuUtilization.csv
         ├─ dev-stress-server1_MemoryUtilization.csv
         ├─ dev-stress-server1_MemoryUtilization0.txt
         ├─ dev-stress-server1_MemoryUtilization1.txt
         └─ dev-stress-server1_MemoryUtilization2.txt
      ```
        - 実行コマンドは下記
          ```
          $ python DataAnalysis.py 1/dev-stress-server1_MemoryUtilization\*
          ```



