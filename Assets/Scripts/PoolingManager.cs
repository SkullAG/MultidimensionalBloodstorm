using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PooledItems
{
	public string name;
	public GameObject objectToPool;
	public int amount;
}

public class PoolingManager : MonoBehaviour
{
	private static PoolingManager _instance;
	public static PoolingManager Instance => _instance;

	[SerializeField]
	private List<PooledItems> pooledList = new List<PooledItems>();

	[SerializeField]
	private Dictionary<string, List<GameObject>> _items = new Dictionary<string, List<GameObject>>();

	void Awake()
	{
		/*if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Debug.Log("AAAAAAAAA");
			Destroy(this);
		}*/

		_instance = this;
	}

	private void Start()
	{
		for (int i = 0; i < pooledList.Count; i++)
		{
			PooledItems l = pooledList[i];
			_items.Add(l.name, new List<GameObject>());
			for (int j = 0; j < l.amount; j++)
			{
				GameObject tmp;
				tmp = Instantiate(l.objectToPool);
				tmp.SetActive(false);
				_items[l.name].Add(tmp);
			}
		}
	}

	public GameObject GetPooledObject(string name)
	{
		List<GameObject> tmp = _items[name];

		for (int i = 0; i < tmp.Count; i++)
		{
			if (!tmp[i].activeInHierarchy)
			{
				return tmp[i];
			}
		}
		return null;
	}
}
