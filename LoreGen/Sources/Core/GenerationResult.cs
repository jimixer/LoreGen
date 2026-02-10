using System;

namespace LoreGen.Core;

/// <summary>
/// 名前生成の結果を格納するクラス。
/// </summary>
public class GenerationResult
{
    /// <summary>生成された名前</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>実際の印象パラメータ</summary>
    public ImpressionVector ActualImpression { get; set; }

    /// <summary>生成時のメタデータ</summary>
    public GenerationMetadata Metadata { get; set; } = new();
}

/// <summary>
/// 名前生成時のメタデータ。
/// </summary>
public class GenerationMetadata
{
    /// <summary>適用されたルール名のリスト</summary>
    public string[] AppliedRules { get; set; } = Array.Empty<string>();

    /// <summary>使用された音節のリスト</summary>
    public string[] UsedSyllables { get; set; } = Array.Empty<string>();

    /// <summary>使用されたルールセットID</summary>
    public string RulesetId { get; set; } = string.Empty;

    /// <summary>発音しやすさスコア（0.0 ~ 1.0）</summary>
    public float EuphonyScore { get; set; }

    /// <summary>目標印象との一致度（0.0 ~ 1.0）</summary>
    public float ImpressionMatchScore { get; set; }
}
