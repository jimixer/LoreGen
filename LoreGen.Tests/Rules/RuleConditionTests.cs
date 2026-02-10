using LoreGen.Core;
using LoreGen.Rules;
using NUnit.Framework;

namespace LoreGen.Tests.Rules;

[TestFixture]
public class RuleConditionTests
{
    [Test]
    public void IsSatisfied_NoConstraints_ReturnsTrue()
    {
        var condition = new RuleCondition();
        var context = new GenerationContext { Type = NameType.Place };

        Assert.That(condition.IsSatisfied("Valdor", context), Is.True);
    }

    [Test]
    public void IsSatisfied_RequiredType_MatchesType()
    {
        var condition = new RuleCondition { RequiredType = NameType.Place };

        var placeContext = new GenerationContext { Type = NameType.Place };
        var personContext = new GenerationContext { Type = NameType.Person };

        Assert.That(condition.IsSatisfied("Valdor", placeContext), Is.True);
        Assert.That(condition.IsSatisfied("Valdor", personContext), Is.False);
    }

    [Test]
    public void IsSatisfied_CustomPredicate_EvaluatesPredicate()
    {
        var condition = new RuleCondition
        {
            CustomPredicate = (name, ctx) => name.Length > 5
        };

        var context = new GenerationContext();

        Assert.That(condition.IsSatisfied("Valdor", context), Is.True);
        Assert.That(condition.IsSatisfied("Val", context), Is.False);
    }

    [Test]
    public void IsSatisfied_BothConstraints_RequiresBothTrue()
    {
        var condition = new RuleCondition
        {
            RequiredType = NameType.Place,
            CustomPredicate = (name, ctx) => name.EndsWith("or")
        };

        var placeContext = new GenerationContext { Type = NameType.Place };
        var personContext = new GenerationContext { Type = NameType.Person };

        Assert.That(condition.IsSatisfied("Valdor", placeContext), Is.True);
        Assert.That(condition.IsSatisfied("Karthal", placeContext), Is.False);
        Assert.That(condition.IsSatisfied("Valdor", personContext), Is.False);
    }

    [Test]
    public void IsSatisfied_NullCustomPredicate_IgnoresPredicate()
    {
        var condition = new RuleCondition
        {
            RequiredType = NameType.Place,
            CustomPredicate = null
        };

        var context = new GenerationContext { Type = NameType.Place };

        Assert.That(condition.IsSatisfied("AnyName", context), Is.True);
    }
}
