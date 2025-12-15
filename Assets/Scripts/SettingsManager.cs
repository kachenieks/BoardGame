using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("UI (optional)")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;

    const string MUSIC_VOL = "MusicVolume";
    const string SFX_VOL = "SFXVolume";
    const string MUSIC_ON = "MusicOn";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    void LoadSettings()
    {
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOL, 0.7f);
        float sfxVol   = PlayerPrefs.GetFloat(SFX_VOL, 0.7f);
        bool musicOn   = PlayerPrefs.GetInt(MUSIC_ON, 1) == 1;

        Debug.Log($"Loading: Music={musicVol}, SFX={sfxVol}, MusicOn={musicOn}");

        if (musicSource != null)
        {
            musicSource.volume = musicVol;
            musicSource.mute = !musicOn;

            if (musicOn && !musicSource.isPlaying)
                musicSource.Play();
        }
        else
        {
            Debug.LogError("Music Source nav piešķirts!");
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVol;
        }
        else
        {
            Debug.LogError("SFX Source nav piešķirts!");
        }

        // UI sinhronizācija
        if (musicSlider) musicSlider.value = musicVol;
        if (sfxSlider) sfxSlider.value = sfxVol;
        if (musicToggle) musicToggle.isOn = musicOn;
    }

    // ---------- UI CALLBACKS ----------

    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
            PlayerPrefs.SetFloat(MUSIC_VOL, value);
        }
    }

    public void SetSFXVolume(float value)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = value;
            PlayerPrefs.SetFloat(SFX_VOL, value);
        }
    }

    public void ToggleMusic(bool isOn)
    {
        if (musicSource != null)
        {
            musicSource.mute = !isOn;

            if (isOn && !musicSource.isPlaying)
                musicSource.Play();

            PlayerPrefs.SetInt(MUSIC_ON, isOn ? 1 : 0);
        }
    }
}