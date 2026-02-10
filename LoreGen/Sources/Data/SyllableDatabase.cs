using System;
using System.Collections.Generic;
using System.Linq;

namespace LoreGen.Data;

/// <summary>
/// 音節データベース: 音節の管理・検索
/// </summary>
public class SyllableDatabase
{
    private readonly Dictionary<string, Syllable> _syllablesById = new();
    private readonly List<Syllable> _allSyllables = new();

    /// <summary>音節をデータベースに追加</summary>
    public void AddSyllable(Syllable syllable)
    {
        if (string.IsNullOrEmpty(syllable.Id))
            throw new ArgumentException("Syllable must have an Id", nameof(syllable));

        _syllablesById[syllable.Id] = syllable;
        if (!_allSyllables.Contains(syllable))
            _allSyllables.Add(syllable);
    }

    /// <summary>IDで音節を取得</summary>
    public Syllable? GetById(string id) =>
        _syllablesById.TryGetValue(id, out var syllable) ? syllable : null;

    /// <summary>すべての音節を取得</summary>
    public IReadOnlyList<Syllable> GetAll() => _allSyllables.AsReadOnly();

    /// <summary>語頭に配置可能な音節を取得</summary>
    public IEnumerable<Syllable> GetInitialSyllables() =>
        _allSyllables.Where(s => s.Constraints.CanBeInitial);

    /// <summary>語末に配置可能な音節を取得</summary>
    public IEnumerable<Syllable> GetFinalSyllables() =>
        _allSyllables.Where(s => s.Constraints.CanBeFinal);

    /// <summary>指定した音節の後に続けられる音節を取得</summary>
    public IEnumerable<Syllable> GetFollowingSyllables(Syllable previous)
    {
        if (previous.Constraints.CanFollowSyllables.Length > 0)
        {
            // 明示的な許可リストがある場合
            return previous.Constraints.CanFollowSyllables
                .Select(id => GetById(id))
                .Where(s => s != null)!;
        }

        // 禁止リストに含まれない音節を返す
        var cannotFollow = previous.Constraints.CannotFollowSyllables.ToHashSet();
        return _allSyllables.Where(s => !cannotFollow.Contains(s.Id));
    }

    /// <summary>データベースをクリア</summary>
    public void Clear()
    {
        _syllablesById.Clear();
        _allSyllables.Clear();
    }
}
