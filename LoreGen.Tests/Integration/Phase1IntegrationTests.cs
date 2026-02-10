using LoreGen.Core;
using LoreGen.Generation;
using NUnit.Framework;

namespace LoreGen.Tests.Integration;

/// <summary>
/// Phase 1 統合テスト: エンドツーエンドのシナリオ検証
/// </summary>
[TestFixture]
public class Phase1IntegrationTests
{
    [Test]
    public void EndToEnd_GenerateFantasyPlaceName()
    {
        // 1. 音節データベース準備
        var database = SampleSyllables.CreateBasicFantasyDatabase();

        // 2. ジェネレータ作成
        var generator = new NameGenerator(database);

        // 3. コンテキスト設定
        var context = new GenerationContext
        {
            Type = NameType.Place,
            Seed = 42,
            Constraints = new StructuralConstraints
            {
                MinSyllables = 2,
                MaxSyllables = 3
            }
        };

        // 4. 名前生成
        var result = generator.Generate(context);

        // 5. 結果検証
        Assert.That(result.Name, Is.Not.Empty, "名前が生成されるべき");
        Assert.That(char.IsUpper(result.Name[0]), Is.True, "先頭は大文字");
        Assert.That(result.Metadata.UsedSyllables.Length, Is.InRange(2, 3), "音節数制約");
        Assert.That(result.ActualImpression.Hardness, Is.InRange(0.0f, 1.0f), "印象値範囲");
    }

    [Test]
    public void EndToEnd_DeterministicGeneration()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database);

        var context1 = new GenerationContext { Seed = 123 };
        var context2 = new GenerationContext { Seed = 123 };

        var result1 = generator.Generate(context1);
        var result2 = generator.Generate(context2);

        Assert.That(result1.Name, Is.EqualTo(result2.Name), "同じシードで同じ結果");
        Assert.That(result1.ActualImpression.Hardness, Is.EqualTo(result2.ActualImpression.Hardness));
    }

    [Test]
    public void EndToEnd_GenerateMultipleNames()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database);

        var names = new List<string>();

        for (int i = 0; i < 20; i++)
        {
            var context = new GenerationContext
            {
                Type = NameType.Place,
                Seed = i
            };

            var result = generator.Generate(context);
            names.Add(result.Name);

            // 各名前の基本検証
            Assert.That(result.Name, Is.Not.Empty);
            Assert.That(char.IsUpper(result.Name[0]), Is.True);
            Assert.That(result.Metadata.UsedSyllables.Length, Is.GreaterThanOrEqualTo(2));
        }

        // 多様性チェック
        var uniqueNames = names.Distinct().Count();
        Assert.That(uniqueNames, Is.GreaterThanOrEqualTo(10), "十分な多様性");
    }

    [Test]
    public void EndToEnd_ImpressionVectorCalculation()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database);

        var context = new GenerationContext { Seed = 42 };
        var result = generator.Generate(context);

        // 印象ベクトルが計算されていることを確認
        var impression = result.ActualImpression;

        Assert.That(impression.Hardness, Is.GreaterThanOrEqualTo(0.0f));
        Assert.That(impression.Sharpness, Is.GreaterThanOrEqualTo(0.0f));
        Assert.That(impression.Complexity, Is.GreaterThanOrEqualTo(0.0f));
        Assert.That(impression.Rhythmicity, Is.GreaterThanOrEqualTo(0.0f));
        Assert.That(impression.Antiquity, Is.GreaterThanOrEqualTo(0.0f));
        Assert.That(impression.Formality, Is.GreaterThanOrEqualTo(0.0f));
        Assert.That(impression.Exoticism, Is.GreaterThanOrEqualTo(0.0f));
        Assert.That(impression.Mysticism, Is.GreaterThanOrEqualTo(0.0f));
    }

    [Test]
    public void EndToEnd_ConstraintValidation()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database);

        // 異なる音節数制約でテスト
        for (int syllables = 2; syllables <= 4; syllables++)
        {
            var context = new GenerationContext
            {
                Seed = syllables * 100,
                Constraints = new StructuralConstraints
                {
                    MinSyllables = syllables,
                    MaxSyllables = syllables
                }
            };

            var result = generator.Generate(context);

            Assert.That(result.Metadata.UsedSyllables.Length, Is.EqualTo(syllables),
                $"{syllables}音節の制約が守られているべき");
        }
    }
}
