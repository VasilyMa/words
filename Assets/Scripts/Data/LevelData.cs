using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    public int LevelID;
    public List<string> Words;

    [Header("Сколько слов собрать за уровень")]public int targetlWordCount;
    [Header("Сколько в начале уровня будет звезд")] public int initialStars;
    [Header("Сколько еще может быть звезд за уровень")]public int spawnedLaterStars; 
    [Header("Вероятность выпадение звезд")]public float chanceToSpawnStars; 
    [Header("Бонус ко времени")]public float timeBonusMultiplier; 
    [Header("Бонус ко времени за собранное слово")]public AnimationCurve bonus; 
}
