using UnityEngine;

public class SoundEffectsScript : MonoBehaviour
{
    public AudioClip[] soundEffects;

    private AudioSource audioSource;

    void Start()
    {
        // 🔗 Paņem SFX AudioSource no SettingsManager
        if (SettingsManager.Instance != null)
        {
            audioSource = SettingsManager.Instance.sfxSource;
        }
        else
        {
            Debug.LogError("❌ SettingsManager nav atrasts!");
        }
    }

    public void Hover()
    {
        audioSource.PlayOneShot(soundEffects[0]);
    }

    public void Click()
    {
        audioSource.PlayOneShot(soundEffects[1]);
    }

    public void OnDice()
    {
        audioSource.loop = true;
        audioSource.clip = soundEffects[2];
        audioSource.Play();
    }

    public void CancelButton()
    {
        audioSource.PlayOneShot(soundEffects[3]);
    }

    public void PlayButton()
    {
        audioSource.PlayOneShot(soundEffects[4]);
    }

    public void NameField()
    {
        audioSource.PlayOneShot(soundEffects[5]);
    }
}
