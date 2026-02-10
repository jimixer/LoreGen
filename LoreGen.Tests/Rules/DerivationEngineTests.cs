using LoreGen.Core;
using LoreGen.Rules;
using NUnit.Framework;

namespace LoreGen.Tests.Rules;

[TestFixture]
public class DerivationEngineTests
{
    private Ruleset CreateTestRuleset()
    {
        var ruleset = new Ruleset
        {
            Id = "test",
            Name = "Test Ruleset",
            DefaultImpression = new ImpressionVector { Formality = 0.5f }
        };

        ruleset.AddRule(new DerivationRule
        {
            Name = "Place to Adjective",
            MatchPattern = @"^(.+)or$",
            ReplacePattern = "$1orian",
            Priority = 1.0f,
            ImpressionShift = new ImpressionVector { Formality = 0.3f }
        });

        ruleset.AddRule(new DerivationRule
        {
            Name = "Land to Lish",
            MatchPattern = @"^(.+)land$",
            ReplacePattern = "$1lish",
            Priority = 1.0f,
            ImpressionShift = new ImpressionVector { Antiquity = 0.2f }
        });

        return ruleset;
    }

    [Test]
    public void Derive_AppliesMatchingRule()
    {
        var ruleset = CreateTestRuleset();
        var engine = new DerivationEngine(ruleset);

        var result = engine.Derive("Valdor", NameType.PlaceAdjective);

        Assert.That(result.Name, Is.EqualTo("Valdorian"));
    }

    [Test]
    public void Derive_AppliesCorrectRule_BasedOnPattern()
    {
        var ruleset = CreateTestRuleset();
        var engine = new DerivationEngine(ruleset);

        var result1 = engine.Derive("Valdor", NameType.PlaceAdjective);
        var result2 = engine.Derive("England", NameType.PlaceAdjective);

        Assert.That(result1.Name, Is.EqualTo("Valdorian"));
        Assert.That(result2.Name, Is.EqualTo("English"));
    }

    [Test]
    public void Derive_NoMatchingRule_ReturnsOriginalName()
    {
        var ruleset = CreateTestRuleset();
        var engine = new DerivationEngine(ruleset);

        var result = engine.Derive("Karthal", NameType.PlaceAdjective);

        Assert.That(result.Name, Is.EqualTo("Karthal"));
    }

    [Test]
    public void Derive_AppliesImpressionShift()
    {
        var ruleset = CreateTestRuleset();
        var engine = new DerivationEngine(ruleset);

        var result = engine.Derive("Valdor", NameType.PlaceAdjective);

        // DefaultImpression.Formality (0.5) + ImpressionShift.Formality (0.3) = 0.8
        Assert.That(result.ActualImpression.Formality, Is.EqualTo(0.8f));
    }

    [Test]
    public void Derive_ClampsImpressionValues()
    {
        var ruleset = new Ruleset
        {
            DefaultImpression = new ImpressionVector { Formality = 0.9f }
        };
        ruleset.AddRule(new DerivationRule
        {
            Name = "Test Rule",
            MatchPattern = @"^(.+)$",
            ReplacePattern = "$1ian",
            ImpressionShift = new ImpressionVector { Formality = 0.5f } // 0.9 + 0.5 = 1.4, clamped to 1.0
        });

        var engine = new DerivationEngine(ruleset);
        var result = engine.Derive("Test", NameType.PlaceAdjective);

        Assert.That(result.ActualImpression.Formality, Is.EqualTo(1.0f));
    }

    [Test]
    public void Derive_PopulatesMetadata()
    {
        var ruleset = CreateTestRuleset();
        var engine = new DerivationEngine(ruleset);

        var result = engine.Derive("Valdor", NameType.PlaceAdjective);

        Assert.That(result.Metadata.AppliedRules.Length, Is.EqualTo(1));
        Assert.That(result.Metadata.AppliedRules[0], Is.EqualTo("Place to Adjective"));
        Assert.That(result.Metadata.RulesetId, Is.EqualTo("test"));
    }

    [Test]
    public void Derive_NoMatchingRule_EmptyAppliedRules()
    {
        var ruleset = CreateTestRuleset();
        var engine = new DerivationEngine(ruleset);

        var result = engine.Derive("Karthal", NameType.PlaceAdjective);

        Assert.That(result.Metadata.AppliedRules.Length, Is.EqualTo(0));
    }

    [Test]
    public void Derive_WithContext_PassesContextToRules()
    {
        var ruleset = new Ruleset();
        ruleset.AddRule(new DerivationRule
        {
            Name = "Place Only",
            MatchPattern = @"^(.+)$",
            ReplacePattern = "$1ian",
            Condition = new RuleCondition { RequiredType = NameType.Place }
        });

        var engine = new DerivationEngine(ruleset);

        var placeContext = new GenerationContext { Type = NameType.Place };
        var personContext = new GenerationContext { Type = NameType.Person };

        var placeResult = engine.Derive("Test", placeContext);
        var personResult = engine.Derive("Test", personContext);

        Assert.That(placeResult.Name, Is.EqualTo("Testian"));
        Assert.That(personResult.Name, Is.EqualTo("Test")); // No rule applied
    }

    [Test]
    public void Derive_EmptyBaseName_ThrowsException()
    {
        var ruleset = CreateTestRuleset();
        var engine = new DerivationEngine(ruleset);

        Assert.Throws<ArgumentException>(() => engine.Derive("", NameType.Place));
    }

    [Test]
    public void Derive_NullBaseName_ThrowsException()
    {
        var ruleset = CreateTestRuleset();
        var engine = new DerivationEngine(ruleset);

        Assert.Throws<ArgumentException>(() => engine.Derive(null!, NameType.Place));
    }

    [Test]
    public void Constructor_NullRuleset_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new DerivationEngine(null!));
    }
}
