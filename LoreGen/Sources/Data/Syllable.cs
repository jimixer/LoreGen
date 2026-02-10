using LoreGen.Core;

namespace LoreGen.Data;

/// <summary>
/// 一つの音節を表すクラス
/// </summary>
public class Syllable
{
    /// <summary>音節の一意識別子</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>音節パターン (例: "kar", "tho", "lin")</summary>
    public string Pattern { get; set; } = string.Empty;

    /// <summary>音節の音韻構造 (CVC, CV, VC など)</summary>
    public SyllableStructure Structure { get; set; } = new();

    /// <summary>音節の印象ベクトル</summary>
    public ImpressionVector Impression { get; set; } = new();

    /// <summary>音韻制約 (配置・連続の制約)</summary>
    public PhoneticConstraints Constraints { get; set; } = new();

    /// <summary>生成時の選択重み (デフォルト: 1.0)</summary>
    public float Weight { get; set; } = 1.0f;
}
