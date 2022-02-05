using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostHandHability : HabilitySistem
{
	public GameObject GhostHandPrefab;
	public Vector2 PositionFromUser;
	public float handAcceleration = 12;
	public float standRange = 1;

	public float facing;

	GameObject hand;
	GraplingHook hook;
	Rigidbody2D handRb;
	EntityControls HabilityUser;
	DistanceJoint2D _joint;


	private void Start()
	{
		hand = Instantiate(GhostHandPrefab, (Vector2)transform.position + PositionFromUser, Quaternion.identity);
		handRb = hand.GetComponent<Rigidbody2D>();
		HabilityUser = GetComponent<EntityControls>();
		hook = hand.GetComponentInChildren<GraplingHook>();

		hand.transform.SetParent(HabilityUser.transform);

		handRb.mass = HabilityUser.GetComponent<Rigidbody2D>().mass;

		_joint = HabilityUser.gameObject.AddComponent<DistanceJoint2D>();
		_joint.enableCollision = true;
		_joint.maxDistanceOnly = true;
		_joint.autoConfigureDistance = false;
		_joint.connectedBody = handRb;
		_joint.distance = standRange;

		//Debug.Log(_joint.enableCollision);
	}
	public override void Activate(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			hook.Shoot();
		}
	}

	private void FixedUpdate()
	{
		if(hook.hook)
        {
			_joint.enabled = true;

			facing = Utility.ExtractSign(HabilityUser.LookingDirectionNormals.x);
			//hand.transform.right = HabilityUser.aimDirectionNormals;

			hand.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(HabilityUser.LookingDirectionNormals.y, HabilityUser.LookingDirectionNormals.x) * Mathf.Rad2Deg);

			//Vector2 distance = (Vector2)transform.position + new Vector2(PositionFromUser.x * facing, PositionFromUser.y) - (Vector2)hand.transform.position;

			handRb.MovePosition(Vector3.Lerp(hand.transform.position, transform.position, handAcceleration * Time.deltaTime));

			//handRb.velocity += distance.normalized * (handAcceleration * Time.deltaTime) * distance.magnitude*100;
			//handRb.velocity = distance.normalized * (handAcceleration * Time.deltaTime) * distance.magnitude * 100;

			hand.transform.localScale = new Vector3(hand.transform.localScale.x, Mathf.Abs(hand.transform.localScale.y) * facing, hand.transform.localScale.z);
		}
		else
		{
			facing = Utility.ExtractSign(HabilityUser.LookingDirectionNormals.x);
			hand.transform.right = HabilityUser.LookingDirectionNormals;

			//Vector2 distance = (Vector2)transform.position + new Vector2(PositionFromUser.x * facing, PositionFromUser.y) - (Vector2)hand.transform.position;

			handRb.MovePosition(Vector3.Lerp(hand.transform.position, (Vector2)transform.position + new Vector2(PositionFromUser.x * facing, PositionFromUser.y), handAcceleration * Time.deltaTime));

			//handRb.velocity += distance.normalized * (handAcceleration * Time.deltaTime) * distance.magnitude*100;
			//handRb.velocity = distance.normalized * (handAcceleration * Time.deltaTime) * distance.magnitude * 100;

			hand.transform.localScale = new Vector3(hand.transform.localScale.x, Mathf.Abs(hand.transform.localScale.y) * facing, hand.transform.localScale.z);
			_joint.enabled = false;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon((Vector2)transform.position + PositionFromUser, "Ghost Hand", false);

		if(standRange < Utility.Hypotenuse(PositionFromUser.x, PositionFromUser.y))
        {
			standRange = Utility.Hypotenuse(PositionFromUser.x, PositionFromUser.y);

		}
	}
}
