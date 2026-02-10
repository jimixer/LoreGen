using System.Collections.Generic;
using System.Linq;
using LoreGen.Core;

namespace LoreGen.Rules;

/// <summary>
/// 派生ルールのコンテナ
/// </summary>
public class Ruleset
{
    /// <summary>ルールセットID</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>ルールセット名</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>説明</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>派生ルールのリスト</summary>
    public List<DerivationRule> DerivationRules { get; set; } = new();

    /// <summary>デフォルト印象ベクトル</summary>
    public ImpressionVector DefaultImpression { get; set; } = new();

    /// <summary>ルールを追加</summary>
    public void AddRule(DerivationRule rule)
    {
        DerivationRules.Add(rule);
    }

    /// <summary>指定された名前とコンテキストに適用可能なルールを取得</summary>
    public IEnumerable<DerivationRule> GetApplicableRules(string name, GenerationContext context)
    {
        return DerivationRules
            .Where(rule => rule.CanApply(name, context))
            .OrderByDescending(rule => rule.Priority);
    }

    /// <summary>最も優先度の高い適用可能なルールを取得</summary>
    public DerivationRule? GetBestRule(string name, GenerationContext context)
    {
        return GetApplicableRules(name, context).FirstOrDefault();
    }
}
