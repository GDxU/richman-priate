// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using UnityEngine;

/// <summary>
/// Sets up the Lumos Analytics powerup.
/// </summary>
public class LumosAnalyticsSetup : ILumosSetup
{
	public string powerupID {
		get { return "analytics"; }
	}

	public void Setup ()
	{
		var lumos = GameObject.Find("Lumos");

		if (lumos != null && lumos.GetComponent<LumosAnalytics>() == null) {
			lumos.AddComponent<LumosAnalytics>();
			LumosUnity.Debug.Log("Lumos Analytics setup complete.", true);
		}
	}
}
