using UnityEngine;
using UnityEngine.Audio;
using WS20.P3.Overcrowded;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] private FloatInstance masterVolume;
    [SerializeField] private FloatInstance musicVolume;
    [SerializeField] private FloatInstance ambienceVolume;
    [SerializeField] private FloatInstance sfxVolume;

    public void SetMasterVolume (float volume)
    {
        audioMixer.SetFloat("masterVolume", volume);
        masterVolume.Float = volume;
    }
    
    public void SetMusicVolume (float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        musicVolume.Float = volume;
    }
    
    public void SetAmbienceVolume (float volume)
    {
        audioMixer.SetFloat("ambienceVolume", volume);
        ambienceVolume.Float = volume;
    }
    
    public void SetSfxVolume (float volume)
    {
        audioMixer.SetFloat("sfxVolume", volume);
        sfxVolume.Float = volume;
    }
}
