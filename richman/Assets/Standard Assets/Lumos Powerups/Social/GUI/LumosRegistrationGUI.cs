// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using UnityEngine;

/// <summary>
/// User interface for the registration form.
/// </summary>
public static class LumosRegistrationGUI
{
	static readonly GUIContent usernameLabel = new GUIContent("Username", "Your unique indentifier.");
	static readonly GUIContent emailLabel = new GUIContent("Email", "Your email address.");
	static readonly GUIContent passwordLabel = new GUIContent("Password", "Your password.");
	static readonly GUIContent confirmPasswordLabel = new GUIContent("Confirm Password", "Your password as typed above.");
	static readonly GUIContent registerLabel = new GUIContent("Register", "Create a new account.");
	static readonly GUIContent loginLabel = new GUIContent("Already signed up?", "Login with your existing username and password.");

	/// <summary>
	/// The user's username.
	/// </summary>
	static string username = "";

	/// <summary>
	/// The user's email address.
	/// </summary>
	static string email = "";

	/// <summary>
	/// The user's password.
	/// </summary>
	static string password = "";

	/// <summary>
	/// Confirmation of the password.
	/// </summary>
	static string passwordConfirmation = "";

	/// <summary>
	/// Displays the registration UI.
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

		// Email address field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(emailLabel);
			email = GUILayout.TextField(email, GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// Password field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(passwordLabel);
			password = GUILayout.PasswordField(password, '\u2022', GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// Password confirmation field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(confirmPasswordLabel);
			passwordConfirmation = GUILayout.PasswordField(passwordConfirmation, '\u2022', GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// Submit button.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button(registerLabel, GUILayout.Width(halfWidth))) {
				RegisterNewUser();
			}
		GUILayout.EndHorizontal();

		LumosSocialGUI.DrawDivider();

		// Login button.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button(loginLabel, GUILayout.Width(halfWidth))) {
				LumosSocialGUI.ShowWindow(LumosGUIWindow.Login);
			}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Registers the new user.
	/// </summary>
	static void RegisterNewUser()
	{
		if (username.Length < 1 || password.Length < 1 || email.Length < 1) {
			LumosSocialGUI.statusMessage = "Please fill in all the fields.";
			return;
		}

		if (password != passwordConfirmation) {
			LumosSocialGUI.statusMessage = "Please supply matching passwords.";
			return;
		}

		LumosSocialGUI.inProgress = true;
		LumosSocialGUI.statusMessage = "Registering...";
		var user = new LumosUser(username, password);
		user.email = email;

		LumosSocial.RegisterUser(user,
			success => {
				LumosSocialGUI.inProgress = false;

				if (success) {
					LumosSocialGUI.statusMessage = null;
					LumosSocialGUI.HideWindow();
				} else {
					LumosSocialGUI.statusMessage = "There was a problem registering. Please try again.";
				}
			});
	}
}
