// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// User interface for displaying the user's profile.
/// </summary>
public static class LumosProfileGUI
{
	static readonly GUIContent achievementsLabel = new GUIContent("Achievements", "View the achievements window.");
	static readonly GUIContent leaderboardsLabel = new GUIContent("Leaderboards", "View the leaderboards window.");
	static readonly GUIContent settingsLabel = new GUIContent("Settings", "View the settings window.");

	/// <summary>
	/// The scroll position for the other user data pane.
	/// </summary>
	static Vector2 otherScrollPos;

	/// <summary>
	/// The scroll position for the friends pane.
	/// </summary>
	static Vector2 friendsScrollPos;

	/// <summary>
	/// The scroll position for the scores pane.
	/// </summary>
	static Vector2 scoresScrollPos;

	/// <summary>
	/// The friend to add.
	/// </summary>
	static string friendToAdd = "";

	/// <summary>
	/// Displays the profile UI.
	/// </summary>
	/// <param name="windowRect">The bounding rect of the window.</param>
	public static void OnGUI (Rect windowRect)
	{
		if (LumosSocialGUI.currentUser == null) {
			LumosSocialGUI.statusMessage = "You must login before viewing your profile.";
			LumosSocialGUI.DrawLoginButton();
			return;
		}

		GUILayout.BeginHorizontal();
			var avatar = GetAvatar();

			if (avatar != null) {
				GUILayout.Label(GetAvatar(), GUILayout.MaxWidth(LumosSocialGUI.avatarSize), GUILayout.MaxHeight(LumosSocialGUI.avatarSize));
			}

			GUILayout.BeginVertical();
				GUILayout.Label(LumosSocialGUI.currentUser.userID);
				GUILayout.Label(LumosSocialGUI.currentUser.email);
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
				if (GUILayout.Button(achievementsLabel)) {
					Social.ShowAchievementsUI();
				}

				if (GUILayout.Button(leaderboardsLabel)) {
					Social.ShowLeaderboardUI();
				}

				if (GUILayout.Button(settingsLabel)) {
					LumosSocialGUI.ShowWindow(LumosGUIWindow.Settings);
				}
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		LumosSocialGUI.DrawDivider();

		GUILayout.BeginHorizontal();
			// Other info.
			if (LumosSocialGUI.currentUser.other != null) {
				GUILayout.BeginVertical();
					GUILayout.Label("Other");

					otherScrollPos = GUILayout.BeginScrollView(otherScrollPos);

					foreach (var other in LumosSocialGUI.currentUser.other) {
						GUILayout.BeginHorizontal();
							GUILayout.Label(other.Key);
							GUILayout.Label(other.Value.ToString());
						GUILayout.EndHorizontal();
					}

					GUILayout.EndScrollView();
				GUILayout.EndVertical();
			}

			// Friends list.
			GUILayout.BeginVertical();
				GUILayout.Label("Friends");

				friendsScrollPos = GUILayout.BeginScrollView(friendsScrollPos);

				GUILayout.BeginHorizontal();
					friendToAdd = GUILayout.TextField(friendToAdd);

					if (GUILayout.Button("Send Friend Request")) {
						if (friendToAdd.Length > 0) {
							LumosSocialGUI.currentUser.SendFriendRequest(friendToAdd,
								success => {
									if (success) {
										LumosSocialGUI.statusMessage = "Friend request sent.";
									} else {
										LumosSocialGUI.statusMessage = "There was a problem processing the friend request. Please try again.";
									}
								});
						}
					}
				GUILayout.EndHorizontal();

				if (LumosSocialGUI.currentUser.friendRequests != null) {
					foreach (var request in LumosSocialGUI.currentUser.friendRequests) {
						GUILayout.BeginHorizontal();
							GUILayout.Label(request.id);

							if (GUILayout.Button("Accept", GUILayout.ExpandWidth(false))) {
								LumosSocialGUI.currentUser.AcceptFriendRequest(request.id,
									success => {
										if (success) {
											LumosSocialGUI.statusMessage = "Friend request accepted.";
										} else {
											LumosSocialGUI.statusMessage = "There was a problem accepting the friend request. Please try again.";
										}
									});
							}

							if (GUILayout.Button("Decline", GUILayout.ExpandWidth(false))) {
								LumosSocialGUI.currentUser.DeclineFriendRequest(request.id,
									success => {
										if (success) {
											LumosSocialGUI.statusMessage = "Friend request declined.";
										} else {
											LumosSocialGUI.statusMessage = "There was a problem declining the friend request. Please try again.";
										}
									});
							}
						GUILayout.EndHorizontal();
					}
				}

				if (LumosSocialGUI.currentUser.friends != null) {
					foreach (var friend in LumosSocialGUI.currentUser.friends) {
						GUILayout.BeginHorizontal();
							GUILayout.Label(friend.id);

							if (GUILayout.Button("Remove")) {
								LumosSocialGUI.currentUser.RemoveFriend(friend.id,
									success => {
										if (success) {
											LumosSocialGUI.statusMessage = "Friend removed.";
										} else {
											LumosSocialGUI.statusMessage = "There was a problem removing this friend. Please try again.";
										}
									});
							}
						GUILayout.EndHorizontal();
					}
				}

				GUILayout.EndScrollView();
			GUILayout.EndVertical();

			LumosSocialGUI.DrawDivider();

			// Scores.
			GUILayout.BeginVertical();
				GUILayout.Label("High Scores");

				scoresScrollPos = GUILayout.BeginScrollView(scoresScrollPos);

				if (LumosSocialGUI.currentUser.scores != null) {
					foreach (var score in LumosSocialGUI.currentUser.scores) {
						GUILayout.Label(score.leaderboardID);
						GUILayout.BeginHorizontal();
							GUILayout.Label(score.value.ToString());
						GUILayout.EndHorizontal();
						LumosSocialGUI.DrawDivider();
					}
				}

				GUILayout.EndScrollView();
			GUILayout.EndVertical();

			LumosSocialGUI.DrawDivider();
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Gets the user's avatar image or a default icon.
	/// </summary>
	/// <returns>The user's avatar.</returns>
	static Texture2D GetAvatar ()
	{
		var avatar = Social.localUser.image;

		// Try using a default image, if one's set.
		if (avatar == null) {
			avatar = LumosSocialGUI.defaultAvatarIcon;
		}

		return avatar;
	}
}
