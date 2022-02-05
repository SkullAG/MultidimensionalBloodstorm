using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MenuPause : MonoBehaviour
{
    public void Activate(bool state)
    {
        gameObject.SetActive(state);

        Time.timeScale = state ? 0 : 1;
    }

    public void AlternateActivation()
    {

        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAA");
        Activate(!gameObject.activeInHierarchy);
    }
}
