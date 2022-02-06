using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NovinaFollow : MonoBehaviour
{
	public Transform Objective;
	public float followRadius = 2;
	public float maxDistanceToObjective = 10;

	public bool isFollowing = true;

	//public float maxVelocity = 10;
	public float acceleration = 4;
	public float dodgeAcceleration = 0.5f;
	//public Vector2 gravityAcceleration = new Vector2(0, 1);

	public Transform GroundChecker;
	public float GCRadius;
	public LayerMask groundMask;

	private Rigidbody2D _rb;
	private Animator _animator;
	private CapsuleCollider2D _collider;

	private bool IsAvoiding;

	public bool IsMoving;
	private bool isGrounded;

	//Vector2 velocity;
	Vector2 DesiredPos = Vector2.zero;
	public Vector2 PhisicsMovement { get; private set; } = Vector2.zero;

	List<Vector2> ClosePointsList = new List<Vector2>();

	public bool CinematicMode;
	public Vector2 CinematicPosition;
	public float CinematicFacing;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();
		_collider = GetComponent<CapsuleCollider2D>();
		DesiredPos = Vector2.zero;
		PhisicsMovement = Vector2.zero;
		//velocity = Vector2.zero;
	}

	private void GetClosePoints()
	{
		Collider2D[] groundCollider = Physics2D.OverlapCircleAll(GroundChecker.position, GCRadius, groundMask);

		ClosePointsList.Clear();

		float closestDistance = followRadius;

		Vector2 movement = Vector2.zero;

		for (int i = 0; i < groundCollider.Length; i++)
		{
			Vector2 p = groundCollider[i].ClosestPoint(transform.position);

			_rb.velocity = Vector2.zero;

			if (Vector2.Distance(p, transform.position) < closestDistance)
			{
				closestDistance = Vector2.Distance(p, transform.position);
				ClosePointsList.Add(p);
			}
		}

		for (int i = ClosePointsList.Count - 1; i >= 0; i--)
		{
			if (Vector2.Distance(ClosePointsList[i], transform.position) > closestDistance)
			{
				ClosePointsList.RemoveAt(i);
			}
			else
			{
				movement += ((Vector2)transform.position - ClosePointsList[i]).normalized * GCRadius;
				//Debug.Log((Vector2)transform.position + ", " + ClosePointsList[i]);

				Debug.DrawLine((Vector2)transform.position, ClosePointsList[i], Color.magenta);
			}
				
		}

		if(movement != Vector2.zero)
		{
			PhisicsMovement = movement / ClosePointsList.Count;
		}
		else
		{
			PhisicsMovement = Vector2.zero;
		}

		//velocity += gravityAcceleration * Time.deltaTime;

		//friction = airFriction;

		IsAvoiding = groundCollider.Length > 0;
	}

	private void checkForGround()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, GCRadius, groundMask);

		isGrounded = hit;
	}

	private void followObjective()
	{
		if(!CinematicMode)
        {
			/////
			GetClosePoints();

			if (Vector2.Distance(Objective.position, transform.position) > followRadius)
			{
				DesiredPos = (Vector2)Objective.position + (Vector2)(transform.position - Objective.position).normalized * followRadius;
				IsMoving = Vector2.Distance(Objective.position, transform.position) > followRadius+0.2f;
			}
			else
			{
				DesiredPos = transform.position;
				IsMoving = false;
			}

			Vector2 tempDir = -new Vector2((transform.position.x - DesiredPos.x) * ( Utility.SignesAreEqual((transform.position.x - DesiredPos.x), PhisicsMovement.x) && Mathf.Abs(PhisicsMovement.x) > 0.5 ? 0 /*Mathf.Abs(1 - PhisicsMovement.x / GCRadius)*/ : 1),
				(transform.position.y - DesiredPos.y) * (Utility.SignesAreEqual((transform.position.y - DesiredPos.y), PhisicsMovement.y) && Mathf.Abs(PhisicsMovement.y) > 0.5 ? 0 /*Mathf.Abs(1 - PhisicsMovement.y / GCRadius)*/ : 1));

			DesiredPos = tempDir + (Vector2)transform.position + (PhisicsMovement * (dodgeAcceleration / acceleration));

			//Debug.Log(PhisicsMovement);

			_rb.MovePosition(Vector2.Lerp(transform.position, DesiredPos, acceleration * (DesiredPos - (Vector2)transform.position).magnitude * Time.deltaTime));

			if(Objective.position.x > transform.position.x)
			{
				GetComponent<TurnManager>().Turn(true);
			}
			else if (Objective.position.x < transform.position.x)
			{
				GetComponent<TurnManager>().Turn(false);
			}

			if (Vector2.Distance(Objective.position, transform.position) > maxDistanceToObjective)
			{
				_rb.velocity = Vector2.zero;
				_rb.MovePosition(Objective.position);
			}
			/////
        }
        else
        {
			_rb.MovePosition(Vector2.Lerp(transform.position, CinematicPosition, acceleration * Time.deltaTime));

			if(Utility.AproximatelyVector2(transform.position, CinematicPosition, 0.1f))
            {
				IsMoving = false;

			}
			else
            {
				IsMoving = true;
			}
			

			if (CinematicFacing > 0)
			{
				GetComponent<TurnManager>().Turn(true);
			}
			else if (CinematicFacing < 0)
			{
				GetComponent<TurnManager>().Turn(false);
			}
		}
		
	}

	/*private void followObjective()
	{
		//vel = acceleration * direction * Time
		///final.vel = this.vel * acceleration * (Obj.pos - This.pos) * Time
		
		GetClosePoints();

		Vector2 DesiredVel = Vector2.zero;

		Vector2 BreakDistanceVector = new Vector2(_rb.velocity.x == 0 ? 0 : Mathf.Pow(_rb.velocity.x, 2) / acceleration, _rb.velocity.y == 0 ? 0 : Mathf.Pow(_rb.velocity.y, 2) / acceleration);

		Vector2 vectorToObjective = Objective.position - transform.position;

		DesiredVel = new Vector2(BreakDistanceVector.x >= Mathf.Abs(vectorToObjective.x) ? 0 : vectorToObjective.normalized.x, BreakDistanceVector.y >= Mathf.Abs(vectorToObjective.y) ? 0 : vectorToObjective.normalized.y);

		//DesiredVel = vectorToObjective.normalized * maxVelocity;

		Debug.Log(BreakDistanceVector + " ; " + (BreakDistanceVector.x >= Mathf.Abs(vectorToObjective.x) ? 0 : vectorToObjective.normalized.x));

		_rb.velocity = Vector2.MoveTowards(_rb.velocity, DesiredVel * maxVelocity, acceleration * Time.deltaTime);

		//if (Vector2.Distance(Objective.position, transform.position) > maxDistanceToObjective)
		//{
		//	_rb.velocity = Vector2.zero;
		//	_rb.MovePosition(Objective.position);
		//}
	}*/

	private void FixedUpdate()
	{
		checkForGround();
		if (isGrounded)
		{
			_rb.velocity = (Vector2)transform.up * -_rb.gravityScale * (Vector2)Physics.gravity * Time.deltaTime;
		}

		if(isFollowing)
		{
			followObjective();
		}

		/*Vector2 AvoidDirection = -(tempDir / groundCollider.Length).normalized;

		movement = AvoidDirection;

		if( Vector2.Distance(Objective.position, transform.position) > followRadius)
		{
			movement += (Vector2)(Objective.position - transform.position).normalized;
		}

		Vector3 vel = _rb.velocity;

		//if (movement != Vector2.zero && (!Utility.SignesAreEqual(movement.x, _rb.velocity.x) || Mathf.Abs(speed) >= Mathf.Abs(_rb.velocity.x)))
		//{
			//Debug.Log(_rb.velocity + ", " + movement * speed + ", " + friction * acceleration * Time.deltaTime);
			vel = Vector3.MoveTowards(_rb.velocity, movement * speed, friction * acceleration * Time.deltaTime);

			//_facing = movement.x;
		//}

		

		_rb.velocity = vel;*/

		//_rb.velocity = AvoidDirection * speed;

		//isGrounded = groundCollider;

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = IsAvoiding ? Color.green : Color.red;
		Gizmos.DrawWireSphere(GroundChecker.position, GCRadius);
	}
}
