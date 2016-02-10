using UnityEngine;
using System.Collections.Generic;

public class UI
{
	static GUISkin mNullSkin = null;
	static GUIContent mContent = null;
	static Color mColor;

	/// <summary>
	/// It's often useful to have a skin that is clean and devoid of borders, padding, margins, etc.
	/// </summary>

	static public GUISkin nullSkin
	{
		get
		{
			if (mNullSkin == null)
			{
				mNullSkin = GUISkin.CreateInstance("GUISkin") as GUISkin;
				mNullSkin.name = "Null Skin (Tools.cs)";

				if (Config.Instance != null)
				{
					mNullSkin.box.normal.background = Config.Instance.windowBorder;
					mNullSkin.box.border.left = Config.Instance.windowPadding;
					mNullSkin.box.border.right = Config.Instance.windowPadding;
					mNullSkin.box.border.top = Config.Instance.windowPadding;
					mNullSkin.box.border.bottom = Config.Instance.windowPadding;
				}
			}
			return mNullSkin;
		}
	}

	/// <summary>
	/// Wraps the angle, ensuring that it's always in the -360 to 360 range.
	/// </summary>

	static public float WrapAngle (float a)
	{
		while (a < -180.0f) a += 360.0f;
		while (a > 180.0f) a -= 360.0f;
		return a;
	}

	/// <summary>
	/// Replaces the GUI color with the specified alpha value.
	/// </summary>

	static public void SetAlpha (float a)
	{
		mColor = GUI.color;
		GUI.color = new Color(1.0f, 1.0f, 1.0f, a);
	}

	/// <summary>
	/// Restores the previous GUI color.
	/// </summary>

	static public void RestoreAlpha () { GUI.color = mColor; }

	/// <summary>
	/// Draws the tiled texture. Like GUI.DrawTexture() but tiled instead of stretched.
	/// </summary>

	static public void DrawTiledTexture (Rect rect, Texture tex)
	{
		GUI.BeginGroup(rect);
		{
			int width  = Mathf.RoundToInt(rect.width);
			int height = Mathf.RoundToInt(rect.height);

			for (int y = 0; y < height; y += tex.height)
			{
				for (int x = 0; x < width; x += tex.width)
				{
					GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
				}
			}
		}
		GUI.EndGroup();
	}

	/// <summary>
	/// Draws the specified tiled texture using the given element's offset.
	/// </summary>

	static public Rect DrawTiledTexture (Rect rect, float offset, Texture tex)
	{
		rect = new Rect(rect.x + offset, rect.y + offset,
			rect.width - offset - offset,
			rect.height - offset - offset);

		DrawTiledTexture(rect, tex);
		return rect;
	}

	/// <summary>
	/// Helper function that draws a texture, cleanly with no border, margin, or padding.
	/// </summary>

	static public Rect DrawTexture (float x, float y, Texture tex)
	{
		Rect rect = new Rect(x, y, tex.width, tex.height);
		GUI.DrawTexture(rect, tex);
		return rect;
	}

	/// <summary>
	/// Helper function that draws a box similar to GUI.Box(), but with a THS border and a tiled background.
	/// </summary>

	static public Rect DrawPanel (Rect rect)
	{
		Rect retVal = DrawTiledTexture(rect,
			Config.Instance.windowPadding,
			Config.Instance.windowBackground);

		GUI.Box(rect, "", nullSkin.box);

		// Content rectangle
		return retVal;
	}

	/// <summary>
	/// Draws a window over the specified rectangle.
	/// </summary>

	static public Rect DrawWindow (Rect rect, string text)
	{
		rect = DrawPanel(rect);
		Rect titleRect = new Rect(rect.x + 50.0f, rect.y - 25.0f, rect.width - 100.0f, 40.0f);
		DrawPanel(titleRect);
		DrawTitle(titleRect, text, Config.Instance.headerStyle);
		return Bevel(rect, 8f);
	}

	/// <summary>
	/// Draws a shadowed (or beveled) label with specified colors.
	/// </summary>

	static public void DrawLabel (Rect rect, string text, GUIStyle style, Color front, Color back)
	{
		DrawLabel(rect, text, style, front, back, false);
	}

	/// <summary>
	/// Draws a shadowed (or beveled) label with specified colors.
	/// </summary>

	static public void DrawLabel (Rect rect, string text, GUIStyle style, Color front, Color back, bool outline)
	{
		Color prev = style.normal.textColor;

		// Shadow
		style.normal.textColor = back;
		GUI.Label(new Rect(rect.x + 1.0f, rect.y + 1.0f, rect.width, rect.height), text, style);

		if (outline)
		{
			GUI.Label(new Rect(rect.x + 1.0f, rect.y - 1.0f, rect.width, rect.height), text, style);
			GUI.Label(new Rect(rect.x - 1.0f, rect.y - 1.0f, rect.width, rect.height), text, style);
			GUI.Label(new Rect(rect.x - 1.0f, rect.y + 1.0f, rect.width, rect.height), text, style);
		}

		// Actual label
		style.normal.textColor = front;
		GUI.Label(rect, text, style);

		style.normal.textColor = prev;
	}

	/// <summary>
	/// Draws a shadowed (or beveled) label with specified colors.
	/// </summary>

	static public void DrawLabel (string text, GUIStyle style, Color front, Color back)
	{
		Color prev = style.normal.textColor;

		// Shadow
		style.normal.textColor = back;
		GUILayout.Label(text, style);

		// Actual label
		style.normal.textColor = front;
		Rect rect = GUILayoutUtility.GetLastRect();
		GUI.Label(new Rect(rect.x - 1.0f, rect.y - 1.0f, rect.width, rect.height), text, style);

		style.normal.textColor = prev;
	}

	/// <summary>
	/// Draws a label with a distinct beveled style.
	/// </summary>

	static public void DrawLabel (string text, GUIStyle style)
	{
		DrawLabel(text, style, style.normal.textColor, style.hover.textColor);
	}

	/// <summary>
	/// Draws a label with a distinct beveled style.
	/// </summary>

	static public void DrawLabel (string text)
	{
		DrawLabel(text, GUI.skin.label);
	}

	/// <summary>
	/// Draws a label with a distinct beveled style.
	/// </summary>

	static public void DrawLabel (Rect rect, string text, GUIStyle style)
	{
		DrawLabel(rect, text, style, style.normal.textColor, style.hover.textColor, false);
	}

	/// <summary>
	/// Draws a label with a distinct beveled style.
	/// </summary>

	static public void DrawLabel (Rect rect, string text)
	{
		DrawLabel(rect, text, GUI.skin.label);
	}

	/// <summary>
	/// Common functionality for DrawTitle functions.
	/// </summary>

	static private void DrawBlobShadow (Rect rect, string text, GUIStyle style)
	{
		if (Config.Instance.townNameBackground != null)
		{
			Vector3 size = style.CalcSize(mContent);

			float padX = size.x * 0.25f;
			float padY = size.y * 0.5f;
			float x = (rect.width - size.x) * 0.5f - padX;
			float y = (rect.height - size.y) * 0.5f - padY;

			Color prev = GUI.color;
			GUI.color = new Color(1.0f, 1.0f, 1.0f, prev.a * 0.5f);
			GUI.DrawTexture(new Rect(rect.x + x, rect.y + y, rect.width - x * 2.0f, rect.height - y * 2.0f),
				Config.Instance.townNameBackground);
			GUI.color = prev;
		}
	}

	/// <summary>
	/// Draws a title label -- beveled header label with a blob shadow.
	/// </summary>

	static public void DrawTitle (Rect rect, string text, GUIStyle style)
	{
		TextAnchor prev = style.alignment;
		style.alignment = TextAnchor.MiddleCenter;

		if (mContent == null) mContent = new GUIContent();
		mContent.text = text;

		DrawBlobShadow(rect, text, style);
		DrawLabel(rect, text, style);
		style.alignment = prev;
	}

	/// <summary>
	/// Draws a title label -- beveled header label with a blob shadow.
	/// </summary>

	static public void DrawTitle (string text, GUIStyle style)
	{
		TextAnchor prev = style.alignment;
		style.alignment = TextAnchor.MiddleCenter;

		if (mContent == null) mContent = new GUIContent();
		mContent.text = text;

		Rect rect = GUILayoutUtility.GetRect(mContent, style);
		DrawBlobShadow(rect, text, style);
		DrawLabel(rect, text, style);
		style.alignment = prev;
	}

	/// <summary>
	/// Bevel the specified rect by the specified number of pixels.
	/// </summary>

	static public Rect Bevel (Rect rect, float pixels)
	{
		return new Rect(rect.x + pixels, rect.y + pixels, rect.width - pixels * 2.0f, rect.height - pixels * 2.0f);
	}

	/// <summary>
	/// Draws a grid of boxes and returns their positions.
	/// </summary>

	static public List<Vector2> DrawGrid (Rect rect, float sizeX, float sizeY, float padX, float padY, float border, int cells)
	{
		List<Vector2> list = new List<Vector2>();

		sizeX += border * 2.0f;
		sizeY += border * 2.0f;

		float x = rect.x;
		float y = rect.y;
		float mx = x + rect.width;
		float my = y + rect.height;
		int count = 0;

		GUIStyle style = Config.Instance.skin.box;

		while (y < my)
		{
			if (y + sizeY > my) break;

			while (x < mx)
			{
				if (x + sizeX > mx) break;

				Vector2 pos = new Vector2(x, y);
				GUI.Box(new Rect(x, y, sizeX, sizeY), "", style);
				x += sizeX + padX;
				pos.x += border;
				pos.y += border;
				list.Add(pos);

				if (++count >= cells) return list;
			}
			y += sizeY + padY;
			x = rect.x;
		}
		return list;
	}

	/// <summary>
	/// Whether the specified screen rectangle contains the mouse position.
	/// </summary>

	static public bool ContainsMouse (Rect rect)
	{
		Vector2 pos = Input.mousePosition;
		pos.y = Screen.height - pos.y;
		return rect.Contains(pos);
	}

	/// <summary>
	/// Gets the mouse position in GUI space (top-left based).
	/// </summary>

	static public Vector2 GetMousePos ()
	{
		Vector2 pos = Input.mousePosition;
		pos.y = Screen.height - pos.y;
		return pos;
	}

	/// <summary>
	/// Gets the screen position of the specified world position.
	/// </summary>

	static public Vector3 GetScreenPos (Vector3 worldPos)
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(worldPos);
		pos.y = Screen.height - pos.y;
		return pos;
	}

	/// <summary>
	/// Whether this screen position is currently visible.
	/// </summary>

	static public bool IsVisible (Vector3 screenPos)
	{
		return !(screenPos.z < 0f ||
			screenPos.x < 0f ||
			screenPos.y < 0f ||
			screenPos.x > Screen.width ||
			screenPos.y > Screen.height);
	}
}