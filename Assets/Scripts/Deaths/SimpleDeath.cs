using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDeath : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<HealthSistem>().OnDead.AddListener(Die);
    }

    void OnDisable()
    {
        GetComponent<HealthSistem>().OnDead.RemoveListener(Die);
    }

    // Update is called once per frame
    void Die()
    {
        this.gameObject.SetActive(false);
    }
}
