using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityControls : MonoBehaviour
{
	public float speed;
	public float jumpForce;
	public float acceleration = 0.5f;
	public float airFriction = 0.1f;

	[Range(0, 100)]
	public float SteepForceLoss = 10;

	public Transform GroundChecker;
	public float GCRadius;
	public LayerMask groundMask;

	public WeaponaryManager weaponManager;

	public bool IsShooting = false;

	public float gunDistanceToCenter;

	public float granadeLaunchForce;
	public GameObject granadePrefab;

	protected Rigidbody2D _rb;
	protected Animator _animator;
	protected CapsuleCollider2D _collider;

	[HideInInspector] public float _facing = 1;

	public bool isGrounded;

	protected float friction = 0;

	[HideInInspector] public Vector2 LookingDirectionNormals;

	[HideInInspector] public Vector2 movement;

	protected TurnManager turnM;

	private void Start()
	{
		_facing = 1;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = isGrounded ? Color.green : Color.red;
		Gizmos.DrawWireSphere(GroundChecker.position, GCRadius);
	}

	protected Collider2D checkForGround()
	{
		Collider2D groundCollider = Physics2D.OverlapCircle(GroundChecker.position, GCRadius, groundMask);
		isGrounded = groundCollider;
		return groundCollider;
	}

	protected struct SteepInfo
	{
		public Vector2 steepNormals;

		public float steepDirection;

	}

	protected SteepInfo GetSteepNormals(Collider2D groundCollider)
	{
		SteepInfo si = new SteepInfo();

		RaycastHit2D hit = Physics2D.Raycast(GroundChecker.position, new Vector2(_facing / 2, -1), GCRadius, groundMask);

		si.steepDirection = 0;

		if (groundCollider)
		{
			Vector2 point = groundCollider.ClosestPoint(GroundChecker.position);
			friction = Mathf.Max(groundCollider.friction, airFriction);

			if (hit)
			{
				si.steepNormals = hit.normal;
				si.steepDirection = _facing;
				Debug.DrawLine(GroundChecker.position, hit.point, Color.yellow);
			}

			else
			{
				si.steepNormals = new Vector2(point.x - GroundChecker.position.x, point.y - GroundChecker.position.y).normalized;
				si.steepDirection = Utility.ExtractSign(si.steepNormals.x);
				Debug.DrawLine(GroundChecker.position, point, Color.yellow);
			}

		}
		else
		{
			friction = airFriction;
			si.steepNormals = Vector2.up;
		}

		if (si.steepNormals.x > 0)
		{
			si.steepNormals = new Vector2(si.steepNormals.x * (1 - SteepForceLoss / 100), si.steepNormals.y);
		}

		return si;
	}

    protected void CalculateVelocity(SteepInfo si)
    {
		Vector3 vel = _rb.velocity;

		if (movement.x != 0 && (!Utility.SignesAreEqual(movement.x, _rb.velocity.x) || Mathf.Abs(speed) >= Mathf.Abs(_rb.velocity.x)))
		{
			vel = Vector3.MoveTowards(_rb.velocity, new Vector2(movement.x * speed * Mathf.Abs(si.steepNormals.y), Utility.MaxAbs(_rb.velocity.y, movement.x * speed * (Mathf.Abs(si.steepNormals.x) * si.steepDirection))), friction * acceleration * Time.deltaTime);

			_facing = movement.x;
		}

		_rb.velocity = vel;
	}
}
