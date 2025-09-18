using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText; // Привяжи сюда UI Text (Legacy) или TMP_Text для TextMeshPro
    private float elapsedTime = 0f;
    bool isStart;
    public void Invoke()
    {
        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        isStart = true;

        while (isStart)
        { 
            // Увеличиваем время
            elapsedTime += Time.deltaTime;

            // Переводим в минуты и секунды
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            // Форматируем как 00:00
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return null;
        }
    } 
}
