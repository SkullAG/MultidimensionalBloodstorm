using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GraplingHook : ShootingSystem
{
	public LayerMask layerMask;
	public float minDistanceBetweenPoints = 0.1f;
	public float pullVelocity = 8;
	public float retractVelocity = 24;
	public float maxRopeDistance = 40;

	public bool bending = true;
	public LayerMask bendingLayerMask;
	public int bendQuality = 2;

	public Transform hook { get; private set; }
	Transform attatchedBody;
	LineRenderer rope;
	List<Vector3> ropePoints = new List<Vector3>();
	public bool isHooked { get; private set; }
	public bool isReturning { get; private set; } = false;

	public int layerWhenAttatched;
	SpriteRenderer sprite;
	Collider2D[] colPoints = new Collider2D[1];
	Rigidbody2D hookRb;

	DistanceJoint2D hookJoint;
	DistanceJoint2D attachedHookJoint;
	DistanceJoint2D launcherJoint;

	float actualRopeDistance;
	float hookRopeDistance;
	float launcherRopeDistance;

	int attatchedBodySavedLayer = 0;

	EntityControls UserEntity;

	private void Start()
	{
		sprite = GetComponent<SpriteRenderer>();
		rope = GetComponent<LineRenderer>();
		UserEntity = GetComponentInParent<EntityControls>();
	}

	//public 

	public override void Shoot()
	{
		if (!hook)
		{
			GameObject h = PoolingManager.Instance.GetPooledObject(shootingData.projectile);

			if(h)
			{
				hook = h.transform;
				hook.position = shootPoint.position;
					
				hook.gameObject.SetActive(true);
				hook.GetComponent<Rigidbody2D>().velocity = transform.right * shootingData.fireForce;

				hook.up = transform.right;

				ropePoints.Add(hook.position);
				ropePoints.Add(transform.position);
				sprite.enabled = false;
				rope.enabled = true;

				hookRb = hook.GetComponent<Rigidbody2D>();

				hookRb.WakeUp();

				hookJoint = hook.gameObject.AddComponent<DistanceJoint2D>();
				hookJoint.enableCollision = true;
				hookJoint.maxDistanceOnly = true;
				hookJoint.autoConfigureDistance = false;
				//hookJoint.connectedBody = handRb;
				hookJoint.connectedAnchor = ropePoints[1];
				//hookJoint.distance = maxRopeDistance;
				hookRopeDistance = maxRopeDistance;

				launcherJoint = GetComponentInParent<Rigidbody2D>().gameObject.AddComponent<DistanceJoint2D>();
				launcherJoint.enableCollision = true;
				launcherJoint.maxDistanceOnly = true;
				launcherJoint.autoConfigureDistance = false;
				//hookJoint.connectedBody = handRb;
				launcherJoint.connectedAnchor = ropePoints[ropePoints.Count - 2];
				//launcherJoint.distance = maxRopeDistance + 0.1f;
				launcherRopeDistance = maxRopeDistance + 0.1f;

				actualRopeDistance = maxRopeDistance;

				isReturning = false;
			}
		}
		else
		{
			if (attachedHookJoint)
			{
				Destroy(attachedHookJoint);
			}
			hookRb.simulated = false;
			hook.SetParent(null);
			if(attatchedBody)
            {
				attatchedBody.gameObject.layer = attatchedBodySavedLayer;
				attatchedBody = null;
				attatchedBodySavedLayer = 0;
			}
			launcherJoint.enabled = false;
			isReturning = true;
		}
	}

	public void ReturnHook()
	{
		hookRb.velocity = Vector3.zero;
		hook.position = Vector3.MoveTowards(hook.position, ropePoints[1], retractVelocity * Time.deltaTime);
		//actualRopeDistance = Vector2.Distance(ropePoints[1], hook.position);
		//hookRb.velocity = (ropePoints[1] - hook.position).normalized * pullVelocity;
		if (Utility.AproximatelyVector2(hook.position, ropePoints[1], minDistanceBetweenPoints))
		{
			if(ropePoints.Count > 2)
			{
				ropePoints.RemoveAt(1);
			}
			else
			{
				ResetHook();
			}
		}
	}

	public void ResetHook()
	{
		hookRb.simulated = true;
		//hookRb.bodyType = RigidbodyType2D.Dynamic;
		hookRb = null;
		hook.SetParent(null);
		hook.gameObject.SetActive(false);
		hook = null;
		attatchedBody = null;
		colPoints[0] = null;

		ropePoints.Clear();
		isHooked = false;

		sprite.enabled = true;
		rope.enabled = false;

		//isReturning = false;

		Destroy(hookJoint);
		Destroy(launcherJoint);
	}

	private void FixedUpdate()
	{
		if (hook)
		{
			UpdateHook();
		}
	}

	private void Update()
	{
		if (hook)
		{
			UpdateRope();
		}
	}

	private void UpdateHook()
	{
		launcherJoint.enabled = isHooked;
		//Debug.Log(hookJoint.enabled);

		actualRopeDistance += UserEntity.movement.y * pullVelocity * Time.deltaTime;
		if (actualRopeDistance > maxRopeDistance)
		{
			actualRopeDistance = maxRopeDistance;

		}
		else if (actualRopeDistance < 0)
		{
			actualRopeDistance = 0;

		}


		if (!isHooked)
		{
			int i = hook.GetComponent<Collider2D>().GetContacts(colPoints);

			isHooked = i > 0;

			hook.up = hookRb.velocity;

			actualRopeDistance = Vector2.Distance(hook.position, transform.position) + hookRb.velocity.magnitude * Time.deltaTime * 2;
			if (actualRopeDistance > maxRopeDistance)
			{
				actualRopeDistance = maxRopeDistance;

			}

			if (colPoints[0])
			{
				attatchedBody = colPoints[0].transform;


				attatchedBodySavedLayer = attatchedBody.gameObject.layer;
				if (attatchedBody.GetComponent<Rigidbody2D>())
				{
					attatchedBody.gameObject.layer = layerWhenAttatched;
				}



				if (attatchedBody.GetComponent<Rigidbody2D>())
				{
					//attatchedBody.
					attachedHookJoint = attatchedBody.gameObject.AddComponent<DistanceJoint2D>();
					attachedHookJoint.enableCollision = true;
					attachedHookJoint.maxDistanceOnly = true;
					attachedHookJoint.autoConfigureDistance = false;
					attachedHookJoint.anchor = attatchedBody.InverseTransformPoint(hook.position);
					attachedHookJoint.connectedAnchor = ropePoints[1];
					attachedHookJoint.distance = maxRopeDistance;
				}

				//hookRb.bodyType = RigidbodyType2D.Static;

				hook.SetParent(attatchedBody);
				hookRb.simulated = false;

			}
		}
		else
		{
			//launcherJoint.enabled = false;
			if (attachedHookJoint)
				hook.localPosition = attachedHookJoint.anchor;

			if (!attatchedBody || !attatchedBody.gameObject.activeInHierarchy)
			{
				if (attachedHookJoint)
				{
					Destroy(attachedHookJoint);
				}
				hookRb.simulated = true;
				hook.SetParent(null);
				attatchedBody = null;
				attatchedBodySavedLayer = 0;
				colPoints[0] = null;
				isHooked = false;
			}
		}

		ropePoints[0] = hook.position;
		ropePoints[ropePoints.Count - 1] = transform.position;
		hookJoint.connectedAnchor = ropePoints[1];
		launcherJoint.connectedAnchor = ropePoints[ropePoints.Count - 2];
		if (attachedHookJoint)
		{
			attachedHookJoint.connectedAnchor = ropePoints[1];
		}

		calculateDistance();

		if (isReturning)
		{
			ReturnHook();
		}
	}
	private void UpdateRope()
	{
		if (!isReturning)
		{
			calculatePoints();
		}

		if (bending && !isReturning)
		{
			BendRope();
		}
		else
		{
			rope.positionCount = ropePoints.Count;
			rope.SetPositions(ropePoints.ToArray());
		}
	}

    void calculateDistance()
	{
		float calculatedDistance = 0;
		if(ropePoints.Count > 2)
		{
			for(int i = 1; i < ropePoints.Count-2; i++)
			{
				calculatedDistance += (ropePoints[i] - ropePoints[i + 1]).magnitude;

			}

			calculatedDistance = actualRopeDistance - calculatedDistance;

			//here goes a right calculation, not as i did =>
			//hookRopeDistance = calculatedDistance / 2;
			//launcherRopeDistance = calculatedDistance  /2 + 0.1f;


		}
		else
		{
			hookRopeDistance = actualRopeDistance;
			launcherRopeDistance = actualRopeDistance + 0.1f;
		}

		hookJoint.distance = hookRopeDistance;
		launcherJoint.distance = launcherRopeDistance;
		if (attachedHookJoint)
		{
			attachedHookJoint.distance = hookRopeDistance;
		}
	}

	void calculatePoints()
	{
		float distance;

		if (ropePoints.Count > 2)
		{
			distance = Vector2.Distance(hook.position, ropePoints[1]);

			RaycastHit2D HookToSecondPoint = Physics2D.Raycast(hook.position, ropePoints[1] - hook.position, distance, layerMask);

			if (HookToSecondPoint && !Utility.Aproximately(distance, HookToSecondPoint.distance, 0.01f) && !Utility.AproximatelyVector2(HookToSecondPoint.point, ropePoints[1], minDistanceBetweenPoints))
			{
				//ropePoints[ropePoints.Count - 1] = HookToSecondPoint.point;
				ropePoints.Insert(1, HookToSecondPoint.collider.ClosestPoint(HookToSecondPoint.point));
			}

			distance = Vector2.Distance(transform.position, ropePoints[ropePoints.Count - 3]);

			RaycastHit2D UserToPreLastPointHit = Physics2D.Raycast(transform.position, ropePoints[ropePoints.Count - 3] - transform.position, distance, layerMask);

			if (!UserToPreLastPointHit || Utility.Aproximately(distance, UserToPreLastPointHit.distance, minDistanceBetweenPoints))
			{
				ropePoints.RemoveAt(ropePoints.Count - 2);
			}

			if (ropePoints.Count > 2)
			{
				distance = Vector2.Distance(hook.position, ropePoints[2]);

				RaycastHit2D HookToThirdPoint = Physics2D.Raycast(hook.position, ropePoints[2] - hook.position, distance, layerMask);

				if (!HookToThirdPoint || Utility.Aproximately(distance, HookToThirdPoint.distance, minDistanceBetweenPoints))
				{
					//ropePoints[ropePoints.Count - 1] = HookToSecondPoint.point;
					ropePoints.RemoveAt(1);
				}
			}
		}

		distance = Vector2.Distance(transform.position, ropePoints[ropePoints.Count - 2]);

		RaycastHit2D UserToLastPointHit = Physics2D.Raycast(transform.position, ropePoints[ropePoints.Count - 2] - transform.position, distance, layerMask);

		if (UserToLastPointHit && !Utility.Aproximately(distance, UserToLastPointHit.distance, 0.01f) && !Utility.AproximatelyVector2(UserToLastPointHit.point, ropePoints[ropePoints.Count - 2], minDistanceBetweenPoints))
		{
			ropePoints.Insert(ropePoints.Count - 1, UserToLastPointHit.collider.ClosestPoint(UserToLastPointHit.point));
		}
	}

	void BendRope()
	{
		List<Vector3> finalList = new List<Vector3>(ropePoints);

		float launcherDistance = Vector2.Distance(ropePoints[ropePoints.Count - 1], ropePoints[ropePoints.Count - 2]);
		float launcherExcedentDistance = launcherRopeDistance - launcherDistance;
		float bendPoints = Utility.Round(Mathf.Min(launcherDistance, launcherExcedentDistance) * bendQuality);
		Vector2 ropeDirection = (ropePoints[ropePoints.Count - 1] - ropePoints[ropePoints.Count - 2]).normalized;

		RaycastHit2D hit;

		for (int i = 0; i < bendPoints; i++)
		{
			float pointPosInTheLine = launcherDistance / bendPoints * i;
			Vector3 pointPosition = (Vector2)ropePoints[ropePoints.Count - 2] + pointPosInTheLine * ropeDirection;

			//float hangDistande = Mathf.Min(pointPosInTheLine, launcherDistance - pointPosInTheLine) / (launcherDistance / 2) * (launcherExcedentDistance/2);
			float hangDistance = launcherExcedentDistance * Mathf.Pow((Mathf.Min(pointPosInTheLine, launcherDistance - pointPosInTheLine) / (launcherDistance / 2)) - 1, 2) + -launcherExcedentDistance;

			hit = Physics2D.Raycast(pointPosition, Vector2.down, -hangDistance, bendingLayerMask);
			
			if(hit)
			{
				pointPosition = hit.point;
				Debug.DrawLine(pointPosition, hit.point, Color.white);
			}
			else
			{
				
				Debug.DrawLine(pointPosition, new Vector2(pointPosition.x, pointPosition.y + hangDistance), Color.white);
				pointPosition = new Vector2(pointPosition.x, pointPosition.y + hangDistance);
			}
			finalList.Insert(finalList.Count - 1, pointPosition);
		}

		rope.positionCount = finalList.Count;
		rope.SetPositions(finalList.ToArray());
	}
}
