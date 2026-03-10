using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Timer timer;
    private void Start()
    {
        timer.UpdateTimer += UpdateTimerUI;
    }

    private void UpdateTimerUI(float hours)
    {
        TimeSpan span = TimeSpan.FromHours(hours);
        timerText.text = span.ToString(@"hh tt");
    }
}