using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.IK;

[System.Serializable]
public struct WeaponableArm
{
	public Solver2D ArmLimbSolver;

	[HideInInspector]
	public Transform defaultIK;

	//[HideInInspector]
	public Vector2 AimNormals;

	public WeaponShoot weapon;

	public Transform AimPivot;
}

public class WeaponaryManager : MonoBehaviour
{

	public WeaponableArm[] weaponableArm;
	//public Transform AimPivot;
	//private Vector2 AimNormals = Vector2.right;
	//public float aimSpeed;

	//public Vector2 offsetBetweenArms = new Vector2(-0.5f, -0.1f);

	public float weaponLaunchSpeed = 4;

	private EntityControls WeaponUser;

	void Start()
	{
		WeaponUser = GetComponent<EntityControls>();

		//weapons = new WeaponShoot[ArmLimbSolvers.Length];
		//defaultIKs = new Transform[ArmLimbSolvers.Length];

		for(int i = 0; i < weaponableArm.Length; i++)
		{
			weaponableArm[i].defaultIK = weaponableArm[i].ArmLimbSolver.GetChain(0).target;
			weaponableArm[i].AimNormals = Vector2.right;
		}
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < weaponableArm.Length; i++)
		{
			if (weaponableArm[i].weapon)
			{
				weaponableArm[i].AimNormals = Vector2.Lerp(weaponableArm[i].AimNormals, WeaponUser.LookingDirectionNormals, (weaponableArm[i].weapon ? weaponableArm[i].weapon.aimSpeed : 1) * Time.deltaTime).normalized;

				weaponableArm[i].AimPivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(weaponableArm[i].AimNormals.y, weaponableArm[i].AimNormals.x) * Mathf.Rad2Deg);

				weaponableArm[i].AimPivot.localScale = new Vector2(1, Utility.ExtractSign(weaponableArm[i].AimPivot.right.x));
			}

			if (WeaponUser.LookingDirectionNormals.x > 0)
			{
				if (weaponableArm[i].weapon)
				{
					//weaponableArm[i].weapon.transform.localPosition = weapons[arm].OffsetFromPivot + offsetBetweenArms * arm;
					if (weaponableArm[i].weapon.HandIKs.Length == 1)
					{
						weaponableArm[i].ArmLimbSolver.GetChain(0).target = weaponableArm[i].weapon.HandIKs[0];
					}
					else
					{
						weaponableArm[i].ArmLimbSolver.GetChain(0).target = weaponableArm[i].weapon.HandIKs[i];
					}

					weaponableArm[i].ArmLimbSolver.GetChain(0).effector.localScale = Vector2.one;
				}
				else
                {
					weaponableArm[i].ArmLimbSolver.GetChain(0).target = weaponableArm[i].defaultIK;

					weaponableArm[i].ArmLimbSolver.GetChain(0).effector.localScale = Vector2.one;

					
                }
			}
			else
			{
				if (weaponableArm[weaponableArm.Length - 1 - i].weapon)
				{
					//weaponableArm[i].weapon.transform.localPosition = weapons[arm].OffsetFromPivot + offsetBetweenArms * (weapons.Length - 1 - arm);
					if(weaponableArm[i].weapon && weaponableArm[i].weapon.HandIKs.Length == 1 )
                    {
						weaponableArm[i].ArmLimbSolver.GetChain(0).target = weaponableArm[weaponableArm.Length - 1 - i].weapon.HandIKs[0];
                    }
                    else if(weaponableArm[weaponableArm.Length - 1 - i].weapon.HandIKs.Length > 1)
					{
						weaponableArm[i].ArmLimbSolver.GetChain(0).target = weaponableArm[weaponableArm.Length - 1 - i].weapon.HandIKs[weaponableArm.Length - 1 - i];
					}
                    else
                    {
						weaponableArm[i].ArmLimbSolver.GetChain(0).target = weaponableArm[weaponableArm.Length - 1 - i].weapon.HandIKs[0];
					}

					weaponableArm[i].ArmLimbSolver.GetChain(0).effector.localScale = -Vector2.one;
				}
				else
				{
					weaponableArm[i].ArmLimbSolver.GetChain(0).target = weaponableArm[i].defaultIK;

					weaponableArm[i].ArmLimbSolver.GetChain(0).effector.localScale = Vector2.one;
				}
				
			}
		}
	}

	public void SetWeapon(Transform weapon, int arm)
	{
		weapon.GetComponent<Rigidbody2D>().simulated = false;
		weapon.GetComponent<Collider2D>().enabled = false;

		weapon.parent = weaponableArm[arm].AimPivot;

		weapon.right = weaponableArm[arm].AimPivot.right;

		if(weaponableArm[arm].weapon)
		{
			weaponableArm[arm].weapon.transform.parent = null;

			weaponableArm[arm].weapon.GetComponent<Rigidbody2D>().simulated = true;
			weaponableArm[arm].weapon.GetComponent<Collider2D>().enabled = true;
			weaponableArm[arm].weapon.GetComponent<Rigidbody2D>().velocity = weaponableArm[arm].AimPivot.right * weaponLaunchSpeed;

			if (weaponableArm[arm].weapon.HandIKs.Length > 1)
            {
				weaponableArm[weaponableArm.Length - 1 - arm].weapon = null;

				weaponableArm[weaponableArm.Length - 1 - arm].ArmLimbSolver.GetChain(0).target = weaponableArm[weaponableArm.Length - 1 - arm].defaultIK;

				weaponableArm[weaponableArm.Length - 1 - arm].ArmLimbSolver.GetChain(0).effector.localScale = Vector2.one;
			}
		}

		weaponableArm[arm].weapon = weapon.GetComponent<WeaponShoot>();

		weaponableArm[arm].weapon.transform.localPosition = weaponableArm[arm].weapon.OffsetFromPivot;

		if(weaponableArm[arm].weapon.HandIKs.Length > 1)
        {
			if (weaponableArm[weaponableArm.Length - 1 - arm].weapon)
			{
				weaponableArm[weaponableArm.Length - 1 - arm].weapon.transform.parent = null;

				weaponableArm[weaponableArm.Length - 1 - arm].weapon.GetComponent<Rigidbody2D>().simulated = true;
				weaponableArm[weaponableArm.Length - 1 - arm].weapon.GetComponent<Collider2D>().enabled = true;
				weaponableArm[weaponableArm.Length - 1 - arm].weapon.GetComponent<Rigidbody2D>().velocity = weaponableArm[weaponableArm.Length - 1 - arm].AimPivot.right * weaponLaunchSpeed;
			}

			weaponableArm[weaponableArm.Length - 1 - arm].weapon = weapon.GetComponent<WeaponShoot>();

			weaponableArm[weaponableArm.Length - 1 - arm].weapon.transform.localPosition = weaponableArm[weaponableArm.Length - 1 - arm].weapon.OffsetFromPivot;
		}

		/*if (WeaponUser.LookingDirectionNormals.x > 0)
		{
			weaponableArm[arm].ArmLimbSolver.GetChain(0).target = weaponableArm[arm].weapon.HandIKs[0];

			//weaponableArm[arm].weapon.HandIKs[0].localScale = new Vector2(1, 1);

		}
		else
		{
			weaponableArm[weaponableArm.Length - 1 - arm].ArmLimbSolver.GetChain(0).target = weaponableArm[arm].weapon.HandIKs[0];

			//weaponableArm[weaponableArm.Length - 1 - arm].weapon.HandIKs[0].localScale = new Vector2(-1, 1);
		}*/
		
		weapon.localScale = Vector2.one;

		//weaponableArm[arm].ArmLimbSolver.GetChain(0).target = weaponableArm[arm].weapon.HandIKs[0];

		//return gameObject;
	}

	public void Shoot(int index)
	{
		if(weaponableArm[index].weapon)
			weaponableArm[index].weapon.Shoot();
		//weaponManager.GetComponent<WeaponaryManager>().Shoot();
	}
	//public void Shoot(int index)
	//{
	//    weapons[index].Shoot();
	//}
}
