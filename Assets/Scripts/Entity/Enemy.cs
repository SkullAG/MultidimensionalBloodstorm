using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EntityControls
{
	public float DetectionDistance;

	bool CanJump;

	Transform objective;
	bool Alarmed = false;
	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();
		_collider = GetComponent<CapsuleCollider2D>();
		weaponManager = GetComponent<WeaponaryManager>();
		turnM = GetComponent<TurnManager>();
	}

	void FixedUpdate()
	{
		objective = FindObjectOfType<Player>().transform;

		Collider2D groundCollider = checkForGround();

		if (isGrounded)
			CanJump = true;

		float effectiveDistance = 0;
		SteepInfo _steepInfo = GetSteepNormals(groundCollider);
		foreach(WeaponableArm w in weaponManager.weaponableArm)
        {
			if (w.weapon)
            {
				float d = 0;
				if (w.weapon.inaccurancy > 0)
				{
					d = 1 / w.weapon.inaccurancy * 50;
				}
			
				if (effectiveDistance < d)
				{
					effectiveDistance = d;
				}
            }
			
        }

		if (Vector2.Distance(objective.position, transform.position) <= DetectionDistance)
			Alarmed = true;

		if (Alarmed && Vector2.Distance(objective.position, transform.position) >= effectiveDistance)
		{
			movement = new Vector2(objective.position.x - transform.position.x, 0).normalized;
		}
		else
		{
			movement = Vector2.zero;
		}

		RaycastHit2D hit = Physics2D.Raycast(GroundChecker.position, movement, speed * Time.deltaTime * (100 / jumpForce), groundMask);

		if (CanJump && (!isGrounded || (movement.x != 0 && hit)))
        {
			CanJump = false;
			_rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        }

		//Debug.Log((!CanJump && hit));

		if (Alarmed && Vector2.Distance(objective.position, transform.position) <= effectiveDistance)
        {
			//Debug.Log(weaponManager.weaponableArm[0].weapon.name);
			if (weaponManager.weaponableArm[0].weapon && Utility.AproximatelyVector2(weaponManager.weaponableArm[0].weapon.transform.right, LookingDirectionNormals, 0.1f))
            {
				weaponManager.Shoot(0);
            }
			if (weaponManager.weaponableArm[1].weapon && Utility.AproximatelyVector2(weaponManager.weaponableArm[1].weapon.transform.right, LookingDirectionNormals, 0.1f))
			{
				weaponManager.Shoot(1);
			}
		}
        else if(!(!CanJump && hit))
        {
			CalculateVelocity(_steepInfo);
        }
			

		LookingDirectionNormals = (objective.position - transform.position).normalized;

		turnM.Turn(LookingDirectionNormals.x > 0);

		_animator.SetInteger("XMotion", Mathf.FloorToInt(movement.x));
		_animator.SetInteger("XLooking", Mathf.FloorToInt(Utility.ExtractSign(LookingDirectionNormals.x)));
		_animator.SetBool("Grounded", isGrounded);
	}
}
