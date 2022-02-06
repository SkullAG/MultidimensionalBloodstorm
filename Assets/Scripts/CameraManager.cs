using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
	public Transform objective;
	public float acceleration;
	public Vector3 Offset = new Vector3(0,0,0);
	Rigidbody2D _rb;

	public bool BlendWithCursor = true;

	[Range(0, 1)]
	public float cursorWeight = 1;

	private Vector3 DesiredPos;

	private PlayerInput _input;
	private InputAction lookAction;

	Camera _camera;
	Vector2 cameraSize;

	public Vector2 effectsPosition;


	bool cinematicMode = false;
	float saveGamePlaySize;

	private void OnEnable()
    {
		CameraEvents.StartCamerasShake += StartCameraShake;
	}
	private void OnDisable()
	{
		CameraEvents.StartCamerasShake -= StartCameraShake;
	}

	private void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		_camera = GetComponent<Camera>();
		cameraSize = new Vector2(_camera.orthographicSize * 2 * _camera.aspect, _camera.orthographicSize * 2);

		effectsPosition = Vector2.zero;

		if (BlendWithCursor)
		{
			_input = GetComponentInParent<PlayerInput>();
			if(!_input)
            {
				Debug.LogError("Requires a player input component");
			}

			lookAction = _input.actions.FindAction("Look");
		}
	}

	private void FixedUpdate()
	{
		if (cinematicMode)
			return;

		if(BlendWithCursor)
		{
			Vector2 looking = lookAction.ReadValue<Vector2>();

			//Debug.Log(looking);

            if(_input.currentControlScheme != "Keyboard&Mouse")
			{
				DesiredPos = (objective.position + (objective.position + new Vector3(looking.x * cameraSize.x, looking.y * cameraSize.y, 0) / 2) * cursorWeight) / 2;
			}
            else
            {
				DesiredPos = (objective.position + (objective.position + _camera.ScreenToWorldPoint(looking)) * cursorWeight) / 2;
			}
		}
		else
		{
			DesiredPos = objective.position;
		}


		_rb.MovePosition(Vector3.Lerp(transform.position, DesiredPos + Offset + (Vector3)effectsPosition, acceleration * Time.deltaTime));

		effectsPosition = Vector2.zero;
	}

	public void StartCameraShake(float intensity, float speed, float time)
	{
		StartCoroutine(ShakeCoroutine(intensity, speed, time));
    }

	IEnumerator ShakeCoroutine(float intensity, float speed, float time)
    {
		Vector2 dir = Vector2.zero;

		float actualSpeed = 0;
		for (float actualTime = 0; actualTime < time; actualTime += Time.deltaTime)
        {
			if(actualSpeed <= 0)
            {
				actualSpeed = speed;
				dir = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value).normalized;
            }
			actualSpeed -= Time.deltaTime;

			effectsPosition += dir;

			yield return new WaitForFixedUpdate();
        }
    }
}
