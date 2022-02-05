using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Explode : MonoBehaviour
{
	public float holeRadius;
	public GameObject explosion;
	public int damage = 0;
	public int dodgeCost = 1;
	public int destructionStrenght = 1;

	public float explosionForce = 20000;

	[Range(0, 3)]
	public int destructionPower = 1;

	public int multipleExplosions = 1;
	private int TotalMultipleExplosions;

	public bool useTimer = false;

	public float timerInSeconds = 0;

	public bool explodeOnCollision = true;

	public bool exclusiveToHit = false;

	bool isExploding = false;

    private void Awake()
    {
		TotalMultipleExplosions = multipleExplosions;

	}

    private void OnEnable()
    {
		isExploding = false;

	}

    public void OnTriggerEnter2D(Collider2D collider)
	{
		if(explodeOnCollision)
        {
			if((collider.GetComponent<ImageDestruction>() && collider.GetComponent<ImageDestruction>().GetResistance() <= destructionPower))
            {
				Detonate(collider);
				return;
			}
			if ((collider.GetComponentInChildren<ImageDestruction>() && collider.GetComponentInChildren<ImageDestruction>().GetResistance() <= destructionPower))
			{
				Detonate(collider);
				return;
			}
			else
            {
				multipleExplosions = 0;
				Detonate(collider);
			}
        }
			
	}

	public void Detonate(Collider2D collider = null)
    {
		if (isExploding) { return; }

		isExploding = true;

		Collider2D[] colliders;
		if (!collider || !exclusiveToHit)
		{
			colliders = Physics2D.OverlapCircleAll(transform.position, holeRadius);
		}
		else
		{
			colliders = new Collider2D[1];
			colliders[0] = collider;
		}
		

		
		//Physics2D.CircleCast(transform.position, holeRadius);
		List<Transform> objs = new List<Transform>();

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].GetType() == typeof(CompositeCollider2D))
            {
				//Collider2D[] innerColliders = colliders[i].GetComponentsInChildren<Collider2D>();
				foreach(PolygonCollider2D c in colliders[i].GetComponentsInChildren<PolygonCollider2D>())
                {
					c.usedByComposite = false;
					if (Vector2.Distance(c.ClosestPoint(transform.position), transform.position) <= holeRadius)
                    {
						//Debug.Log(c.name);
						objs.Add(c.transform);
					}
					c.usedByComposite = true;
				}

			}
            else if(!objs.Contains(colliders[i].transform))
            {
				objs.Add(colliders[i].transform);
            }
			

			//CompositeCollider2D a;

			//Debug.Log(a.bounds.);
		}

		/*for(int obj1 = objs.Count-1; obj1 >= 0; obj1--)
        {
			bool duplicateFound = false;
			for (int obj2 = 0; obj2 < objs.Count && !duplicateFound; obj2++)
			{
				//Debug.Log(objs[obj1].name + " ?= " + objs[obj2].name + " = " + (objs[obj1] == objs[obj2]));
				if (obj1 != obj2 && objs[obj1] == objs[obj2])
                {
					objs.RemoveAt(obj1);
					duplicateFound = true;

				}
			}
		}*/

		objs = objs.Distinct().ToList();

		//Debug.Log("cosas destruidas: " + objs.Count);

		//for(int i = 0; i < objs.Count; i++)
		//{
		//	Debug.LogWarning("cosa detectada: " + objs[i].gameObject.name);
		//}

		for (int i = 0; i < objs.Count; i++)
		{
			//Debug.Log(objs[i].name);

			if (objs[i].GetComponent<ImageDestruction>() != null && objs[i].GetComponent<ImageDestruction>().GetResistance() <= destructionPower)
			{
				//Sprite selectedHole = holeShapes[Random.Range(0, holeShapes.Length)];
				objs[i].GetComponent<ImageDestruction>().makeCircleHole(transform.position, holeRadius);
			}
			if (objs[i].GetComponent<HealthSistem>() != null)
			{
				
				objs[i].GetComponent<HealthSistem>().Hurt(damage, dodgeCost);
			}
			if (objs[i].GetComponent<Rigidbody2D>() != null)
			{
				objs[i].GetComponent<Rigidbody2D>().AddForce(((Vector2)objs[i].position - (Vector2)transform.position).normalized * explosionForce);
			}
		}

		if (explosion != null)
		{
			GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
			exp.transform.localScale = new Vector3(holeRadius / 2, holeRadius / 2, holeRadius / 2);
			//explosion.gameObject.transform.position = this.gameObject.transform.position;
			exp.GetComponent<ParticleSystem>().Play();
			this.gameObject.SetActive(false);
		}

		multipleExplosions--;

		if (multipleExplosions <= 0)
        {
			multipleExplosions = TotalMultipleExplosions;
			gameObject.SetActive(false);
        }
        else if(GetComponent<HighSpeedProjectile>())
        {
			GetComponent<HighSpeedProjectile>().calculateHit();


		}
			
	}

    private void Update()
    {
        if(useTimer)
        {
			timerInSeconds -= Time.deltaTime;
			if(timerInSeconds <= 0)
            {
				Detonate();
            }
		}
    }

	public void Die()
    {
		Detonate();
	}

    [ExecuteInEditMode]

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, holeRadius);
	}
}
