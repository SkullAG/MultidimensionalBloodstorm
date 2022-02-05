using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsDestroyer : MonoBehaviour
{
	public bool destroyOnUpBounds = true;
	public bool destroyOnDownBounds = true;
	public bool destroyOnRightBounds = true;
	public bool destroyOnLeftBounds = true;

	void FixedUpdate()
	{
		Vector3 pos = transform.position;
		Vector3 normpos = Camera.main.WorldToViewportPoint(pos);
		if ((normpos.x < 0 && destroyOnLeftBounds) || (normpos.y > 1 && destroyOnUpBounds) || (normpos.x > 1 && destroyOnRightBounds) || (normpos.y < 0 && destroyOnDownBounds))
		{
			gameObject.SetActive(false);
		}
	}
}
