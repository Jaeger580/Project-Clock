using UnityEngine;

public class Timer : MonoBehaviour
{
    [Tooltip("Total amount of time (in seconds) we want the night to take.")]
    [SerializeField] private float totalShiftTime = 600f;
    public float TotalShiftTime => totalShiftTime;
    [Tooltip("Time that would show on the clock, in hours. endTime must be greater than startTime.")]
    [SerializeField] private float startTime, endTime;
    private float totalElapsedTime;
    public float TotalElapsedTime => totalElapsedTime;
    [Tooltip("Time to show on the clock, CONVERTED TO SECONDS.")]
    [SerializeField, ReadOnly] private float visualTime;

    public delegate void TimerEvent(float timer);
    public TimerEvent UpdateTimer;

    private bool paused; //TODO: TEMP, replace with pause system's pause state

    private void OnDestroy()
    {
        UpdateTimer = null;
    }

    private void Update()
    {
        if (ShiftComplete()) return;
        if (paused) return;

        totalElapsedTime += Time.deltaTime;
        visualTime = Mathf.Repeat(Mathf.Lerp(startTime, endTime, totalElapsedTime / totalShiftTime), 24);
        UpdateTimer?.Invoke(visualTime);
    }

    public bool ShiftComplete()
    {
        return totalElapsedTime >= totalShiftTime;
    }
}
