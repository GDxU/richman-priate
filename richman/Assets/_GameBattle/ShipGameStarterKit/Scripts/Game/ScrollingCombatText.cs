using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Game/SCT")]
public class ScrollingCombatText : MonoBehaviour 
{	
	private class Entry
	{
		public string	 mText;
		public float	 mStart;
		public Color	 mColor;
		public float	 mCurrentOffset;
		public float	 mTargetOffset;
	};
	
	private List<Entry> mUsed = new List<Entry>();
	private	List<Entry> mUnused = new List<Entry>();
	
	private SplineF mScale 	= new SplineF();
	private SplineF mOffset = new SplineF();
	private SplineF mAlpha 	= new SplineF();

	private GUIContent mContent = null;
	
	// TODO: Externalize this into a separate configuration class.
	public GUIStyle style = null;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	
	void Start() 
	{
		// Set up the alpha spline
		mAlpha.AddKey(0.0f, 0.1f);
		mAlpha.AddKey(0.15f, 1.0f);
		mAlpha.AddKey(1.8f, 1.0f);
		mAlpha.AddKey(2.8f, 0.0f);
	
		// Set up the scale spline
		mScale.AddKey(0.0f, 0.5f);
		mScale.AddKey(0.2f, 1.25f);
		mScale.AddKey(0.3f, 1.0f);
	
		// Set up the offset spline
		mOffset.AddKey(0.0f, 0.0f);
		mOffset.AddKey(0.2f, 0.0f);
		mOffset.AddKey(2.8f, 90.0f);
	}
	
	/// <summary>
	/// Convenience function for adding new SCT entries.
	/// </summary>

	public static void Print (GameObject go, string text, Color color)
	{
		ScrollingCombatText sct = go.GetComponent<ScrollingCombatText>();

		if (sct == null)
		{
			sct = go.AddComponent<ScrollingCombatText>();
			sct.style = Config.Instance.infoStyle;
		}
		sct.Add(text, color);
	}
	
	/// <summary>
	/// Add some text above the game object
	/// </summary>

	public void Add (string text, Color color)
	{
		Entry ent;
		
		if (mUnused.Count > 0)
		{
			ent = mUnused[0];
			mUnused.RemoveAt(0);
		}
		else
		{
			ent = new Entry();
		}
		ent.mStart = Time.time;
		ent.mText  = text;
		ent.mCurrentOffset = 0.0f;
		ent.mColor = color;
		mUsed.Add(ent);
	}
	
	/// <summary>
	/// Draw the scrolling text.
	/// </summary>
	
	void OnGUI()
	{
		if (Event.current.type != EventType.Repaint) return;
		if (Camera.main == null) return;
		
		Vector3 v = transform.position;
		Vector2 pos2D = Camera.main.WorldToScreenPoint(v);
		pos2D.y = Screen.height - pos2D.y;
		
		for (int i = mUsed.Count; i > 0;)
		{
			// Calculate the current fade-out factor
			Entry ent = mUsed[--i];
	
			float elapsed = (Time.time - ent.mStart);
			float alpha = mAlpha.Sample(elapsed, false);
	
			if (alpha > 0.001f)
			{
				Vector3 pos = pos2D;
				float offset = mOffset.Sample(elapsed, false);
				
				if (i < mUsed.Count - 1)
				{
					Entry nextEnt = mUsed [i + 1];
					float diff = offset - nextEnt.mCurrentOffset;
	
					if (diff < style.fontSize)
					{
						offset += (style.fontSize - diff);
					}
				}
	
				ent.mTargetOffset = offset;
				ent.mCurrentOffset = Mathf.Lerp(ent.mCurrentOffset, ent.mTargetOffset, 0.5f);
				pos.y -= ent.mCurrentOffset;
				
				int fontSize = style.fontSize;
				Color textColor = style.normal.textColor;
				
				if (mContent == null) mContent = new GUIContent();
				mContent.text = ent.mText;
				
				style.fontSize = Mathf.RoundToInt(fontSize * mScale.Sample(elapsed, false));
				Vector2 size = style.CalcSize(mContent);
				
				style.normal.textColor = new Color(0f, 0f, 0f, alpha * ent.mColor.a);
				GUI.Label(new Rect(pos.x + 1f - size.x * 0.5f, pos.y + 1f, size.x, size.y), ent.mText, style);
				
				Color c = ent.mColor;
				c.a *= alpha;
				style.normal.textColor = c;
				GUI.Label(new Rect(pos.x - size.x * 0.5f, pos.y, size.x, size.y), ent.mText, style);
				
				style.fontSize = fontSize;
				style.normal.textColor = textColor;
			}
			else
			{
				mUsed.RemoveAt(i);
				mUnused.Add(ent);
			}
		}
	}
}
