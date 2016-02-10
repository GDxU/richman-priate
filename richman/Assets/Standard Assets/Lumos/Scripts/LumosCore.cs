// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes Lumos.
/// </summary>
public class LumosCore : ILumosPowerup
{
	public string id { get { return "core"; } }
	public string version { get { return Lumos.version; } }
	public string baseURL { get { return _baseURL; } }

	public static string _baseURL = "https://core.lumospowered.com/api/1";

	static LumosCore _instance;
	static LumosCore instance
	{
		get {
			if (_instance == null) {
				_instance = new LumosCore();
			}

			return _instance;
		}
	}

	/// <summary>
	/// Asks the server to generate and return a new player ID.
	/// </summary>
	public static Coroutine RequestPlayerID ()
	{
		return LumosRequest.Send(instance, "/players", LumosRequest.Method.POST,
			success => {
				var idPrefsKey = "lumospowered_" + Lumos.credentials.gameID + "_playerid";
				var resp = success as Dictionary<string, object>;
				Lumos.playerID = resp["player_id"].ToString();
				PlayerPrefs.SetString(idPrefsKey, Lumos.playerID);
				LumosUnity.Debug.Log("Using new player " + Lumos.playerID);
			});
	}

	/// <summary>
	/// Notifies the server that the player is playing.
	/// </summary>
	public static Coroutine Ping (Action<bool> callback)
	{
		return LumosRequest.Send(instance, "/ping", LumosRequest.Method.POST,
			success => {
				LumosUnity.Debug.Log("Returned Ping request with user " + Lumos.playerID);
				var resp = success as Dictionary<string, object>;
				var powerupInfo = resp["powerups"] as IList;
				LumosPowerups.LoadPowerupInfo(powerupInfo);
				callback(true);
			},
			error => {
				var message = "There was a problem establishing communication with Lumos. No information will be recorded for this play session.";

				if (error != null) {
					var resp = error as Dictionary<string, object>;

					if (resp.ContainsKey("message")) {
						message += " Error: " + resp["message"];
					}
				}

				LumosUnity.Debug.LogWarning(message, true);
				callback(false);
			});
	}
}
