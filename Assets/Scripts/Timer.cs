using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeInSeconds = 1;
    public Action activate;

    // Update is called once per frame
    void Update()
    {
        timeInSeconds -= Time.deltaTime;
        if(timeInSeconds <= 0)
        {
            activate();
        }
    }
}
