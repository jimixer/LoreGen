using System;

namespace LoreGen.Data;

/// <summary>
/// 音韻制約: 音節の配置・連続可能性の制約
/// </summary>
public class PhoneticConstraints
{
    /// <summary>この音節の後に続けられる音節ID配列</summary>
    public string[] CanFollowSyllables { get; set; } = Array.Empty<string>();

    /// <summary>この音節の後に続けることができない音節ID配列</summary>
    public string[] CannotFollowSyllables { get; set; } = Array.Empty<string>();

    /// <summary>語頭に配置可能か</summary>
    public bool CanBeInitial { get; set; } = true;

    /// <summary>語末に配置可能か</summary>
    public bool CanBeFinal { get; set; } = true;
}
