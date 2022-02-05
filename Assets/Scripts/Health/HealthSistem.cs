using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthSistem : MonoBehaviour
{
	public int health = 100;
	public int MaxHealth = 100;

	public UnityEvent OnDead;
	//[Serializable]public class OnLife : UnityEvent<int> {}

	public UnityEvent<float> OnLifeChange;
	public UnityEvent<float> OnMaxLifeChange;

	private void Start()
	{
		Hurt(0);
		//OnLifeChange.se
		OnMaxLifeChange.Invoke(MaxHealth);
	}

	public virtual void Hurt(int damage, int DodgeCost = 0)
	{
		health -= damage;

		if (health > MaxHealth)
		{
			health = MaxHealth;
		}
		else if (health < 0)
		{
			health = 0;
		}

		OnLifeChange.Invoke(health);


		if (health<=0)
		{
			OnDead.Invoke();
		}
	}
}
