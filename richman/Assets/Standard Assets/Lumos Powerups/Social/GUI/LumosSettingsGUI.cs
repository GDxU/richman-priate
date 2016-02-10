// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// User interface for user settings.
/// </summary>
public static class LumosSettingsGUI
{
	static readonly GUIContent nameLabel = new GUIContent("Name", "Your name.");
	static readonly GUIContent emailLabel = new GUIContent("Email", "Your email address.");
	static readonly GUIContent currentPasswordLabel = new GUIContent("Current Password", "Your current password.");
	static readonly GUIContent passwordLabel = new GUIContent("New Password", "Your new password.");
	static readonly GUIContent confirmPasswordLabel = new GUIContent("Confirm New Password", "Your new password as typed above.");
	static readonly GUIContent otherLabel = new GUIContent("Other", "Additional information.");
	static readonly GUIContent updateLabel = new GUIContent("Update Settings", "Save the updated settings.");

	/// <summary>
	/// The user's name.
	/// </summary>
	static string name = "";

	/// <summary>
	/// The user's current password.
	/// </summary>
	static string currentPassword = "";

	/// <summary>
	/// The user's new password.
	/// </summary>
	static string password = "";

	/// <summary>
	/// Confirmation of the new password.
	/// </summary>
	static string passwordConfirmation = "";

	/// <summary>
	/// The user's email address.
	/// </summary>
	static string email = "";

	/// <summary>
	/// Additional information about the user.
	/// </summary>
	static Dictionary<string, object> other;

	/// <summary>
	/// Scroll position.
	/// </summary>
	static Vector2 scrollPos;

	/// <summary>
	/// Displays the settings UI.
	/// </summary>
	/// <param name="windowRect">The bounding rect of the window.</param>
	public static void OnGUI (Rect windowRect)
	{
		if (LumosSocialGUI.currentUser == null) {
			LumosSocialGUI.statusMessage = "You must login before viewing your settings.";
			LumosSocialGUI.DrawLoginButton();
			return;
		}

		var halfWidth = windowRect.width / 2;
		scrollPos = GUILayout.BeginScrollView(scrollPos);

		// Name field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(nameLabel);
			name = GUILayout.TextField(name, GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// Current Password field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(currentPasswordLabel);
		currentPassword = GUILayout.PasswordField(currentPassword, '*', GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// New Password field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(passwordLabel);
			password = GUILayout.PasswordField(password, '*', GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// New Password confirmation field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(confirmPasswordLabel);
			passwordConfirmation = GUILayout.PasswordField(passwordConfirmation, '*', GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		// Email address field.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(emailLabel);
			email = GUILayout.TextField(email, GUILayout.Width(halfWidth));
		GUILayout.EndHorizontal();

		if (LumosSocialGUI.currentUser.other != null) {
			if (other == null) {
				other = new Dictionary<string, object>(LumosSocialGUI.currentUser.other);
			}

			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(otherLabel);

				GUILayout.BeginVertical(GUILayout.Width(halfWidth));

				foreach (var entry in other) {
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(entry.Key);
						other[entry.Key] = GUILayout.TextField(entry.Value as string, GUILayout.Width(halfWidth));
					GUILayout.EndHorizontal();
				}

				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		// Submit button.
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(updateLabel, GUILayout.Width(halfWidth))) {
				SaveSettings();
			}
		GUILayout.EndHorizontal();

		GUILayout.EndScrollView();
	}

	/// <summary>
	/// Saves the settings.
	/// </summary>
	static void SaveSettings()
	{
		if (password.Length > 0 && password != passwordConfirmation) {
			LumosSocialGUI.statusMessage = "The supplied passwords do not match.";
			return;
		} else if (password.Length > 0 && currentPassword.Length == 0) {
			LumosSocialGUI.statusMessage = "You must also provide your current password in order to change it.";
			return;
		} else if (currentPassword.Length > 0 && password.Length == 0) {
			LumosSocialGUI.statusMessage = "Your new password cannot be blank.";
			return;
		}

		LumosSocialGUI.inProgress = true;
		LumosSocialGUI.statusMessage = "Updating settings...";

		LumosSocialGUI.currentUser.UpdateInfo(name, email, currentPassword, password, other,
			success => {
				if (success) {
					LumosSocialGUI.inProgress = false;
					other = null; // Force dictionary to reload
					LumosSocialGUI.statusMessage = "Settings saved.";
				} else {
					LumosSocialGUI.statusMessage = "There was a problem saving settings. Please try again.";
				}
			});
	}
}
