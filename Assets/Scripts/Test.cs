using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
	Texture2D originalTexture;
	public Texture2D returnedTexture;
	SpriteRenderer _renderer;

	string alphaName;

	Sprite destructibleSprite;

	static int textureAlphaId = 0;

	void Start()
	{
		_renderer = GetComponent<SpriteRenderer>();

		if(GetComponent<PolygonCollider2D>() == null)
		{
			gameObject.AddComponent<PolygonCollider2D>();
		}

		originalTexture = GetComponent<SpriteRenderer>().sprite.texture;
		returnedTexture = new Texture2D(originalTexture.width, originalTexture.height);
		returnedTexture.SetPixels(originalTexture.GetPixels());
		returnedTexture.Apply();

		textureAlphaId++;

		alphaName = originalTexture.name + "_Alpha_" + textureAlphaId.ToString("D3");

		returnedTexture.name = alphaName;


		destructibleSprite = Sprite.Create(returnedTexture, new Rect(0, 0, returnedTexture.width, returnedTexture.height), new Vector2(0.5f, 0.5f), GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);

		returnedTexture.filterMode = FilterMode.Point;

		GetComponent<SpriteRenderer>().sprite = destructibleSprite;
	}

	public void makeHole(Vector3 holeCenter, Sprite holeSpriteShape, Vector2 explosionSize)
	{
		Texture2D holeTexture = holeSpriteShape.texture;

		float objPixelsPerWorldUnitX = destructibleSprite.pixelsPerUnit / transform.lossyScale.x;
		float objPixelsPerWorldUnitY = destructibleSprite.pixelsPerUnit / transform.lossyScale.y;

		float holePixelsPerWorldUnitX = (float)holeTexture.width / (explosionSize.x * 2);
		float holePixelsPerWorldUnitY = (float)holeTexture.height / (explosionSize.y * 2);

		Vector2 overlapDownLeftPoint = new Vector2(Mathf.Max(holeCenter.x - explosionSize.x, transform.position.x - _renderer.bounds.extents.x), Mathf.Max(holeCenter.y - explosionSize.y, transform.position.y - _renderer.bounds.extents.y));
		Vector2 overlapBox = new Vector2(Mathf.Min(holeCenter.x + explosionSize.x, transform.position.x + _renderer.bounds.extents.x) - overlapDownLeftPoint.x, Mathf.Min(holeCenter.y + explosionSize.y, transform.position.y + _renderer.bounds.extents.y) - overlapDownLeftPoint.y);

		Vector2Int overlapDownLeftPointInPixels = new Vector2Int(Mathf.FloorToInt((overlapDownLeftPoint.x - (transform.position.x - _renderer.bounds.extents.x)) * objPixelsPerWorldUnitX), Mathf.FloorToInt((overlapDownLeftPoint.y - (transform.position.y - _renderer.bounds.extents.y)) * objPixelsPerWorldUnitY));
		Vector2Int overlapBoxInPixels = new Vector2Int(Mathf.FloorToInt(overlapBox.x * objPixelsPerWorldUnitX), Mathf.FloorToInt(overlapBox.y * objPixelsPerWorldUnitX));

		Vector2Int overlapDownLeftPointInHolePixels = new Vector2Int(Mathf.FloorToInt((overlapDownLeftPoint.x - (holeCenter.x - explosionSize.x)) * holePixelsPerWorldUnitX), Mathf.FloorToInt((overlapDownLeftPoint.y - (holeCenter.y - explosionSize.y)) * holePixelsPerWorldUnitY));
		Vector2Int overlapBoxInHolePixels = new Vector2Int(Mathf.FloorToInt(overlapBox.x * holePixelsPerWorldUnitX), Mathf.FloorToInt(overlapBox.y * holePixelsPerWorldUnitY));

		Debug.Log(overlapDownLeftPointInPixels.x + " , " + overlapDownLeftPointInPixels.y + " :: " + overlapBoxInPixels.x + " , " + overlapBoxInPixels.y);

		if (overlapDownLeftPointInPixels.x < 0)
			overlapDownLeftPointInPixels.x = 0;
		if (overlapDownLeftPointInPixels.y < 0)
			overlapDownLeftPointInPixels.y = 0;
		if (overlapDownLeftPointInHolePixels.x < 0)
			overlapDownLeftPointInHolePixels.x = 0;
		if (overlapDownLeftPointInHolePixels.y < 0)
			overlapDownLeftPointInHolePixels.y = 0;

		Color[] colors = returnedTexture.GetPixels(overlapDownLeftPointInPixels.x, overlapDownLeftPointInPixels.y, overlapBoxInPixels.x, overlapBoxInPixels.y);
		Color[] holeColors = holeTexture.GetPixels(overlapDownLeftPointInHolePixels.x, overlapDownLeftPointInHolePixels.y, overlapBoxInHolePixels.x, overlapBoxInHolePixels.y);

		float pixelProportionX = (float)overlapBoxInHolePixels.x / (float)overlapBoxInPixels.x;

		float pixelProportionY = (float)overlapBoxInHolePixels.y / (float)overlapBoxInPixels.y;

		int row = 0;

		for (int i = 0; i < colors.Length; i++)
		{
			if(i > overlapBoxInPixels.x * (row+1))
			{
				row++;
			}

			int convertedPixel = Mathf.FloorToInt(i * pixelProportionX + overlapBoxInHolePixels.x * Mathf.FloorToInt((float)row * (pixelProportionY-1)));

			if (convertedPixel < holeColors.Length && holeColors[convertedPixel] != Color.clear)
				colors[i] = Color.clear;
		}

		returnedTexture.SetPixels(overlapDownLeftPointInPixels.x, overlapDownLeftPointInPixels.y, overlapBoxInPixels.x, overlapBoxInPixels.y, colors);
		
		returnedTexture.Apply();

		Color[] opCheck = returnedTexture.GetPixels();

		bool textureIsEmpty = true;

		for(int i = 0; i < opCheck.Length; i++)
        {
			if(opCheck[i] != Color.clear)
            {
				textureIsEmpty = false;
				break;

			}

		}

		if(GetComponent<PolygonCollider2D>() != null)
		{
			Destroy(GetComponent<PolygonCollider2D>());

			if (!textureIsEmpty)
			{
				gameObject.AddComponent<PolygonCollider2D>();
			}
		}
	}

	public void MakeHoleV2(Vector3 holeCenter, Sprite holeSpriteShape, Vector2 explosionSize)
    {
		Texture2D holeTexture = holeSpriteShape.texture;

		float objPixelsPerWorldUnitX = destructibleSprite.pixelsPerUnit / transform.lossyScale.x;
		float objPixelsPerWorldUnitY = destructibleSprite.pixelsPerUnit / transform.lossyScale.y;

		holeTexture.Resize(Mathf.FloorToInt(objPixelsPerWorldUnitX * explosionSize.x), Mathf.FloorToInt(objPixelsPerWorldUnitY * explosionSize.y));

		Vector2 overlapDownLeftPoint = new Vector2(Mathf.Max(holeCenter.x - explosionSize.x, transform.position.x - _renderer.bounds.extents.x), Mathf.Max(holeCenter.y - explosionSize.y, transform.position.y - _renderer.bounds.extents.y));
		Vector2 overlapBox = new Vector2(Mathf.Min(holeCenter.x + explosionSize.x, transform.position.x + _renderer.bounds.extents.x) - overlapDownLeftPoint.x, Mathf.Min(holeCenter.y + explosionSize.y, transform.position.y + _renderer.bounds.extents.y) - overlapDownLeftPoint.y);

		Vector2Int overlapDownLeftPointInPixels = new Vector2Int(Mathf.FloorToInt((overlapDownLeftPoint.x - (transform.position.x - _renderer.bounds.extents.x)) * objPixelsPerWorldUnitX), Mathf.FloorToInt((overlapDownLeftPoint.y - (transform.position.y - _renderer.bounds.extents.y)) * objPixelsPerWorldUnitY));
		Vector2Int overlapBoxInPixels = new Vector2Int(Mathf.FloorToInt(overlapBox.x * objPixelsPerWorldUnitX), Mathf.FloorToInt(overlapBox.y * objPixelsPerWorldUnitX));

		Vector2Int overlapDownLeftPointInHolePixels = new Vector2Int(Mathf.FloorToInt((overlapDownLeftPoint.x - (holeCenter.x - explosionSize.x)) * objPixelsPerWorldUnitX), Mathf.FloorToInt((overlapDownLeftPoint.y - (holeCenter.y - explosionSize.y)) * objPixelsPerWorldUnitY));
		Vector2Int overlapBoxInHolePixels = new Vector2Int(Mathf.FloorToInt(overlapBox.x * objPixelsPerWorldUnitX), Mathf.FloorToInt(overlapBox.y * objPixelsPerWorldUnitY));

		if (overlapDownLeftPointInPixels.x < 0)
			overlapDownLeftPointInPixels.x = 0;
		if (overlapDownLeftPointInPixels.y < 0)
			overlapDownLeftPointInPixels.y = 0;
		if (overlapDownLeftPointInHolePixels.x < 0)
			overlapDownLeftPointInHolePixels.x = 0;
		if (overlapDownLeftPointInHolePixels.y < 0)
			overlapDownLeftPointInHolePixels.y = 0;

		Color[] colors = returnedTexture.GetPixels(overlapDownLeftPointInPixels.x, overlapDownLeftPointInPixels.y, overlapBoxInPixels.x, overlapBoxInPixels.y);
		Color[] holeColors = holeTexture.GetPixels(overlapDownLeftPointInHolePixels.x, overlapDownLeftPointInHolePixels.y, overlapBoxInPixels.x, overlapBoxInPixels.y);

		for (int i = 0; i < colors.Length; i++)
		{

			if (holeColors[i] != Color.clear)
			{
				colors[i] = Color.clear;
			}
		}

		returnedTexture.SetPixels(overlapDownLeftPointInPixels.x, overlapDownLeftPointInPixels.y, overlapBoxInPixels.x, overlapBoxInPixels.y, colors);

		returnedTexture.Apply();

		if (GetComponent<PolygonCollider2D>() != null)
		{
			Destroy(GetComponent<PolygonCollider2D>());

			//if (!textureIsEmpty)
			{
				gameObject.AddComponent<PolygonCollider2D>();
			}
		}
	}
}
