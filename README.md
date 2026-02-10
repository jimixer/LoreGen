# LoreGen

音韻学に基づく固有名詞生成ライブラリ（.NET Standard 2.1）

## 概要

LoreGenは、印象パラメータで制御可能なファンタジー風の固有名詞（地名・人名）を生成するライブラリです。

## 特徴

- **印象ベースの生成**: 硬度・鋭さ・古風さなど8次元の印象パラメータで制御
- **音節組み合わせ**: 音韻制約を考慮した自然な発音
- **決定論的生成**: シード値による再現性
- **Unity互換**: .NET Standard 2.1（Unity 2021+対応）

## クイックスタート

```csharp
using LoreGen.Core;
using LoreGen.Data;
using LoreGen.Generation;

// 音節データベース準備
var database = new SyllableDatabase();
database.AddSyllable(new Syllable
{
    Id = "kar",
    Pattern = "kar",
    Structure = new SyllableStructure { Onset = "k", Nucleus = "a", Coda = "r" },
    Impression = new ImpressionVector { Hardness = 0.7f },
    Constraints = new PhoneticConstraints { CanBeInitial = true, CanBeFinal = true },
    Weight = 1.0f
});

// 名前生成
var generator = new NameGenerator(database);
var result = generator.Generate(new GenerationContext
{
    Type = NameType.Place,
    Seed = 42,
    Constraints = new StructuralConstraints
    {
        MinSyllables = 2,
        MaxSyllables = 3
    }
});

Console.WriteLine(result.Name);  // 例: "Kardor"
Console.WriteLine($"硬度: {result.ActualImpression.Hardness}");
```

## 印象パラメータ

| パラメータ | 説明 | 低い例 | 高い例 |
|-----------|------|--------|--------|
| Hardness | 硬さ | Miral (0.3) | Kardor (0.7) |
| Sharpness | 鋭さ | Thorun (0.2) | Kiriel (0.8) |
| Complexity | 複雑度 | Rom (0.1) | Xyzanthius (0.9) |
| Antiquity | 古風さ | Nex (0.2) | Aethelred (0.8) |
| Formality | 格式 | Bob (0.1) | Alexandrius (0.9) |
| Exoticism | 異国性 | John (0.1) | Xochitl (0.9) |
| Mysticism | 神秘性 | Stone (0.2) | Azathoth (0.9) |

## プロジェクト構成

```
LoreGen/
├── Sources/
│   ├── Core/          # データモデル（ImpressionVector, GenerationContext）
│   ├── Data/          # 音節システム（Syllable, SyllableDatabase）
│   ├── Generation/    # 名前生成エンジン
│   └── Utilities/     # RandomProvider等
└── Tests/
    ├── Manual/        # 手動実行デモ（VS Code Test Explorerで実行）
    └── Integration/   # 統合テスト
```

## 現在の状態

**Phase 1 完成** - 基礎システム（音節組み合わせ生成、印象パラメータ、テスト41個合格）

## 今後の実装予定

- Phase 2: 派生ルール（正規表現変換、地名→形容詞化）
- Phase 3: マルコフ連鎖生成、概念パラメータ
- Phase 4: 音韻規則エンジン（母音調和、鼻音同化）

## ライセンス

個人プロジェクト

## 技術スタック

- .NET Standard 2.1
- C# 10.0（LangVersion）
- NUnit (テストフレームワーク)
