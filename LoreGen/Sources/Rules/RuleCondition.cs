using System;
using LoreGen.Core;

namespace LoreGen.Rules;

/// <summary>
/// ルール適用の条件
/// </summary>
public class RuleCondition
{
    /// <summary>必須のNameType（nullの場合は制約なし）</summary>
    public NameType? RequiredType { get; set; }

    /// <summary>カスタム述語（追加の条件判定）</summary>
    public Func<string, GenerationContext, bool>? CustomPredicate { get; set; }

    /// <summary>条件を満たすか判定</summary>
    public bool IsSatisfied(string name, GenerationContext context)
    {
        // NameType制約チェック
        if (RequiredType.HasValue && context.Type != RequiredType.Value)
            return false;

        // カスタム述語チェック
        if (CustomPredicate != null && !CustomPredicate(name, context))
            return false;

        return true;
    }
}
