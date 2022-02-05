using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[Serializable]
public struct skinSwap
{
	public SpriteRenderer skinA;
	public SpriteRenderer skinB;
	public bool useFixedSprite;
	public Sprite sprite;
}

public class TurnManager : MonoBehaviour
{
	public bool Swapped { get; private set; } = true;
	public bool Turned { get; private set; } = false;

	public Transform[] turningObjs;

	public skinSwap[] swappingSkins;

	public void Turn(bool way)
    {
		if(Swapped != way)
        {
			Swapped = way;

			foreach (Transform t in turningObjs)
			{
				t.localScale = new Vector2(Mathf.Abs(t.localScale.x) * (way ? 1f : -1f), t.localScale.y);
			}

			for (int i = 0; i < swappingSkins.Length; i++)
			{
				skinSwap s = swappingSkins[i];

				if (!s.useFixedSprite)
				{
					Sprite tempSprite = s.skinA.sprite;

					s.skinA.sprite = s.skinB.sprite;

					s.skinB.sprite = tempSprite;
				}
				else
				{
					Sprite tempSprite = s.skinA.sprite;

					s.skinA.sprite = s.sprite;

					s.sprite = tempSprite;
				}
			}
		}
	}
}
