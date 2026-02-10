using System;
using LoreGen.Core;

namespace LoreGen.Rules;

/// <summary>
/// 派生ルール適用エンジン
/// </summary>
public class DerivationEngine
{
    private readonly Ruleset _ruleset;

    public DerivationEngine(Ruleset ruleset)
    {
        _ruleset = ruleset ?? throw new ArgumentNullException(nameof(ruleset));
    }

    /// <summary>基本名から派生名を生成</summary>
    public GenerationResult Derive(string baseName, NameType targetType, int? seed = null)
    {
        var context = new GenerationContext
        {
            Type = targetType,
            BaseName = baseName,
            Seed = seed
        };

        return Derive(baseName, context);
    }

    /// <summary>基本名とコンテキストから派生名を生成</summary>
    public GenerationResult Derive(string baseName, GenerationContext context)
    {
        if (string.IsNullOrEmpty(baseName))
            throw new ArgumentException("Base name cannot be null or empty", nameof(baseName));

        // 適用可能なルールを取得
        var rule = _ruleset.GetBestRule(baseName, context);

        if (rule == null)
        {
            // 適用可能なルールがない場合は元の名前をそのまま返す
            return new GenerationResult
            {
                Name = baseName,
                ActualImpression = _ruleset.DefaultImpression,
                Metadata = new GenerationMetadata
                {
                    AppliedRules = Array.Empty<string>(),
                    RulesetId = _ruleset.Id
                }
            };
        }

        // ルールを適用
        var derivedName = rule.Apply(baseName);

        // 印象ベクトルを計算（デフォルト + シフト）
        var impression = CalculateImpression(_ruleset.DefaultImpression, rule.ImpressionShift);

        return new GenerationResult
        {
            Name = derivedName,
            ActualImpression = impression,
            Metadata = new GenerationMetadata
            {
                AppliedRules = new[] { rule.Name },
                RulesetId = _ruleset.Id
            }
        };
    }

    /// <summary>印象ベクトルのシフト適用</summary>
    private ImpressionVector CalculateImpression(ImpressionVector baseImpression, ImpressionVector shift)
    {
        return new ImpressionVector
        {
            Hardness = Clamp(baseImpression.Hardness + shift.Hardness),
            Sharpness = Clamp(baseImpression.Sharpness + shift.Sharpness),
            Complexity = Clamp(baseImpression.Complexity + shift.Complexity),
            Rhythmicity = Clamp(baseImpression.Rhythmicity + shift.Rhythmicity),
            Antiquity = Clamp(baseImpression.Antiquity + shift.Antiquity),
            Formality = Clamp(baseImpression.Formality + shift.Formality),
            Exoticism = Clamp(baseImpression.Exoticism + shift.Exoticism),
            Mysticism = Clamp(baseImpression.Mysticism + shift.Mysticism)
        };
    }

    /// <summary>値を0.0-1.0の範囲にクランプ</summary>
    private float Clamp(float value)
    {
        if (value < 0.0f) return 0.0f;
        if (value > 1.0f) return 1.0f;
        return value;
    }
}
