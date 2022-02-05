using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDeath : MonoBehaviour
{
    public GameObject explosion;

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
        GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
        //explosion.gameObject.transform.position = this.gameObject.transform.position;
        exp.GetComponent<ParticleSystem>().Play();
        this.gameObject.SetActive(false);
    }
}
