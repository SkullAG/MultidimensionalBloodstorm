using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class WeaponShoot : ShootingSystem
{
	public int maxAmmo;
	public float reloadTime;
	public float fireRate;
	public float inaccurancy;

	public Transform[] HandIKs;

	public Vector2 OffsetFromPivot = new Vector2(0.5f, 0);


	int actualAmmo;
	float actualReloadTime;
	float actualFireTime;

	public bool IsAutomatic = false;

	public bool isReloading { get; private set; }

	public float aimSpeed = 1f;
    internal Transform aimPivot;

	public AudioClip[] shootSounds;
	public AudioClip reloadSound;

	public Light2D flashLight;
	public float flashLightDuration;

	IEnumerator flash()
    {
		if(flashLight)
        {
			for(int i = 0; i < 2; i++)
			{
				if (i == 0) 
					flashLight.enabled = true;
				else
					flashLight.enabled = false;
				yield return new WaitForSeconds(flashLightDuration);
			}
        }
		
    }

    //AudioSource _audioSource;

    private void Start()
    {
		actualAmmo = maxAmmo;
		actualReloadTime = 0;
		actualFireTime = 0;

		if (flashLight)
			flashLight.enabled = false;

		//if (GetComponent<AudioSource>())
		//	_audioSource = GetComponent<AudioSource>();
		//else
		//	_audioSource = gameObject.AddComponent<AudioSource>();
	}
    public override void Shoot(int BulletLayer)
	{
		if(actualAmmo > 0 && actualFireTime <= 0 && !isReloading)
        {

			GameObject shot = PoolingManager.Instance.GetPooledObject(shootingData.projectile);

			if (shot != null)
			{
				SoundDelegation.PlaySoundEffect(shootSounds[UnityEngine.Random.Range(0, shootSounds.Length-1)]);

				shot.transform.position = shootPoint.position;
				shot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, shootPoint.rotation.eulerAngles.z + UnityEngine.Random.Range(inaccurancy, -inaccurancy)));
				shot.SetActive(true);
				shot.GetComponent<Rigidbody2D>().velocity = shot.transform.right * shootingData.fireForce;

				shot.layer = BulletLayer;

				StopCoroutine(flash());
				StartCoroutine(flash());
				//CameraEvents.ShakeAllCameras(shakeIntensity, shakespeed, shaketime);
				//WaitForFixedUpdate
				//if(shot.GetComponent<HighSpeedProjectile>())
				//{
				//	shot.GetComponent<HighSpeedProjectile>().calculateHit(position: shootPoint.position, velocity: shot.transform.right * shootingData.fireForce);
				//}

				actualFireTime = fireRate;
				actualAmmo--;
			}
		}
        else if(actualAmmo <= 0)
        {
			SoundDelegation.PlaySoundEffect(reloadSound);
			actualReloadTime = reloadTime;
			actualAmmo = maxAmmo;
		}
	}

    private void Update()
    {
        if(actualReloadTime > 0)
        {
			actualReloadTime -= Time.deltaTime;
			isReloading = true;
		}
        else
        {
			isReloading = false;
		}

		if (actualFireTime > 0)
		{
			actualFireTime -= Time.deltaTime;

		}
	}
}
