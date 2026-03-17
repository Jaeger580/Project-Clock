using System;
using TMPro;
using UnityEngine;

public class ShiftTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI digiClock;

    private string finalTime;

    [SerializeField]
    private int startHour, startMinute, endHour, endMinute;
    private DateTime gameStartTime;
    [SerializeField]
    private float timeScale = 60f; // This is how many seconds IN-GAME will pass for ever ONE real second.

    private bool hasWon = false;

    private bool pauseTime = false;

    private void Start()
    {
        hasWon = false;
        gameStartTime = new DateTime(2026, 02, 25, startHour, startMinute, 0);
    }

    private void Update()
    {
        UpdateTime();

        if (!hasWon) 
        {
            if (gameStartTime.Hour == endHour && gameStartTime.Minute == endMinute)
            {
                TriggerWin();
            }
        }
    }

    private void UpdateTime() 
    {
        if (!pauseTime)
        {
            double secondsToAdd = Time.deltaTime * timeScale;
            gameStartTime = gameStartTime.AddSeconds(secondsToAdd);
            finalTime = gameStartTime.ToString("h:00 tt");

            digiClock.text = finalTime;
        }
    }

    private void TriggerWin() 
    {
        pauseTime = true;
        hasWon = true;
        Debug.Log("USE SURVIVED THE NIGHT!!!!!!!!!!");
    }

    public float TotalNightTime()
    {
        return 0;
    }
}
