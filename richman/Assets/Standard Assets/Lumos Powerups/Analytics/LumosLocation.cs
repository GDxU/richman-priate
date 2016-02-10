// Copyright (c) 2013 Rebel Hippo Inc. All rights reserve

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Records the players language and website they're playing from.
/// <summary>
public static class LumosLocation
{
	/// <summary>
	/// Record this player's location info.
	/// </summary>
	public static void Record()
	{
		var prefsKey = "lumospowered_" + Lumos.credentials.gameID + "_" + Lumos.playerID + "_sent_location";

		// Only record location information once.
		if (PlayerPrefs.HasKey(prefsKey)) {
			return;
		}

		var endpoint = "/location/" + Lumos.playerID;
		var payload = new Dictionary<string, object>() {
			{ "language", Application.systemLanguage.ToString() }
		};

		if (Application.isWebPlayer) {
			payload["origin"] = Application.absoluteURL;
		}

		LumosRequest.Send(LumosAnalytics.instance, endpoint, LumosRequest.Method.PUT, payload,
			success => {
				PlayerPrefs.SetString(prefsKey, System.DateTime.Now.ToString());
				LumosUnity.Debug.Log("Location information successfully sent.");
			},
			error => {
				LumosUnity.Debug.LogError("Failed to send Location information.");
			});
	}
}
