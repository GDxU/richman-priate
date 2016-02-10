using UnityEngine;
using System.Collections;

public class LumosFeedbackGUI 
{
	public delegate void CloseHandler();

	/// <summary>
	/// A callback that triggers when the window is closed.
	/// </summary>
	public static event CloseHandler windowClosed;

	/// <summary>
	/// The skin to use for the GUI window.
	/// </summary>
	public static GUISkin skin { get; set; }

	const int windowId = 345992; // Random to hopefully be a unique window ID.
	const int margin = 10;
	static Rect windowRect;
	static bool visible;
	static bool sentMessage;

	static string email = "";
	static string message = "";
	static string category = "Feature Request";

	LumosFeedbackGUI () {}
	
	
	/// <summary>
	/// Displays a window where the player can enter their email and feedback.
	/// </summary>
	public static void OnGUI ()
	{
		if (!visible) {
			return;
		}

		if (skin != null) {
			GUI.skin = skin;
		}

		windowRect = new Rect(margin, margin, Screen.width - (2 * margin),
		                      Screen.height - (2 * margin));
		// Register the window
		GUILayout.Window(windowId, windowRect, DisplayWindow, "Leave Feedback");
	}

	/// <summary>
	/// Displays the window.
	/// </summary>
	/// <param name="windowID">The window's ID.</param>
	static void DisplayWindow (int windowId)
	{
		GUI.BringWindowToFront(windowId);

		GUILayout.BeginHorizontal();

			GUILayout.Label("Email (optional)", GUILayout.ExpandWidth(false));
			email = GUILayout.TextField(email, 320);

		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();

			GUILayout.Label("Type", GUILayout.ExpandWidth(false));
			category = GUILayout.TextField(category);

		GUILayout.EndHorizontal();

		if (!sentMessage) {
			message = GUILayout.TextArea(message, GUILayout.MinHeight(200));
		} else {
			GUILayout.BeginHorizontal();

				GUILayout.FlexibleSpace();
				GUILayout.Label("Your message has been sent.");
				GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			if (!sentMessage) {
				if (GUILayout.Button("Cancel")) {
					HideDialog();
				}

				if (GUILayout.Button("Send")) {
					LumosFeedback.Record(message, email, category);
					message = "";
					sentMessage = true;
				}
			} else {
				if (GUILayout.Button("OK")) {
					HideDialog();
					sentMessage = false;
				}
			}

		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Shows the feedback window.
	/// </summary>
	public static void ShowDialog ()
	{
		visible = true;
	}

	/// <summary>
	/// Hides the feedback window.
	/// </summary>
	public static void HideDialog ()
	{
		// Trigger callback function if one has been specified.
		if (windowClosed != null) {
			windowClosed();
		}

		visible = false;
	}
}
