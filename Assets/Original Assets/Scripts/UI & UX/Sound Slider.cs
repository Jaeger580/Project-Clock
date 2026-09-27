using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Terrible code for setting volume, but works for now.
// Would be better to convert this script to handle arrays of all the sliders,
// instead multiple  copies of this script are needed for each slider.


public class SoundSlider : MonoBehaviour
{
    [SerializeField] Slider soundSlider;
    [SerializeField] AudioMixer soundMixer;
    [SerializeField] string groupName;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var prefValue = PlayerPrefs.GetFloat(groupName);
        // if player pref exist, load it
        if (prefValue != 0.0f)
        {
            SetVolume(prefValue);
        }
    }

    private void SetVolume(float volume) 
    {
        if(volume < 1) 
        {
            volume = .001f;
        }

        RefreshSlider(volume);
        PlayerPrefs.SetFloat(groupName, volume);
        soundMixer.SetFloat(groupName, Mathf.Log10(volume / 100) * 20);
    }

    public void SetVolumeFromSlider() 
    {
        SetVolume(soundSlider.value);
    }

    public void RefreshSlider(float volume)
    {
        soundSlider.value = volume;
    }
}
