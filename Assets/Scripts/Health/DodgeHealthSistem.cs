using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DodgeHealthSistem : HealthSistem
{
	public int MaxDodges = 3;
	public int dodges = 3;
	public float RechargeTime = 60;
	public float RechargePerSecond = 1;
	private float RechargeState = 0;


	public UnityEvent<int> OnDodge;
	public UnityEvent<int> OnMaxDodge;
	public UnityEvent<float> OnRechargeTimer;
	public UnityEvent<float> OnMaxRechargeTimer;

	private void Start()
	{
		OnMaxDodge.Invoke(MaxDodges);
		OnDodge.Invoke(dodges);
		OnMaxLifeChange.Invoke(MaxHealth);
		OnLifeChange.Invoke(health);
		OnMaxRechargeTimer.Invoke(RechargeTime);
		OnRechargeTimer.Invoke(RechargeState);
	}

    private void Update()
    {
        if(dodges < MaxDodges)
        {
			RechargeState += RechargePerSecond * Time.deltaTime;

			if(RechargeState >= RechargeTime)
            {
				dodges++;
				RechargeState = 0;

				OnDodge.Invoke(dodges);
			}

			OnRechargeTimer.Invoke(RechargeState);
		}
		else
        {
			RechargeState = 0;
			OnRechargeTimer.Invoke(RechargeState);
		}
    }

    public override void Hurt(int damage, int DodgeCost = 1)
	{
		//Debug.Log("A"+ DodgeCost);
		if(dodges > 0)
        {
			int dc = DodgeCost;
			dc -= dodges;
			dodges -= DodgeCost;

			if (dc > 0)
            {
				damage = damage / (DodgeCost / dc);
            }
			else
            {
				damage = 0;
			}

			OnDodge.Invoke(dodges);
		}

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


		if (health <= 0)
		{
			OnDead.Invoke();
		}
	}
}
