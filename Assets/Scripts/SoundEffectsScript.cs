using UnityEngine;

public class SoundEffectsScript : MonoBehaviour
{
    public AudioClip[] soundEffects;

    private AudioSource GetSFXSource()
    {
        if (SettingsManager.Instance != null && SettingsManager.Instance.sfxSource != null)
        {
            return SettingsManager.Instance.sfxSource;
        }
        
        Debug.LogError("⚠️ SettingsManager vai sfxSource nav pieejams!");
        return null;
    }

    public void Hover()
    {
        AudioSource sfx = GetSFXSource();
        if (sfx != null && soundEffects.Length > 0)
        {
            sfx.PlayOneShot(soundEffects[0]);
        }
    }

    public void Click()
    {
        AudioSource sfx = GetSFXSource();
        if (sfx != null && soundEffects.Length > 1)
        {
            sfx.PlayOneShot(soundEffects[1]);
        }
    }

    public void OnDice()
    {
        AudioSource sfx = GetSFXSource();
        if (sfx != null && soundEffects.Length > 2)
        {
            sfx.loop = true;
            sfx.clip = soundEffects[2];
            sfx.Play();
        }
    }

    public void CancelButton()
    {
        AudioSource sfx = GetSFXSource();
        if (sfx != null && soundEffects.Length > 3)
        {
            sfx.PlayOneShot(soundEffects[3]);
        }
    }

    public void PlayButton()
    {
        AudioSource sfx = GetSFXSource();
        if (sfx != null && soundEffects.Length > 4)
        {
            sfx.PlayOneShot(soundEffects[4]);
        }
    }

    public void NameField()
    {
        AudioSource sfx = GetSFXSource();
        if (sfx != null && soundEffects.Length > 5)
        {
            sfx.PlayOneShot(soundEffects[5]);
        }
    }
}