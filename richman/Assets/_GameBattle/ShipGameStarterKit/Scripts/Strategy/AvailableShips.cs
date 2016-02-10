using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Strategy/Available Ships")]
public class AvailableShips : MonoBehaviour
{
	[System.Serializable]
	public class Template
	{
		public string name;
		public string description;
		public Texture2D icon;
		public GameObject prefab;
		public int price;
		public int strength;
		public int speed;
		public int acceleration;
		public int cargo;
	}

	[System.Serializable]
	public class Owned
	{
		public Template prefab 		 = null;
		public TradeRoute tradeRoute = null;
		public GameObject asset 	 = null;
	}

	public static AvailableShips Instance = null;
	
	public List<Template> prefabs = new List<Template>();
	
	[HideInInspector()]
	public List<Owned> list = new List<Owned>();
	
	void Start()
	{
		Instance = this;
	}
	
	void OnDestroy()
	{
		if (Instance == this) Instance = null;
	}
}