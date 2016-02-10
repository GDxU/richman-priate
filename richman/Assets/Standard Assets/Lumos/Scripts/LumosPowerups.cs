// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Stores information about the game's powerups.
/// </summary>
public static class LumosPowerups
{
	public struct Powerup
	{
		public bool playerInQuota;
		public Dictionary<string, object> settings;
	}

	public static Dictionary<string, Powerup> powerups { get; private set; }

	public static void LoadPowerupInfo (IList powerupInfo)
	{
		powerups = new Dictionary<string, Powerup>();

		foreach (Dictionary<string, object> info in powerupInfo) {
			var powerupID = info["id"] as string;
			var playerInQuota = (bool)info["player_in_quota"];
			var settings = info["settings"] as Dictionary<string, object>;
			powerups[powerupID] = new Powerup() { playerInQuota = playerInQuota, settings = settings };
		}
	}
}
