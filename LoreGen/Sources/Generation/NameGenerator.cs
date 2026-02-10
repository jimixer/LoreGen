using System;
using System.Collections.Generic;
using System.Linq;
using LoreGen.Core;
using LoreGen.Data;
using LoreGen.Rules;
using LoreGen.Utilities;

namespace LoreGen.Generation;

/// <summary>
/// 名前生成器: 音節組み合わせ + 派生ルール対応
/// </summary>
public class NameGenerator
{
    private readonly SyllableDatabase _database;
    private readonly DerivationEngine? _derivationEngine;

    public NameGenerator(SyllableDatabase database) : this(database, null)
    {
    }

    public NameGenerator(SyllableDatabase database, DerivationEngine? derivationEngine)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _derivationEngine = derivationEngine;
    }

    /// <summary>指定されたコンテキストで名前を生成</summary>
    public GenerationResult Generate(GenerationContext context)
    {
        // BaseNameが指定されている場合は派生生成
        if (!string.IsNullOrEmpty(context.BaseName) && _derivationEngine != null)
        {
            return _derivationEngine.Derive(context.BaseName, context);
        }

        var random = new RandomProvider(context.Seed ?? Guid.NewGuid().GetHashCode());
        var constraints = context.Constraints ?? new StructuralConstraints();

        // 音節数の決定
        int syllableCount = DetermineSyllableCount(constraints, random);

        // 音節の選択と組み合わせ
        var syllables = SelectSyllables(syllableCount, constraints, random);
        var name = CombineSyllables(syllables);

        // 印象ベクトルの計算
        var impression = CalculateImpression(syllables);

        return new GenerationResult
        {
            Name = name,
            ActualImpression = impression,
            Metadata = new GenerationMetadata
            {
                UsedSyllables = syllables.Select(s => s.Pattern).ToArray(),
                RulesetId = string.Empty
            }
        };
    }

    private int DetermineSyllableCount(StructuralConstraints constraints, RandomProvider random)
    {
        int min = constraints.MinSyllables ?? 2;
        int max = constraints.MaxSyllables ?? 3;
        return random.Next(min, max + 1);
    }

    private List<Syllable> SelectSyllables(int count, StructuralConstraints constraints, RandomProvider random)
    {
        var result = new List<Syllable>();

        for (int i = 0; i < count; i++)
        {
            Syllable? selected;

            if (i == 0)
            {
                // 語頭: 語頭配置可能な音節から選択
                var candidates = _database.GetInitialSyllables().ToList();
                selected = random.ChooseWeighted(candidates, s => s.Weight);
            }
            else if (i == count - 1)
            {
                // 語末: 語末配置可能 & 先行音節との連続可能性を考慮
                var previous = result[i - 1];
                var followingCandidates = _database.GetFollowingSyllables(previous).ToList();
                var finalCandidates = followingCandidates.Where(s => s.Constraints.CanBeFinal).ToList();

                selected = finalCandidates.Count > 0
                    ? random.ChooseWeighted(finalCandidates, s => s.Weight)
                    : random.ChooseWeighted(followingCandidates, s => s.Weight);
            }
            else
            {
                // 中間: 先行音節との連続可能性を考慮
                var previous = result[i - 1];
                var candidates = _database.GetFollowingSyllables(previous).ToList();
                selected = random.ChooseWeighted(candidates, s => s.Weight);
            }

            if (selected == null)
                throw new InvalidOperationException($"No suitable syllable found at position {i}");

            result.Add(selected);
        }

        return result;
    }

    private string CombineSyllables(List<Syllable> syllables)
    {
        if (syllables.Count == 0)
            return string.Empty;

        var name = string.Join("", syllables.Select(s => s.Pattern));

        // 先頭を大文字化
        if (name.Length > 0)
            name = char.ToUpperInvariant(name[0]) + name.Substring(1);

        return name;
    }

    private ImpressionVector CalculateImpression(List<Syllable> syllables)
    {
        if (syllables.Count == 0)
            return new ImpressionVector();

        // 各音節の印象ベクトルの平均を算出
        var sum = new ImpressionVector();
        foreach (var syllable in syllables)
        {
            sum.Hardness += syllable.Impression.Hardness;
            sum.Sharpness += syllable.Impression.Sharpness;
            sum.Complexity += syllable.Impression.Complexity;
            sum.Rhythmicity += syllable.Impression.Rhythmicity;
            sum.Antiquity += syllable.Impression.Antiquity;
            sum.Formality += syllable.Impression.Formality;
            sum.Exoticism += syllable.Impression.Exoticism;
            sum.Mysticism += syllable.Impression.Mysticism;
        }

        float count = syllables.Count;
        return new ImpressionVector
        {
            Hardness = sum.Hardness / count,
            Sharpness = sum.Sharpness / count,
            Complexity = sum.Complexity / count,
            Rhythmicity = sum.Rhythmicity / count,
            Antiquity = sum.Antiquity / count,
            Formality = sum.Formality / count,
            Exoticism = sum.Exoticism / count,
            Mysticism = sum.Mysticism / count
        };
    }
}
