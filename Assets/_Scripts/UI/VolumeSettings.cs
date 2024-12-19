using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Awake()
    {
        var audioManager = AudioManager.Instance;
        InitSlider(_musicSlider, audioManager.SetMusicVolume, AudioManager.GROUP_MUSIC);
        InitSlider(_sfxSlider, audioManager.SetSFXVolume, AudioManager.GROUP_SFX);

        static void InitSlider(Slider slider, UnityAction<float> callback, string channel)
        {
            slider.onValueChanged.AddListener(callback);
            slider.value = PlayerPrefs.GetFloat(channel, .5f);
            callback(slider.value);
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat(AudioManager.GROUP_MUSIC, _musicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.GROUP_SFX, _sfxSlider.value);
        PlayerPrefs.Save();
    }
}
