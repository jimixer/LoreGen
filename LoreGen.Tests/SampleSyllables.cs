using LoreGen.Core;
using LoreGen.Data;

namespace LoreGen.Tests;

/// <summary>
/// テスト用のサンプル音節データを提供するヘルパークラス
/// </summary>
public static class SampleSyllables
{
    /// <summary>基本的なファンタジー風音節データベースを作成</summary>
    public static SyllableDatabase CreateBasicFantasyDatabase()
    {
        var db = new SyllableDatabase();

        // === 語頭専用音節（硬い音） ===
        db.AddSyllable(new Syllable
        {
            Id = "kar",
            Pattern = "kar",
            Structure = new SyllableStructure { Onset = "k", Nucleus = "a", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.7f, Sharpness = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "kro",
            Pattern = "kro",
            Structure = new SyllableStructure { Onset = "kr", Nucleus = "o", Coda = "" },
            Impression = new ImpressionVector { Hardness = 0.8f, Sharpness = 0.6f, Complexity = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = false },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "gal",
            Pattern = "gal",
            Structure = new SyllableStructure { Onset = "g", Nucleus = "a", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.7f, Formality = 0.4f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "brak",
            Pattern = "brak",
            Structure = new SyllableStructure { Onset = "br", Nucleus = "a", Coda = "k" },
            Impression = new ImpressionVector { Hardness = 0.9f, Sharpness = 0.7f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = false },
            Weight = 0.8f
        });

        db.AddSyllable(new Syllable
        {
            Id = "tho",
            Pattern = "tho",
            Structure = new SyllableStructure { Onset = "th", Nucleus = "o", Coda = "" },
            Impression = new ImpressionVector { Hardness = 0.6f, Antiquity = 0.7f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = false },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "drak",
            Pattern = "drak",
            Structure = new SyllableStructure { Onset = "dr", Nucleus = "a", Coda = "k" },
            Impression = new ImpressionVector { Hardness = 0.85f, Sharpness = 0.7f, Mysticism = 0.3f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = false },
            Weight = 0.9f
        });

        // === 語頭専用音節（柔らかい音） ===
        db.AddSyllable(new Syllable
        {
            Id = "val",
            Pattern = "val",
            Structure = new SyllableStructure { Onset = "v", Nucleus = "a", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.4f, Formality = 0.6f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "mir",
            Pattern = "mir",
            Structure = new SyllableStructure { Onset = "m", Nucleus = "i", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.3f, Mysticism = 0.7f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "syl",
            Pattern = "syl",
            Structure = new SyllableStructure { Onset = "s", Nucleus = "y", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.35f, Mysticism = 0.6f, Sharpness = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "fel",
            Pattern = "fel",
            Structure = new SyllableStructure { Onset = "f", Nucleus = "e", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.3f, Formality = 0.4f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "wen",
            Pattern = "wen",
            Structure = new SyllableStructure { Onset = "w", Nucleus = "e", Coda = "n" },
            Impression = new ImpressionVector { Hardness = 0.2f, Antiquity = 0.6f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "el",
            Pattern = "el",
            Structure = new SyllableStructure { Onset = "", Nucleus = "e", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.25f, Mysticism = 0.5f, Formality = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        // === 中間音節 ===
        db.AddSyllable(new Syllable
        {
            Id = "tar",
            Pattern = "tar",
            Structure = new SyllableStructure { Onset = "t", Nucleus = "a", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.65f, Sharpness = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "mor",
            Pattern = "mor",
            Structure = new SyllableStructure { Onset = "m", Nucleus = "o", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.5f, Antiquity = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "ven",
            Pattern = "ven",
            Structure = new SyllableStructure { Onset = "v", Nucleus = "e", Coda = "n" },
            Impression = new ImpressionVector { Hardness = 0.4f, Formality = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "lin",
            Pattern = "lin",
            Structure = new SyllableStructure { Onset = "l", Nucleus = "i", Coda = "n" },
            Impression = new ImpressionVector { Hardness = 0.3f, Sharpness = 0.6f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "ra",
            Pattern = "ra",
            Structure = new SyllableStructure { Onset = "r", Nucleus = "a", Coda = "" },
            Impression = new ImpressionVector { Hardness = 0.5f, Sharpness = 0.4f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = false },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "no",
            Pattern = "no",
            Structure = new SyllableStructure { Onset = "n", Nucleus = "o", Coda = "" },
            Impression = new ImpressionVector { Hardness = 0.4f, Formality = 0.4f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = false },
            Weight = 1.0f
        });

        // === 語末専用音節（派生ルール対応: -or, -al, -us, -on, -is, -ia, -land） ===
        db.AddSyllable(new Syllable
        {
            Id = "dor",
            Pattern = "dor",
            Structure = new SyllableStructure { Onset = "d", Nucleus = "o", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.6f, Formality = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.2f
        });

        db.AddSyllable(new Syllable
        {
            Id = "thor",
            Pattern = "thor",
            Structure = new SyllableStructure { Onset = "th", Nucleus = "o", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.7f, Antiquity = 0.8f, Formality = 0.6f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.1f
        });

        db.AddSyllable(new Syllable
        {
            Id = "kal",
            Pattern = "kal",
            Structure = new SyllableStructure { Onset = "k", Nucleus = "a", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.65f, Sharpness = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.2f
        });

        db.AddSyllable(new Syllable
        {
            Id = "thal",
            Pattern = "thal",
            Structure = new SyllableStructure { Onset = "th", Nucleus = "a", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.7f, Antiquity = 0.75f, Formality = 0.65f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.1f
        });

        db.AddSyllable(new Syllable
        {
            Id = "rus",
            Pattern = "rus",
            Structure = new SyllableStructure { Onset = "r", Nucleus = "u", Coda = "s" },
            Impression = new ImpressionVector { Hardness = 0.6f, Antiquity = 0.8f, Formality = 0.7f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "lus",
            Pattern = "lus",
            Structure = new SyllableStructure { Onset = "l", Nucleus = "u", Coda = "s" },
            Impression = new ImpressionVector { Hardness = 0.4f, Antiquity = 0.75f, Formality = 0.65f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "don",
            Pattern = "don",
            Structure = new SyllableStructure { Onset = "d", Nucleus = "o", Coda = "n" },
            Impression = new ImpressionVector { Hardness = 0.6f, Antiquity = 0.6f, Formality = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "kon",
            Pattern = "kon",
            Structure = new SyllableStructure { Onset = "k", Nucleus = "o", Coda = "n" },
            Impression = new ImpressionVector { Hardness = 0.65f, Antiquity = 0.6f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "nis",
            Pattern = "nis",
            Structure = new SyllableStructure { Onset = "n", Nucleus = "i", Coda = "s" },
            Impression = new ImpressionVector { Hardness = 0.4f, Antiquity = 0.7f, Sharpness = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "ris",
            Pattern = "ris",
            Structure = new SyllableStructure { Onset = "r", Nucleus = "i", Coda = "s" },
            Impression = new ImpressionVector { Hardness = 0.5f, Antiquity = 0.7f, Sharpness = 0.55f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "thia",
            Pattern = "thia",
            Structure = new SyllableStructure { Onset = "th", Nucleus = "ia", Coda = "" },
            Impression = new ImpressionVector { Hardness = 0.3f, Antiquity = 0.8f, Formality = 0.7f, Mysticism = 0.4f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.1f
        });

        db.AddSyllable(new Syllable
        {
            Id = "ria",
            Pattern = "ria",
            Structure = new SyllableStructure { Onset = "r", Nucleus = "ia", Coda = "" },
            Impression = new ImpressionVector { Hardness = 0.35f, Formality = 0.6f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "land",
            Pattern = "land",
            Structure = new SyllableStructure { Onset = "l", Nucleus = "a", Coda = "nd" },
            Impression = new ImpressionVector { Hardness = 0.5f, Formality = 0.5f, Antiquity = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 0.9f
        });

        // === 語末音節（一般） ===
        db.AddSyllable(new Syllable
        {
            Id = "win",
            Pattern = "win",
            Structure = new SyllableStructure { Onset = "w", Nucleus = "i", Coda = "n" },
            Impression = new ImpressionVector { Hardness = 0.3f, Sharpness = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "din",
            Pattern = "din",
            Structure = new SyllableStructure { Onset = "d", Nucleus = "i", Coda = "n" },
            Impression = new ImpressionVector { Hardness = 0.55f, Sharpness = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "ran",
            Pattern = "ran",
            Structure = new SyllableStructure { Onset = "r", Nucleus = "a", Coda = "n" },
            Impression = new ImpressionVector { Hardness = 0.5f, Sharpness = 0.4f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "del",
            Pattern = "del",
            Structure = new SyllableStructure { Onset = "d", Nucleus = "e", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.4f, Formality = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "var",
            Pattern = "var",
            Structure = new SyllableStructure { Onset = "v", Nucleus = "a", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.45f, Formality = 0.5f },
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        return db;
    }

    /// <summary>最小限の音節データベースを作成（制約テスト用）</summary>
    public static SyllableDatabase CreateMinimalDatabase()
    {
        var db = new SyllableDatabase();

        db.AddSyllable(new Syllable
        {
            Id = "test1",
            Pattern = "te",
            Structure = new SyllableStructure { Onset = "t", Nucleus = "e", Coda = "" },
            Impression = new ImpressionVector(),
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = false },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "test2",
            Pattern = "st",
            Structure = new SyllableStructure { Onset = "s", Nucleus = "", Coda = "t" },
            Impression = new ImpressionVector(),
            Constraints = new PhoneticConstraints { CanBeInitial = false, CanBeFinal = true },
            Weight = 1.0f
        });

        return db;
    }
}
