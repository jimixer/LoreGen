using System;
using System.Collections.Generic;
using System.Linq;

namespace LoreGen.Utilities;

/// <summary>
/// シード可能な乱数提供クラス。
/// 決定論的な生成を可能にする。
/// </summary>
public class RandomProvider
{
    private readonly Random _random;

    /// <summary>
    /// 使用されているシード値
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// シード値を指定してRandomProviderを初期化する。
    /// </summary>
    /// <param name="seed">乱数シード</param>
    public RandomProvider(int seed)
    {
        Seed = seed;
        _random = new Random(seed);
    }

    /// <summary>
    /// 現在時刻をシードとしてRandomProviderを初期化する。
    /// </summary>
    public RandomProvider() : this(Environment.TickCount)
    {
    }

    /// <summary>
    /// 0以上maxValue未満のランダムな整数を返す。
    /// </summary>
    public int Next(int maxValue) => _random.Next(maxValue);

    /// <summary>
    /// minValue以上maxValue未満のランダムな整数を返す。
    /// </summary>
    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);

    /// <summary>
    /// 0.0以上1.0未満のランダムな浮動小数点数を返す。
    /// </summary>
    public double NextDouble() => _random.NextDouble();

    /// <summary>
    /// 0.0以上1.0未満のランダムなfloat値を返す。
    /// </summary>
    public float NextFloat() => (float)_random.NextDouble();

    /// <summary>
    /// 配列からランダムに要素を選択する。
    /// </summary>
    public T Choose<T>(T[] items)
    {
        if (items.Length == 0)
            throw new ArgumentException("配列が空です", nameof(items));

        return items[_random.Next(items.Length)];
    }

    /// <summary>
    /// リストからランダムに要素を選択する。
    /// </summary>
    public T Choose<T>(IList<T> items)
    {
        if (items.Count == 0)
            throw new ArgumentException("リストが空です", nameof(items));

        return items[_random.Next(items.Count)];
    }

    /// <summary>
    /// 重み付きリストからランダムに要素を選択する。
    /// </summary>
    public T ChooseWeighted<T>(IList<T> items, IList<float> weights)
    {
        if (items.Count != weights.Count)
            throw new ArgumentException("アイテムと重みの数が一致しません");

        if (items.Count == 0)
            throw new ArgumentException("リストが空です", nameof(items));

        var totalWeight = weights.Sum();
        var randomValue = NextFloat() * totalWeight;

        float cumulativeWeight = 0;
        for (int i = 0; i < items.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
                return items[i];
        }

        return items[items.Count - 1];
    }

    /// <summary>
    /// 指定された確率でtrueを返す。
    /// </summary>
    /// <param name="probability">確率（0.0 ~ 1.0）</param>
    public bool Chance(float probability) => NextFloat() < probability;
}
