using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedProjectile : MonoBehaviour
{
	public LayerMask layers;

	public bool isConstant;

	Rigidbody2D _rb;
	
	private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		//Debug.Log(transform.position);
		//calculateHit();
		//transform.position -= (Vector3)_rb.velocity * Time.deltaTime * 2;
	}

    private void OnEnable()
    {
		//Debug.Log(Time.deltaTime);
		//calculateHit();
		//_rb = GetComponent<Rigidbody2D>();
		//Debug.Log(_rb.velocity);
		//_rb.MovePosition(transform.position - (Vector3)_rb.velocity * Time.deltaTime * 100);
	}

	private void FixedUpdate()
	{
		if(_rb.gameObject.activeInHierarchy)
			calculateHit();
	}

	public void calculateHit()
    {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, _rb.velocity.normalized, _rb.velocity.magnitude * Time.deltaTime, layers);
		
		if (hit)
		{
			//Debug.DrawLine((Vector2)transform.position, hit.point, Color.green, 1000);
			
			_rb.MovePosition(hit.point);
			//transform.position = hit.point;

			//gameObject.SendMessage("OnTriggerEnter2D", hit.collider, SendMessageOptions.DontRequireReceiver);
			//gameObject.SendMessage("OnCollisionEnter2D", hit.collider, SendMessageOptions.DontRequireReceiver);
			//transform.position = hit.point;
		}
		//else
		//{
		//	Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + _rb.velocity * Time.deltaTime, Color.cyan, 1000);
		//}
	}
}
