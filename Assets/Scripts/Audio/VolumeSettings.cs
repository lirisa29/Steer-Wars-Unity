using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    [Header("Display")]
    [SerializeField] private Toggle fullscreenToggle;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
        
        if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
            Screen.fullScreen = isFullscreen;
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = isFullscreen;
        }
        else
        {
            // Default: fullscreen on
            Screen.fullScreen = true;
            PlayerPrefs.SetInt("fullscreen", 1);
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = true;
        }
    }
    
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        
        SetMusicVolume();
        SetSFXVolume();
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }
}
