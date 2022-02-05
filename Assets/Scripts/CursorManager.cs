using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
	public Texture2D cursorTexture;

	void Awake() {
		Vector2 pivot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
		Cursor.SetCursor(cursorTexture, pivot, CursorMode.ForceSoftware);
		//Cursor.lockState = CursorLockMode.None;
	}
}
