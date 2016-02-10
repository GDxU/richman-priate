// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// User interface for the login form.
/// </summary>
public static class LumosLoginGUI
{
	static readonly GUIContent usernameLabel = new GUIContent("Username", "Your unique indentifier.");
	static readonly GUIContent passwordLabel = new GUIContent("Password", "Your password.");
	static readonly GUIContent loginLabel = new GUIContent("Login", "Login with the given username and password.");
	static readonly GUIContent forgotPasswordLabel = new GUIContent("Forgot password?", "Reset your password via email.");
	static readonly GUIContent registerLabel = new GUIContent("Need an account?", "Create a new account.");

	/// <summary>
	/// The user's username.
	/// </summary>
	static string username = "";

	/// <summary>
	/// The user's password.
	/// </summary>
	static string password = "";

	/// <summary>
	/// Displays the login UI.
	/// </summary>
	/// <param name="windowRect">The bounding rect of the window.</param>
	public static void OnGUI (Rect windowRect)
	{
		var halfWidth = windowRect.width / 2;
		GUI.enabled = !LumosSocialGUI.inProgress;

		// Username field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(usernameLabel);
			username = GUILayout.TextField(username, GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// Password field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(passwordLabel);
			password = GUILayout.PasswordField(password, '\u2022', GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// Submit button.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button(loginLabel, GUILayout.Width(halfWidth))) {
				SubmitLoginCredentials();
			}
		GUILayout.EndHorizontal();

		LumosSocialGUI.DrawDivider();

		// Forgot password button.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button(forgotPasswordLabel, GUILayout.Width(halfWidth))) {
				LumosSocialGUI.ShowWindow(LumosGUIWindow.ResetPassword);
			}
		GUILayout.EndHorizontal();

		// Register new account button.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button(registerLabel, GUILayout.Width(halfWidth))) {
				LumosSocialGUI.ShowWindow(LumosGUIWindow.Registration);
				return;
			}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Submits the username and password.
	/// </summary>
	static void SubmitLoginCredentials ()
	{
		if (username.Length < 1) {
			LumosSocialGUI.statusMessage = "Please enter a username.";
			return;
		}

		if (password.Length < 1) {
			LumosSocialGUI.statusMessage = "Please enter a password.";
			return;
		}

		LumosSocialGUI.inProgress = true;
		LumosSocialGUI.statusMessage = "Logging in...";
		var user = new LumosUser(username, password);

		Social.Active.Authenticate(user,
			success => {
				LumosSocialGUI.inProgress = false;

				if (success) {
					LumosSocialGUI.HideWindow();
					LumosSocialGUI.statusMessage = null;
				} else {
					LumosSocialGUI.statusMessage = "There was a problem signing in. Perhaps your username and password do not match.";
				}
			});
	}
}
