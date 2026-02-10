using LoreGen.Core;
using LoreGen.Rules;
using NUnit.Framework;

namespace LoreGen.Tests.Rules;

[TestFixture]
public class DerivationRuleTests
{
    [Test]
    public void CanApply_MatchingPattern_ReturnsTrue()
    {
        var rule = new DerivationRule
        {
            MatchPattern = @"^(.+)or$"
        };
        var context = new GenerationContext { Type = NameType.Place };

        Assert.That(rule.CanApply("Valdor", context), Is.True);
    }

    [Test]
    public void CanApply_NonMatchingPattern_ReturnsFalse()
    {
        var rule = new DerivationRule
        {
            MatchPattern = @"^(.+)or$"
        };
        var context = new GenerationContext { Type = NameType.Place };

        Assert.That(rule.CanApply("Karthal", context), Is.False);
    }

    [Test]
    public void CanApply_WithCondition_EvaluatesCondition()
    {
        var rule = new DerivationRule
        {
            MatchPattern = @"^(.+)or$",
            Condition = new RuleCondition { RequiredType = NameType.Place }
        };

        var placeContext = new GenerationContext { Type = NameType.Place };
        var personContext = new GenerationContext { Type = NameType.Person };

        Assert.That(rule.CanApply("Valdor", placeContext), Is.True);
        Assert.That(rule.CanApply("Valdor", personContext), Is.False);
    }

    [Test]
    public void Apply_Substitutes_CorrectPattern()
    {
        var rule = new DerivationRule
        {
            MatchPattern = @"^(.+)or$",
            ReplacePattern = "$1orian"
        };

        var result = rule.Apply("Valdor");

        Assert.That(result, Is.EqualTo("Valdorian"));
    }

    [Test]
    public void Apply_ComplexPattern_SubstitutesCorrectly()
    {
        var rule = new DerivationRule
        {
            MatchPattern = @"^(.+)land$",
            ReplacePattern = "$1lish"
        };

        var result = rule.Apply("England");

        Assert.That(result, Is.EqualTo("English"));
    }

    [Test]
    public void Apply_CaseInsensitive_WorksCorrectly()
    {
        var rule = new DerivationRule
        {
            MatchPattern = @"^(.+)OR$",
            ReplacePattern = "$1orian"
        };

        var result = rule.Apply("valdor");

        Assert.That(result, Is.EqualTo("valdorian"));
    }

    [Test]
    public void ImpressionShift_IsStored()
    {
        var rule = new DerivationRule
        {
            ImpressionShift = new ImpressionVector { Formality = 0.3f }
        };

        Assert.That(rule.ImpressionShift.Formality, Is.EqualTo(0.3f));
    }

    [Test]
    public void Priority_DefaultsToZero()
    {
        var rule = new DerivationRule();

        Assert.That(rule.Priority, Is.EqualTo(0.0f));
    }

    [Test]
    public void Priority_CanBeSet()
    {
        var rule = new DerivationRule { Priority = 5.0f };

        Assert.That(rule.Priority, Is.EqualTo(5.0f));
    }
}
