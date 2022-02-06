using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrab : MonoBehaviour
{
    public Transform GrabChecker;
    public float GCRadius;
    public LayerMask grabMask;

    private Transform detectedWeapon;

    //public WeaponaryManager weaponManager;

    private void FixedUpdate()
    {
        Collider2D[] groundCollider = Physics2D.OverlapCircleAll(GrabChecker.position, GCRadius, grabMask);

        float dist = GCRadius;


        foreach (Collider2D c in groundCollider)
        {
            if (c.tag == "Weapon" && dist > Vector2.Distance(GrabChecker.position, c.transform.position))
            {
                detectedWeapon = c.transform;
                dist = Vector2.Distance(GrabChecker.position, c.transform.position);
            }
            else if (c.tag == "Item")
            {
                //pick bullets
            }
        }

        if(dist == GCRadius)
        {
            detectedWeapon = null;
        }
        
    }

    public Transform GrabWeapon()
    {
        if(detectedWeapon)
        {
            return detectedWeapon;
        }
        else
        {
            //Debug.LogWarning("No weapon found");
            return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = detectedWeapon ? Color.green : Color.red;
        Gizmos.DrawWireSphere(GrabChecker.position, GCRadius);
    }
}
