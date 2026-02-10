using LoreGen.Core;
using LoreGen.Data;
using NUnit.Framework;

namespace LoreGen.Tests.Data;

[TestFixture]
public class SyllableDatabaseTests
{
    private SyllableDatabase _database = null!;

    [SetUp]
    public void SetUp()
    {
        _database = new SyllableDatabase();
    }

    [Test]
    public void AddSyllable_ValidSyllable_AddsSuccessfully()
    {
        var syllable = CreateTestSyllable("kar");

        _database.AddSyllable(syllable);

        Assert.That(_database.GetById("kar"), Is.Not.Null);
    }

    [Test]
    public void AddSyllable_EmptyId_ThrowsException()
    {
        var syllable = CreateTestSyllable("");

        Assert.Throws<ArgumentException>(() => _database.AddSyllable(syllable));
    }

    [Test]
    public void GetById_ExistingSyllable_ReturnsSyllable()
    {
        var syllable = CreateTestSyllable("tho");
        _database.AddSyllable(syllable);

        var result = _database.GetById("tho");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo("tho"));
    }

    [Test]
    public void GetById_NonExistingSyllable_ReturnsNull()
    {
        var result = _database.GetById("xyz");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetAll_ReturnsAllAddedSyllables()
    {
        _database.AddSyllable(CreateTestSyllable("kar"));
        _database.AddSyllable(CreateTestSyllable("tho"));

        var all = _database.GetAll();

        Assert.That(all.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetInitialSyllables_FiltersCorrectly()
    {
        var initial = CreateTestSyllable("kar");
        initial.Constraints.CanBeInitial = true;
        initial.Constraints.CanBeFinal = false;

        var notInitial = CreateTestSyllable("ing");
        notInitial.Constraints.CanBeInitial = false;
        notInitial.Constraints.CanBeFinal = true;

        _database.AddSyllable(initial);
        _database.AddSyllable(notInitial);

        var result = _database.GetInitialSyllables().ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo("kar"));
    }

    [Test]
    public void GetFinalSyllables_FiltersCorrectly()
    {
        var final = CreateTestSyllable("dor");
        final.Constraints.CanBeInitial = false;
        final.Constraints.CanBeFinal = true;

        var notFinal = CreateTestSyllable("str");
        notFinal.Constraints.CanBeInitial = true;
        notFinal.Constraints.CanBeFinal = false;

        _database.AddSyllable(final);
        _database.AddSyllable(notFinal);

        var result = _database.GetFinalSyllables().ToList();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo("dor"));
    }

    [Test]
    public void GetFollowingSyllables_WithCanFollowList_ReturnsOnlyAllowed()
    {
        var first = CreateTestSyllable("kar");
        first.Constraints.CanFollowSyllables = new[] { "tho", "lin" };

        var allowed1 = CreateTestSyllable("tho");
        var allowed2 = CreateTestSyllable("lin");
        var notAllowed = CreateTestSyllable("xyz");

        _database.AddSyllable(first);
        _database.AddSyllable(allowed1);
        _database.AddSyllable(allowed2);
        _database.AddSyllable(notAllowed);

        var result = _database.GetFollowingSyllables(first).ToList();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(s => s.Id == "tho"), Is.True);
        Assert.That(result.Any(s => s.Id == "lin"), Is.True);
    }

    [Test]
    public void GetFollowingSyllables_WithCannotFollowList_ExcludesForbidden()
    {
        var first = CreateTestSyllable("kar");
        first.Constraints.CannotFollowSyllables = new[] { "xyz" };

        var allowed = CreateTestSyllable("tho");
        var forbidden = CreateTestSyllable("xyz");

        _database.AddSyllable(first);
        _database.AddSyllable(allowed);
        _database.AddSyllable(forbidden);

        var result = _database.GetFollowingSyllables(first).ToList();

        Assert.That(result.Any(s => s.Id == "tho"), Is.True);
        Assert.That(result.Any(s => s.Id == "xyz"), Is.False);
    }

    [Test]
    public void Clear_RemovesAllSyllables()
    {
        _database.AddSyllable(CreateTestSyllable("kar"));
        _database.AddSyllable(CreateTestSyllable("tho"));

        _database.Clear();

        Assert.That(_database.GetAll().Count, Is.EqualTo(0));
    }

    private Syllable CreateTestSyllable(string id)
    {
        return new Syllable
        {
            Id = id,
            Pattern = id,
            Structure = new SyllableStructure { Onset = "k", Nucleus = "a", Coda = "r" },
            Impression = new ImpressionVector(),
            Constraints = new PhoneticConstraints(),
            Weight = 1.0f
        };
    }
}
