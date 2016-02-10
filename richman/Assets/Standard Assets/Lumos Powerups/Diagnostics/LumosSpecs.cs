// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Records the player's software and hardware capabilities. OS, RAM, etc.
/// </summary>
public static class LumosSpecs
{
	/// <summary>
	/// Sends system information.
	/// </summary>
	public static void Record ()
	{
		var prefsKey = "lumospowered_" + Lumos.credentials.gameID + "_" + Lumos.playerID + "_sent_specs";

		// Only record system information once.
		if (PlayerPrefs.HasKey(prefsKey)) {
			return;
		}

		var endpoint = "/specs/" + Lumos.playerID;
		var payload = new Dictionary<string, object>() {
#if !UNITY_IPHONE
			{ "os", SystemInfo.operatingSystem },
			{ "processor", SystemInfo.processorType },
			{ "processor_count", SystemInfo.processorCount },
			{ "ram", SystemInfo.systemMemorySize },
			{ "vram", SystemInfo.graphicsMemorySize },
			{ "graphics_card", SystemInfo.graphicsDeviceName }
#else
			{ "os", "iOS"}
#endif
		};

		LumosRequest.Send(LumosDiagnostics.instance, endpoint, LumosRequest.Method.PUT, payload,
			success => {
				PlayerPrefs.SetString(prefsKey, System.DateTime.Now.ToString());
				LumosUnity.Debug.Log("System information successfully sent.");
			},
			error => {
				LumosUnity.Debug.LogError("Failed to send system information.");
			}
		);
	}
}
