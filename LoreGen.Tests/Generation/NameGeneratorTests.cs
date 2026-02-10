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
}
