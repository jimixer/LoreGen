using LoreGen.Core;
using LoreGen.Rules;
using NUnit.Framework;

namespace LoreGen.Tests.Rules;

[TestFixture]
public class RulesetTests
{
    [Test]
    public void AddRule_AddsRuleToCollection()
    {
        var ruleset = new Ruleset();
        var rule = new DerivationRule { Name = "Test Rule" };

        ruleset.AddRule(rule);

        Assert.That(ruleset.DerivationRules.Count, Is.EqualTo(1));
        Assert.That(ruleset.DerivationRules[0].Name, Is.EqualTo("Test Rule"));
    }

    [Test]
    public void GetApplicableRules_ReturnsOnlyMatchingRules()
    {
        var ruleset = new Ruleset();
        ruleset.AddRule(new DerivationRule
        {
            Name = "Rule1",
            MatchPattern = @"^(.+)or$"
        });
        ruleset.AddRule(new DerivationRule
        {
            Name = "Rule2",
            MatchPattern = @"^(.+)land$"
        });

        var context = new GenerationContext();
        var applicable = ruleset.GetApplicableRules("Valdor", context).ToList();

        Assert.That(applicable.Count, Is.EqualTo(1));
        Assert.That(applicable[0].Name, Is.EqualTo("Rule1"));
    }

    [Test]
    public void GetApplicableRules_OrdersByPriorityDescending()
    {
        var ruleset = new Ruleset();
        ruleset.AddRule(new DerivationRule
        {
            Name = "LowPriority",
            MatchPattern = @"^(.+)$",
            Priority = 1.0f
        });
        ruleset.AddRule(new DerivationRule
        {
            Name = "HighPriority",
            MatchPattern = @"^(.+)$",
            Priority = 10.0f
        });
        ruleset.AddRule(new DerivationRule
        {
            Name = "MediumPriority",
            MatchPattern = @"^(.+)$",
            Priority = 5.0f
        });

        var context = new GenerationContext();
        var applicable = ruleset.GetApplicableRules("Test", context).ToList();

        Assert.That(applicable.Count, Is.EqualTo(3));
        Assert.That(applicable[0].Name, Is.EqualTo("HighPriority"));
        Assert.That(applicable[1].Name, Is.EqualTo("MediumPriority"));
        Assert.That(applicable[2].Name, Is.EqualTo("LowPriority"));
    }

    [Test]
    public void GetBestRule_ReturnsHighestPriorityRule()
    {
        var ruleset = new Ruleset();
        ruleset.AddRule(new DerivationRule
        {
            Name = "Rule1",
            MatchPattern = @"^(.+)$",
            Priority = 1.0f
        });
        ruleset.AddRule(new DerivationRule
        {
            Name = "Rule2",
            MatchPattern = @"^(.+)$",
            Priority = 5.0f
        });

        var context = new GenerationContext();
        var best = ruleset.GetBestRule("Test", context);

        Assert.That(best, Is.Not.Null);
        Assert.That(best!.Name, Is.EqualTo("Rule2"));
    }

    [Test]
    public void GetBestRule_NoApplicableRules_ReturnsNull()
    {
        var ruleset = new Ruleset();
        ruleset.AddRule(new DerivationRule
        {
            Name = "Rule1",
            MatchPattern = @"^(.+)or$"
        });

        var context = new GenerationContext();
        var best = ruleset.GetBestRule("Karthal", context);

        Assert.That(best, Is.Null);
    }

    [Test]
    public void GetApplicableRules_RespectsConditions()
    {
        var ruleset = new Ruleset();
        ruleset.AddRule(new DerivationRule
        {
            Name = "PlaceOnly",
            MatchPattern = @"^(.+)$",
            Condition = new RuleCondition { RequiredType = NameType.Place }
        });

        var placeContext = new GenerationContext { Type = NameType.Place };
        var personContext = new GenerationContext { Type = NameType.Person };

        var placeRules = ruleset.GetApplicableRules("Test", placeContext).ToList();
        var personRules = ruleset.GetApplicableRules("Test", personContext).ToList();

        Assert.That(placeRules.Count, Is.EqualTo(1));
        Assert.That(personRules.Count, Is.EqualTo(0));
    }
}
