# 乱将棋 アプリのソースコード

6×6盤の将棋の変種「乱将棋」を遊ぶスマートフォンアプリ（iOS / Android）の、自作部分のソースコードです。

思考エンジンにGPLv3のソフトウェア（やねうら王の改造版）を使っているため、GPLライセンスに従ってアプリ側のソースコードも公開しています。

思考エンジンは別リポジトリにあります。

- エンジン: https://github.com/algorithms-inc/YaneuraOu_Ranshogi
  （[やねうら王](https://github.com/yaneurao/YaneuraOu)を乱将棋向けに改造したもの。GPLv3）

## ライセンス

GNU General Public License v3 です。アプリはGPLv3の思考エンジンをリンクして動くため、同じ条件で提供します。

## 入っているもの

| | |
|---|---|
| `Assets/Scripts/` | 自作のコード（283ファイル）。ゲームのルール、通信、画面の組み立て、端末内AIの呼び出しなど |
| `ProjectSettings/` | Unityのプロジェクト設定 |
| `Packages/manifest.json` | 使っているUnityパッケージの一覧 |

主なところは次のとおりです。

- `Assets/Scripts/Game/` — 盤・駒・指し手・局面（`Position`, `Move`, `GameController`）
- `Assets/Scripts/Network/` — サーバとのHTTP/WebSocket通信
- `Assets/Scripts/ViewControllers/` — 画面ごとの処理
- `Assets/Scripts/Design/GFramework/` — 画面の部品とレイアウトの自作フレームワーク
- `Assets/Scripts/RanshogiEngine.cs` — 端末内エンジン（上記のエンジンリポジトリ）のC APIを呼ぶ部分

## 入っていないもの

そのままではビルドできません。次を外しています。

| 外したもの | 理由 |
|---|---|
| Firebase Unity SDK、Google Mobile Ads、External Dependency Manager | 他社のSDKで、再配布する権利がありません |
| Firebaseの設定ファイル（`GoogleService-Info*.plist`、`google-services.xml`） | 自分のプロジェクトの設定のため |
| 画像・音声・シーン・フォント・プレハブ | アプリの素材のため |
| 評価関数 `nn.bin` | 学習させたデータのため |
| 広告ユニットID、署名鍵の場所 | 伏せ字にしています |

動かす場合は、Unity 6000.2.14f1 で新しいプロジェクトを作り、このリポジトリの `Assets/Scripts` を置いた上で、Firebase Unity SDK 13.6.0（App / Auth）、Google Mobile Ads、Unity IAP を入れてください。素材とシーンは含まれないので、画面は組み上がりません。コードを読むためのものと考えてください。
