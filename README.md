# Constellation Observation

VRChat 向けの星座・惑星観測ワールドです。プラネタリウム形式のガイドツアーで、夜空の星座と太陽系の惑星をひとつずつ日本語ナレーション付きで紹介します。

## 機能

### 星座ツアー
- 21 星座を順番に紹介（プラネタリウム形式）
- 表示中の星座だけを点灯し、他は非表示にすることで視認性を確保
- 各星座に日本語ナレーション音声を収録

### 惑星ツアー
- 太陽系 8 惑星 + 太陽を太陽に近い順でガイド（計 9 天体）
- 紹介中の惑星がプレイヤーの正面に移動し、グロウライトでハイライト
- 各惑星に名前表示と日本語ナレーション音声を収録

### その他
- BGM 3 曲収録（環境音楽）
- 落下防止セーフティ
- スカイボックス: カスタム Skybox シェーダー（VRChat / VR 対応パッチ済み）

## プロジェクト構成

```
ConstellationObservation/Assets/
├── Audio/
│   ├── BGM/          BGM（mp3 × 3）
│   └── Voice/
│       ├── 天体観測/  星座ナレーション（mp3 × 21）
│       └── 太陽系/    惑星ナレーション（mp3 × 9）
├── FORGE3D/          サードパーティ: Planets アセットパック
├── Scenes/           メインシーン（VRCDefaultWorldScene.unity）
├── SerializedUdonPrograms/  Udon コンパイル済みプログラム（自動生成）
├── Skybox/           スカイボックスシェーダー + マテリアル
├── UdonSharp/        カスタム UdonSharp スクリプト群
└── XR/               XR 設定

Docs/
└── Screenshots/      スクリーンショット
```

## 主なスクリプト

| スクリプト | 役割 |
|---|---|
| `ExperienceManager` | 星座 / 惑星ツアーの切り替え管理 |
| `ConstellationTourController` | 星座ツアー進行（表示切り替え・ナレーション再生） |
| `PlanetTourController` | 惑星ツアー進行（移動・ハイライト・ナレーション再生） |
| `BGMPlayer` | BGM 再生管理 |
| `SelectionCube` | ツアー選択 UI（キューブのアニメーション付き） |
| `FallSafety` | プレイヤー落下防止 |
| `MaterialPulse` | マテリアルのパルスアニメーション |
| `FaceLocalPlayer` | オブジェクトをプレイヤーに向け続ける |

## 使用アセット

- **FORGE3D – Planets** (Unity Asset Store): 惑星・衛星 3D モデル
- **Super Simple Skybox** (カスタム修正版): VRChat の Shader Graph VR 対応バグをパッチ済み

## 動作環境

- Unity 2022.3.x
- VRChat SDK (Worlds)
- UdonSharp
