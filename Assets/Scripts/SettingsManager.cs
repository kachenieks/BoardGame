using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

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
        int musicOn    = PlayerPrefs.GetInt(MUSIC_ON, 1);

        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;
        musicSource.mute = musicOn == 0;
    }

    // ---------- UI FUNKCIJAS ----------

    public void SetMusicVolume(float value)
{
    musicSource.volume = value;
    musicSource.mute = value <= 0.01f ? true : false;

    if (!musicSource.isPlaying && !musicSource.mute)
        musicSource.Play();

    PlayerPrefs.SetFloat("MusicVolume", value);
}


    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat(SFX_VOL, value);
    }

    public void ToggleMusic(bool isOn)
{
    musicSource.mute = !isOn;

    if (isOn && !musicSource.isPlaying)
        musicSource.Play();

    PlayerPrefs.SetInt("MusicOn", isOn ? 1 : 0);
}

}
