using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ShootingSystem : MonoBehaviour
{
    public Transform shootPoint;

    public ShootingSystemData shootingData;

    public abstract void Shoot(int BulletLayer = 7);
}
