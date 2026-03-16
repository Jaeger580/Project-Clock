using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private bool militaryTime = false, hoursOnly = false;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Timer timer;
    private void Start()
    {
        if (timer == null)
            timer = FindAnyObjectByType<Timer>();

        timer.UpdateTimer += UpdateTimerUI;
    }

    private void UpdateTimerUI(float hours)
    {
        if (!militaryTime)
        {
            var ampm = hours < 12 ? "AM" : "PM";
            if(hours > 12)
            {
                hours -= 12;
            }
            TimeSpan span = TimeSpan.FromHours(hours);
            var shownTime = hoursOnly ? $"{span.Hours}" : $"{span:hh\\:mm}";
            var txt = $"{shownTime} {ampm}";
            timerText.text = txt;
        }
        else
        {
            TimeSpan span = TimeSpan.FromHours(hours);
            var shownTime = hoursOnly ? $"{span.Hours}" : $"{span:hh\\:mm}";
            timerText.text = shownTime;
        }

    }
}