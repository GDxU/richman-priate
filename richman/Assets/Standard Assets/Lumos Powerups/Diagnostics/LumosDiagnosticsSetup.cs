// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using UnityEngine;

/// <summary>
/// Sets up the Lumos Analytics powerup.
/// </summary>
public class LumosDiagnosticsSetup : ILumosSetup
{
	public string powerupID {
		get { return "diagnostics"; }
	}

	public void Setup ()
	{
		var lumos = GameObject.Find("Lumos");

		if (lumos != null && lumos.GetComponent<LumosDiagnostics>() == null) {
			lumos.AddComponent<LumosDiagnostics>();
			LumosUnity.Debug.Log("Lumos Diagnostics setup complete.", true);
		}
	}
}
