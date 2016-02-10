// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// User interface to display leaderboards.
/// </summary>
public static class LumosLeaderboardsGUI
{
	/// <summary>
	/// The current leaderboard.
	/// </summary>
	public static LumosLeaderboard currentLeaderboard { get; private set; }

	/// <summary>
	/// The offset.
	/// </summary>
	static int offset;

	/// <summary>
	/// Displays the leaderboards UI.
	/// </summary>
	/// <param name="windowRect">The bounding rect of the window.</param>
	public static void OnGUI (Rect windowRect)
	{
		if (LumosSocialGUI.currentUser == null) {
			LumosSocialGUI.statusMessage = "You must login before viewing leaderboards.";
			LumosSocialGUI.DrawLoginButton();
			return;
		}

		if (LumosSocial.leaderboards == null) {
			LumosSocialGUI.statusMessage = "Loading leaderboards...";

			if (!LumosSocialGUI.inProgress) {
				LumosSocial.LoadLeaderboardDescriptions(success => {
					LumosSocialGUI.statusMessage = null;
				});
				LumosSocialGUI.inProgress = true;
			}

			return;
		}

		if (LumosSocial.leaderboards.Length > 0) {
			foreach (var leaderboard in LumosSocial.leaderboards) {
				if (leaderboard.loading) {
					GUILayout.Label("Loading...");
					GUI.enabled = false;
				}

				if (GUILayout.Button(leaderboard.title)) {
					currentLeaderboard = leaderboard as LumosLeaderboard;
					LumosSocialGUI.ShowWindow(LumosGUIWindow.Scores);

					if (currentLeaderboard.scores == null) {
						Social.LoadScores(currentLeaderboard.id, null);
					}
				}

				GUI.enabled = true;
			}

			LumosSocialGUI.DrawDivider();
		}
	}
}
