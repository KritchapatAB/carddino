using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SettingsPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button closeButton;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    // Default Values
    private const float defaultVolume = 1.0f;
    private const int defaultScreenMode = 0;

    // JSON Save File Path
    private string settingsFilePath;

    private void Awake()
    {
        // Define the file path
        settingsFilePath = Path.Combine(Application.persistentDataPath, "settingSave.json");
    }

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);

        // Populate Screen Mode Dropdown
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> {
            "Fullscreen", "Windowed", "Borderless"
        });

        // Load Settings from JSON File or Set Default
        LoadSettingsFromFile();

        // Add Listeners and Save Directly
        screenModeDropdown.onValueChanged.AddListener((int index) =>
        {
            SetScreenMode(index);
            SaveSettings();
        });

        masterVolumeSlider.onValueChanged.AddListener((float volume) =>
        {
            SetMasterVolume(volume);
            SaveSettings();
        });

        bgmVolumeSlider.onValueChanged.AddListener((float volume) =>
        {
            SetBGMVolume(volume);
            SaveSettings();
        });

        sfxVolumeSlider.onValueChanged.AddListener((float volume) =>
        {
            SetSFXVolume(volume);
            SaveSettings();
        });
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

    private void LoadSettings()
    {
        // Load Settings from JSON File
        LoadSettingsFromFile();

        // ✅ Apply loaded settings
        SetScreenMode(screenModeDropdown.value);
        SetMasterVolume(masterVolumeSlider.value);
        SetBGMVolume(bgmVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);

        Debug.Log("Settings loaded successfully.");
    }

    public void InitializeSettings()
    {
        LoadSettings();
    }

    // 🔥 New Method: Save Settings to JSON File
    public void SaveSettings()
    {
        SettingsData data = new SettingsData
        {
            ScreenMode = screenModeDropdown.value,
            MasterVolume = masterVolumeSlider.value,
            BGMVolume = bgmVolumeSlider.value,
            SFXVolume = sfxVolumeSlider.value
        };

        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(settingsFilePath, jsonData);

        Debug.Log($"Settings saved to {settingsFilePath}");
    }

    // 🔥 New Method: Load Settings from JSON File
    private void LoadSettingsFromFile()
    {
        if (File.Exists(settingsFilePath))
        {
            string jsonData = File.ReadAllText(settingsFilePath);
            SettingsData data = JsonUtility.FromJson<SettingsData>(jsonData);

            // Apply Loaded Data
            screenModeDropdown.value = data.ScreenMode;
            masterVolumeSlider.value = data.MasterVolume;
            bgmVolumeSlider.value = data.BGMVolume;
            sfxVolumeSlider.value = data.SFXVolume;

            Debug.Log($"Settings loaded from {settingsFilePath}");
        }
        else
        {
            Debug.Log("No settings file found. Using default values.");
        }
    }

    // 🔥 Settings Data Class for JSON Serialization
    [System.Serializable]
    private class SettingsData
    {
        public int ScreenMode;
        public float MasterVolume;
        public float BGMVolume;
        public float SFXVolume;
    }
}
