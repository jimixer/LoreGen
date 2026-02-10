using LoreGen.Core;
using LoreGen.Generation;
using NUnit.Framework;

namespace LoreGen.Tests.Manual;

/// <summary>
/// 名前生成のデモ（手動実行のみ）
/// Visual Studio Test Explorer で右クリック → "Run Selected Tests" で実行
/// </summary>
[TestFixture]
public class NameGenerationDemo
{
    [Test]
    [Explicit("手動実行専用：生成結果の観察用")]
    public void GenerateFantasyPlaceNames()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database);

        Console.WriteLine("=== Fantasy Place Names ===\n");

        for (int i = 0; i < 15; i++)
        {
            var context = new GenerationContext
            {
                Type = NameType.Place,
                Constraints = new StructuralConstraints
                {
                    MinSyllables = 2,
                    MaxSyllables = 3
                }
            };

            var result = generator.Generate(context);
            var imp = result.ActualImpression;

            Console.WriteLine($"{i + 1,2}. {result.Name,-12} " +
                $"[{string.Join("-", result.Metadata.UsedSyllables)}]");
            Console.WriteLine($"    印象: 硬度={imp.Hardness:F2} 鋭さ={imp.Sharpness:F2} " +
                $"複雑度={imp.Complexity:F2}");
            Console.WriteLine($"    　　　古風={imp.Antiquity:F2} 格式={imp.Formality:F2} " +
                $"神秘={imp.Mysticism:F2}\n");
        }
    }

    [Test]
    [Explicit("手動実行専用：音節数比較")]
    public void CompareSyllableCounts()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database);

        Console.WriteLine("=== Syllable Count Comparison ===\n");

        for (int syllables = 2; syllables <= 4; syllables++)
        {
            Console.WriteLine($"--- {syllables} Syllables ---");

            for (int i = 0; i < 5; i++)
            {
                var context = new GenerationContext
                {
                    Constraints = new StructuralConstraints
                    {
                        MinSyllables = syllables,
                        MaxSyllables = syllables
                    }
                };

                var result = generator.Generate(context);
                Console.WriteLine($"  {result.Name}");
            }
            Console.WriteLine();
        }
    }

    [Test]
    [Explicit("手動実行専用：印象値分布の観察")]
    public void ObserveImpressionDistribution()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database);

        Console.WriteLine("=== Impression Distribution (50 samples) ===\n");

        var hardnessValues = new List<float>();
        var sharpnessValues = new List<float>();
        var names = new List<string>();

        for (int i = 0; i < 50; i++)
        {
            var result = generator.Generate(new GenerationContext());
            hardnessValues.Add(result.ActualImpression.Hardness);
            sharpnessValues.Add(result.ActualImpression.Sharpness);
            names.Add(result.Name);
        }

        Console.WriteLine($"硬度 (Hardness):  平均={hardnessValues.Average():F3} " +
            $"最小={hardnessValues.Min():F3} 最大={hardnessValues.Max():F3}");
        Console.WriteLine($"鋭さ (Sharpness): 平均={sharpnessValues.Average():F3} " +
            $"最小={sharpnessValues.Min():F3} 最大={sharpnessValues.Max():F3}\n");

        Console.WriteLine("硬い名前 Top 5:");
        var hardest = hardnessValues.Select((h, idx) => (h, names[idx]))
            .OrderByDescending(x => x.h).Take(5);
        foreach (var (h, name) in hardest)
            Console.WriteLine($"  {name,-12} (硬度: {h:F3})");

        Console.WriteLine("\n柔らかい名前 Top 5:");
        var softest = hardnessValues.Select((h, idx) => (h, names[idx]))
            .OrderBy(x => x.h).Take(5);
        foreach (var (h, name) in softest)
            Console.WriteLine($"  {name,-12} (硬度: {h:F3})");
    }
}
