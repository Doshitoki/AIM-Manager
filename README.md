# AIM Manager

**AsoViva Interactive Music Manager**

AIM Managerは、Unity上でインタラクティブBGMの状態遷移とレイヤー管理を行うための制作補助ツールです。

ゲーム内のBGMを「Walking」「Talking」などの状態に分け、それぞれの状態に「Base」「Guitar」「Cello」などの音楽レイヤーを登録できます。
ゲーム進行に応じてレイヤーを解放し、現在の状態に対応する音源だけを自動でフェードイン / フェードアウトさせることで、プレイヤーの行動や物語の進行に合わせて音楽が変化する仕組みを簡単に実装できます。

---

## Features

* BGM状態の登録

  * 例：`Walking`, `Talking`

* 状態ごとの音楽レイヤー管理

  * 例：`Base`, `Guitar`, `Cello`, `Violin`, `Marimba`

* AudioClipからAudioSourceを自動生成

* 状態遷移時の自動フェード

* 解放済みレイヤーのみを再生対象にする判定処理

* Staticクラスによるレイヤー解放状況のシーン間保持

* Inspector上での直感的なデバッグ操作

---

## Installation

### Unity Package Managerから追加する場合

1. Unity上部メニューから `Window > Package Manager` を開く
2. 左上の `+` ボタンを押す
3. `Add package from disk...` を選択
4. `com.asoviva.aim-manager/package.json` を選択

---

### Assetsフォルダに直接入れる場合

以下の構成をUnityプロジェクトの `Assets` フォルダ内に配置してください。

```txt
Assets/
  AIM_Manager/
    Runtime/
      AsoVivaInteractiveMusicManager.cs
      AsoVivaMusicProgress.cs
    Editor/
      AsoVivaInteractiveMusicManagerEditor.cs
```

`Editor` 用スクリプトは、必ず `Editor` フォルダ内に配置してください。

---

## Folder Structure

```txt
com.asoviva.aim-manager/
  package.json
  README.md
  Runtime/
    AsoVivaInteractiveMusicManager.cs
    AsoVivaMusicProgress.cs
  Editor/
    AsoVivaInteractiveMusicManagerEditor.cs
```

---

## Quick Start

### 1. Managerをシーンに配置する

空のGameObjectを作成し、`AsoVivaInteractiveMusicManager` をアタッチします。

例：

```txt
AsoViva Music Manager
```

---

### 2. 音楽状態を登録する

Inspector上で `States` を設定します。

例：

```txt
State 0: Walking
State 1: Talking
```

`Default State` には、最初に再生したい状態名を入力します。

例：

```txt
Default State: Walking
```

`Default State` と `State Name` は完全一致している必要があります。
例：`Walking` と `Walkling` のようにスペルが違うと、音量が0のままになります。

---

### 3. 各StateにLayerを登録する

各Stateの中に音楽レイヤーを登録します。

例：

```txt
Walking
- Base
- Guitar
- Cello

Talking
- Base
- Guitar
- Cello
```

同じ役割のレイヤーは、状態が異なっていても同じ `layerName` を設定します。

例：

```txt
Walking_Guitar → layerName: Guitar
Talking_Guitar → layerName: Guitar
```

これにより、`Guitar` レイヤーが解放されたとき、現在のStateに対応したGuitarレイヤーが再生対象になります。

---

### 4. AudioClipを登録する

各LayerにAudioClipを登録します。

実行時にAudioSourceは自動生成されるため、事前にAudioSourceを個別に配置する必要はありません。

基本設定例：

```txt
Layer Name: Base
Audio Clip: 任意のAudioClip
Always On: true
Target Volume: 1
```

Baseレイヤーは、基本的に `Always On` を有効にしてください。

---

## Usage

### 状態を切り替える

会話開始時など、音楽状態を切り替えたいタイミングで以下のように呼び出します。

```csharp
public AsoVivaInteractiveMusicManager music;

public void StartDialogue()
{
    music.ChangeState("Talking");
}
```

会話終了時には以下のように呼び出します。

```csharp
public void EndDialogue()
{
    music.ChangeState("Walking");
}
```

これにより、現在のStateのレイヤーはフェードアウトし、切り替え先のStateのうち、解放済みレイヤーのみがフェードインします。

---

### レイヤーを解放する

ゲーム進行に応じて楽器レイヤーを追加したい場合は、以下のように呼び出します。

```csharp
music.UnlockLayer("Guitar");
```

例：

```csharp
public void ClearCar1()
{
    music.UnlockLayer("Guitar");
}

public void ClearCar2()
{
    music.UnlockLayer("Cello");
}
```

`UnlockLayer()` は内部でStaticな進行状況管理クラスに解放状態を保存し、現在の音楽状態にも即時反映します。

---

## How It Works

AIM Managerでは、BGMを「State」と「Layer」の組み合わせとして扱います。

例：

```txt
Walking / Base
Walking / Guitar
Talking / Base
Talking / Guitar
```

現在のStateと、解放済みLayerの情報をもとに、各AudioSourceの目標音量を決定します。

```txt
現在のStateである
かつ
Always On または 解放済みLayerである
→ targetVolumeへ

それ以外
→ 0へ
```

この仕組みにより、以下の処理を一元管理できます。

* WalkingからTalkingへの切り替え
* TalkingからWalkingへの復帰
* 解放済みレイヤーの追加
* 未解放レイヤーの無音化
* 状態遷移時のフェード処理

---

## Scene Transition

レイヤーの解放状況は `AsoVivaMusicProgress` にStaticで保存されます。

そのため、あるシーンで `Guitar` を解放した場合、次のシーンでも `Guitar` は解放済みとして扱われます。

```csharp
AsoVivaMusicProgress.Unlock("Guitar");
```

ただし、通常はManager経由で呼び出すことを推奨します。

```csharp
music.UnlockLayer("Guitar");
```

Manager経由で呼び出すことで、Staticへの保存と現在再生中の音楽への反映を同時に行えます。

---

## Inspector Debug Controls

`AsoVivaInteractiveMusicManagerEditor` により、Inspector上にデバッグ用ボタンが表示されます。

主な操作：

```txt
Play All Synced
Change to Walking
Change to Talking
Unlock Guitar
Unlock Cello
Unlock ViolinViola
Unlock Marimba
Reset Progress
Stop All
```

これにより、ゲーム中にスクリプトから呼び出す前に、Inspector上で状態遷移やレイヤー解放の挙動を確認できます。

---

## Notes

* `Default State` と `State Name` は完全一致させてください。
* 各Layerの `Target Volume` が `0` の場合、解放済みでも音は聞こえません。
* Baseレイヤーは基本的に `Always On` を有効にしてください。
* AudioClipが未設定のLayerは、AudioSourceが自動生成されません。
* `Editor` スクリプトは必ず `Editor` フォルダ内に配置してください。

---

## Technologies

* Unity
* C#
* Unity Editor Extension
* AudioSource Control
* Static Progress Management

---

## Development Background

本ツールは、自作ゲーム制作におけるBGM管理の煩雑さを解消するために制作しました。

自作ゲームでは、プレイヤーが移動する場面とNPCと会話する場面で異なるBGM状態を持ち、さらにゲーム進行に応じてギター、チェロ、マリンバなどのレイヤーが追加される仕様を採用しています。

これらを個別のスクリプトで管理すると、状態遷移、フェード、レイヤー解放、シーン間の引き継ぎが複雑になります。
AIM Managerでは、それらの処理をUnity上で視覚的に設定し、簡単なメソッド呼び出しで制御できるようにしました。

---

## Author

Studio AsoViva
吉永

---
