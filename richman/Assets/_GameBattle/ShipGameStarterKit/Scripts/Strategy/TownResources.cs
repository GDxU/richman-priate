using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TownResource
{
	public string name;
	public string description;
	public Texture2D icon;
}

[AddComponentMenu("Strategy/Town Resources")]
public class TownResources : MonoBehaviour
{
	public static TownResources Instance = null;
	
	public List<TownResource> list = new List<TownResource>();
	
	public int count { get { return list.Count; } }
	
	void OnEnable()
	{
		Instance = this;
	}
	
	void OnDisable()
	{
		if (Instance == this) Instance = null;
	}
	
	public TownResource Get (int index)
	{
		return index < list.Count ? list[index] : null;
	}
	
	public Vector2 GetIconSize()
	{
		TownResource tr = Get(0);

		if (tr != null && tr.icon != null)
		{
			return new Vector2(tr.icon.width, tr.icon.height);
		}
		return new Vector2(50.0f, 50.0f);
	}
}