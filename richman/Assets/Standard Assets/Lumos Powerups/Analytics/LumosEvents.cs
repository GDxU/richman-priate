// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows custom events to be sent.
/// </summary>
public static class LumosEvents
{
	/// <summary>
	/// The stored events as a dictionary of dictionaries.
	/// </summary>
	static Dictionary<string, Dictionary<string, object>> events = new Dictionary<string, Dictionary<string, object>>();

	/// <summary>
	/// Keys of unique (non-repeated) events that have yet to be recorded.
	/// </summary>
	static HashSet<string> unsentUniqueEvents = new HashSet<string>();

	/// <summary>
	/// Records an event.
	/// </summary>
	/// <param name="category">The event's category.</param>
	/// <param name="eventID">The event identifier.</param>
	/// <param name="value">An arbitrary value to send with the event.</param>
	/// <param name="repeatable">Whether this event should only be logged once.</param>
	public static void Record (string category, string eventID, float? val, bool repeatable)
	{
		// Checks if Lumos and LumosAnalytics is installed correctly
		if (!LumosAnalytics.IsInitialized()) {
			return;
		}

		if (eventID == null || eventID == "") {
			LumosUnity.Debug.LogWarning("An event ID and category must be supplied. Event not recorded.");
			return;
		}

		if (category == null || category == "") {
			if (LumosAnalytics.levelsAsCategories) {
				category = Application.loadedLevelName;
			} else {
				category = "default";
			}
		}

		var key = category + ":" + eventID;
		var prefsKey = "lumospowered_event_" + key + "_recorded";

		// Ensure unrepeatable event hasn't been logged before.
		if (!repeatable) {
			if (PlayerPrefs.HasKey(prefsKey) || unsentUniqueEvents.Contains(key)) {
				return;
			}

			unsentUniqueEvents.Add(key);
		}

		var evt = new Dictionary<string, object>() {
			{ "category", category },
			{ "event_id", eventID }
		};

		if (val.HasValue) {
			evt["value"] = val.Value;
		}

		events[key] = evt;
	}

	/// <summary>
	/// Sends the events.
	/// </summary>
	public static void Send ()
	{
		if (events.Count == 0) {
			return;
		}

		var endpoint = "/events";
		var payload = new List<Dictionary<string, object>>(events.Values);

		LumosRequest.Send(LumosAnalytics.instance, endpoint, LumosRequest.Method.POST, payload,
			success => {
				var now = System.DateTime.Now.ToString();

				// Save unrepeatable events to player prefs with a timestamp.
				foreach (var key in unsentUniqueEvents) {
					var prefsKey = "lumospowered_event_" + key + "_recorded";
					PlayerPrefs.SetString(prefsKey, now);
				}

				events.Clear();
				unsentUniqueEvents.Clear();
			},
			error => {
				LumosUnity.Debug.LogWarning("Events not sent. Will try again at next timer interval.");
			});
	}
}
