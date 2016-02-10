// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

/// <summary>
/// The Lumos social platform. Handles achievements, leaderboards, and users.
/// </summary>
public partial class LumosSocial : ISocialPlatform, ILumosPowerup
{
	public string id { get { return "social"; } }
	public string version { get { return "1.4"; } }
	public string baseURL { get { return _baseURL; } }

	public static string _baseURL = "https://social.lumospowered.com/api/1";

	public static bool useGameCenter { private set; get; }
	public static GameCenterPlatform gameCenterPlatform { private set; get; }
	public static LumosSocial instance { get; private set; }

	public LumosSocial ()
	{
		instance = this;

		if (Lumos.ready) {
			InitializeSettings();
		} else {
			Lumos.OnReady += InitializeSettings;
		}
	}

	void InitializeSettings ()
	{
		// For now Social settings are only used for Game Center.
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			var settings = LumosPowerups.powerups["social"].settings;
			var gameCenterKey = "use_game_center";

			if (settings != null && settings.ContainsKey(gameCenterKey)) {
				useGameCenter = System.Convert.ToBoolean(settings[gameCenterKey]);
			}

			if (useGameCenter) {
				gameCenterPlatform = new GameCenterPlatform();

				gameCenterPlatform.localUser.Authenticate(success => {
					if (success) {
						LumosUnity.Debug.Log("Authenticated with game center.");
					}
				});
			}
		}
	}
}
