using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//20 * Mathf.Log10(x);

public static class SoundDelegation
{
	public static event Action<AudioClip> OnPlaySoundEffect = (AudioClip) => { };
	public static event Action<AudioClip> OnChangeMusic = (AudioClip) => { };
	public static event Action<float> OnMasterVolumeChange = (AudioClip) => { };
	public static event Action<float> OnMusicVolumeChange = (AudioClip) => { };
	public static event Action<float> OnEffectsVolumeChange = (AudioClip) => { };

	public static float RawMasterVolume { get; private set; } = 1;
	public static float RawMusicVolume { get; private set; } = 1;
	public static float RawEffectsVolume { get; private set; } = 1;

	public static float RealMasterVolume { get; private set; } = 1;
	public static float RealMusicVolume { get; private set; } = 1;
	public static float RealEffectsVolume { get; private set; } = 1;

	static SoundDelegation()
    {
		//Debug.Log("hello");
		if(PlayerPrefs.HasKey("MasterVolume"))
        {
			RawMasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
		else
        {
			PlayerPrefs.SetFloat("MasterVolume", RawMasterVolume);
		}

		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			RawMusicVolume = PlayerPrefs.GetFloat("MusicVolume");
		}
		else
		{
			PlayerPrefs.SetFloat("MusicVolume", RawMusicVolume);
		}

		if (PlayerPrefs.HasKey("EffectsVolume"))
		{
			RawEffectsVolume = PlayerPrefs.GetFloat("EffectsVolume");
		}
		else
		{
			PlayerPrefs.SetFloat("EffectsVolume", RawEffectsVolume);
		}

		RecalculateSound();
		
	}

	public static void PlaySoundEffect(AudioClip clip)
	{
		OnPlaySoundEffect(clip);
	}

	public static void ChangeMasterVolume(float volume)
	{
		RawMasterVolume = volume;
		PlayerPrefs.SetFloat("MasterVolume", RawMasterVolume);
		RecalculateSound();
	}

	public static void ChangeMusicVolume(float volume)
	{
		RawMusicVolume = volume;
		PlayerPrefs.SetFloat("MusicVolume", RawMusicVolume);
		RecalculateSound();
	}

	public static void ChangeEffectsVolume(float volume)
	{
		RawEffectsVolume = volume;
		PlayerPrefs.SetFloat("EffectsVolume", RawEffectsVolume);
		RecalculateSound();
	}

	private static void RecalculateSound()
	{
		RealMasterVolume = RawMasterVolume * RawMasterVolume;
		RealMusicVolume = (RawMusicVolume * RawMusicVolume) * RealMasterVolume;
		RealEffectsVolume = (RawEffectsVolume * RawEffectsVolume) * RealMasterVolume;

		OnMusicVolumeChange(RealMusicVolume);
		OnEffectsVolumeChange(RealEffectsVolume);

		//Debug.Log(RealMasterVolume + ", " + RealMusicVolume + ", " + RealEffectsVolume);
	}
}
