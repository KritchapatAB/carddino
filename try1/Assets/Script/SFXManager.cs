using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip SFXtest;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = sfxMixerGroup;
        audioSource.playOnAwake = false;
    }

    // Public Method to Play SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Play Button Click SFX
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    // Play Test SFX
    public void PlayTestSFX()
    {
        PlaySFX(SFXtest);
    }
}
