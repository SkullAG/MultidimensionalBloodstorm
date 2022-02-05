using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HurtSistem : MonoBehaviour
{
    public int damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<HealthSistem>() != null)
            other.GetComponent<HealthSistem>().Hurt(damage);
    }
}
