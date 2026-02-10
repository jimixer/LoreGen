# LoreGen アーキテクチャ設計書

## 概要

LoreGenは、架空の固有名詞（地名・人名など）を生成するPure C#ライブラリです。言語学的知見に基づいた音韻規則と、パラメータによる印象制御を組み合わせることで、世界観に沿った説得力のある名前を生成します。

### プロジェクトの特性
- **目的**: 架空世界の固有名詞生成エンジン
- **対象**: 地名、人名、その他の命名可能なエンティティ
- **アプローチ**: 音韻学ベースのルールシステム + 印象パラメータ
- **技術スタック**: Pure C# (.NET Standard 2.1)

---

## アーキテクチャの基本原則

### 1. Pure C# Logic
- Unity依存性ゼロ（`UnityEngine`への参照なし）
- .NET Standard 2.1ターゲット
- Unity 2021+との互換性維持
- 他のゲームエンジン・フレームワークでも利用可能

### 2. 決定論的生成
- シード値による再現可能な生成
- 同じ入力から同じ結果を保証
- デバッグとテストの容易性

### 3. パラメータ駆動設計
- 印象パラメータによる結果制御
- 文化圏・時代性・音響特性などを数値化
- 合成的なパラメータ管理（階層的な組み合わせ）

### 4. 拡張可能なルールシステム
- 複数の生成手法をサポート
- カスタムルールの追加が容易
- 段階的な機能拡張を想定

### 5. データ駆動
- ルールセットとデータを外部化（JSON/YAML）
- プログラマー以外でも編集可能
- 多言語・多文化圏への対応

---

## コア概念

### 階層的な構成要素

```
┌─────────────────────────────────────┐
│  固有名詞 (Proper Noun)              │  意味的レイヤー
│  "Karthian" = カース地方の住民       │
└──────────────┬──────────────────────┘
               │ 派生・変換
┌──────────────▼──────────────────────┐
│  単語 (Word/Name)                    │  語彙的レイヤー
│  "Karth" = 基本地名                  │
└──────────────┬──────────────────────┘
               │ 組み合わせ + ルール
┌──────────────▼──────────────────────┐
│  音節 (Syllable)                     │  音韻的レイヤー
│  "kar" + "th"                        │
└──────────────┬──────────────────────┘
               │ 組み合わせ
┌──────────────▼──────────────────────┐
│  文字/音素 (Phoneme)                 │  音響的レイヤー
│  [k] [a] [r] [θ]                    │
└─────────────────────────────────────┘
```

各レイヤーで**印象パラメータ**を持たせ、合成的に結果を制御します。

---

## 印象パラメータシステム

### パラメータの種類

#### 1. 音響的特性
生成される名前の音の質感を制御

| パラメータ | 範囲 | 説明 | 例 |
|-----------|------|------|-----|
| **hardness** | 0.0 ~ 1.0 | 硬さ | 0.0: Melina, 1.0: Kalgrok |
| **sharpness** | 0.0 ~ 1.0 | 鋭さ | 0.0: Thorun, 1.0: Kiriel |
| **complexity** | 0.0 ~ 1.0 | 複雑度 | 0.0: Rom, 1.0: Xyzanthius |
| **rhythmicity** | 0.0 ~ 1.0 | リズム規則性 | 0.0: Krthn, 1.0: Talala |

**音響特性の音韻マッピング:**
```
硬度 (hardness):
  - 高: k, t, g, d, p, b (破裂音)
  - 低: m, n, l, r, w, y (鼻音・流音・接近音)

鋭さ (sharpness):
  - 高: i, e (前舌母音), s, z, f (摩擦音)
  - 低: o, u, a (後舌母音・開母音)

複雑度 (complexity):
  - 高: 子音クラスタ, 多音節, 長い単語
  - 低: CV構造, 2音節以内, 短い単語

リズム性 (rhythmicity):
  - 高: CV-CV-CV (規則的)
  - 低: CCV-VC-CVC (不規則)
```

#### 2. 文化的印象
世界観や文化圏の雰囲気を制御

| パラメータ | 範囲 | 説明 | 例 |
|-----------|------|------|-----|
| **antiquity** | 0.0 ~ 1.0 | 古風さ | 0.0: Nex, 1.0: Aethelred |
| **formality** | 0.0 ~ 1.0 | 格式 | 0.0: Bob, 1.0: Alexandrius |
| **exoticism** | 0.0 ~ 1.0 | 異国性 | 0.0: John, 1.0: Xochitl |
| **mysticism** | 0.0 ~ 1.0 | 神秘性 | 0.0: Stone, 1.0: Azathoth |

#### 3. 構造的特性
名前の構造に関する制約

```csharp
public class StructuralConstraints {
    public int? MinSyllables { get; set; }      // 最小音節数
    public int? MaxSyllables { get; set; }      // 最大音節数
    public int? PreferredLength { get; set; }   // 推奨文字数
    public bool AllowConsonantClusters { get; set; }
    public bool RequireVowelHarmony { get; set; }
    public string? MustStartWith { get; set; }  // 特定文字で開始
    public string? MustEndWith { get; set; }    // 特定文字で終了
}
```

### 印象ベクトル

パラメータを統合的に管理する構造体。8つのパラメータ（音響4つ + 文化4つ）を保持し、ベクトル演算（Blend, Distance）をサポート。

詳細な定義は「データモデル」セクションを参照。

## ルール定義システム

### ハイブリッド階層方式

複数のルール表現を組み合わせ、段階的に複雑度を上げる設計:

```
レイヤー1: テンプレートベース生成
    ↓
レイヤー2: 正規表現による変換
    ↓
レイヤー3: 音韻規則による高度な変化
    ↓
レイヤー4: カスタムアクション
```

### レイヤー1: テンプレートベース生成

**用途**: 基本的な名前生成、プロトタイプ、シンプルなケース

**設定項目**: `syllablePool`（音節リスト）, `structure`（音節数・パターン）, `suffixes`（接尾辞）

**生成例**: "Karthor", "Valdun", "Thorvalir"

完全なJSON例は実装時に `Data/rulesets/ancient_norse.json` として作成。

### レイヤー2: 正規表現変換ルール

**用途**: 既存単語の派生、形容詞化、住民名化など

**ルール構成**: `match`（正規表現）, `replace`（置換パターン）, `impressionShift`（印象変化）, `condition`（適用条件）

**変換例**: Valdor → Valdorian, England → English, Carthage → Carthaginian

### レイヤー3: 音韻規則 (Phonological Rules)

**用途**: 言語学的に自然な音韻変化、高度な派生

**規則例**: 母音調和（kar-e-thor → kar-o-thor）, 鼻音同化（kamkor → kankor）, 語末無声化（valdeg → valdek）

**形式**: YAML/JSON形式で定義。`context`（適用文脈）, `change`（変化規則）を記述。

**用途**: プログラム可能な複雑な変換

```csharp
public interface INameTransformation {
    string Transform(string input, GenerationContext context);
    ImpressionVector GetImpressionShift();
}

// 実装例: 重複子音化
public class GeminationTransform : INameTransformation {
    public string Transform(string input, GenerationContext context) {
        if (input.Length < 2) return input;

        var lastChar = input[input.Length - 1];
        if (IsConsonant(lastChar)) {
            return input + lastChar + "e";
        }
        return input;
    }

    public ImpressionVector GetImpressionShift() {
        return new ImpressionVector {
            Hardness = 0.2f,
            Complexity = 0.1f
        };
    }
}
```

---

## データモデル

### 音節データベース

```csharp
public class Syllable {
    public string Id { get; set; }
    public string Pattern { get; set; }              // "kar", "tho", "lin"
    public SyllableStructure Structure { get; set; } // CVC, CV, VC...
    public ImpressionVector Impression { get; set; }
    public PhoneticConstraints Constraints { get; set; }
    public float Weight { get; set; } = 1.0f;        // 生成時の重み
}

public class SyllableStructure {
    public string Onset { get; set; }    // "k", "th", "str"
    public string Nucleus { get; set; }  // "a", "o", "ei"
    public string Coda { get; set; }     // "r", "th", "nt"
}

public class PhoneticConstraints {
    public string[] CanFollowSyllables { get; set; }
    public string[] CannotFollowSyllables { get; set; }
    public bool CanBeInitial { get; set; }
    public bool CanBeFinal { get; set; }
}
```

### ルールセット

```csharp
public class GenerationRuleset {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // 基本生成設定
    public BaseGenerationConfig BaseGeneration { get; set; }

    // 派生ルール
    public List<DerivationRule> DerivationRules { get; set; }

    // 音韻規則
    public List<PhonologicalRule> PhonologicalRules { get; set; }

    // 制約条件
    public List<IConstraint> Constraints { get; set; }

    // デフォルト印象
    public ImpressionVector DefaultImpression { get; set; }

    // 音節データベース参照
    public string SyllableDatabaseId { get; set; }
}

public class DerivationRule {
    public string Name { get; set; }
    public string MatchPattern { get; set; }        // 正規表現
    public string ReplacePattern { get; set; }
    public RuleCondition? Condition { get; set; }
    public ImpressionVector ImpressionShift { get; set; }
    public float Priority { get; set; } = 0.0f;
}

public class RuleCondition {
    public NameType? RequiredType { get; set; }
    public ImpressionRange? RequiredImpression { get; set; }
    public Func<string, bool>? CustomPredicate { get; set; }
}
```

### 生成コンテキスト

```csharp
public class GenerationContext {
    // 基本設定
    public NameType Type { get; set; }               // Person, Place, Title...
    public string? RulesetId { get; set; }           // 使用するルールセット

    // 目標印象
    public ImpressionVector? TargetImpression { get; set; }

    // 構造制約
    public StructuralConstraints? Constraints { get; set; }

    // シード（再現性）
    public int? Seed { get; set; }

    // 派生元（既存単語からの変換時）
    public string? BaseName { get; set; }

    // 概念パラメータ（Phase 3以降）
    public string[]? ConceptTags { get; set; }       // ["element_fire", "ancient"]
}

public enum NameType {
    Person,          // 人名
    Place,           // 地名
    PlaceAdjective,  // 地名の形容詞形
    PlaceResident,   // 地名の住民名
    Title,           // 称号
    Artifact,        // アーティファクト名
    Organization     // 組織名
}
```

### 生成結果

```csharp
public class GenerationResult {
    public string Name { get; set; }
    public ImpressionVector ActualImpression { get; set; }
    public GenerationMetadata Metadata { get; set; }
}

public class GenerationMetadata {
    public string[] AppliedRules { get; set; }
    public Syllable[] UsedSyllables { get; set; }
    public string RulesetId { get; set; }
    public float EuphonyScore { get; set; }         // 発音しやすさ
    public float ImpressionMatchScore { get; set; }  // 目標印象との一致度
}
```

---

## コンポーネント構成

```
LoreGen/
├── Sources/
│   ├── Core/                           # コアシステム
│   │   ├── INameGenerator.cs          # ジェネレータ基底インターフェース
│   │   ├── NameGenerator.cs           # メイン生成エンジン
│   │   ├── GenerationContext.cs       # 生成コンテキスト
│   │   ├── GenerationResult.cs        # 生成結果
│   │   ├── ImpressionVector.cs        # 印象パラメータ
│   │   └── StructuralConstraints.cs   # 構造制約
│   │
│   ├── Names/                          # 人名生成
│   │   ├── PersonNameGenerator.cs
│   │   └── PersonNameRuleset.cs
│   │
│   ├── Places/                         # 地名生成
│   │   ├── PlaceNameGenerator.cs
│   │   ├── PlaceType.cs               # 都市、山、川など
│   │   └── GeographicNamingRules.cs
│   │
│   ├── Rules/                          # ルールシステム (新規)
│   │   ├── IRuleset.cs
│   │   ├── Ruleset.cs
│   │   ├── DerivationRule.cs          # 正規表現変換
│   │   ├── PhonologicalRule.cs        # 音韻規則
│   │   └── RuleEngine.cs              # ルール適用エンジン
│   │
│   ├── Syllables/                     # 音節システム (新規)
│   │   ├── Syllable.cs
│   │   ├── SyllableDatabase.cs
│   │   ├── SyllableStructure.cs
│   │   └── PhoneticConstraints.cs
│   │
│   ├── Markov/                         # マルコフ連鎖生成
│   │   ├── MarkovChainGenerator.cs
│   │   ├── MarkovModel.cs
│   │   └── CorpusTrainer.cs
│   │
│   ├── Evaluation/                    # 評価システム (新規)
│   │   ├── EuphonyScorer.cs          # 発音しやすさ評価
│   │   ├── ImpressionMatcher.cs      # 印象一致度評価
│   │   └── DiversityChecker.cs       # 多様性チェック
│   │
│   ├── Data/                          # データファイル
│   │   ├── rulesets/
│   │   │   ├── ancient_norse.json
│   │   │   ├── medieval_english.json
│   │   │   └── fantasy_generic.json
│   │   ├── syllables/
│   │   │   ├── western.json
│   │   │   └── eastern.json
│   │   └── schemas/
│   │       └── ruleset_schema.json
│   │
│   └── Utilities/                     # ユーティリティ
│       ├── RandomProvider.cs          # シード可能な乱数生成
│       ├── StringHelper.cs            # 文字列操作
│       └── JsonLoader.cs              # データローダー
```

---

## 生成フロー

### 基本的な生成フロー

```
1. コンテキスト受付
   ↓
2. ルールセット選択
   ↓
3. 音節生成/選択
   ↓
4. 音節組み合わせ
   ↓
5. 派生ルール適用
   ↓
6. 音韻規則適用
   ↓
7. 制約チェック
   ↓
8. 評価・スコアリング
   ↓
9. 結果返却
```

### 既存単語からの派生フロー

```
1. コンテキスト受付 (BaseNameを含む)
   ↓
2. ルールセット選択
   ↓
3. 該当する派生ルールを検索
   ↓
4. ルール適用（正規表現変換）
   ↓
5. 音韻規則適用
   ↓
6. 印象シフト計算
   ↓
7. 結果返却
```

### 使用例

```csharp
// 基本的な生成
var generator = new NameGenerator();
var result = generator.Generate(new GenerationContext {
    Type = NameType.Place,
    RulesetId = "ancient_norse",
    TargetImpression = new ImpressionVector { Hardness = 0.7f },
    Seed = 12345
});
// result.Name: "Valdor"

// 派生（地名 → 形容詞）
var adj = generator.Generate(new GenerationContext {
    Type = NameType.PlaceAdjective,
    BaseName = "Valdor"
});
// adj.Name: "Valdorian"
```

---

## 生成手法の比較

### 実装予定の手法

| 手法 | 制御性 | 多様性 | 一貫性 | データ要件 | 実装フェーズ |
|------|--------|--------|--------|-----------|-------------|
| **音節組み合わせ + ルール** | ★★★ | ★★☆ | ★★★ | 中 | Phase 1-2 |
| **マルコフ連鎖** | ★☆☆ | ★★★ | ★★☆ | 大 | Phase 3 |
| **確率的CFG** | ★★★ | ★★☆ | ★★★ | 中 | Phase 4 (オプション) |

### 手法1: 音節組み合わせ + ルール（メイン手法）

**特徴:**
- 事前定義された音節から組み合わせて生成
- 正規表現と音韻規則で変換
- パラメータによる細かい制御

**強み:**
- ✅ 印象パラメータとの相性が良い
- ✅ 言語学的知見を直接組み込める
- ✅ 結果の説明可能性が高い
- ✅ データ要件が比較的小さい

**弱み:**
- ❌ 音節データベースの整備が必要
- ❌ 極端に多様な結果は出にくい

### 手法2: マルコフ連鎖（補助手法）

**特徴:**
- 既存の名前コーパスから学習
- n-gram統計に基づく生成
- より「人間らしい」結果

**強み:**
- ✅ 学習データがあれば高品質
- ✅ 実在の名前に近い自然さ
- ✅ 多様性が高い

**弱み:**
- ❌ パラメータ制御が難しい
- ❌ 大量の学習データが必要
- ❌ 結果の予測が困難

**用途:**
- ルールベース生成の補完
- リアルな地域名のバリエーション
- 学習データが豊富な場合

---

## 評価システム

### 主要な評価指標

- **発音しやすさ (Euphony Score)**: 子音クラスタや不自然な音の組み合わせにペナルティ、母音子音バランスにボーナス
- **印象一致度 (Impression Match Score)**: 目標パラメータと実際の生成結果のユークリッド距離
- **多様性チェック (Diversity)**: Levenshtein類似度による重複検出

## 世界観パラメータ（Phase 3-4）

### 概念レイヤーの導入

抽象的な世界観概念（元素属性、古代の雰囲気等）を音韻パラメータにマッピング。

**例: element_fire**
- impressionMapping: hardness 0.8, sharpness 0.7
- preferredPhonemes: k, t, r, s / a, i
- 生成例: "Kartor", "Takirus"

**例: element_water**
- impressionMapping: hardness 0.2, sharpness 0.3
- preferredPhonemes: m, n, l, w / u, o
- 生成例: "Mulon", "Nalwe"

### 使用例

```csharp
var result = generator.Generate(new GenerationContext {
    ConceptTags = new[] { "element_fire", "ancient" }
    // → 自動的にImpressionVectorに変換
});
// 火の要素 + 古代の雰囲気を持つ名前
```

複数の概念は重み付けでブレンドされるImpressionVectorに変換される。

### PhoneticConverter (別プロジェクト)

アルファベット⇔カタカナ変換機能は、LoreGenとは独立した別ライブラリとして実装を推奨。

**理由**: 単一責任の原則、他プロジェクトでの再利用性、LoreGenの依存関係を軽量に保つ

**機能**: `ToKatakana()`, `ToRomaji()`, `EstimatePronunciation()`

**使用例**: `PhoneticConverter.ToKatakana("Karthor")` → "カルソール"

**依存関係**: LoreGenはPhoneticConverterに依存しない（オプション参照のみ）

---

## 実装フェーズ

| Phase | 目標 | 主要機能 | 期間 |
|-------|------|---------|------|
| **Phase 1** | 基礎システム | 音節組み合わせ生成、データモデル、基本評価 | 1-2週間 |
| **Phase 2** | 派生・変換 | 正規表現ルール、印象パラメータ制御、複数ルールセット | 2-3週間 |
| **Phase 3** | 高度な手法 | マルコフ連鎖、概念パラメータ、ベンチマーク | 3-4週間 |
| **Phase 4** | 拡張機能 (オプション) | 音韻規則エンジン、カスタムアクション、ツール | 4-6週間 |

### Phase詳細

**Phase 1**: `Syllable`, `ImpressionVector`, `GenerationContext`の実装。音節組み合わせによる基本生成。
**Phase 2**: `DerivationRule`, `RuleEngine`の実装。地名→形容詞等の派生機能。
**Phase 3**: マルコフ連鎖ジェネレータ、複数手法の比較評価システム。
**Phase 4**: 音韻規則の完全実装、プラグイン可能なカスタムルール。

---

## 技術的考慮事項

### パフォーマンス

- **音節キャッシュ**: 頻繁に使用する音節組み合わせをキャッシュ
- **遅延評価**: ルール適用は必要な時のみ
- **並列生成**: バッチ生成時はParallel.ForEach

### テスト戦略

- **決定論性テスト**: 同じシードで同じ結果を生成
- **印象パラメータテスト**: 目標パラメータと実際の生成結果の一致度
- **派生ルールテスト**: 正しい変換規則の適用
- **制約チェックテスト**: 音節数、文字数等の制約遵守

### エラーハンドリング

- `GenerationException`: 生成失敗時の詳細情報（コンテキスト、失敗ルール）
- `RulesetValidator`: ルールセットの妥当性検証（循環参照、正規表現、データベース参照）

---

## まとめ

LoreGenは、**音韻学に基づいた印象制御可能な固有名詞生成エンジン**として設計されています。

### 主要な設計決定
1. **ハイブリッドルールシステム** - 段階的に複雑度を上げる
2. **印象パラメータ駆動** - 音響・文化・構造の多次元制御
3. **既存単語の変化対応** - 正規表現と音韻規則
4. **決定論的生成** - シード値による再現性
5. **Pure C#** - Unity以外でも利用可能

### 実装優先度
- **Phase 1-2**: 実用的な基本機能（必須）
- **Phase 3**: マルコフ連鎖と概念パラメータ（推奨）
- **Phase 4**: 音韻規則と高度なカスタマイズ（オプション）

### 拡張性
- 新しいルールセットの追加が容易
- カスタム変換ロジックのプラグイン
- データ駆動による多言語・多文化圏対応

このアーキテクチャにより、説得力のある架空の固有名詞を、制御可能な形で生成できます。
