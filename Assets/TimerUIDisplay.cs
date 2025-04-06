using TMPro;
using UnityEngine;

public class TimerUIDisplay : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    void Update()
    {
        if (TimerManager.Instance != null)
        {
            float time = TimerManager.Instance.GetTime();
            timerText.text = FormatTime(time);
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}