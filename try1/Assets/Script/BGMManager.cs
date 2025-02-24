using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioClip bgmClip;
    private AudioSource bgmSource;
    public static BGMManager Instance;

    private void Awake()
    {
        bgmSource = GetComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
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

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }
}
