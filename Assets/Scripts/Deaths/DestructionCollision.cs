using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionCollision : MonoBehaviour
{
    public Sprite[] holeShapes;
    public int holeRadius;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        if (collider.GetComponent<Test>() != null)
        {
            Sprite selectedHole = holeShapes[Random.Range(0, holeShapes.Length)];
            collider.GetComponent<Test>().makeHole(transform.position, selectedHole, new Vector2(holeRadius, holeRadius));
            Destroy(gameObject);
        }
    }

    [ExecuteInEditMode]

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, holeRadius);
    }
}
