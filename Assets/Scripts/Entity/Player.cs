using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : EntityControls
{
	private PlayerInput _input;

	private bool touchingLeftWall = false;
	private bool touchingRightWall = false;
	[Range(0, 90)]
	public int wallJumpAngle = 45;
	public LayerMask wallMask;
	public float wallsFriction = 0;

	private InputAction moveAction;
	private InputAction lookAction;

	private ObjectGrab grabManager;

	bool IsThrowing = false;

	private void Awake()
	{
		_input = GetComponent<PlayerInput>();
		_rb = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();
		_collider = GetComponent<CapsuleCollider2D>();
		weaponManager = GetComponent<WeaponaryManager>();
		grabManager = GetComponent<ObjectGrab>();
		turnM = GetComponent<TurnManager>();

		moveAction = _input.actions.FindAction("Move");
		lookAction = _input.actions.FindAction("Look");
		if(PlayerPrefs.HasKey("Weapon1"))
        {
			GameObject variableForPrefab = Resources.Load(PlayerPrefs.GetString("Weapon1")) as GameObject;
			weaponManager.SetWeapon(GameObject.Instantiate(variableForPrefab.transform), 0);
        }

		if (PlayerPrefs.HasKey("Weapon2"))
		{
			GameObject variableForPrefab = Resources.Load(PlayerPrefs.GetString("Weapon2")) as GameObject;
			weaponManager.SetWeapon(GameObject.Instantiate(variableForPrefab.transform), 1);
		}
	}

	private bool ShootingRight = false;
	private bool ShootingLeft = false;
	public void Shoot(InputAction.CallbackContext context)
    {
		if(context.action.name == "Fire1")
        {
			if(context.started)
            {
				weaponManager.Shoot(0);
				ShootingLeft = true;
			}
			if (context.canceled)
			{
				ShootingLeft = false;
			}
		}
		else if (context.action.name == "Fire2")
		{
			if (context.started)
			{
				weaponManager.Shoot(1);
				ShootingRight = true;
			}
			if (context.canceled)
			{
				ShootingRight = false;
			}
		}
	}

	public void PrepareToThrow(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			IsThrowing = true;
		}
		else if (context.canceled)
		{
			IsThrowing = false;
		}
	}

	public void GrabWeapon(InputAction.CallbackContext context)
	{
		if (context.action.name == "Grab1")
		{
			if(context.started && IsThrowing)
            {
				weaponManager.ThrowWeapon(0);
			}

			if (context.started && grabManager.GrabWeapon())
			{
				weaponManager.SetWeapon(grabManager.GrabWeapon(), 0);
			}
		}
		else if (context.action.name == "Grab2")
		{
			if (context.started && IsThrowing)
			{
				weaponManager.ThrowWeapon(1);
			}

			if (context.started && grabManager.GrabWeapon())
			{
				weaponManager.SetWeapon(grabManager.GrabWeapon(), 1);
			}
		}
	}

	public void ActivateHability(InputAction.CallbackContext context)
	{
		GetComponent<HabilitySistem>().Activate(context);
	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (context.started)
        {
			if (isGrounded)
			{
				_rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
			}
			else if (touchingRightWall)
			{
				_rb.velocity = new Vector2(-Mathf.Cos(Mathf.Deg2Rad * wallJumpAngle), Mathf.Sin(Mathf.Deg2Rad * wallJumpAngle) * wallsFriction) * jumpForce;
			}
			else if (touchingLeftWall)
			{
				_rb.velocity = new Vector2(Mathf.Cos(Mathf.Deg2Rad * wallJumpAngle), Mathf.Sin(Mathf.Deg2Rad * wallJumpAngle) * wallsFriction) * jumpForce;
			}
        }

	}

	public void LaunchGranade(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Vector2 looking = lookAction.ReadValue<Vector2>();

			GameObject g = Instantiate<GameObject>(granadePrefab);
			g.transform.position = transform.position;
			g.GetComponent<Rigidbody2D>().AddForce(LookingDirectionNormals * granadeLaunchForce);
		}
	}

	void FixedUpdate()
	{
		Collider2D groundCollider = checkForGround();

		movement = Utility.RoundV2(moveAction.ReadValue<Vector2>());
		Vector2 looking = lookAction.ReadValue<Vector2>();

		SteepInfo _steepInfo = GetSteepNormals(groundCollider);

		checkWalls();

		//Special velocity calculations;

		Vector3 vel = _rb.velocity;

		if (movement.x != 0 && (!Utility.SignesAreEqual(movement.x, _rb.velocity.x) || Mathf.Abs(speed) >= Mathf.Abs(_rb.velocity.x)) && !((movement.x < 0 && touchingLeftWall) || (movement.x > 0 && touchingRightWall)))
		{
			vel = Vector3.MoveTowards(_rb.velocity, new Vector2(movement.x * speed * Mathf.Abs(_steepInfo.steepNormals.y), Utility.MaxAbs(_rb.velocity.y, movement.x * speed * (Mathf.Abs(_steepInfo.steepNormals.x) * _steepInfo.steepDirection))), friction * acceleration * Time.deltaTime);

			_facing = movement.x;
		}

		_rb.velocity = vel;

		//end of velocity calculations;

		Vector2 mousePosition = _input.currentControlScheme != "Keyboard&Mouse" ? looking : ((Vector2)Camera.main.ScreenToWorldPoint(looking) - (Vector2)transform.position);
		LookingDirectionNormals = mousePosition.normalized;

		turnM.Turn(LookingDirectionNormals.x > 0);

		_animator.SetInteger("XMotion", Mathf.FloorToInt(movement.x));
		_animator.SetInteger("XLooking", Mathf.FloorToInt(Utility.ExtractSign(LookingDirectionNormals.x)));
		_animator.SetBool("Grounded", isGrounded);
		_animator.SetBool("WallLeft", touchingLeftWall);
		_animator.SetBool("WallRight", touchingRightWall);
	}

    private void Update()
    {
		if (ShootingLeft && weaponManager.weaponableArm[0].weapon && weaponManager.weaponableArm[0].weapon.IsAutomatic)
		{
			weaponManager.Shoot(0);
		}
		if (ShootingRight && weaponManager.weaponableArm[1].weapon && weaponManager.weaponableArm[1].weapon.IsAutomatic)
		{
			weaponManager.Shoot(1);
		}
	}

    private void checkWalls()
	{
		Vector2 posOff = (Vector2)transform.position + _collider.offset;
		Vector2 rayOriginRight = new Vector2(posOff.x + (_collider.size.x / 2 + 0.03f), posOff.y - (_collider.size.y / 2 - _collider.size.x / 2));
		float heightRight = (posOff.y + (_collider.size.y / 2 - _collider.size.x / 2)) - rayOriginRight.y;
		RaycastHit2D hitRight = Physics2D.Raycast(rayOriginRight, transform.up, heightRight, wallMask);

		

		if(hitRight)
		{
			Debug.DrawLine(rayOriginRight, hitRight.point, Color.yellow);
			touchingRightWall = true;
			//Debug.Log(hitRight.normal);
		}

		else
		{
			Debug.DrawLine(rayOriginRight, rayOriginRight + (Vector2)transform.up * heightRight, Color.yellow);
			touchingRightWall = false;
		}

		Vector2 rayOriginLeft = new Vector2(posOff.x - (_collider.size.x / 2 + 0.03f), posOff.y - (_collider.size.y / 2 - _collider.size.x / 2));
		float heightLeft = (posOff.y + (_collider.size.y / 2 - _collider.size.x / 2)) - rayOriginLeft.y;
		RaycastHit2D hitLeft = Physics2D.Raycast(rayOriginLeft, transform.up, heightLeft, wallMask);

		if (hitLeft)
		{
			Debug.DrawLine(rayOriginLeft, hitLeft.point, Color.yellow);
			touchingLeftWall = true;
		}
		else
		{
			Debug.DrawLine(rayOriginLeft, rayOriginLeft + (Vector2)transform.up * heightLeft, Color.yellow);
			touchingLeftWall = false;
		}

		wallsFriction = Mathf.Max(hitRight ? hitRight.collider.friction : 0, hitLeft ? hitLeft.collider.friction : 0);
	}
}
