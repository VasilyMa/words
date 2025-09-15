using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    public int LevelID;
    public List<string> Words;

    public int totalStars;        // сколько всего звёзд нужно собрать для победы
    public int initialStars;      // сколько звёзд размещается сразу на поле
    public int spawnedLaterStars; // сколько добавляется потом при падении букв
}
