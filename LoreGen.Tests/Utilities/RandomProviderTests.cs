using LoreGen.Utilities;

namespace LoreGen.Tests.Utilities;

[TestFixture]
public class RandomProviderTests
{
    [Test]
    public void Constructor_WithSeed_StoresSeed()
    {
        var provider = new RandomProvider(12345);

        Assert.That(provider.Seed, Is.EqualTo(12345));
    }

    [Test]
    public void SameSeed_ProducesSameSequence()
    {
        var provider1 = new RandomProvider(42);
        var provider2 = new RandomProvider(42);

        var value1 = provider1.Next(100);
        var value2 = provider2.Next(100);

        Assert.That(value1, Is.EqualTo(value2));
    }

    [Test]
    public void NextFloat_ReturnsValueInRange()
    {
        var provider = new RandomProvider(123);

        for (int i = 0; i < 100; i++)
        {
            var value = provider.NextFloat();
            Assert.That(value, Is.GreaterThanOrEqualTo(0.0f));
            Assert.That(value, Is.LessThan(1.0f));
        }
    }

    [Test]
    public void Choose_ReturnsItemFromArray()
    {
        var provider = new RandomProvider(456);
        var items = new[] { "a", "b", "c" };

        var chosen = provider.Choose(items);

        Assert.That(items, Does.Contain(chosen));
    }

    [Test]
    public void Choose_EmptyArray_ThrowsException()
    {
        var provider = new RandomProvider(789);
        var items = Array.Empty<string>();

        Assert.Throws<ArgumentException>(() => provider.Choose(items));
    }

    [Test]
    public void Chance_AlwaysTrue_ReturnsTrue()
    {
        var provider = new RandomProvider(111);

        var result = provider.Chance(1.0f);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Chance_AlwaysFalse_ReturnsFalse()
    {
        var provider = new RandomProvider(222);

        var result = provider.Chance(0.0f);

        Assert.That(result, Is.False);
    }
}
