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

    // Phase 2.5: 発音品質改善テスト

    [Test]
    public void EndsWithVowel_NoCoda_ReturnsTrue()
    {
        var structure = new SyllableStructure
        {
            Onset = "k",
            Nucleus = "a",
            Coda = ""
        };

        Assert.That(structure.EndsWithVowel(), Is.True);
    }

    [Test]
    public void EndsWithVowel_WithCoda_ReturnsFalse()
    {
        var structure = new SyllableStructure
        {
            Onset = "k",
            Nucleus = "a",
            Coda = "r"
        };

        Assert.That(structure.EndsWithVowel(), Is.False);
    }

    [Test]
    public void EndsWithVowel_NoNucleus_ReturnsFalse()
    {
        var structure = new SyllableStructure
        {
            Onset = "s",
            Nucleus = "",
            Coda = "t"
        };

        Assert.That(structure.EndsWithVowel(), Is.False);
    }

    [Test]
    public void StartsWithVowel_NoOnset_ReturnsTrue()
    {
        var structure = new SyllableStructure
        {
            Onset = "",
            Nucleus = "a",
            Coda = "r"
        };

        Assert.That(structure.StartsWithVowel(), Is.True);
    }

    [Test]
    public void StartsWithVowel_WithOnset_ReturnsFalse()
    {
        var structure = new SyllableStructure
        {
            Onset = "k",
            Nucleus = "a",
            Coda = ""
        };

        Assert.That(structure.StartsWithVowel(), Is.False);
    }

    [Test]
    public void StartsWithVowel_NoNucleus_ReturnsFalse()
    {
        var structure = new SyllableStructure
        {
            Onset = "s",
            Nucleus = "",
            Coda = "t"
        };

        Assert.That(structure.StartsWithVowel(), Is.False);
    }

    [Test]
    public void EndsAndStartsWithVowel_CVAndVC_TrueAndTrue()
    {
        var cv = new SyllableStructure { Onset = "k", Nucleus = "a", Coda = "" };
        var vc = new SyllableStructure { Onset = "", Nucleus = "e", Coda = "l" };

        Assert.That(cv.EndsWithVowel(), Is.True, "CV should end with vowel");
        Assert.That(vc.StartsWithVowel(), Is.True, "VC should start with vowel");
    }
}
