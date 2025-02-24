using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    // [SerializeField] private Toggle muteToggle;
    [SerializeField] private Button closeButton;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    // Default Values
    private const float defaultVolume = 1.0f;
    private const int defaultScreenMode = 0;
    // private const bool defaultMute = false;

    private void Start()
    {
        // settingsPanel.SetActive(false);
        closeButton.onClick.AddListener(ClosePopup);

        // Populate Screen Mode Dropdown
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> {
            "Fullscreen", "Windowed", "Borderless"
        });

        // 🔥 Apply Saved Settings Directly to AudioMixer
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        }
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume"));
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        }

        // Load Saved Settings or Set Default
        LoadSettings();

        // Add Listeners and Save Directly
        screenModeDropdown.onValueChanged.AddListener((int index) => {
            SetScreenMode(index);
            PlayerPrefs.SetInt("ScreenMode", index);
            PlayerPrefs.Save();
        });

        masterVolumeSlider.onValueChanged.AddListener((float volume) => {
            SetMasterVolume(volume);
            PlayerPrefs.SetFloat("MasterVolume", volume);
            PlayerPrefs.Save();
        });

        bgmVolumeSlider.onValueChanged.AddListener((float volume) => {
            SetBGMVolume(volume);
            PlayerPrefs.SetFloat("BGMVolume", volume);
            PlayerPrefs.Save();
        });

        sfxVolumeSlider.onValueChanged.AddListener((float volume) => {
            SetSFXVolume(volume);
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
        });

        // muteToggle.onValueChanged.AddListener((bool isMuted) => {
        //     SetMute(isMuted);
        //     PlayerPrefs.SetInt("Mute", isMuted ? 1 : 0);
        //     PlayerPrefs.Save();
        // });
    }

    public void OpenPopup()
    {
        settingsPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        settingsPanel.SetActive(false);
    }

    private void SetScreenMode(int index)
    {
        switch (index)
        {
            case 0: // Fullscreen
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.fullScreen = true;
                break;
            case 1: // Windowed
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.fullScreen = false;
                break;
            case 2: // Borderless
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                Screen.fullScreen = true;
                break;
        }
    }

    private void SetMasterVolume(float volume)
    {
        if (volume <= 0.001f)
        {
            audioMixer.SetFloat("MasterVolume", -80f); // Mute when slider is at 0
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        }
    }

    private void SetBGMVolume(float volume)
    {
        if (volume <= 0.001f)
        {
            audioMixer.SetFloat("BGMVolume", -80f); // Mute when slider is at 0
        }
        else
        {
            audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        }
    }

    private void SetSFXVolume(float volume)
    {
        if (volume <= 0.001f)
        {
            audioMixer.SetFloat("SFXVolume", -80f); // Mute when slider is at 0
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
    }

    // private void SetMute(bool isMuted)
    // {
    //     AudioListener.pause = isMuted;
    // }

    private void LoadSettings()
    {
        // ✅ Check if any volume settings exist
        bool hasMaster = PlayerPrefs.HasKey("MasterVolume");
        bool hasBGM = PlayerPrefs.HasKey("BGMVolume");
        bool hasSFX = PlayerPrefs.HasKey("SFXVolume");

        // ✅ Load Settings
        screenModeDropdown.value = PlayerPrefs.GetInt("ScreenMode", defaultScreenMode);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", defaultVolume);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", defaultVolume);
        // muteToggle.isOn = PlayerPrefs.GetInt("Mute", defaultMute ? 1 : 0) == 1;

        // ✅ Apply loaded settings
        SetScreenMode(screenModeDropdown.value);
        SetMasterVolume(masterVolumeSlider.value);
        SetBGMVolume(bgmVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);
        // SetMute(muteToggle.isOn);

        Debug.Log("Settings loaded successfully.");
    }

    public void InitializeSettings()
    {
        LoadSettings();
    }
}
