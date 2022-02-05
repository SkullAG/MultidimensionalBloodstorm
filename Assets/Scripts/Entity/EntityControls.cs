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

	private void Start()
    {
		_facing = 1;
    }

	private void OnDrawGizmos()
	{
		Gizmos.color = isGrounded ? Color.green : Color.red;
		Gizmos.DrawWireSphere(GroundChecker.position, GCRadius);
	}
}
