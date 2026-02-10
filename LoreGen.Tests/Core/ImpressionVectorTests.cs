using LoreGen.Core;

namespace LoreGen.Tests.Core;

[TestFixture]
public class ImpressionVectorTests
{
    [Test]
    public void Zero_ReturnsAllZeroValues()
    {
        var zero = ImpressionVector.Zero;

        Assert.That(zero.Hardness, Is.EqualTo(0.0f));
        Assert.That(zero.Sharpness, Is.EqualTo(0.0f));
        Assert.That(zero.Complexity, Is.EqualTo(0.0f));
        Assert.That(zero.Antiquity, Is.EqualTo(0.0f));
    }

    [Test]
    public void Distance_SameVectors_ReturnsZero()
    {
        var vector = new ImpressionVector
        {
            Hardness = 0.5f,
            Sharpness = 0.7f
        };

        var distance = vector.Distance(vector);

        Assert.That(distance, Is.EqualTo(0.0f).Within(0.001f));
    }

    [Test]
    public void Distance_DifferentVectors_ReturnsPositive()
    {
        var vector1 = new ImpressionVector { Hardness = 0.0f };
        var vector2 = new ImpressionVector { Hardness = 1.0f };

        var distance = vector1.Distance(vector2);

        Assert.That(distance, Is.GreaterThan(0.0f));
    }

    [Test]
    public void Blend_HalfWeight_ReturnsMiddle()
    {
        var vector1 = new ImpressionVector { Hardness = 0.0f };
        var vector2 = new ImpressionVector { Hardness = 1.0f };

        var blended = ImpressionVector.Blend(vector1, vector2, 0.5f);

        Assert.That(blended.Hardness, Is.EqualTo(0.5f).Within(0.001f));
    }

    [Test]
    public void Blend_ZeroWeight_ReturnsFirstVector()
    {
        var vector1 = new ImpressionVector { Hardness = 0.3f };
        var vector2 = new ImpressionVector { Hardness = 0.9f };

        var blended = ImpressionVector.Blend(vector1, vector2, 0.0f);

        Assert.That(blended.Hardness, Is.EqualTo(0.3f).Within(0.001f));
    }

    [Test]
    public void Normalize_ClampsValuesToRange()
    {
        var vector = new ImpressionVector
        {
            Hardness = 1.5f,
            Sharpness = -0.5f,
            Complexity = 0.5f
        };

        var normalized = vector.Normalize();

        Assert.That(normalized.Hardness, Is.EqualTo(1.0f));
        Assert.That(normalized.Sharpness, Is.EqualTo(0.0f));
        Assert.That(normalized.Complexity, Is.EqualTo(0.5f));
    }
}
