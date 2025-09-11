using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class GoogleParser
{
    /*public static async Task<LoadedData> LoadDataAsync(string csvUrl, LoadedData fallbackData)
    {
        try
        {
            string csv = await DownloadCSV(csvUrl);
            return ParseCSV(csv);
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка загрузки CSV: {e.Message}");

            if (fallbackData != null && fallbackData.levels != null && fallbackData.levels.Count > 0)
            {
                Debug.Log("Используем локальные данные ScriptableObject");
                return fallbackData;
            }
            else
            {
                Debug.LogError("Нет локальных данных! Требуется интернет-соединение.");
                return null;
            }
        }
    }

    private static async Task<string> DownloadCSV(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var tcs = new TaskCompletionSource<bool>();
            var operation = www.SendWebRequest();

            operation.completed += _ => tcs.TrySetResult(true);
            await tcs.Task;

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception(www.error);

            return www.downloadHandler.text;
        }
    }

    private static LoadedData ParseCSV(string csvText)
    {
        var allLevelsData = ScriptableObject.CreateInstance<LoadedData>();
        allLevelsData.levels = new List<LevelData>();

        var lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++) // пропускаем заголовок
        {
            var cells = lines[i].Split(',');
            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.LevelID = int.Parse(cells[0]);
            level.Words = new List<string>();

            for (int j = 1; j < cells.Length; j++)
            {
                if (!string.IsNullOrWhiteSpace(cells[j]))
                    level.Words.Add(cells[j].Trim());
            }

            allLevelsData.levels.Add(level);
        }

        Debug.Log($"CSV успешно распарсен! Уровней: {allLevelsData.levels.Count}");
        return allLevelsData;
    }*/
}