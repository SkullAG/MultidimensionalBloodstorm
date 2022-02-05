using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ImageDestruction : MonoBehaviour
{
	Texture2D originalTexture;
	public Texture2D returnedTexture;
	SpriteRenderer _renderer;

	string alphaName;

	Sprite destructibleSprite;

	static int textureAlphaId = 0;
	bool mustApply = false;

	[SerializeField]
	[Range(0, 4)]
	int destructionResistance = 1;

	Vector2 bounds;

	Vector2 objPixelsPerUnit;

	public int GetResistance()
    {
		return destructionResistance;
    }

	private void FixedUpdate()
	{
		if(mustApply)
		{
			mustApply = false;

			returnedTexture.Apply();

			CalculatePolygon();
		}
	}


	void Start()
	{
		_renderer = GetComponent<SpriteRenderer>();

		if (GetComponent<PolygonCollider2D>() == null)
		{
			gameObject.AddComponent<PolygonCollider2D>();
		}

		originalTexture = _renderer.sprite.texture;
		returnedTexture = new Texture2D(originalTexture.width, originalTexture.height);
		returnedTexture.SetPixels(originalTexture.GetPixels());
		returnedTexture.Apply();

		textureAlphaId++;

		alphaName = originalTexture.name + "_Alpha_" + textureAlphaId.ToString("D3");

		returnedTexture.name = alphaName;


		destructibleSprite = Sprite.Create(returnedTexture, new Rect(0, 0, returnedTexture.width, returnedTexture.height), new Vector2(0.5f, 0.5f), GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);

		returnedTexture.filterMode = FilterMode.Point;

		objPixelsPerUnit = new Vector2(destructibleSprite.pixelsPerUnit /*/ transform.localScale.x*/, destructibleSprite.pixelsPerUnit /*/ transform.localScale.y*/);

		bounds = new Vector2(returnedTexture.width / objPixelsPerUnit.x, returnedTexture.height / objPixelsPerUnit.y);

		GetComponent<SpriteRenderer>().sprite = destructibleSprite;
	}

	public void makeCircleHole(Vector3 holeCenter, float explosionRadius)
	{
		//Vector2 normalizedAngle = new Vector2(holeCenter.x - transform.position.x, holeCenter.y - transform.position.y).normalized;
		//
		//
		//
		//float angleRadians = Mathf.Deg2Rad * transform.rotation.eulerAngles.z - Mathf.Atan2(normalizedAngle.y, normalizedAngle.x) + Mathf.Deg2Rad*90;
		////float angleRadians = Mathf.Deg2Rad * angle;
		//holeCenter = transform.position + Vector3.Distance(holeCenter, transform.position) * new Vector3(Mathf.Sin(angleRadians), Mathf.Cos(angleRadians), 0);
		holeCenter = transform.InverseTransformPoint(holeCenter);
		explosionRadius = explosionRadius / transform.lossyScale.x;

		Vector2 overlapDownLeftPoint = new Vector2(Mathf.Max(holeCenter.x - explosionRadius, -bounds.x / 2), Mathf.Max(holeCenter.y - explosionRadius, -bounds.y / 2));
		Vector2 overlapBox = new Vector2(Mathf.Min(holeCenter.x + explosionRadius, bounds.x / 2) - overlapDownLeftPoint.x, Mathf.Min(holeCenter.y + explosionRadius, bounds.y / 2) - overlapDownLeftPoint.y);

		//Debug.Log(overlapDownLeftPoint);

		Vector2Int overlapDownLeftPointInPixels = new Vector2Int(Mathf.FloorToInt((overlapDownLeftPoint.x + bounds.x / 2) * objPixelsPerUnit.x), Mathf.FloorToInt((overlapDownLeftPoint.y + bounds.y / 2) * objPixelsPerUnit.y));
		Vector2Int overlapBoxInPixels = new Vector2Int(Mathf.Abs(Mathf.FloorToInt(overlapBox.x * objPixelsPerUnit.x)), Mathf.Abs(Mathf.FloorToInt(overlapBox.y * objPixelsPerUnit.y)));

		Vector2Int holeCenterInPixels = new Vector2Int(Mathf.FloorToInt((holeCenter.x - overlapDownLeftPoint.x) * objPixelsPerUnit.x), Mathf.FloorToInt((holeCenter.y - overlapDownLeftPoint.y) * objPixelsPerUnit.y));
		int holeRadiusInPixels = Mathf.FloorToInt(explosionRadius * objPixelsPerUnit.x);

		//Debug.Log(holeCenterInPixels);

		if (overlapDownLeftPointInPixels.x < 0)
			overlapDownLeftPointInPixels.x = 0;
		if (overlapDownLeftPointInPixels.y < 0)
			overlapDownLeftPointInPixels.y = 0;

		if (overlapDownLeftPointInPixels.x + overlapBoxInPixels.x >= returnedTexture.width - 1)
			overlapBoxInPixels.x = returnedTexture.width - overlapDownLeftPointInPixels.x;
		if (overlapDownLeftPointInPixels.y + overlapBoxInPixels.y >= returnedTexture.height - 1)
			overlapBoxInPixels.y = returnedTexture.height - overlapDownLeftPointInPixels.y;

		Color[] colors = returnedTexture.GetPixels(overlapDownLeftPointInPixels.x, overlapDownLeftPointInPixels.y, overlapBoxInPixels.x, overlapBoxInPixels.y);

		int row = 0;

		for (int i = 0; i < colors.Length; i++)
		{
			row = i / overlapBoxInPixels.x;

			if (Vector2Int.Distance(holeCenterInPixels, new Vector2Int(i % overlapBoxInPixels.x, row)) <= holeRadiusInPixels)
				colors[i] = Color.clear;
		}

		returnedTexture.SetPixels(overlapDownLeftPointInPixels.x, overlapDownLeftPointInPixels.y, overlapBoxInPixels.x, overlapBoxInPixels.y, colors);

		mustApply = true;
	}

	public void CalculatePolygon()
	{
		Color[] opCheck = returnedTexture.GetPixels();

		bool textureIsEmpty = true;

		for (int i = 0; i < opCheck.Length; i++)
		{
			if (opCheck[i] != Color.clear)
			{
				textureIsEmpty = false;
				break;

			}

		}

		if (GetComponent<PolygonCollider2D>() != null)
		{
			PolygonCollider2D oldCol = GetComponent<PolygonCollider2D>();

			if (!textureIsEmpty)
			{
				//PolygonCollider2D newCol = gameObject.AddComponent<PolygonCollider2D>(oldCol);
				PolygonCollider2D newCol = gameObject.AddComponent<PolygonCollider2D>();
				newCol.isTrigger = oldCol.isTrigger;
				newCol.usedByComposite = oldCol.usedByComposite;
				newCol.usedByEffector = oldCol.usedByEffector;

			}
			else
			{
				gameObject.SetActive(false);
			}
			
			Destroy(oldCol);
		}
	}
}
