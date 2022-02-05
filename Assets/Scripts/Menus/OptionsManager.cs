using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
	public Slider MasterVolume;
	public Slider MusicVolume;
	public Slider EffectsVolume;

    private void OnEnable()
    {
        //SoundDelegation.OnMasterVolumeChange += ChangeMasterVolume;
        //SoundDelegation.OnMusicVolumeChange += ChangeMusicVolume;
        //SoundDelegation.OnEffectsVolumeChange += ChangeEffectsVolume;

        MasterVolume.onValueChanged.AddListener((float a) => { SoundDelegation.ChangeMasterVolume(MasterVolume.value); });
        MusicVolume.onValueChanged.AddListener((float a) => { SoundDelegation.ChangeMusicVolume(MusicVolume.value); });
        EffectsVolume.onValueChanged.AddListener((float a) => { SoundDelegation.ChangeEffectsVolume(EffectsVolume.value); });
    }

    private void OnDisable()
    {
        //MasterVolume.onValueChanged

        //SoundDelegation.OnMasterVolumeChange -= ChangeMasterVolume;
        //SoundDelegation.OnMusicVolumeChange -= ChangeMusicVolume;
        //SoundDelegation.OnEffectsVolumeChange -= ChangeEffectsVolume;

        MasterVolume.onValueChanged.RemoveListener((float a) => { SoundDelegation.ChangeMasterVolume(MasterVolume.value); });
        MusicVolume.onValueChanged.RemoveListener((float a) => { SoundDelegation.ChangeMusicVolume(MusicVolume.value); });
        EffectsVolume.onValueChanged.RemoveListener((float a) => { SoundDelegation.ChangeEffectsVolume(EffectsVolume.value); });
    }

    private void Start()
    {
        MasterVolume.value = SoundDelegation.RawMasterVolume;
        MusicVolume.value = SoundDelegation.RawMusicVolume;
        EffectsVolume.value = SoundDelegation.RawEffectsVolume;
    }

    public void Activate(bool state)
    {
        gameObject.SetActive(state);
    }
}
