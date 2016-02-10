// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

//using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// User interface for displaying achievements.
/// </summary>
public static class LumosAchievementsGUI
{
	/// <summary>
	/// The window scroll position.
	/// </summary>
	static Vector2 scrollPos;

	/// <summary>
	/// Descriptions of the available achievements.
	/// </summary>
	static IAchievementDescription[] achievementDescriptions;

	/// <summary>
	/// Displays the achievements UI.
	/// </summary>
	/// <param name="windowRect">The bounding rect of the window.</param>
	public static void OnGUI (Rect windowRect)
	{
		if (LumosSocialGUI.currentUser == null) {
			LumosSocialGUI.statusMessage = "You must login before viewing your achievements.";
			LumosSocialGUI.DrawLoginButton();
			return;
		}

		// Load achievements if necessary.
		if (achievementDescriptions == null) {
			LumosSocialGUI.statusMessage = "Loading achievements...";

			if (!LumosSocialGUI.inProgress) {
				LumosSocialGUI.inProgress = true;
				Social.LoadAchievements(null);

				Social.LoadAchievementDescriptions(
					descriptions => {
						LumosSocialGUI.inProgress = false;
						achievementDescriptions = descriptions;
						LumosSocialGUI.statusMessage = null;
					});
			}

			return;
		}

		scrollPos = GUILayout.BeginScrollView(scrollPos);

		foreach (var description in achievementDescriptions) {
			// Skip achievements the user isn't supposed to see.
			if (description.hidden && !LumosSocial.HasAchievement(description.id)) {
				continue;
			}

			GUI.enabled = !LumosSocial.HasAchievement(description.id);

			GUILayout.BeginHorizontal(GUI.skin.box);
				var icon = GetIcon(description);

				if (icon != null) {
					GUILayout.Label(icon);
				}

				GUILayout.BeginVertical();
					GUILayout.Label(description.title);

					if (LumosSocial.HasAchievement(description.id)) {
						GUILayout.Label(description.achievedDescription);
					} else {
						GUILayout.Label(description.unachievedDescription);
					}
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			LumosSocialGUI.DrawDivider();
		}

		GUILayout.EndScrollView();
	}

	/// <summary>
	/// Gets the achievement's icon or a default one.
	/// </summary>
	/// <returns>The achievement's icon.</returns>
	static Texture2D GetIcon (IAchievementDescription description)
	{
		var icon = description.image;

		// Try using a default image, if one's set.
		if (icon == null) {
			icon = LumosSocialGUI.defaultAchIcon;
		}

		return icon;
	}
}
