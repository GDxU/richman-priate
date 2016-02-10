// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using UnityEngine;

/// <summary>
/// User interface for displaying a dialog to reset the user's password.
/// </summary>
public static class LumosResetPasswordGUI
{
	static readonly GUIContent backLabel = new GUIContent("Back to Login", "Return to the login window.");
	static readonly GUIContent usernameLabel = new GUIContent("Username", "Your unique indentifier.");
	static readonly GUIContent resetLabel = new GUIContent("Reset Password", "Request that your password be reset.");

	/// <summary>
	/// The user's username.
	/// </summary>
	static string username = "";

	/// <summary>
	/// Displays the password reset UI.
	/// </summary>
	/// <param name="windowRect">The bounding rect of the window.</param>
	public static void OnGUI (Rect windowRect)
	{
		var halfWidth = windowRect.width / 2;

		if (GUILayout.Button(backLabel, GUILayout.ExpandWidth(false))) {
			LumosSocialGUI.ShowWindow(LumosGUIWindow.Login);
		}

		LumosSocialGUI.DrawDivider();

       	// Username field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(usernameLabel);
			username = GUILayout.TextField(username, GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// Submit button.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button(resetLabel, GUILayout.Width(halfWidth))) {
				LumosSocial.ResetPassword(username,
					success => {
						if (success) {
							LumosSocialGUI.statusMessage = "An email has been sent to confirm your password reset.";
						} else {
							LumosSocialGUI.statusMessage = "There was a problem resetting your password. Please try again.";
						}
				});
			}
		GUILayout.EndHorizontal();
    }
}
