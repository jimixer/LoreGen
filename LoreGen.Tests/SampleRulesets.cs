using LoreGen.Core;
using LoreGen.Rules;

namespace LoreGen.Tests;

/// <summary>
/// テストおよびデモ用のサンプルルールセット
/// </summary>
public static class SampleRulesets
{
    /// <summary>ファンタジー風の派生ルールセット</summary>
    public static Ruleset CreateFantasyRuleset()
    {
        var ruleset = new Ruleset
        {
            Id = "fantasy",
            Name = "Fantasy Derivation",
            Description = "ファンタジー世界の地名派生ルール",
            DefaultImpression = new ImpressionVector
            {
                Formality = 0.5f,
                Antiquity = 0.6f
            }
        };

        // -or → -orian（形容詞化）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Place to Adjective (-or → -orian)",
            MatchPattern = @"^(.+)or$",
            ReplacePattern = "$1orian",
            Priority = 2.0f,
            Condition = new RuleCondition { RequiredType = NameType.PlaceAdjective },
            ImpressionShift = new ImpressionVector { Formality = 0.3f, Antiquity = 0.1f }
        });

        // -al → -alan（住民名化）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Place to Resident (-al → -alan)",
            MatchPattern = @"^(.+)al$",
            ReplacePattern = "$1alan",
            Priority = 2.0f,
            Condition = new RuleCondition { RequiredType = NameType.PlaceResident },
            ImpressionShift = new ImpressionVector { Formality = 0.2f }
        });

        // 一般的な形容詞化（-ian）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Generic Adjective (-ian)",
            MatchPattern = @"^(.+)$",
            ReplacePattern = "$1ian",
            Priority = 0.5f,
            Condition = new RuleCondition { RequiredType = NameType.PlaceAdjective },
            ImpressionShift = new ImpressionVector { Formality = 0.2f }
        });

        // 一般的な住民名化（-ese）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Generic Resident (-ese)",
            MatchPattern = @"^(.+)$",
            ReplacePattern = "$1ese",
            Priority = 0.3f,
            Condition = new RuleCondition { RequiredType = NameType.PlaceResident },
            ImpressionShift = new ImpressionVector { Formality = 0.1f }
        });

        return ruleset;
    }

    /// <summary>英語風の派生ルールセット</summary>
    public static Ruleset CreateEnglishStyleRuleset()
    {
        var ruleset = new Ruleset
        {
            Id = "english",
            Name = "English-Style Derivation",
            Description = "英語的な地名派生ルール",
            DefaultImpression = new ImpressionVector
            {
                Formality = 0.6f,
                Antiquity = 0.5f
            }
        };

        // -land → -lish（England → English）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Land to Lish",
            MatchPattern = @"^(.+)land$",
            ReplacePattern = "$1lish",
            Priority = 3.0f,
            ImpressionShift = new ImpressionVector { Formality = 0.1f, Antiquity = 0.2f }
        });

        // -ia → -ian（India → Indian）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Ia to Ian",
            MatchPattern = @"^(.+)ia$",
            ReplacePattern = "$1ian",
            Priority = 2.0f,
            ImpressionShift = new ImpressionVector { Formality = 0.2f }
        });

        // -y → -ian（Germany → German）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Y to Ian",
            MatchPattern = @"^(.+)y$",
            ReplacePattern = "$1ian",
            Priority = 2.0f,
            ImpressionShift = new ImpressionVector { Formality = 0.2f }
        });

        // -a → -an（America → American）
        ruleset.AddRule(new DerivationRule
        {
            Name = "A to An",
            MatchPattern = @"^(.+)a$",
            ReplacePattern = "$1an",
            Priority = 1.5f,
            ImpressionShift = new ImpressionVector { Formality = 0.15f }
        });

        // -e → -ese（China → Chinese）
        ruleset.AddRule(new DerivationRule
        {
            Name = "E to Ese",
            MatchPattern = @"^(.+)a$",
            ReplacePattern = "$1ese",
            Priority = 1.5f,
            Condition = new RuleCondition
            {
                CustomPredicate = (name, ctx) => name.EndsWith("in")
            },
            ImpressionShift = new ImpressionVector { Exoticism = 0.2f }
        });

        // 汎用 -ian
        ruleset.AddRule(new DerivationRule
        {
            Name = "Generic Ian",
            MatchPattern = @"^(.+)$",
            ReplacePattern = "$1ian",
            Priority = 0.5f,
            ImpressionShift = new ImpressionVector { Formality = 0.1f }
        });

        return ruleset;
    }

    /// <summary>古風な派生ルールセット</summary>
    public static Ruleset CreateAncientRuleset()
    {
        var ruleset = new Ruleset
        {
            Id = "ancient",
            Name = "Ancient Derivation",
            Description = "古代風の地名派生ルール",
            DefaultImpression = new ImpressionVector
            {
                Formality = 0.7f,
                Antiquity = 0.8f
            }
        };

        // -us → -ian（ラテン語風）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Us to Ian (Latin style)",
            MatchPattern = @"^(.+)us$",
            ReplacePattern = "$1ian",
            Priority = 2.0f,
            ImpressionShift = new ImpressionVector { Antiquity = 0.1f, Formality = 0.2f }
        });

        // -um → -an（forum → foran）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Um to An",
            MatchPattern = @"^(.+)um$",
            ReplacePattern = "$1an",
            Priority = 2.0f,
            ImpressionShift = new ImpressionVector { Antiquity = 0.1f }
        });

        // -is → -ite（ギリシャ風）
        ruleset.AddRule(new DerivationRule
        {
            Name = "Is to Ite (Greek style)",
            MatchPattern = @"^(.+)is$",
            ReplacePattern = "$1ite",
            Priority = 2.0f,
            ImpressionShift = new ImpressionVector { Antiquity = 0.15f, Mysticism = 0.1f }
        });

        // -on → -onian
        ruleset.AddRule(new DerivationRule
        {
            Name = "On to Onian",
            MatchPattern = @"^(.+)on$",
            ReplacePattern = "$1onian",
            Priority = 2.0f,
            ImpressionShift = new ImpressionVector { Antiquity = 0.1f, Formality = 0.15f }
        });

        return ruleset;
    }
}
