// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;


// Functions for fetching achievements and recording progress.
public partial class LumosSocial
{
	static Dictionary<string, LumosAchievement> _achievements = new Dictionary<string, LumosAchievement>();

	/// <summary>
	/// Achievement information.
	/// </summary>
	public static IAchievementDescription[] achievementDescriptions { get; private set; }

	/// <summary>
	/// The achievements that the user has earned or made progress on.
	/// </summary>
	public static IAchievement[] achievements
	{
		get {
			if (_achievements == null) {
				return null;
			} else {
				return _achievements.Values.ToArray();
			}
		}
	}

	/// <summary>
	/// Whether we're currently fetching the achievement descriptions.
	/// </summary>
	public static bool loadingAchievementDescriptions { get; private set; }

	/// <summary>
	/// Whether we're currently fetching the achievements.
	/// </summary>
	public static bool loadingAchievements { get; private set; }

	/// Creates an empty achievement object.
	public IAchievement CreateAchievement ()
	{
		var achievement = new LumosAchievement();
		return achievement;
	}

	/// Fetches the achievement descriptions.
	public void LoadAchievementDescriptions (Action<IAchievementDescription[]> callback)
	{
		if (achievementDescriptions == null && !loadingAchievementDescriptions) {
			// Load the achievement descriptions from the server.
			loadingAchievementDescriptions = true;
			var endpoint = "/achievements";

			LumosRequest.Send(LumosSocial.instance, endpoint, LumosRequest.Method.GET,
				success => {
					var resp = success as IList;
					achievementDescriptions = new LumosAchievementDescription[resp.Count];

					for (int i = 0; i < resp.Count; i++) {
						achievementDescriptions[i] = new LumosAchievementDescription(resp[i] as Dictionary<string, object>);
					}

					loadingAchievementDescriptions = false;

					if (callback != null) {
						callback(achievementDescriptions);
					}
				},
				error => {
					loadingAchievementDescriptions = false;

					if (callback != null) {
						callback(null);
					}
				});
		} else {
			// Use the cached achievement descriptions.
			callback(achievementDescriptions);
		}
	}

	/// Loads the player's earned achievements.
	public void LoadAchievements (Action<IAchievement[]> callback)
	{
		if ((achievements == null || achievements.Length == 0) && !loadingAchievements) {
			// Load the achievements from the server.
			loadingAchievements = true;
			var endpoint = "/users/" + localUser.id + "/achievements";

			LumosRequest.Send(LumosSocial.instance, endpoint, LumosRequest.Method.GET,
				success => {
					var resp = success as IList;
					_achievements = new Dictionary<string, LumosAchievement>();

					foreach (Dictionary<string, object> info in resp) {
						var achievement = new LumosAchievement(info);
						_achievements[achievement.id] = achievement;
					}

					loadingAchievements = false;

					if (callback != null) {
						callback(achievements);
					}
				},
				error => {
					loadingAchievements = false;

					if (callback != null) {
						callback(null);
					}
				});
		} else {
			// Use the cached achievements.
			if (callback != null) {
				callback(achievements);
			}
		}
	}

	/// Updates a player's progress for an achievement.
	public void ReportProgress (string achievementID, double percentCompleted, Action<bool> callback)
	{
		var achievement = GetAchievement(achievementID);

		if (achievement == null) {
			// Create new achievement.
			achievement = new LumosAchievement(achievementID, percentCompleted, false, DateTime.Now);
			_achievements[achievement.id] = achievement;
		} else {
			// Update existing achievement.
			achievement.percentCompleted = percentCompleted;
		}

		achievement.ReportProgress(callback);
	}

	/// Shows the achievements UI.
	public void ShowAchievementsUI()
	{
		LumosSocialGUI.ShowWindow(LumosGUIWindow.Achievements);
	}

	#region Added Functions

	/// <summary>
	/// Awards an achievement.
	/// </summary>
	/// <param name="achievementID">The achievement identifier.</param>
	/// <param name="callback">Callback.</param>
	public static void AwardAchievement (string achievementID, Action<bool> callback)
	{
		Social.ReportProgress(achievementID, 100, callback);
	}

	/// <summary>
	/// Gets an achievement by its ID.
	/// </summary>
	/// <param name="achievementID">The achievement identifier.</param>
	/// <returns type="LumosAchievement">The achievement.</returns>
	public static LumosAchievement GetAchievement (string achievementID)
	{
		if (_achievements != null && _achievements.ContainsKey(achievementID)) {
			return _achievements[achievementID];
		} else {
			return null;
		}
	}

	/// <summary>
	/// Determines whether the user has earned an achievement.
	/// </summary>
	/// <param name="achievementID">The achievement identifier.</param>
	/// <returns type="bool">True if the user has earned the achievement.</returns>
	public static bool HasAchievement (string achievementID)
	{
		return _achievements != null &&
		       _achievements.ContainsKey(achievementID) &&
		       _achievements[achievementID].completed;
	}

	#endregion
}
