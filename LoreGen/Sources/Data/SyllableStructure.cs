namespace LoreGen.Data;

/// <summary>
/// 音節の音韻構造（頭子音・核母音・尾子音）
/// </summary>
public class SyllableStructure
{
    /// <summary>頭子音 (Onset): "k", "th", "str" など</summary>
    public string Onset { get; set; } = string.Empty;

    /// <summary>核母音 (Nucleus): "a", "o", "ei" など</summary>
    public string Nucleus { get; set; } = string.Empty;

    /// <summary>尾子音 (Coda): "r", "th", "nt" など</summary>
    public string Coda { get; set; } = string.Empty;

    /// <summary>構造タイプを取得 (例: "CVC", "CV", "VC")</summary>
    public string GetStructureType()
    {
        var type = string.Empty;
        if (!string.IsNullOrEmpty(Onset)) type += "C";
        if (!string.IsNullOrEmpty(Nucleus)) type += "V";
        if (!string.IsNullOrEmpty(Coda)) type += "C";
        return type;
    }

    /// <summary>構造を文字列として取得 (onset + nucleus + coda)</summary>
    public string GetPattern() => Onset + Nucleus + Coda;
}
