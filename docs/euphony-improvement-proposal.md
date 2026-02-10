# 発音品質改善提案

## 現状の課題

Phase 2実装完了時点で、以下の発音上の課題が観察されました：

1. **同じ母音が連続する**: Elria (e-i-a), Morriavar (o-i-a-a)
2. **同じ音節が連続する**: Mormor, Dordor, Krokrovar

これらは言語学的に不自然で、発音しづらい名前を生成する原因となります。

---

## アーキテクチャとの整合性

### 既存の設計による対応可能性

architecture.md の設計思想と照らし合わせると、これらの課題は**想定済み**であり、段階的な実装計画に組み込まれています：

#### 1. PhoneticConstraints（Phase 1: ✅ 実装済み）

```csharp
public class PhoneticConstraints {
    public string[] CanFollowSyllables { get; set; }
    public string[] CannotFollowSyllables { get; set; }
    public bool CanBeInitial { get; set; }
    public bool CanBeFinal { get; set; }
}
```

**現状**: 音節レベルの連続制約に対応
**限界**: 同一音節の連続を直接的に防げない（"dor" → "dor" の連続）

#### 2. EuphonyScorer（Phase 3: ⏳ 計画中）

architecture.md より：
> **発音しやすさ (Euphony Score)**: 子音クラスタや不自然な音の組み合わせにペナルティ、母音子音バランスにボーナス

**設計意図**: 総合的な発音品質評価システム
**適用範囲**: 母音連続、子音クラスタ、音節パターンを統合評価

#### 3. PhonologicalRule（Phase 3-4: ⏳ 計画中）

```yaml
# 母音調和の例
context: "kar-e-thor"
change: "kar-o-thor"  # 母音を調和させる
```

**設計意図**: 言語学的に自然な音韻変化
**適用範囲**: 母音調和、鼻音同化などの高度な変換

---

## 解決方針の推奨

### 🎯 短期的解決（Phase 2拡張として実装）

**目的**: 現行システムの範囲内で即座に改善
**アプローチ**: 音節選択ロジックに軽量な制約を追加

#### 実装案A: PhoneticConstraints拡張

```csharp
public class PhoneticConstraints {
    // 既存のプロパティ
    public string[] CanFollowSyllables { get; set; }
    public string[] CannotFollowSyllables { get; set; }
    public bool CanBeInitial { get; set; }
    public bool CanBeFinal { get; set; }

    // 新規追加（Phase 2.5）
    public bool PreventConsecutiveSame { get; set; } = true;  // 同一音節連続防止
    public int? MinVowelInterval { get; set; }                // 母音間の最小子音数
}
```

#### 実装案B: SyllableStructure拡張

```csharp
public class SyllableStructure {
    public string Onset { get; set; }
    public string Nucleus { get; set; }
    public string Coda { get; set; }

    // 新規追加（Phase 2.5）
    public VowelType MainVowel { get; set; }  // a, e, i, o, u
    public bool HasConsonantCluster { get; set; }
}

public enum VowelType {
    None,   // 子音のみ
    A, E, I, O, U
}
```

#### 実装箇所: NameGenerator.SelectSyllables()

```csharp
private List<Syllable> SelectSyllables(int count, StructuralConstraints constraints, RandomProvider random)
{
    var result = new List<Syllable>();
    Syllable? previous = null;

    for (int i = 0; i < count; i++)
    {
        var candidates = GetCandidates(i, count, previous, result);

        // 新規: 同一音節連続を除外
        if (previous != null)
        {
            candidates = candidates.Where(s => s.Id != previous.Id).ToList();
        }

        // 新規: 母音連続を回避
        if (previous != null && HasVowelEnding(previous))
        {
            candidates = candidates.Where(s => !HasVowelStart(s)).ToList();
        }

        var selected = random.ChooseWeighted(candidates, s => s.Weight);
        result.Add(selected);
        previous = selected;
    }

    return result;
}

private bool HasVowelEnding(Syllable syllable)
{
    return string.IsNullOrEmpty(syllable.Structure.Coda) &&
           !string.IsNullOrEmpty(syllable.Structure.Nucleus);
}

private bool HasVowelStart(Syllable syllable)
{
    return string.IsNullOrEmpty(syllable.Structure.Onset);
}
```

**メリット**:
- ✅ 既存アーキテクチャとの整合性が高い
- ✅ 後方互換性を維持（デフォルト動作を変えない）
- ✅ データ駆動（音節データベースで制御可能）
- ✅ 実装コストが低い（1-2日）

**デメリット**:
- ❌ 完全な解決ではない（複雑なパターンは未対応）
- ❌ パフォーマンスへの軽微な影響

---

### 🚀 中長期的解決（Phase 3として実装）

**目的**: 発音品質を本格的に評価・最適化
**アプローチ**: 評価システムの導入とスコアリング統合

#### Phase 3: 評価システム実装

```csharp
// Evaluation/EuphonyScorer.cs
public class EuphonyScorer
{
    public float CalculateScore(string name, List<Syllable> syllables)
    {
        float score = 1.0f;

        // ペナルティ計算
        score -= DetectVowelCluster(name) * 0.3f;       // 母音連続: -30%
        score -= DetectConsonantCluster(name) * 0.2f;   // 子音クラスタ: -20%
        score -= DetectIdenticalSyllables(syllables) * 0.4f;  // 同一音節: -40%

        // ボーナス計算
        score += CalculateVowelConsonantBalance(name) * 0.2f;  // バランス: +20%
        score += CalculateRhythmicVariety(syllables) * 0.1f;   // リズム: +10%

        return Math.Clamp(score, 0.0f, 1.0f);
    }

    private float DetectVowelCluster(string name)
    {
        // 母音が2つ以上連続する回数をカウント
        var vowels = "aeiouAEIOU";
        int clusters = 0;
        int consecutiveVowels = 0;

        foreach (char c in name)
        {
            if (vowels.Contains(c))
            {
                consecutiveVowels++;
                if (consecutiveVowels >= 2) clusters++;
            }
            else
            {
                consecutiveVowels = 0;
            }
        }

        return clusters / (float)Math.Max(name.Length / 3, 1);
    }

    private float DetectIdenticalSyllables(List<Syllable> syllables)
    {
        float penalty = 0.0f;
        for (int i = 1; i < syllables.Count; i++)
        {
            if (syllables[i].Id == syllables[i - 1].Id)
                penalty += 1.0f;
        }
        return penalty / Math.Max(syllables.Count - 1, 1);
    }
}
```

#### GenerationResult への統合

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

    // Phase 3で追加
    public float EuphonyScore { get; set; }             // 0.0 ~ 1.0
    public float ImpressionMatchScore { get; set; }
    public string[] QualityWarnings { get; set; }       // ["vowel_cluster", "identical_syllables"]
}
```

#### NameGenerator への統合（オプション）

```csharp
public class NameGenerator
{
    private readonly EuphonyScorer? _euphonyScorer;

    public NameGenerator(
        SyllableDatabase database,
        DerivationEngine? derivationEngine = null,
        EuphonyScorer? euphonyScorer = null)  // 新規
    {
        _database = database;
        _derivationEngine = derivationEngine;
        _euphonyScorer = euphonyScorer;
    }

    public GenerationResult Generate(GenerationContext context)
    {
        // ... 既存の生成ロジック ...

        // Phase 3: 品質評価
        if (_euphonyScorer != null)
        {
            var euphonyScore = _euphonyScorer.CalculateScore(name, syllables);

            // 低品質な場合は再生成（オプション）
            if (euphonyScore < 0.5f && context.RequireHighQuality)
            {
                return Generate(context);  // リトライ
            }

            result.Metadata.EuphonyScore = euphonyScore;
        }

        return result;
    }
}
```

**メリット**:
- ✅ 包括的な品質評価
- ✅ 将来の拡張が容易（新しい評価指標の追加）
- ✅ デバッグとチューニングが容易（スコア可視化）
- ✅ オプション機能として追加（既存コードへの影響なし）

**デメリット**:
- ❌ 実装コストが高い（1-2週間）
- ❌ パフォーマンスオーバーヘッド
- ❌ 過度な制約は多様性を損なう可能性

---

### 🔧 音韻規則による事後修正（Phase 4: オプション）

**目的**: 生成後の名前を言語学的に自然に修正
**アプローチ**: 音韻規則による変換

#### 音韻規則の例

```yaml
# rules/euphony_fixes.yaml
rules:
  - name: "母音連続の子音挿入"
    pattern: "([aeiou])([aeiou])"
    replacement: "$1r$2"
    examples:
      - "Elria" → "Elrria"（ただし別ルールで3連続を回避）

  - name: "同一音節連続の母音挿入"
    pattern: "(\\w+)\\1"
    replacement: "$1e$1"
    examples:
      - "Mormor" → "Moremor"
      - "Dordor" → "Doredor"
```

**メリット**:
- ✅ 既存の生成ロジックを変更不要
- ✅ 柔軟なルール定義（データ駆動）
- ✅ 言語学的に正確な変換

**デメリット**:
- ❌ 実装コストが非常に高い（Phase 4相当）
- ❌ ルール設計の難易度が高い
- ❌ 印象ベクトルの再計算が必要

---

## 推奨実装ロードマップ

### 📅 タイムライン

| フェーズ | 実装内容 | 期間 | 優先度 |
|---------|---------|------|--------|
| **Phase 2.5** | SelectSyllables()に軽量制約追加 | 1-2日 | ★★★ 必須 |
| **Phase 3.1** | EuphonyScorer基本実装 | 3-5日 | ★★☆ 推奨 |
| **Phase 3.2** | GenerationResultへの統合 | 1-2日 | ★★☆ 推奨 |
| **Phase 3.3** | 低品質時の自動リトライ | 1日 | ★☆☆ オプション |
| **Phase 4** | 音韻規則エンジン | 2-3週間 | ☆☆☆ 将来的 |

### 🎬 即座に実装すべき内容（Phase 2.5）

1. **同一音節連続防止**
```csharp
// SelectSyllables() 内
if (previous != null)
{
    candidates = candidates.Where(s => s.Id != previous.Id).ToList();
}
```

2. **母音連続回避**
```csharp
// SyllableStructure にヘルパーメソッド追加
public bool EndsWithVowel() => string.IsNullOrEmpty(Coda);
public bool StartsWithVowel() => string.IsNullOrEmpty(Onset);

// SelectSyllables() 内
if (previous?.Structure.EndsWithVowel() == true)
{
    candidates = candidates.Where(s => !s.Structure.StartsWithVowel()).ToList();
}
```

3. **テストケース追加**
```csharp
[Test]
public void Generate_AvoidsDuplicateSyllables()
{
    var result = generator.Generate(new GenerationContext {
        Constraints = new StructuralConstraints { MinSyllables = 3 }
    });

    var syllables = result.Metadata.UsedSyllables;
    for (int i = 1; i < syllables.Length; i++)
    {
        Assert.That(syllables[i].Id, Is.Not.EqualTo(syllables[i-1].Id));
    }
}

[Test]
public void Generate_AvoidsVowelClusters()
{
    // 連続母音を検出する正規表現テスト
    var result = generator.Generate(new GenerationContext());
    Assert.That(Regex.IsMatch(result.Name, "[aeiou]{2}"), Is.False);
}
```

---

## まとめ

### ✅ アーキテクチャとの整合性

現状の課題は、**architecture.md で明示的に計画されている**機能領域です：

- ✅ EuphonyScorer: 「発音しやすさ評価」として設計済み
- ✅ PhonologicalRule: 「音韻規則」として Phase 3-4 に配置
- ✅ データ駆動設計: 音節データベースでの細かい制御

### 🎯 推奨アクション

1. **即時（Phase 2.5）**: SelectSyllables()に軽量制約を追加（1-2日）
   - 同一音節連続防止
   - 基本的な母音連続回避

2. **近い将来（Phase 3）**: EuphonyScorer実装（1週間）
   - 総合的な品質評価
   - Metadata への統合

3. **長期的（Phase 4）**: 音韻規則エンジン（オプション）
   - 高度な音韻変化
   - 言語学的精度の向上

### 📊 トレードオフ

| 手法 | 実装コスト | 効果 | 拡張性 | 推奨度 |
|------|----------|------|--------|--------|
| Phase 2.5 軽量制約 | ★☆☆ | ★★☆ | ★★☆ | ★★★ |
| Phase 3 評価システム | ★★☆ | ★★★ | ★★★ | ★★☆ |
| Phase 4 音韻規則 | ★★★ | ★★★ | ★★★ | ★☆☆ |

現状の音節データベース（37音節）でも、軽量制約の追加だけで大幅に品質が向上します。**Phase 2.5の実装を強く推奨します。**
