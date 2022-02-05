using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLaunch : MonoBehaviour
{
    public float force;

    bool launched = false;

    [Range(0, 360)]
    public float angle;
    void Start()
    {
        Rigidbody2D _rb = GetComponent<Rigidbody2D>();

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        _rb.velocity = transform.right * force;

        launched = true;
    }

    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if(!launched)
            Debug.DrawLine(transform.position,transform.position + new Vector3(force * Mathf.Cos(Mathf.Deg2Rad * angle), force * Mathf.Sin(Mathf.Deg2Rad * angle), 0)/100);
    }
}
