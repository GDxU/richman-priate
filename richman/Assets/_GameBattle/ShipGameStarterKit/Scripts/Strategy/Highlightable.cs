using UnityEngine;

[AddComponentMenu("Strategy/Highlightable")]
public class Highlightable : MonoBehaviour
{
	private Color 	mColor;
	private Color	mTargetColor = new Color(1.5f, 1.5f, 1.5f, 1.0f);
	private bool 	mHighlight 	= false;
	private bool 	mModified 	= false;
	private float 	mAlpha 		= 0.0f;
	
	void Start()
	{
		if (renderer == null) Object.Destroy(this);
		else mColor = renderer.material.color;
	}
	
	// NOTE: Unity currently has a bug where OnMouseEnter and OnMouseExit gets fired every few frames
	// when one or more keys or mouse buttons are held. This is assumed to be a bug as no information
	// could be found clarifying the cause of this issue. If this is ever fixed, remove this note.
	// Attendum: This only seems to be happening in the editor. The player is unaffected.

	void OnMouseEnter() { mHighlight = true; }
	void OnMouseExit()  { mHighlight = false; }
	
	void Update()
	{
		if (!mHighlight && !mModified) return;
		
		float factor = Mathf.Min(1.0f, Time.deltaTime * 10.0f);
		float target = mHighlight ? 1.0f : 0.0f;
		mAlpha = Mathf.Lerp(mAlpha, target, factor);
		
		if (!mHighlight && mAlpha < 0.001f)
		{
			mModified = false;
			renderer.material.color = mColor;
		}
		else
		{
			mModified = true;
			renderer.material.color = Color.Lerp(mColor, mTargetColor, mAlpha);
		}
	}
}