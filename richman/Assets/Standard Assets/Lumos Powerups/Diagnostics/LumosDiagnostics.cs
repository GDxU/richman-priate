// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using UnityEngine;

/// <summary>
/// Lumos diagnostics.
/// </summary>
public class LumosDiagnostics : MonoBehaviour, ILumosPowerup
{
	public string id { get { return "diagnostics"; } }
	public string version { get { return "1.4"; } }
	public string baseURL { get { return _baseURL; } }

	public static string _baseURL = "https://diagnostics.lumospowered.com/api/1";

	#region Inspector Settings

	public bool recordLogs = false;
	public bool recordWarnings = true;
	public bool recordErrors = true;

	public static bool recordDebugLogs { get { return instance.recordLogs; } }
	public static bool recordDebugWarnings { get { return instance.recordWarnings; } }
	public static bool recordDebugErrors { get { return instance.recordErrors; } }

	#endregion

	public static LumosDiagnostics instance { get; private set; }

	LumosDiagnostics () {}
	
	void Awake ()
	{
		instance = this;
		Lumos.OnReady += Ready;
	}

	void OnGUI ()
	{
		LumosFeedbackGUI.OnGUI();
	}

	void Ready ()
	{
		if (!LumosPowerups.powerups.ContainsKey(id)) {
			enabled = false;
			return;
		}

		Lumos.OnTimerFinish += LumosLogs.Send;
		LumosSpecs.Record();
		
		// Set up debug log redirect.
		Application.RegisterLogCallback(LumosLogs.Record);
	}

	public static bool IsInitialized ()
	{
		GameObject lumosGO = GameObject.Find("Lumos");
		
		if (lumosGO == null) {
			Debug.LogWarning("The Lumos Game Object has not been added to your initial scene.");
			return false;
		}
		
		if (lumosGO.GetComponent<LumosDiagnostics>() == null) {
			Debug.LogWarning("The LumosDiagnostics script has not been added to the Lumos GameObject.");
			return false;
		}
		
		return true;
	}
}
