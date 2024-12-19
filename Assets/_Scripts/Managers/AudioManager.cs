using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    public const string GROUP_MASTER = "Master";
    public const string GROUP_MUSIC = "Music";
    public const string GROUP_SFX = "SFXs";

    [Header("Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Pitch")]
    [SerializeField] private Vector2 _randomPitchRange = new(.95f, 1.15f);

    public void SetMusic(AudioClip clip, bool looping = true)
    {
        _musicSource.clip = clip;
        _musicSource.loop = looping;
    }

    public void PlayOneShot(AudioClip clip, float pitch)
    {
        _sfxSource.PlayOneShot(clip, pitch);
    }

    public void PlayOneShot(AudioClip clip)
    {
        PlayOneShot(clip, Random.Range(_randomPitchRange.x, _randomPitchRange.y));
    }

    public void SetMasterVolume(float t) => SetChannelVolume(GROUP_MASTER, t);

    public void SetMusicVolume(float t) => SetChannelVolume(GROUP_MUSIC, t);

    public void SetSFXVolume(float t) => SetChannelVolume(GROUP_SFX, t);

    private void SetChannelVolume(string channel, float t)
    {
        _audioMixer.SetFloat(channel, Mathf.Log10(t) * 20f);
    }
}
