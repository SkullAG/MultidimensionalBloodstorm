using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public string DefaultScene;

	public void Load()
	{
		Time.timeScale = 1;

		if(PlayerPrefs.HasKey("Level"))
			SceneManager.LoadScene(PlayerPrefs.GetString("Level"), LoadSceneMode.Single);
		else
			SceneManager.LoadScene(DefaultScene, LoadSceneMode.Single);

	}
}
