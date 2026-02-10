namespace LoreGen.Core;

/// <summary>
/// 名前の構造に関する制約条件。
/// </summary>
public class StructuralConstraints
{
    /// <summary>最小音節数</summary>
    public int? MinSyllables { get; set; }

    /// <summary>最大音節数</summary>
    public int? MaxSyllables { get; set; }

    /// <summary>推奨文字数</summary>
    public int? PreferredLength { get; set; }

    /// <summary>子音クラスタを許可するか</summary>
    public bool AllowConsonantClusters { get; set; } = true;

    /// <summary>母音調和を要求するか</summary>
    public bool RequireVowelHarmony { get; set; } = false;

    /// <summary>特定の文字で開始する必要がある場合</summary>
    public string? MustStartWith { get; set; }

    /// <summary>特定の文字で終了する必要がある場合</summary>
    public string? MustEndWith { get; set; }
}
