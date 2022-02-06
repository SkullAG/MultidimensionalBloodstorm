using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public string NextLevelScene;

    Animator _animator;

    public float shakeIntensity, shakespeed, shaketime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            playWinAnim();
    }

    public void playWinAnim()
    {
        _animator.SetTrigger("playAnim");
    }

    public void shake()
    {
        CameraEvents.ShakeAllCameras(shakeIntensity, shakespeed, shaketime);
    }

    public void WinLevel()
    {
        PlayerPrefs.SetString("Level", NextLevelScene);

        //variableForPrefab = Resources.Load("prefabs/prefab1") as GameObject;

        Player player = FindObjectOfType<Player>();
        if(player.weaponManager.weaponableArm[0].weapon)
            PlayerPrefs.SetString("Weapon1", "Prefabs/Weapons/" + player.weaponManager.weaponableArm[0].weapon.name.Split(' ')[0]);
        else
            PlayerPrefs.DeleteKey("Weapon1");
        

        if (player.weaponManager.weaponableArm[1].weapon)
            PlayerPrefs.SetString("Weapon2", "Prefabs/Weapons/" + player.weaponManager.weaponableArm[1].weapon.name.Split(' ')[0]);
        else
            PlayerPrefs.DeleteKey("Weapon2");

        SceneManager.LoadScene(NextLevelScene, LoadSceneMode.Single);
    }
}
