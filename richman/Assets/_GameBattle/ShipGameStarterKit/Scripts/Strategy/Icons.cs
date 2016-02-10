using UnityEngine;

[AddComponentMenu("Strategy/Icons")]
public class Icons : MonoBehaviour
{
	public static Icons Instance = null;
	
	public Texture2D	arrowRight	= null;
	public Texture2D	arrowLeft	= null;
	public Texture2D	gold		= null;
	
	void Start()
	{
		Instance = this;
	}
	
	void OnApplicationQuit()
	{
		if (Instance == this) Instance = null;
	}
}