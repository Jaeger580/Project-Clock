using UnityEngine;

public class FPSLimit : MonoBehaviour
{
    [SerializeField]
    private int frameRateTarget = 60;

    // Turn off vysnc and set the target frame rate.
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRateTarget;
    }
}
