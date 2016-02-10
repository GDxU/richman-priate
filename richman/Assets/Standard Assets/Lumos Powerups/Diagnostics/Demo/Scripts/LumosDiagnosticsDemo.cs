using UnityEngine;
using System.Collections;

public class LumosDiagnosticsDemo : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		Lumos.debug = true;

		// Lumos must initialize before events can be properly recorded.
		// This is quick and won't cause issues for most games so long as the Lumos prefab is in your initial scene.
		// This is a method to notify you when Lumos has finished initializing.
		Lumos.OnReady += RecordLogs;
	}

	public static void RecordLogs ()
	{
		Debug.Log("Look at the LumosDiagnostics.cs component on the Lumos prefab to configure which log types you care to record.");

		Debug.Log("Basic logs can be captured and recorded to the Lumos website.");
		Debug.LogWarning("Warnings can be captured and recorded to the Lumos website.");
		Debug.LogError("Errors and exceptions can be captured and recorded to the Lumos website.");

		LumosUnity.Debug.Log("You can use this function (in combination with Lumos.debug=True) to make logs that are not recorded.");
	}
}
