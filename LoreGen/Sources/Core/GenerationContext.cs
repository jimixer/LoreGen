namespace LoreGen.Core;

/// <summary>
/// 名前生成のコンテキスト情報を格納するクラス。
/// 生成時の各種パラメータや制約を指定する。
/// </summary>
public class GenerationContext
{
    /// <summary>生成する名前の種類</summary>
    public NameType Type { get; set; } = NameType.Place;

    /// <summary>使用するルールセットのID（nullの場合はデフォルト）</summary>
    public string? RulesetId { get; set; }

    /// <summary>目標とする印象パラメータ</summary>
    public ImpressionVector? TargetImpression { get; set; }

    /// <summary>構造的制約（音節数、文字数など）</summary>
    public StructuralConstraints? Constraints { get; set; }

    /// <summary>乱数シード（再現性のため）</summary>
    public int? Seed { get; set; }

    /// <summary>派生元の名前（既存単語からの変換時）</summary>
    public string? BaseName { get; set; }

    /// <summary>概念タグ（Phase 3以降）</summary>
    public string[]? ConceptTags { get; set; }
}

/// <summary>
/// 名前の種類を表す列挙型。
/// </summary>
public enum NameType
{
    /// <summary>人名</summary>
    Person,

    /// <summary>地名</summary>
    Place,

    /// <summary>地名の形容詞形（例: England → English）</summary>
    PlaceAdjective,

    /// <summary>地名の住民名（例: Rome → Roman）</summary>
    PlaceResident,

    /// <summary>称号</summary>
    Title,

    /// <summary>アーティファクト名</summary>
    Artifact,

    /// <summary>組織名</summary>
    Organization
}
