using LoreGen.Data;
using NUnit.Framework;

namespace LoreGen.Tests.Data;

[TestFixture]
public class SyllableStructureTests
{
    [Test]
    public void GetStructureType_CVC_ReturnsCorrectType()
    {
        var structure = new SyllableStructure
        {
            Onset = "k",
            Nucleus = "a",
            Coda = "r"
        };

        Assert.That(structure.GetStructureType(), Is.EqualTo("CVC"));
    }

    [Test]
    public void GetStructureType_CV_ReturnsCorrectType()
    {
        var structure = new SyllableStructure
        {
            Onset = "th",
            Nucleus = "o",
            Coda = string.Empty
        };

        Assert.That(structure.GetStructureType(), Is.EqualTo("CV"));
    }

    [Test]
    public void GetStructureType_VC_ReturnsCorrectType()
    {
        var structure = new SyllableStructure
        {
            Onset = string.Empty,
            Nucleus = "a",
            Coda = "n"
        };

        Assert.That(structure.GetStructureType(), Is.EqualTo("VC"));
    }

    [Test]
    public void GetStructureType_V_ReturnsCorrectType()
    {
        var structure = new SyllableStructure
        {
            Onset = string.Empty,
            Nucleus = "e",
            Coda = string.Empty
        };

        Assert.That(structure.GetStructureType(), Is.EqualTo("V"));
    }

    [Test]
    public void GetPattern_CombinesAllComponents()
    {
        var structure = new SyllableStructure
        {
            Onset = "str",
            Nucleus = "ei",
            Coda = "nt"
        };

        Assert.That(structure.GetPattern(), Is.EqualTo("streint"));
    }
}
