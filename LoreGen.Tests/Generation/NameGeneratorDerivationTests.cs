using LoreGen.Core;
using LoreGen.Generation;
using LoreGen.Rules;
using NUnit.Framework;

namespace LoreGen.Tests.Generation;

[TestFixture]
public class NameGeneratorDerivationTests
{
    private Ruleset CreateTestRuleset()
    {
        var ruleset = new Ruleset
        {
            Id = "fantasy",
            Name = "Fantasy Derivation",
            DefaultImpression = new ImpressionVector { Formality = 0.5f }
        };

        ruleset.AddRule(new DerivationRule
        {
            Name = "Place to Adjective (-orian)",
            MatchPattern = @"^(.+)or$",
            ReplacePattern = "$1orian",
            Priority = 1.0f,
            ImpressionShift = new ImpressionVector { Formality = 0.2f }
        });

        ruleset.AddRule(new DerivationRule
        {
            Name = "Place to Resident (-ian)",
            MatchPattern = @"^(.+)$",
            ReplacePattern = "$1ian",
            Priority = 0.5f,
            ImpressionShift = new ImpressionVector { Formality = 0.1f }
        });

        return ruleset;
    }

    [Test]
    public void Generate_WithBaseName_AppliesDerivation()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var engine = new DerivationEngine(CreateTestRuleset());
        var generator = new NameGenerator(database, engine);

        var context = new GenerationContext
        {
            Type = NameType.PlaceAdjective,
            BaseName = "Valdor"
        };

        var result = generator.Generate(context);

        Assert.That(result.Name, Is.EqualTo("Valdorian"));
    }

    [Test]
    public void Generate_WithBaseName_AppliesCorrectRule()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var engine = new DerivationEngine(CreateTestRuleset());
        var generator = new NameGenerator(database, engine);

        var context = new GenerationContext
        {
            Type = NameType.PlaceResident,
            BaseName = "Karthal"  // Doesn't match -or pattern, uses generic -ian
        };

        var result = generator.Generate(context);

        Assert.That(result.Name, Is.EqualTo("Karthalian"));
    }

    [Test]
    public void Generate_WithBaseName_AppliesImpressionShift()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var engine = new DerivationEngine(CreateTestRuleset());
        var generator = new NameGenerator(database, engine);

        var context = new GenerationContext
        {
            Type = NameType.PlaceAdjective,
            BaseName = "Valdor"
        };

        var result = generator.Generate(context);

        // DefaultImpression (0.5) + ImpressionShift (0.2) = 0.7
        Assert.That(result.ActualImpression.Formality, Is.EqualTo(0.7f));
    }

    [Test]
    public void Generate_WithoutDerivationEngine_GeneratesFromSyllables()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var generator = new NameGenerator(database); // No engine

        var context = new GenerationContext
        {
            Type = NameType.Place,
            BaseName = "Valdor", // Should be ignored
            Seed = 42
        };

        var result = generator.Generate(context);

        // Should generate from syllables, not derive from Valdor
        Assert.That(result.Name, Is.Not.EqualTo("Valdor"));
        Assert.That(result.Name, Is.Not.EqualTo("Valdorian"));
    }

    [Test]
    public void Generate_NoBaseName_GeneratesFromSyllables()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var engine = new DerivationEngine(CreateTestRuleset());
        var generator = new NameGenerator(database, engine);

        var context = new GenerationContext
        {
            Type = NameType.Place,
            Seed = 42
        };

        var result = generator.Generate(context);

        // Should use syllable generation
        Assert.That(result.Name, Is.Not.Empty);
        Assert.That(result.Metadata.UsedSyllables.Length, Is.GreaterThan(0));
    }

    [Test]
    public void Generate_WithBaseName_PopulatesMetadata()
    {
        var database = SampleSyllables.CreateBasicFantasyDatabase();
        var engine = new DerivationEngine(CreateTestRuleset());
        var generator = new NameGenerator(database, engine);

        var context = new GenerationContext
        {
            Type = NameType.PlaceAdjective,
            BaseName = "Valdor"
        };

        var result = generator.Generate(context);

        Assert.That(result.Metadata.AppliedRules.Length, Is.EqualTo(1));
        Assert.That(result.Metadata.AppliedRules[0], Is.EqualTo("Place to Adjective (-orian)"));
        Assert.That(result.Metadata.RulesetId, Is.EqualTo("fantasy"));
    }
}
