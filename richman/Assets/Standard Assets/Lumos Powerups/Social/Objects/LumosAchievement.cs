// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

/// <summary>
/// An achievement a player has earned.
/// </summary>
public class LumosAchievement : IAchievement
{
	/// Unique identifier for the achievement.
	public string id { get; set; }

	/// The amount of the achievement completed.
	public double percentCompleted { get; set; }

	/// Indicates whether this achievement has been earned.
	public bool completed
	{
		get { return (int)percentCompleted == 100; }
	}

	/// Indicates whether this achievement is hidden.
	public bool hidden { get; set; }

	/// The date the achievement was last updated.
	public DateTime lastReportedDate { get; private set; }

	/// Creates a new achievement object.
	public LumosAchievement () {}

	/// Creates a new achievement object.
	public LumosAchievement (Dictionary<string, object> info)
	{
		this.id = info["achievement_id"] as string;
		this.percentCompleted = Convert.ToDouble(info["percent_completed"]);

		var timestamp = Convert.ToDouble(info["updated"]);
		this.lastReportedDate = LumosUnity.Util.UnixTimestampToDateTime(timestamp);

		if (info.ContainsKey("hidden")) {
			var intHidden = Convert.ToInt32(info["hidden"]);
			this.hidden = Convert.ToBoolean(intHidden);
		}
	}

	/// <summary>
	/// Creates a new achievement object.
	/// </summary>
	/// <param name="id">A unique identifier.</param>
	/// <param name="percentCompleted">Percent completed.</param>
	/// <param name="completed">Completed.</param>
	/// <param name="hidden">Hidden.</param>
	/// <param name="lastReportedDate">Last reported date.</param>
	public LumosAchievement (string id, double percentCompleted, bool hidden, DateTime lastReportedDate)
	{
		this.id = id;
		this.percentCompleted = percentCompleted;
		this.hidden = hidden;
		this.lastReportedDate = lastReportedDate;
	}

	/// Reports progress for the achievement.
	public void ReportProgress (Action<bool> callback)
	{
		if (Social.localUser == null) {
			LumosUnity.Debug.LogWarning("The user must be authenticated before reporting an achievement.", true);
			callback(false);
			return;
		}

		var endpoint = "/users/" + Social.localUser.id + "/achievements/" + id;

		var payload = new Dictionary<string, object>() {
			{ "percent_completed", percentCompleted }
		};

		LumosRequest.Send(LumosSocial.instance, endpoint, LumosRequest.Method.PUT, payload,
			success => {
				var info = success as Dictionary<string, object>;

				// Update timestamp.
				var timestamp = Convert.ToDouble(info["updated"]);
				lastReportedDate = LumosUnity.Util.UnixTimestampToDateTime(timestamp);

				if (Application.platform == RuntimePlatform.IPhonePlayer && LumosSocial.useGameCenter) {
					ReportProgressToGameCenter(id, percentCompleted);
				}

				if (callback != null) {
					callback(true);
				}
			},
			error => {
				if (callback != null) {
					callback(false);
				}
			});
	}

	#region Added Functions

	void ReportProgressToGameCenter (string achievementID, double percentCompleted)
	{
		LumosSocial.gameCenterPlatform.ReportProgress(achievementID, percentCompleted, delegate {
			LumosUnity.Debug.Log("Reported achievement progress to Game Center.");
		});
	}

	#endregion
}
