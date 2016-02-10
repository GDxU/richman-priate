// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A service players to send feedback to the developers of a game.
/// </summary>
public static class LumosFeedback
{
	/// <summary>
	/// Records feedback.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="category">The category (bug report, feature request, etc).</param>
	/// <param name="email">The email address of the player sending the message.</param>
	public static void Record (string message, string email, string category)
	{
		// Don't record empty messages
		if (message == null || message == "") {
			return;
		}

		var feedback = new Dictionary<string, object>() {
			{ "message", message },
			{ "category", category },
			{ "email", email }
		};

		Send(feedback);
	}

	/// <summary>
	/// Sends feedback to the server.
	/// </summary>
	public static void Send (Dictionary<string, object> feedback)
	{
		var endpoint = "/feedback";

		LumosRequest.Send(LumosDiagnostics.instance, endpoint, LumosRequest.Method.POST, feedback,
			success => {
				LumosUnity.Debug.Log("Feedback sent.");
			},
			error => {
				LumosUnity.Debug.LogWarning("Log messages not sent. Will try again at next timer interval.");
			}
		);
	}
}
