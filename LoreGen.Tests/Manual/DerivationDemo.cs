using LoreGen.Core;
using LoreGen.Generation;
using LoreGen.Rules;
using NUnit.Framework;

namespace LoreGen.Tests.Manual;

/// <summary>
/// 派生ルール機能のデモ（Phase 2手動実行専用）
/// Visual Studio Test Explorer で右クリック → "Run Selected Tests" で実行
/// </summary>
[TestFixture]
public class DerivationDemo
{
    /// <summary>
    /// 地名から形容詞・住民名への派生を観察
    /// </summary>
    [Test]
    [Explicit("手動実行専用：派生機能の基本動作")]
    public void DeriveAdjectivesAndResidents()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database);

        Console.WriteLine("=== 地名から派生形を生成 ===\n");

        // まず基本の地名を生成
        var baseNames = new List<string>();
        for (int i = 0; i < 8; i++)
        {
            var result = generator.Generate(new GenerationContext
            {
                Type = NameType.Place,
                Seed = i * 10,
                Constraints = new StructuralConstraints { MinSyllables = 2, MaxSyllables = 3 }
            });
            baseNames.Add(result.Name);
        }

        // Fantasy ルールセットで派生
        var ruleset = SampleRulesets.CreateFantasyRuleset();
        var engine = new DerivationEngine(ruleset);
        var derivativeGenerator = new NameGenerator(database, engine);

        Console.WriteLine($"{"地名",-15} {"形容詞形",-15} {"住民名",-15}");
        Console.WriteLine(new string('-', 48));

        foreach (var baseName in baseNames)
        {
            var adjective = derivativeGenerator.Generate(new GenerationContext
            {
                Type = NameType.PlaceAdjective,
                BaseName = baseName
            });

            var resident = derivativeGenerator.Generate(new GenerationContext
            {
                Type = NameType.PlaceResident,
                BaseName = baseName
            });

            Console.WriteLine($"{baseName,-15} {adjective.Name,-15} {resident.Name,-15}");
        }
    }

    /// <summary>
    /// 3つのルールセット（Fantasy/English/Ancient）を比較
    /// </summary>
    [Test]
    [Explicit("手動実行専用：ルールセット比較")]
    public void CompareRulesets()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var baseGenerator = new NameGenerator(database);

        var fantasy = SampleRulesets.CreateFantasyRuleset();
        var english = SampleRulesets.CreateEnglishStyleRuleset();
        var ancient = SampleRulesets.CreateAncientRuleset();

        Console.WriteLine("=== ルールセット比較 ===\n");

        // テスト用の地名
        var testNames = new[] { "Valdor", "Karthal", "America", "England", "Romulus", "Macedon" };

        Console.WriteLine($"{"地名",-12} {"Fantasy",-15} {"English",-15} {"Ancient",-15}");
        Console.WriteLine(new string('-', 60));

        foreach (var baseName in testNames)
        {
            var fantasyResult = ApplyRuleset(fantasy, baseName);
            var englishResult = ApplyRuleset(english, baseName);
            var ancientResult = ApplyRuleset(ancient, baseName);

            Console.WriteLine($"{baseName,-12} {fantasyResult,-15} {englishResult,-15} {ancientResult,-15}");
        }

        string ApplyRuleset(Ruleset ruleset, string baseName)
        {
            var engine = new DerivationEngine(ruleset);
            var generator = new NameGenerator(database, engine);
            var result = generator.Generate(new GenerationContext { BaseName = baseName });
            return result.Name;
        }
    }

    /// <summary>
    /// 派生前後の印象値変化を観察
    /// </summary>
    [Test]
    [Explicit("手動実行専用：印象値シフト")]
    public void ObserveImpressionShift()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var baseGenerator = new NameGenerator(database);

        var ruleset = SampleRulesets.CreateFantasyRuleset();
        var engine = new DerivationEngine(ruleset);
        var derivativeGenerator = new NameGenerator(database, engine);

        Console.WriteLine("=== 派生による印象値変化 ===\n");

        for (int i = 0; i < 10; i++)
        {
            // 基本地名を生成
            var baseResult = baseGenerator.Generate(new GenerationContext
            {
                Type = NameType.Place,
                Seed = i * 10,
                Constraints = new StructuralConstraints { MinSyllables = 2, MaxSyllables = 3 }
            });

            // 形容詞形に派生
            var derivedResult = derivativeGenerator.Generate(new GenerationContext
            {
                Type = NameType.PlaceAdjective,
                BaseName = baseResult.Name
            });

            var baseImp = baseResult.ActualImpression;
            var derivedImp = derivedResult.ActualImpression;

            Console.WriteLine($"{i + 1,2}. {baseResult.Name} → {derivedResult.Name}");
            Console.WriteLine($"    格式: {baseImp.Formality:F2} → {derivedImp.Formality:F2} " +
                $"(変化: {derivedImp.Formality - baseImp.Formality:+0.00;-0.00})");
            Console.WriteLine($"    古風: {baseImp.Antiquity:F2} → {derivedImp.Antiquity:F2} " +
                $"(変化: {derivedImp.Antiquity - baseImp.Antiquity:+0.00;-0.00})");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// 優先度による派生ルール選択を観察
    /// </summary>
    [Test]
    [Explicit("手動実行専用：優先度システム")]
    public void ObserveRulePriority()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();

        // 優先度の異なるルールセットを作成
        var highPriorityRuleset = new Ruleset
        {
            Id = "priority-test",
            Name = "Priority Test",
            DerivationRules = new List<DerivationRule>
            {
                new DerivationRule
                {
                    Name = "High Priority: -land → -lish",
                    MatchPattern = "land$",
                    ReplacePattern = "lish",
                    Priority = 3.0f,
                    ImpressionShift = new ImpressionVector { Formality = 0.2f }
                },
                new DerivationRule
                {
                    Name = "Low Priority: Generic -ian",
                    MatchPattern = ".*",
                    ReplacePattern = "$0ian",
                    Priority = 1.0f,
                    ImpressionShift = new ImpressionVector { Formality = 0.1f }
                }
            }
        };

        var engine = new DerivationEngine(highPriorityRuleset);
        var generator = new NameGenerator(database, engine);

        Console.WriteLine("=== 優先度による派生ルール選択 ===\n");
        Console.WriteLine("ルール構成:");
        Console.WriteLine("  1. -land → -lish (優先度: 3.0)");
        Console.WriteLine("  2. 汎用 → +ian   (優先度: 1.0)");
        Console.WriteLine();

        var testCases = new[]
        {
            ("England", "landパターンに一致 → 高優先度ルール適用"),
            ("Scotland", "landパターンに一致 → 高優先度ルール適用"),
            ("Valdor", "landパターン不一致 → 汎用ルール適用"),
            ("Karthal", "landパターン不一致 → 汎用ルール適用")
        };

        foreach (var (baseName, explanation) in testCases)
        {
            var result = generator.Generate(new GenerationContext { BaseName = baseName });
            Console.WriteLine($"{baseName,-12} → {result.Name,-15} ({explanation})");
        }
    }

    /// <summary>
    /// エンドツーエンド：生成から派生までの完全なワークフロー
    /// </summary>
    [Test]
    [Explicit("手動実行専用：完全ワークフロー")]
    public void EndToEndWorkflow()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var ruleset = SampleRulesets.CreateFantasyRuleset();
        var engine = new DerivationEngine(ruleset);

        Console.WriteLine("=== エンドツーエンド：架空の王国とその住民 ===\n");

        for (int i = 0; i < 5; i++)
        {
            // 1. 王国名を生成
            var baseGenerator = new NameGenerator(database);
            var kingdom = baseGenerator.Generate(new GenerationContext
            {
                Type = NameType.Place,
                Seed = i * 100,
                Constraints = new StructuralConstraints { MinSyllables = 2, MaxSyllables = 3 }
            });

            // 2. 派生ジェネレータを作成
            var derivativeGenerator = new NameGenerator(database, engine);

            // 3. 形容詞形を派生
            var adjective = derivativeGenerator.Generate(new GenerationContext
            {
                Type = NameType.PlaceAdjective,
                BaseName = kingdom.Name
            });

            // 4. 住民名を派生
            var resident = derivativeGenerator.Generate(new GenerationContext
            {
                Type = NameType.PlaceResident,
                BaseName = kingdom.Name
            });

            // 5. 王名を生成（別の人物名シード）
            var king = baseGenerator.Generate(new GenerationContext
            {
                Type = NameType.Person,
                Seed = i * 100 + 50,
                Constraints = new StructuralConstraints { MinSyllables = 2, MaxSyllables = 2 }
            });

            Console.WriteLine($"【王国 #{i + 1}】");
            Console.WriteLine($"  王国名: {kingdom.Name}王国");
            Console.WriteLine($"  形容詞: {adjective.Name}の～");
            Console.WriteLine($"  住民名: {resident.Name}人");
            Console.WriteLine($"  統治者: {king.Name}王");
            Console.WriteLine($"  例文  : {king.Name}王は{kingdom.Name}王国の{resident.Name}人を率いて、");
            Console.WriteLine($"          {adjective.Name}の伝統を守り続けた。");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// 条件付きルールの動作確認
    /// </summary>
    [Test]
    [Explicit("手動実行専用：条件付きルール")]
    public void ObserveConditionalRules()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();

        // NameTypeによって異なる派生を行うルールセット
        var conditionalRuleset = new Ruleset
        {
            Id = "conditional",
            Name = "Conditional Rules",
            DerivationRules = new List<DerivationRule>
            {
                new DerivationRule
                {
                    Name = "形容詞専用: -or → -orian",
                    MatchPattern = "or$",
                    ReplacePattern = "orian",
                    Priority = 2.0f,
                    Condition = new RuleCondition
                    {
                        RequiredType = NameType.PlaceAdjective
                    }
                },
                new DerivationRule
                {
                    Name = "住民名専用: -or → -oran",
                    MatchPattern = "or$",
                    ReplacePattern = "oran",
                    Priority = 2.0f,
                    Condition = new RuleCondition
                    {
                        RequiredType = NameType.PlaceResident
                    }
                },
                new DerivationRule
                {
                    Name = "汎用フォールバック",
                    MatchPattern = ".*",
                    ReplacePattern = "$0ese",
                    Priority = 1.0f
                }
            }
        };

        var engine = new DerivationEngine(conditionalRuleset);
        var generator = new NameGenerator(database, engine);

        Console.WriteLine("=== 条件付きルール（NameType依存） ===\n");
        Console.WriteLine("ルール構成:");
        Console.WriteLine("  1. -or → -orian (形容詞専用, 優先度 2.0)");
        Console.WriteLine("  2. -or → -oran   (住民名専用, 優先度 2.0)");
        Console.WriteLine("  3. +ese          (汎用, 優先度 1.0)");
        Console.WriteLine();

        var testNames = new[] { "Valdor", "Karthor", "Thromor" };

        foreach (var baseName in testNames)
        {
            var adjective = generator.Generate(new GenerationContext
            {
                Type = NameType.PlaceAdjective,
                BaseName = baseName
            });

            var resident = generator.Generate(new GenerationContext
            {
                Type = NameType.PlaceResident,
                BaseName = baseName
            });

            var generic = generator.Generate(new GenerationContext
            {
                Type = NameType.Place,
                BaseName = baseName
            });

            Console.WriteLine($"{baseName}:");
            Console.WriteLine($"  形容詞形: {adjective.Name,-12} (条件: PlaceAdjective → -orian適用)");
            Console.WriteLine($"  住民名  : {resident.Name,-12} (条件: PlaceResident  → -oran適用)");
            Console.WriteLine($"  汎用    : {generic.Name,-12} (条件不一致 → +ese適用)");
            Console.WriteLine();
        }
    }
}
