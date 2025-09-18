using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText; // Привяжи сюда UI Text (Legacy) или TMP_Text для TextMeshPro 

    public void CountdownChange(float value)
    { 
        // Переводим в минуты и секунды
        int minutes = Mathf.FloorToInt(value / 60);
        int seconds = Mathf.FloorToInt(value % 60);

        // Форматируем как 00:00
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    } 
}
