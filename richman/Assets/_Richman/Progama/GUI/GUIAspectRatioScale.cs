using UnityEngine;
using System.Collections;

public class GUIAspectRatioScale : MonoBehaviour
{
	public Vector2 scaleOnRatio1 = new Vector2(0.1f, 0.1f);
	private Transform myTrans;
	private float widthHeightRatio;
	
	void Start ()
	{
		myTrans = transform;
		SetScale();
	}
	//call on an event that tells if the aspect ratio changed
	void SetScale()
	{
		//find the aspect ratio
		widthHeightRatio = (float)Screen.width/Screen.height;
		//Apply the scale. We only calculate y since our aspect ratio is x (width) authoritative: width/height (x/y)
		myTrans.localScale = new Vector3 (scaleOnRatio1.x, widthHeightRatio * scaleOnRatio1.y, 1);
	}
}