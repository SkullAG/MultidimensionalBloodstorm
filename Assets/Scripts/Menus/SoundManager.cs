using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioSource EffectsSource;

    private void OnEnable()
    {
        SoundDelegation.OnChangeMusic += ChangeMusic;
        SoundDelegation.OnPlaySoundEffect += PlaySound;
        SoundDelegation.OnMusicVolumeChange += ChangeMusicVolume;
        SoundDelegation.OnEffectsVolumeChange += ChangeEffectsVolume;

    }
    private void OnDisable()
    {
        SoundDelegation.OnChangeMusic -= ChangeMusic;
        SoundDelegation.OnPlaySoundEffect -= PlaySound;
        SoundDelegation.OnMusicVolumeChange -= ChangeMusicVolume;
        SoundDelegation.OnEffectsVolumeChange -= ChangeEffectsVolume;
    }

    private void Start()
    {
        MusicSource.volume = SoundDelegation.RealMusicVolume;
        EffectsSource.volume = SoundDelegation.RealEffectsVolume;
    }

    public void PlaySound(AudioClip clip)
    {
        if(clip)
            EffectsSource.PlayOneShot(clip);
    }

    public void ChangeMusic(AudioClip clip)
    {
        MusicSource.Stop();
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    private void ChangeMusicVolume(float vol)
    {
        MusicSource.volume = vol;
    }

    private void ChangeEffectsVolume(float vol)
    {
        EffectsSource.volume = vol;
    }
}
