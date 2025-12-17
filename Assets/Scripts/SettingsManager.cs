using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio UI")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;

    [Header("Graphics UI")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    const string MUSIC_VOL = "MusicVolume";
    const string SFX_VOL = "SFXVolume";
    const string MUSIC_ON = "MusicOn";
    const string RESOLUTION_INDEX = "ResolutionIndex";
    const string FULLSCREEN = "Fullscreen";

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetupResolutions();
        LoadSettings();
        SetupUIListeners();
    }

    void OnEnable()
    {
        // Katru reizi kad panel atveras, pārlādē settings
        LoadSettings();
    }

    void SetupResolutions()
    {
        // Iegūst visas pieejamās rezolūcijas
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        // Filtrē lai nebūtu dublikāti (tikai atšķirīgi width/height)
        HashSet<string> uniqueResolutions = new HashSet<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionString = resolutions[i].width + " x " + resolutions[i].height;
            
            if (!uniqueResolutions.Contains(resolutionString))
            {
                filteredResolutions.Add(resolutions[i]);
                uniqueResolutions.Add(resolutionString);
            }
        }

        // Ja ir dropdown, pievieno opcijas
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();

            int currentResolutionIndex = 0;

            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
                options.Add(option);

                // Atrod pašreizējo rezolūciju
                if (filteredResolutions[i].width == Screen.width && 
                    filteredResolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            Debug.Log($"✅ Ielādētas {filteredResolutions.Count} rezolūcijas");
        }
    }

    void SetupUIListeners()
    {
        // Audio listeners
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (musicToggle != null)
        {
            musicToggle.onValueChanged.RemoveAllListeners();
            musicToggle.onValueChanged.AddListener(ToggleMusic);
        }

        // Graphics listeners
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveAllListeners();
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    void LoadSettings()
    {
        // Audio settings
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOL, 0.7f);
        float sfxVol   = PlayerPrefs.GetFloat(SFX_VOL, 0.7f);
        bool musicOn   = PlayerPrefs.GetInt(MUSIC_ON, 1) == 1;

        Debug.Log($"Loading Audio: Music={musicVol}, SFX={sfxVol}, MusicOn={musicOn}");

        if (musicSource != null)
        {
            musicSource.volume = musicVol;
            musicSource.mute = !musicOn;

            if (musicOn && !musicSource.isPlaying)
                musicSource.Play();
        }
        else
        {
            Debug.LogError("Music Source nav piesķirts!");
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVol;
        }
        else
        {
            Debug.LogError("SFX Source nav piesķirts!");
        }

        // Graphics settings
        int savedResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX, -1);
        bool isFullscreen = PlayerPrefs.GetInt(FULLSCREEN, 1) == 1;

        Debug.Log($"Loading Graphics: Resolution Index={savedResolutionIndex}, Fullscreen={isFullscreen}");

        // Ja ir saglabāta rezolūcija, izmanto to
        if (savedResolutionIndex >= 0 && savedResolutionIndex < filteredResolutions.Count)
        {
            Resolution savedRes = filteredResolutions[savedResolutionIndex];
            Screen.SetResolution(savedRes.width, savedRes.height, isFullscreen);
        }
        else
        {
            // Citādi tikai fullscreen statusu
            Screen.fullScreen = isFullscreen;
        }

        // UI sinhronizācija (BEZ triggeru!)
        if (musicSlider) musicSlider.SetValueWithoutNotify(musicVol);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(sfxVol);
        if (musicToggle) musicToggle.SetIsOnWithoutNotify(musicOn);
        if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
        
        if (resolutionDropdown && savedResolutionIndex >= 0)
        {
            resolutionDropdown.SetValueWithoutNotify(savedResolutionIndex);
        }
    }
    
    // PUBLIC metode lai ārēji var izsaukt reload
    public void ReloadSettings()
    {
        LoadSettings();
    }

    // ---------- AUDIO CALLBACKS ----------

    public void SetMusicVolume(float value)
    {
        Debug.Log($"SetMusicVolume: {value}");
        
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
        
        PlayerPrefs.SetFloat(MUSIC_VOL, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        Debug.Log($"SetSFXVolume: {value}");
        
        if (sfxSource != null)
        {
            sfxSource.volume = value;
        }
        
        PlayerPrefs.SetFloat(SFX_VOL, value);
        PlayerPrefs.Save();
    }

    public void ToggleMusic(bool isOn)
    {
        Debug.Log($"ToggleMusic: {isOn}");
        
        if (musicSource != null)
        {
            musicSource.mute = !isOn;

            if (isOn && !musicSource.isPlaying)
                musicSource.Play();
        }

        PlayerPrefs.SetInt(MUSIC_ON, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ---------- GRAPHICS CALLBACKS ----------

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= filteredResolutions.Count)
        {
            Debug.LogError($"❌ Nederīgs rezolūcijas index: {resolutionIndex}");
            return;
        }

        Resolution resolution = filteredResolutions[resolutionIndex];
        bool isFullscreen = Screen.fullScreen;

        Debug.Log($"🖥️ Maina rezolūciju uz: {resolution.width}x{resolution.height}");

        Screen.SetResolution(resolution.width, resolution.height, isFullscreen);

        PlayerPrefs.SetInt(RESOLUTION_INDEX, resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Debug.Log($"🖥️ Fullscreen: {isFullscreen}");

        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt(FULLSCREEN, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Publiska metode lai PlayOneShot no jebkuras vietas
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}