using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraEvents
{
    public static event Action<float, float, float> StartCamerasShake = delegate { };

    public static void ShakeAllCameras(float intensity, float speed, float time)
    {
        StartCamerasShake(intensity, speed, time);
    }
}
