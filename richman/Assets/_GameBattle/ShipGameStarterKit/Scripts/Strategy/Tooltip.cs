using UnityEngine;

[AddComponentMenu("Strategy/Tooltip")]
public class Tooltip : MonoBehaviour
{
	public delegate void OnDrawTooltip (Vector2 pos, object param);
	
	static Rect mRect;
	static OnDrawTooltip mCallback;
	static object mParam;
	
	public float delay = 1.0f;
	
	float mTimestamp = 0.0f;
	Vector2 mLastPos;
	
	/// <summary>
	/// Adds a new tooltip area. Will check against the mouse position prior to adding.
	/// </summary>
	
	static public void AddArea (Rect inRect, OnDrawTooltip inCallback, object inParam)
	{
		if (UI.ContainsMouse(inRect))
		{
			mRect 	 	= inRect;
			mCallback 	= inCallback;
			mParam 		= inParam;
		}
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	
	void Start()
	{
		Config.Instance.onLateGUI.Add(DrawGUI);
	}
	
	/// <summary>
	/// Will draw the tooltip if it's time.
	/// </summary>
	
	void DrawGUI()
	{
		if (mCallback == null) return;
		
		Vector2 pos = Input.mousePosition;
		Vector2 diff = pos - mLastPos;
		float movement = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
		
		if (movement > 0.0f)
		{
			mLastPos = pos;
			mTimestamp = Time.time;
		}
		else if (mTimestamp + delay < Time.time)
		{
			pos.y = Screen.height - pos.y;
			
			if (mRect.Contains(pos))
			{
				mCallback(pos, mParam);
			}
			else
			{
				mCallback = null;
				mParam = null;
			}
		}
	}
}