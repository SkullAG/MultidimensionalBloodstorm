using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
	public string sceneName;
	public bool SaveInChange;

	public Player player;

	public void Change()
	{
		Time.timeScale = 1;

		if(SaveInChange)
		{
			PlayerPrefs.SetString("Level", sceneName);

			PlayerPrefs.DeleteKey("Weapon1");
			PlayerPrefs.DeleteKey("Weapon2");

		}

		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}
}
