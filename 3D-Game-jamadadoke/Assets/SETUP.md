# シーンセットアップ手順

## 1. Input System の設定

Edit > Project Settings > Player > Active Input Handling  
→ **Both** または **Input System Package (New)** に変更して再起動

---

## 2. ステージを作る（仮）

1. Hierarchy で右クリック > 3D Object > **Plane** を作成
   - Scale: (10, 1, 10) → ホームの床
2. NavMesh をベイク
   - Window > AI > **Navigation** を開く
   - Plane を選択 > Navigation Static にチェック
   - Bake タブ > **Bake**

---

## 3. プレイヤーを作る

1. 空の GameObject を作成、名前: **Player**
2. Add Component > **Character Controller** (Radius=0.4, Height=1.8)
3. Add Component > **PlayerController**
4. Add Component > **PlayerAttack**
5. Player の子に空 GameObject「CameraRoot」を追加 (Y=1.6)
6. CameraRoot の子に **Main Camera** を移動
7. PlayerController の CameraRoot フィールドに「CameraRoot」をアサイン
8. PlayerAttack の:
   - Main Camera → Main Camera
   - Throw Origin → CameraRoot（またはその子に別途作成）
   - Phone Prefab → 後述のスマホPrefab

---

## 4. スマホ Prefab を作る

1. 3D Object > Cube、Scale: (0.07, 0.15, 0.01)
2. Add Component > **Rigidbody**
3. Add Component > **ThrowablePhone**
4. Assets/Prefabs フォルダに **PhonePrefab** として保存

---

## 5. NPC Prefab を作る（歩きスマホ用）

1. 3D Object > Capsule、名前: **PhoneUser**
2. Add Component > **NavMesh Agent**
3. Add Component > **Rigidbody** (Constraints: Freeze Rotation X/Z)
4. Add Component > **NPCController**
   - NPC Type: **Phone User**
   - Body Renderer: Capsule の MeshRenderer をアサイン
5. Assets/Prefabs に **PhoneUserPrefab** として保存
6. 同様に **InnocentPrefab** を作成（NPC Type: **Innocent**）

---

## 6. Spawner を置く

1. 空の GameObject「NPCSpawner」を作成
2. Add Component > **NPCSpawner**
   - Phone User Prefab / Innocent Prefab をアサイン
3. ステージ中央付近に配置

---

## 7. マネージャーを置く

1. 空の GameObject「GameManager」> Add Component > **GameManager**
2. 空の GameObject「ScoreManager」> Add Component > **ScoreManager**

---

## 8. HUD を作る

1. Hierarchy > UI > **Canvas** を作成
2. Canvas の子に以下のTextMeshPro（UI）を追加:
   - `ScoreText`（左上）
   - `ComboText`（左上・スコアの下）
   - `InnocentCountText`（右上）
3. GameOver パネルを作成（初期非アクティブ）
   - `GameOverReasonText`（中央）
   - `RestartButton`
4. 空の GameObject「HUD」> Add Component > **HUDController**
   - 各フィールドに上記 UI をアサイン

---

## 動作確認

Play ボタンを押して：
- WASD で移動
- マウスで視点回転
- Shift でダッシュ
- 左クリックでNPCを叩く → スコア上昇・コンボ
- 右クリックでスマホ投擲（叩いてスマホを奪った後）
- 一般人を3人以上叩くとゲームオーバー
