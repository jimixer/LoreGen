using LoreGen.Core;
using LoreGen.Generation;
using NUnit.Framework;

namespace LoreGen.Tests.Generation;

[TestFixture]
public class NameGeneratorTests
{
    private NameGenerator _generator = null!;

    [SetUp]
    public void SetUp()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        _generator = new NameGenerator(database);
    }

    [Test]
    public void Generate_WithBasicContext_ReturnsValidName()
    {
        var context = new GenerationContext
        {
            Type = NameType.Place,
            Seed = 12345
        };

        var result = _generator.Generate(context);

        Assert.That(result.Name, Is.Not.Empty);
        Assert.That(char.IsUpper(result.Name[0]), Is.True, "名前は大文字で始まるべき");
    }

    [Test]
    public void Generate_WithSameSeed_ReturnsSameName()
    {
        var context1 = new GenerationContext { Seed = 42 };
        var context2 = new GenerationContext { Seed = 42 };

        var result1 = _generator.Generate(context1);
        var result2 = _generator.Generate(context2);

        Assert.That(result1.Name, Is.EqualTo(result2.Name));
    }

    [Test]
    public void Generate_WithDifferentSeeds_ReturnsDifferentNames()
    {
        var context1 = new GenerationContext { Seed = 123 };
        var context2 = new GenerationContext { Seed = 456 };

        var result1 = _generator.Generate(context1);
        var result2 = _generator.Generate(context2);

        Assert.That(result1.Name, Is.Not.EqualTo(result2.Name));
    }

    [Test]
    public void Generate_WithMinSyllables_RespectsConstraint()
    {
        var context = new GenerationContext
        {
            Seed = 12345,
            Constraints = new StructuralConstraints
            {
                MinSyllables = 3,
                MaxSyllables = 3
            }
        };

        var result = _generator.Generate(context);

        Assert.That(result.Metadata.UsedSyllables.Length, Is.EqualTo(3));
    }

    [Test]
    public void Generate_WithMaxSyllables_RespectsConstraint()
    {
        var context = new GenerationContext
        {
            Seed = 12345,
            Constraints = new StructuralConstraints
            {
                MinSyllables = 2,
                MaxSyllables = 2
            }
        };

        var result = _generator.Generate(context);

        Assert.That(result.Metadata.UsedSyllables.Length, Is.EqualTo(2));
    }

    [Test]
    public void Generate_CalculatesImpression()
    {
        var context = new GenerationContext { Seed = 42 };

        var result = _generator.Generate(context);

        Assert.That(result.ActualImpression.Hardness, Is.GreaterThanOrEqualTo(0.0f));
        Assert.That(result.ActualImpression.Hardness, Is.LessThanOrEqualTo(1.0f));
    }

    [Test]
    public void Generate_StoresUsedSyllables()
    {
        var context = new GenerationContext { Seed = 42 };

        var result = _generator.Generate(context);

        Assert.That(result.Metadata.UsedSyllables, Is.Not.Empty);
        Assert.That(result.Metadata.UsedSyllables.All(s => !string.IsNullOrEmpty(s)), Is.True);
    }

    [Test]
    public void Generate_MultipleGenerations_ProduceVariedResults()
    {
        var names = new HashSet<string>();

        for (int i = 0; i < 10; i++)
        {
            var context = new GenerationContext { Seed = i };
            var result = _generator.Generate(context);
            names.Add(result.Name);
        }

        // 10回生成して、少なくとも3種類以上の名前が生成されることを期待
        Assert.That(names.Count, Is.GreaterThanOrEqualTo(3));
    }

    // Phase 2.5: 発音品質改善テスト

    [Test]
    public void Generate_AvoidsDuplicateSyllables()
    {
        // 複数回生成して同一音節の連続がないことを確認
        for (int seed = 0; seed < 20; seed++)
        {
            var result = _generator.Generate(new GenerationContext
            {
                Seed = seed,
                Constraints = new StructuralConstraints { MinSyllables = 3, MaxSyllables = 3 }
            });

            var syllables = result.Metadata.UsedSyllables;
            for (int i = 1; i < syllables.Length; i++)
            {
                Assert.That(syllables[i], Is.Not.EqualTo(syllables[i - 1]),
                    $"同一音節が連続: {syllables[i - 1]} -> {syllables[i]} in {result.Name}");
            }
        }
    }

    [Test]
    public void Generate_MinimizesVowelClusters()
    {
        // 複数回生成して母音連続が最小化されていることを確認
        int vowelClusterCount = 0;
        const int iterations = 50;

        for (int seed = 0; seed < iterations; seed++)
        {
            var result = _generator.Generate(new GenerationContext
            {
                Seed = seed,
                Constraints = new StructuralConstraints { MinSyllables = 2, MaxSyllables = 4 }
            });

            // 母音連続を検出（単純化: 連続する2文字が母音かチェック）
            var vowels = "aeiouAEIOU";
            for (int i = 0; i < result.Name.Length - 1; i++)
            {
                if (vowels.Contains(result.Name[i]) && vowels.Contains(result.Name[i + 1]))
                {
                    vowelClusterCount++;
                    break; // この名前では1回だけカウント
                }
            }
        }

        // 50回中、母音連続は10回以下であることを期待（80%以上で回避）
        Assert.That(vowelClusterCount, Is.LessThanOrEqualTo(10),
            $"母音連続が頻発: {vowelClusterCount}/{iterations}回");
    }

    [Test]
    public void Generate_ProducesNaturalSoundingNames()
    {
        // 総合的な発音品質確認（統合テスト）
        var problematicPatterns = new[]
        {
            @"(\w)\1{2,}",  // 3文字以上の同じ文字連続
            @"[bcdfghjklmnpqrstvwxz]{5,}"  // 5文字以上の子音連続（4文字までは許容）
        };

        for (int seed = 0; seed < 30; seed++)
        {
            var result = _generator.Generate(new GenerationContext
            {
                Seed = seed,
                Constraints = new StructuralConstraints { MinSyllables = 2, MaxSyllables = 3 }
            });

            foreach (var pattern in problematicPatterns)
            {
                Assert.That(System.Text.RegularExpressions.Regex.IsMatch(result.Name.ToLower(), pattern),
                    Is.False,
                    $"不自然なパターン検出: {result.Name} matches {pattern}");
            }
        }
    }
}
