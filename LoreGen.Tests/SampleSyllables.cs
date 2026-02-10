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

        // 語頭音節
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
            Id = "tho",
            Pattern = "tho",
            Structure = new SyllableStructure { Onset = "th", Nucleus = "o", Coda = "" },
            Impression = new ImpressionVector { Hardness = 0.6f, Antiquity = 0.7f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = false },
            Weight = 1.0f
        });

        db.AddSyllable(new Syllable
        {
            Id = "val",
            Pattern = "val",
            Structure = new SyllableStructure { Onset = "v", Nucleus = "a", Coda = "l" },
            Impression = new ImpressionVector { Hardness = 0.4f, Formality = 0.6f },
            Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
            Weight = 1.0f
        });

        // 中間・語末音節
        db.AddSyllable(new Syllable
        {
            Id = "dor",
            Pattern = "dor",
            Structure = new SyllableStructure { Onset = "d", Nucleus = "o", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.6f, Formality = 0.5f },
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
            Id = "mir",
            Pattern = "mir",
            Structure = new SyllableStructure { Onset = "m", Nucleus = "i", Coda = "r" },
            Impression = new ImpressionVector { Hardness = 0.3f, Mysticism = 0.5f },
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
