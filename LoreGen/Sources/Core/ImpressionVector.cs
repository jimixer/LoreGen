using System;

namespace LoreGen.Core;

/// <summary>
/// 名前生成における印象パラメータを表現する構造体。
/// 音響的特性（硬度、鋭さ、複雑度、リズム性）と
/// 文化的特性（古風さ、格式、異国性、神秘性）の8次元を持つ。
/// </summary>
public struct ImpressionVector
{
    // 音響的特性（0.0 ~ 1.0）

    /// <summary>硬さ（0.0: 柔らかい（m,n,l） ~ 1.0: 硬い（k,t,g））</summary>
    public float Hardness { get; set; }

    /// <summary>鋭さ（0.0: 鈍い（o,u） ~ 1.0: 鋭い（i,e））</summary>
    public float Sharpness { get; set; }

    /// <summary>複雑度（0.0: シンプル ~ 1.0: 複雑）</summary>
    public float Complexity { get; set; }

    /// <summary>リズム規則性（0.0: 不規則 ~ 1.0: 規則的）</summary>
    public float Rhythmicity { get; set; }

    // 文化的特性（0.0 ~ 1.0）

    /// <summary>古風さ（0.0: 現代的 ~ 1.0: 古風）</summary>
    public float Antiquity { get; set; }

    /// <summary>格式（0.0: カジュアル ~ 1.0: 格式高い）</summary>
    public float Formality { get; set; }

    /// <summary>異国性（0.0: 馴染み深い ~ 1.0: 異国的）</summary>
    public float Exoticism { get; set; }

    /// <summary>神秘性（0.0: 平凡 ~ 1.0: 神秘的）</summary>
    public float Mysticism { get; set; }

    /// <summary>
    /// 全パラメータが0のベクトルを返す。
    /// </summary>
    public static ImpressionVector Zero => new();

    /// <summary>
    /// 2つの印象ベクトルをブレンドする。
    /// </summary>
    /// <param name="a">ベクトルA</param>
    /// <param name="b">ベクトルB</param>
    /// <param name="weight">ベクトルBの重み（0.0 ~ 1.0）</param>
    /// <returns>ブレンドされたベクトル</returns>
    public static ImpressionVector Blend(ImpressionVector a, ImpressionVector b, float weight)
    {
        weight = Math.Clamp(weight, 0.0f, 1.0f);
        var invWeight = 1.0f - weight;

        return new ImpressionVector
        {
            Hardness = a.Hardness * invWeight + b.Hardness * weight,
            Sharpness = a.Sharpness * invWeight + b.Sharpness * weight,
            Complexity = a.Complexity * invWeight + b.Complexity * weight,
            Rhythmicity = a.Rhythmicity * invWeight + b.Rhythmicity * weight,
            Antiquity = a.Antiquity * invWeight + b.Antiquity * weight,
            Formality = a.Formality * invWeight + b.Formality * weight,
            Exoticism = a.Exoticism * invWeight + b.Exoticism * weight,
            Mysticism = a.Mysticism * invWeight + b.Mysticism * weight
        };
    }

    /// <summary>
    /// このベクトルと別のベクトルとのユークリッド距離を計算する。
    /// </summary>
    /// <param name="other">比較対象のベクトル</param>
    /// <returns>ユークリッド距離</returns>
    public float Distance(ImpressionVector other)
    {
        var dh = Hardness - other.Hardness;
        var ds = Sharpness - other.Sharpness;
        var dc = Complexity - other.Complexity;
        var dr = Rhythmicity - other.Rhythmicity;
        var da = Antiquity - other.Antiquity;
        var df = Formality - other.Formality;
        var de = Exoticism - other.Exoticism;
        var dm = Mysticism - other.Mysticism;

        return (float)Math.Sqrt(
            dh * dh + ds * ds + dc * dc + dr * dr +
            da * da + df * df + de * de + dm * dm
        );
    }

    /// <summary>
    /// 全パラメータを0.0～1.0の範囲にクランプする。
    /// </summary>
    /// <returns>正規化されたベクトル</returns>
    public ImpressionVector Normalize()
    {
        return new ImpressionVector
        {
            Hardness = Math.Clamp(Hardness, 0.0f, 1.0f),
            Sharpness = Math.Clamp(Sharpness, 0.0f, 1.0f),
            Complexity = Math.Clamp(Complexity, 0.0f, 1.0f),
            Rhythmicity = Math.Clamp(Rhythmicity, 0.0f, 1.0f),
            Antiquity = Math.Clamp(Antiquity, 0.0f, 1.0f),
            Formality = Math.Clamp(Formality, 0.0f, 1.0f),
            Exoticism = Math.Clamp(Exoticism, 0.0f, 1.0f),
            Mysticism = Math.Clamp(Mysticism, 0.0f, 1.0f)
        };
    }

    public override string ToString()
    {
        return $"Impression(H:{Hardness:F2}, S:{Sharpness:F2}, C:{Complexity:F2}, " +
               $"R:{Rhythmicity:F2}, A:{Antiquity:F2}, F:{Formality:F2}, " +
               $"E:{Exoticism:F2}, M:{Mysticism:F2})";
    }
}
