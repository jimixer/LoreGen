using System.Text.RegularExpressions;
using LoreGen.Core;

namespace LoreGen.Rules;

/// <summary>
/// 正規表現による名前派生ルール
/// </summary>
public class DerivationRule
{
    /// <summary>ルール名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>マッチパターン（正規表現）</summary>
    public string MatchPattern { get; set; } = string.Empty;

    /// <summary>置換パターン</summary>
    public string ReplacePattern { get; set; } = string.Empty;

    /// <summary>適用条件</summary>
    public RuleCondition? Condition { get; set; }

    /// <summary>印象ベクトルの変化量</summary>
    public ImpressionVector ImpressionShift { get; set; } = new();

    /// <summary>優先度（高い方が優先）</summary>
    public float Priority { get; set; } = 0.0f;

    /// <summary>指定された名前にこのルールを適用可能か判定</summary>
    public bool CanApply(string name, GenerationContext context)
    {
        // 正規表現マッチング
        if (!Regex.IsMatch(name, MatchPattern, RegexOptions.IgnoreCase))
            return false;

        // 条件評価
        if (Condition != null && !Condition.IsSatisfied(name, context))
            return false;

        return true;
    }

    /// <summary>名前にルールを適用</summary>
    public string Apply(string name)
    {
        return Regex.Replace(name, MatchPattern, ReplacePattern, RegexOptions.IgnoreCase);
    }
}
