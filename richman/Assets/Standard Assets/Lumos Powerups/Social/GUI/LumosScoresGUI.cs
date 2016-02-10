// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// User interface for displaying leaderboard scores.
/// </summary>
public static class LumosScoresGUI
{
	/// <summary>
	/// The friend scores scroll position.
	/// </summary>
	static Vector2 friendScoresScrollPos;

	/// <summary>
	/// All scores scroll position.
	/// </summary>
	static Vector2 allScoresScrollPos;

	/// <summary>
	/// Displays the scores UI.
	/// </summary>
	/// <param name="windowRect">The bounding rect of the window.</param>
	public static void OnGUI (Rect windowRect)
	{
		if (LumosLeaderboardsGUI.currentLeaderboard == null) {
			return;
		}

		if (LumosLeaderboardsGUI.currentLeaderboard.scores == null) {
			GUILayout.Label("Loading scores...");
			return;
		}

		if (GUILayout.Button("Leaderboards List", GUILayout.ExpandWidth(false))) {
			LumosSocialGUI.ShowWindow(LumosGUIWindow.Leaderboards);
		}

		LumosSocialGUI.DrawDivider();

		// Title
		GUILayout.Label(LumosLeaderboardsGUI.currentLeaderboard.title);

		LumosSocialGUI.DrawDivider();

		// Friend Scores
		if (LumosLeaderboardsGUI.currentLeaderboard.friendScores != null) {
			DisplayScoreLabel("Friends");
			GUILayout.BeginVertical(GUI.skin.box);
			friendScoresScrollPos = GUILayout.BeginScrollView(friendScoresScrollPos);
			DisplayScoreData(LumosLeaderboardsGUI.currentLeaderboard.friendScores);
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}

		// All Scores
		DisplayScoreLabel("All Scores");
		GUILayout.BeginVertical(GUI.skin.box);
		allScoresScrollPos = GUILayout.BeginScrollView(allScoresScrollPos);
		DisplayScoreData(LumosLeaderboardsGUI.currentLeaderboard.scores);
		GUILayout.EndScrollView();
		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(false))) {
				Social.LoadScores(LumosLeaderboardsGUI.currentLeaderboard.id, null);
			}

			if (GUILayout.Button("More...", GUILayout.ExpandWidth(false))) {
				var length = LumosLeaderboardsGUI.currentLeaderboard.scores.Length -1;
				var lastScore = LumosLeaderboardsGUI.currentLeaderboard.scores[length];

				LumosLeaderboardsGUI.currentLeaderboard.LoadScores(1, lastScore.rank,
					scores => {
						// Nothing yet.
					});
			}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Displays the score label.
	/// </summary>
	/// <param name="label">Label.</param>
	static void DisplayScoreLabel (string label)
	{
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(label);
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Displays score data.
	/// </summary>
	/// <param name="scores">Scores.</param>
	static void DisplayScoreData (IScore[] scores)
	{
		foreach (var score in scores) {
			GUILayout.BeginHorizontal();
				GUILayout.Label(score.rank.ToString());
				GUILayout.Label(LumosSocialGUI.defaultAvatarIcon);

				GUILayout.BeginVertical();
					GUILayout.Label(score.userID);
					GUILayout.Label(score.value.ToString());
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}
}
