// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sends custom events and tracks player engagement.
/// </summary>
public partial class LumosAnalytics : MonoBehaviour, ILumosPowerup
{
	public string id { get { return "analytics"; } }
	public string version { get { return "1.4"; } }
	public string baseURL { get { return _baseURL; } }

	public static string _baseURL = "https://analytics.lumospowered.com/api/1";

	#region Inspector Variables

	public bool useLevelsAsCategories = false;
	public bool recordLevelCompletionEvents = false;

	#endregion

	public static bool levelsAsCategories { get { return instance.useLevelsAsCategories; } }
	public static float levelStartTime { get; private set; }
	public static LumosAnalytics instance { get; private set; }

	LumosAnalytics () {}

	void Awake ()
	{
		instance = this;
		Lumos.OnReady += Ready;
	}

	void OnLevelWasLoaded ()
	{
		if (recordLevelCompletionEvents) {
			RecordEvent("level-started", true);
			RecordEvent("level-completion-time", Time.time - levelStartTime, true);
			levelStartTime = Time.time;
		}
	}

	void Ready ()
	{
		if (!LumosPowerups.powerups.ContainsKey(id)) {
			enabled = false;
			return;
		}

		Lumos.OnTimerFinish += LumosEvents.Send;
		LumosLocation.Record();

		if (recordLevelCompletionEvents) {
			levelStartTime = Time.time;
			RecordEvent("level-started", true);
		}
	}

	public static bool IsInitialized ()
	{
		GameObject lumosGO = GameObject.Find("Lumos");

		if (lumosGO == null) {
			LumosUnity.Debug.LogWarning("The Lumos Game Object has not been added to your initial scene.", true);
			return false;
		}

		if (lumosGO.GetComponent<LumosAnalytics>() == null) {
			LumosUnity.Debug.LogWarning("The LumosAnalytics script has not been added to the Lumos GameObject.", true);
			return false;
		}

		return true;
	}
}
