using UnityEngine;

[AddComponentMenu("Game/Town Name")]
public class TownName : MonoBehaviour
{
	private static GUIStyle 	mStyle 	 = null;
	private static GUIContent 	mContent = null;
	
	private float 	mAlpha 		= 0.0f;
	private int 	mPaddingX 	= 30;
	private int 	mPaddingY 	= 10;
	private Town 	mTown 		= null;
	
	void Start()
	{
		Config.Instance.onGUI.Add(DrawGUI);
	}
	
	void DrawGUI()
	{
		if (Event.current.type != EventType.Repaint) return;
		if (Config.Instance == null || Camera.main == null || !enabled) return;
		if (Config.Instance.townNameFont == null) return;

		if (mTown == null)
		{
			mTown = Town.Find(gameObject);
			
			if (mTown == null)
			{
				DebugExt.LogError("'TownName' expects 'Town' to exist on the GameObject or one of its parents");
				enabled = false;
				return;
			}
		}

		Vector3 v = transform.position;
		Vector3 pos = Camera.main.WorldToScreenPoint(v);
		if (pos.z < 0f) return;
		
		float targetAlpha = (pos.x > 0.0f && pos.y > 0.0f && pos.x < Screen.width && pos.y < Screen.height) &&
			Vector3.Distance(v, Camera.main.transform.position) < 60.0f ? 1.0f : 0.0f;

		mAlpha = Mathf.Lerp(mAlpha, targetAlpha, Time.deltaTime * 10.0f);
		if (mAlpha < 0.001f) return;
		
		if (mStyle == null)
		{
			mStyle = new GUIStyle();
			mStyle.alignment = TextAnchor.MiddleCenter;
		}
		
		if (mContent == null)
		{
			mContent = new GUIContent();
		}
		
		mStyle.font = Config.Instance.townNameFont;
		mContent.text = mTown.name;
		
		Vector2 size = mStyle.CalcSize(mContent);
		
		Rect rect = new Rect(pos.x - size.x * 0.5f - mPaddingX,
				Screen.height - pos.y - size.y * 0.5f - mPaddingY,
				size.x + mPaddingX * 2.0f, size.y + mPaddingY * 2.0f);
		
		if (Config.Instance.townNameBackground != null)
		{
			Color prev = GUI.color;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, mAlpha);
			GUI.DrawTexture(rect, Config.Instance.townNameBackground);
			GUI.color = prev;
		}
		
		rect.xMin += 1.0f;
		rect.xMax += 1.0f;
		rect.yMin += 1.0f;
		rect.yMax += 1.0f;
		
		mStyle.normal.textColor = new Color(0.0f, 0.0f, 0.0f, mAlpha);
		GUI.Label(rect, mContent, mStyle);
		
		rect.xMin -= 1.0f;
		rect.xMax -= 1.0f;
		rect.yMin -= 1.0f;
		rect.yMax -= 1.0f;
		
		mStyle.normal.textColor = new Color(1.0f, 1.0f, 1.0f, mAlpha);
		GUI.Label(rect, mContent, mStyle);
	}
}