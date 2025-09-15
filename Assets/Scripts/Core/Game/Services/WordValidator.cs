using System.Collections.Generic;
using UnityEngine;

public static class WordValidator
{
    private static HashSet<string> dictionary;
    private static HashSet<string> levelWords;

    public static bool IsInitialized => dictionary != null;

    /// <summary>
    /// »нициализаци€ словар€ и слов уровн€
    /// </summary>
    public static void Init(LevelData levelData, TextAsset dictionaryFile)
    {
        if (dictionary == null)
        {
            dictionary = new HashSet<string>(
                dictionaryFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries),
                System.StringComparer.OrdinalIgnoreCase
            );
            Debug.Log($"[WordValidator] «агружено {dictionary.Count} слов в словаре");
        }

        // перезаписываем LevelData дл€ нового уровн€
        levelWords = new HashSet<string>(levelData.Words, System.StringComparer.OrdinalIgnoreCase);
        Debug.Log($"[WordValidator] «агружено {levelWords.Count} слов уровн€");
    }

    /// <summary>
    /// ѕроверка слова
    /// </summary>
    public static WordCheckResult CheckWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return WordCheckResult.None;

        word = word.ToLower();

        if (levelWords != null && levelWords.Contains(word))
            return WordCheckResult.LevelWord;

        if (dictionary != null && dictionary.Contains(word))
            return WordCheckResult.Bonus;

        return WordCheckResult.None;
    }
} 
public enum WordCheckResult
{
    None,       // не слово
    Bonus,      // допустимое слово из словар€
    LevelWord   // слово из LevelData (об€зательное дл€ победы)
} 
