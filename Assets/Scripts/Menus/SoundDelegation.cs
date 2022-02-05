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

	public static void PlaySoundEffect(AudioClip clip)
	{
		OnPlaySoundEffect(clip);
	}

	public static void ChangeMasterVolume(float volume)
	{
		RawMasterVolume = volume;
		RecalculateSound();
		OnMusicVolumeChange(RealMusicVolume);
		OnEffectsVolumeChange(RealEffectsVolume);
	}

	public static void ChangeMusicVolume(float volume)
	{
		RawMusicVolume = volume;
		RecalculateSound();
		OnMusicVolumeChange(RealMusicVolume);
	}

	public static void ChangeEffectsVolume(float volume)
	{
		RawEffectsVolume = volume;
		RecalculateSound();
		OnEffectsVolumeChange(RealEffectsVolume);
	}

	private static void RecalculateSound()
	{
		RealMasterVolume = RawMasterVolume * RawMasterVolume;
		RealMusicVolume = (RawMusicVolume * RawMusicVolume) * RealMasterVolume;
		RealEffectsVolume = (RawEffectsVolume * RawEffectsVolume) * RealMasterVolume;

		//Debug.Log(RealMasterVolume + ", " + RealMusicVolume + ", " + RealEffectsVolume);
	}
}
