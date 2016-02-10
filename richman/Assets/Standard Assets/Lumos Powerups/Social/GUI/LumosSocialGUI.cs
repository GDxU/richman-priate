// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum LumosGUIWindow { None, Achievements, Login, Leaderboards, Profile, Registration, ResetPassword, Scores, Settings }

/// <summary>
/// Manages windows for displaying information like leaderboards and achievements.
/// </summary>
public class LumosSocialGUI : MonoBehaviour
{
	#region Public Inspector Settings

	/// <summary>
	/// The default user icon.
	/// </summary>
	public Texture2D defaultAvatar;

	/// <summary>
	/// The default achievement icon.
	/// </summary>
	public Texture2D defaultAchievementIcon;

	public static Texture2D defaultAvatarIcon { get { return instance.defaultAvatar; } }
	public static Texture2D defaultAchIcon { get { return instance.defaultAchievementIcon; } }

	#endregion

	static LumosUser _currentUser;

	/// <summary>
	/// The current user.
	/// </summary>
	public static LumosUser currentUser
	{
		get {
			if (_currentUser == null) {
				_currentUser = Social.localUser as LumosUser;
			}

			return _currentUser;
		}
	}

	/// <summary>
	/// A message to communicate problems to the user.
	/// </summary>
	public static string statusMessage { private get; set; }

	/// <summary>
	/// Whether a request is currently in progress.
	/// </summary>
	public static bool inProgress { get; set; }

	/// <summary>
	/// The maximum width and height of the user's icon.
	/// </summary>
	public const float avatarSize = 50;

	/// <summary>
	/// The height of dividers between GUI elements.
	/// </summary>
	const float dividerHeight = 10;

	/// <summary>
	/// Titles to display above each window.
	/// </summary>
	static readonly Dictionary<LumosGUIWindow, string> windowTitles = new Dictionary<LumosGUIWindow, string>() {
		{ LumosGUIWindow.Achievements, "Achievements" },
		{ LumosGUIWindow.Login, "Login" },
		{ LumosGUIWindow.Leaderboards, "Leaderboards" },
		{ LumosGUIWindow.Profile, "Profile" },
		{ LumosGUIWindow.Registration, "Registration" },
		{ LumosGUIWindow.ResetPassword, "Reset Password" },
		{ LumosGUIWindow.Scores, "Scores" },
		{ LumosGUIWindow.Settings, "Settings" }
	};

	static readonly GUIContent loginLabel = new GUIContent("Login", "Go to the login window.");
	static readonly GUIContent closeLabel = new GUIContent("\u00D7", "Close this window.");

	/// <summary>
	/// The pane currently displaying.
	/// </summary>
	static LumosGUIWindow visibleWindow;

	/// <summary>
	/// The bounding rect of the window.
	/// </summary>
	Rect windowRect;

	static LumosSocialGUI instance;
	LumosSocialGUI () {}

	void Awake()
	{
		// Prevent multiple instances of LumosSocialGUI from existing.
		// Necessary because DontDestroyOnLoad keeps the object between scenes.
		if (instance != null) {
			Destroy(gameObject);
			return;
		}

		instance = this;
		Social.Active = new LumosSocial();
		windowRect = DetermineWindowRect();
	}

	void OnGUI()
	{
		if (visibleWindow == LumosGUIWindow.None) {
			return;
		}

		windowRect = GUI.Window(0, windowRect, SocialWindow, windowTitles[visibleWindow]);
	}

	/// <summary>
	/// Displays a window encompassing the various GUI panes.
	/// </summary>
	/// <param name="windowID">The window ID.</param>
	void SocialWindow (int windowID)
	{
		GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.FlexibleSpace();

			if (Social.localUser != null) {
				if (GUILayout.Button("My Profile", GUILayout.ExpandWidth(false))) {
					ShowWindow(LumosGUIWindow.Profile);
				}
			}

			if (GUILayout.Button(closeLabel)) {
				HideWindow();
			}
		GUILayout.EndHorizontal();

		DrawDivider();

		switch(visibleWindow) {
			case LumosGUIWindow.Achievements:
				LumosAchievementsGUI.OnGUI(windowRect);
				break;
			case LumosGUIWindow.Leaderboards:
				LumosLeaderboardsGUI.OnGUI(windowRect);
				break;
			case LumosGUIWindow.Login:
				LumosLoginGUI.OnGUI(windowRect);
				break;
			case LumosGUIWindow.Profile:
				LumosProfileGUI.OnGUI(windowRect);
				break;
			case LumosGUIWindow.Registration:
				LumosRegistrationGUI.OnGUI(windowRect);
				break;
			case LumosGUIWindow.ResetPassword:
				LumosResetPasswordGUI.OnGUI(windowRect);
				break;
			case LumosGUIWindow.Scores:
				LumosScoresGUI.OnGUI(windowRect);
				break;
			case LumosGUIWindow.Settings:
				LumosSettingsGUI.OnGUI(windowRect);
				break;
		}

		// Display status message box.
		if (statusMessage != null && statusMessage != "") {
			GUILayout.FlexibleSpace();

			GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label(statusMessage);
			GUILayout.EndVertical();
		}

		GUI.enabled = true;
	}

	/// <summary>
	/// Displays the specified window.
	/// </summary>
	/// <param name="window">The window to show.</param>
	public static void ShowWindow (LumosGUIWindow window)
	{
		statusMessage = null;
		visibleWindow = window;
	}

	/// <summary>
	/// Hides the active window.
	/// </summary>
	public static void HideWindow ()
	{
		visibleWindow = LumosGUIWindow.None;
	}

	// Displays a visible divider between GUI elements.
	public static void DrawDivider ()
	{
		GUILayout.Space(dividerHeight);
	}

	// Displays a login button.
	public static void DrawLoginButton ()
	{
		if (GUILayout.Button(loginLabel, GUILayout.ExpandWidth(false))) {
			ShowWindow(LumosGUIWindow.Login);
		}
	}
	
	public static bool isShowing ()
	{
		return visibleWindow != LumosGUIWindow.None;
	}

	/// <summary>
	/// Centers the window.
	/// </summary>
	static Rect DetermineWindowRect ()
	{
		var width = Screen.width - (Screen.width * 0.3f);
		var height = Screen.height - (Screen.height * 0.3f);
		var x = (Screen.width - width) / 2;
		var y = (Screen.height - height) / 2;
		var rect = new Rect(x, y, width, height);
		return rect;
	}
}
