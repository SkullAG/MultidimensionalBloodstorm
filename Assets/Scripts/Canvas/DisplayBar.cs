using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DisplayBar : MonoBehaviour
{
#if UNITY_EDITOR
	[MenuItem("GameObject/HUD/DisplayBar")]
	static void CreateBar(MenuCommand menuCommand)
	{
		GameObject barBackground = new GameObject();
		barBackground.name = "BarBackground";
		barBackground.AddComponent<Image>().color = Color.black;
		barBackground.AddComponent<DisplayBar>();
		barBackground.GetComponent<RectTransform>().offsetMax = new Vector2(50, -30);

		GameObjectUtility.SetParentAndAlign(barBackground, menuCommand.context as GameObject);
	}
#endif
	

	public float _value = 100;

	public float MaxValue = 100;

	[Range(0, 360)]
	public int angle = 0;

	public RectTransform barTransform;

	public RectTransform containerTransform;

	private void Start()
    {
		if(!containerTransform)
        {
			GameObject BarContainer = new GameObject();
			BarContainer.name = "Container";
			BarContainer.AddComponent<RectMask2D>();
			containerTransform = BarContainer.GetComponent<RectTransform>();
			containerTransform.SetParent(transform);

			//containerTransform.pivot = new Vector2(1, 0.5f);
			containerTransform.anchorMin = new Vector2(0, 0);
			containerTransform.anchorMax = new Vector2(1, 1);

			containerTransform.offsetMin = new Vector2(4, 4);
			containerTransform.offsetMax = new Vector2(0, 0);

			containerTransform.localPosition = Vector3.zero;
			containerTransform.localScale = Vector3.one;
        }



		if (!barTransform)
		{
			GameObject Bar = new GameObject();
			Bar.name = "Bar";
			Bar.AddComponent<Image>();
			barTransform = Bar.GetComponent<RectTransform>();
			barTransform.SetParent(containerTransform);

			barTransform.pivot = new Vector2(0.5f, 0.5f);
			barTransform.anchorMin = new Vector2(0, 0);
			barTransform.anchorMax = new Vector2(1, 1);

			barTransform.offsetMin = new Vector2(0, 0);
			barTransform.offsetMax = new Vector2(0, 0);

			barTransform.localPosition = Vector3.zero;
			barTransform.localScale = Vector3.one;
		}

		UpdateValues();
	}
#if UNITY_EDITOR
	void OnValidate()
    {
		UpdateValues();
	}
#endif

	private void UpdateValues()
    {
		if (_value < 0)
			_value = 0;
		if (_value > MaxValue)
			_value = MaxValue;

		barTransform.localPosition = new Vector2(-containerTransform.rect.width + containerTransform.rect.width * (_value / MaxValue), -containerTransform.rect.height + containerTransform.rect.height * (_value / MaxValue)) * new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));

	}

    public void SetValue(float value)
	{
		//Debug.Log(value);
		_value = value;
		UpdateValues();
	}

	public void SetMaxValue(float value)
	{
		//Debug.Log(value);
		MaxValue = value;
		UpdateValues();
	}
}
