using LoreGen.Core;
using LoreGen.Generation;
using LoreGen.Rules;
using NUnit.Framework;

namespace LoreGen.Tests.Integration;

/// <summary>
/// サンプルルールセットの統合テスト
/// </summary>
[TestFixture]
public class SampleRulesetsIntegrationTests
{
    [Test]
    public void FantasyRuleset_DerivesCorrectly()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var ruleset = SampleRulesets.CreateFantasyRuleset();
        var engine = new DerivationEngine(ruleset);
        var generator = new NameGenerator(database, engine);

        // -or → -orian
        var valdorResult = generator.Generate(new GenerationContext
        {
            Type = NameType.PlaceAdjective,
            BaseName = "Valdor"
        });
        Assert.That(valdorResult.Name, Is.EqualTo("Valdorian"));

        // -al → -alan
        var karthalResult = generator.Generate(new GenerationContext
        {
            Type = NameType.PlaceResident,
            BaseName = "Karthal"
        });
        Assert.That(karthalResult.Name, Is.EqualTo("Karthalan"));

        // Generic -ian
        var thorinResult = generator.Generate(new GenerationContext
        {
            Type = NameType.PlaceAdjective,
            BaseName = "Thorin"
        });
        Assert.That(thorinResult.Name, Is.EqualTo("Thorinian"));
    }

    [Test]
    public void EnglishStyleRuleset_DerivesCorrectly()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var ruleset = SampleRulesets.CreateEnglishStyleRuleset();
        var engine = new DerivationEngine(ruleset);
        var generator = new NameGenerator(database, engine);

        // -land → -lish
        var englandResult = generator.Generate(new GenerationContext
        {
            BaseName = "England"
        });
        Assert.That(englandResult.Name, Is.EqualTo("English"));

        // -a → -an
        var americaResult = generator.Generate(new GenerationContext
        {
            BaseName = "America"
        });
        Assert.That(americaResult.Name, Is.EqualTo("American"));
    }

    [Test]
    public void AncientRuleset_DerivesCorrectly()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var ruleset = SampleRulesets.CreateAncientRuleset();
        var engine = new DerivationEngine(ruleset);
        var generator = new NameGenerator(database, engine);

        // -us → -ian
        var romulus = generator.Generate(new GenerationContext
        {
            BaseName = "Romulus"
        });
        Assert.That(romulus.Name, Is.EqualTo("Romulian"));

        // -on → -onian
        var macedon = generator.Generate(new GenerationContext
        {
            BaseName = "Macedon"
        });
        Assert.That(macedon.Name, Is.EqualTo("Macedonian"));

        // -is → -ite
        var athens = generator.Generate(new GenerationContext
        {
            BaseName = "Athenis"
        });
        Assert.That(athens.Name, Is.EqualTo("Athenite"));
    }

    [Test]
    public void FantasyRuleset_AppliesImpressionShift()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var ruleset = SampleRulesets.CreateFantasyRuleset();
        var engine = new DerivationEngine(ruleset);
        var generator = new NameGenerator(database, engine);

        var result = generator.Generate(new GenerationContext
        {
            Type = NameType.PlaceAdjective,
            BaseName = "Valdor"
        });

        // DefaultImpression.Formality (0.5) + Shift (0.3) = 0.8
        Assert.That(result.ActualImpression.Formality, Is.EqualTo(0.8f).Within(0.01f));
        // DefaultImpression.Antiquity (0.6) + Shift (0.1) = 0.7
        Assert.That(result.ActualImpression.Antiquity, Is.EqualTo(0.7f).Within(0.01f));
    }

    [Test]
    public void EnglishStyleRuleset_HasHighPriorityRules()
    {
        var ruleset = SampleRulesets.CreateEnglishStyleRuleset();

        // -land should have priority 3.0
        var landRule = ruleset.DerivationRules.Find(r => r.Name == "Land to Lish");
        Assert.That(landRule, Is.Not.Null);
        Assert.That(landRule!.Priority, Is.EqualTo(3.0f));
    }

    [Test]
    public void AllRulesets_HaveValidMetadata()
    {
        var fantasy = SampleRulesets.CreateFantasyRuleset();
        var english = SampleRulesets.CreateEnglishStyleRuleset();
        var ancient = SampleRulesets.CreateAncientRuleset();

        Assert.That(fantasy.Id, Is.EqualTo("fantasy"));
        Assert.That(fantasy.Name, Is.Not.Empty);
        Assert.That(fantasy.DerivationRules.Count, Is.GreaterThan(0));

        Assert.That(english.Id, Is.EqualTo("english"));
        Assert.That(english.Name, Is.Not.Empty);
        Assert.That(english.DerivationRules.Count, Is.GreaterThan(0));

        Assert.That(ancient.Id, Is.EqualTo("ancient"));
        Assert.That(ancient.Name, Is.Not.Empty);
        Assert.That(ancient.DerivationRules.Count, Is.GreaterThan(0));
    }
}
