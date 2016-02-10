// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A service that sends debug logs for remote viewing.
/// </summary>
public static class LumosLogs
{
	/// <summary>
	/// Log messages that Lumos should ignore.
	/// </summary>
	static List<string> toIgnore = new List<string> {
		"You are trying to load data from a www stream which had the following error when downloading."
	};

	/// <summary>
	/// The stored logs as a dictionary of dictionaries.
	/// </summary>
	static Dictionary<string, Dictionary<string, object>> logs = new Dictionary<string, Dictionary<string, object>>();

	/// <summary>
	/// The log type labels.
	/// </summary>
	static readonly Dictionary<LogType, string> typeLabels = new Dictionary<LogType, string>() {
		{ LogType.Assert, "assertion" },
		{ LogType.Error, "error" },
		{ LogType.Exception, "exception" },
		{ LogType.Log, "info" },
		{ LogType.Warning, "warning" },
	};

	/// <summary>
	/// Records a log message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="trace">Details of the log's origin.</param>
	/// <param name="type">Message type (debug, warning, error, etc.).</param>
	public static void Record (string message, string trace, LogType type)
	{
		// Checks if Lumos and LumosDiagnostics is installed correctly
		if (!LumosDiagnostics.IsInitialized()) {
			return;
		}

		// Ignore logs in editor is the option is selected.
		if (Application.isEditor && !Lumos.runInEditor) {
			return;
		}

		// Ignore messages logged by Lumos.
		if (message.StartsWith(LumosUnity.Debug.prefix)) {
			return;
		}

		// Don't record empty messages
		if (message == null || message == "") {
			return;
		}

		// Only log message types that the user specifies.
		if (type == LogType.Assert ||
				(type == LogType.Log && !LumosDiagnostics.recordDebugLogs) ||
				(type == LogType.Warning && !LumosDiagnostics.recordDebugWarnings) ||
				(type == LogType.Error && !LumosDiagnostics.recordDebugErrors) ||
				(type == LogType.Exception && !LumosDiagnostics.recordDebugErrors)) {
			return;
		}

		// Skip messages that the user explicitly wishes to ignore.
		foreach (var ignoreMessage in toIgnore) {
			if (message.StartsWith(ignoreMessage)) {
				return;
			}
		}

		var hash = LumosUnity.Util.MD5Hash(typeLabels[type], message, trace);

		if (logs.ContainsKey(hash)) {
			// Increment an identical log's total.
			logs[hash]["total"] = (int)logs[hash]["total"] + 1;
		} else {
			// Otherwise create a new one.
			logs[hash] = new Dictionary<string, object>() {
				{ "log_id", hash },
				{ "type", typeLabels[type] },
				{ "message", message },
				{ "level", Application.loadedLevelName },
				{ "total", 1 }
			};

			LumosUnity.Util.AddToDictionaryIfNonempty(logs[hash], "trace", trace);
		}
	}

	/// <summary>
	/// Sends the queued logs to the server.
	/// </summary>
	public static void Send ()
	{
		if (logs.Count == 0) {
			return;
		}

		var endpoint = "/logs";
		var payload = new List<Dictionary<string, object>>(logs.Values);

		LumosRequest.Send(LumosDiagnostics.instance, endpoint, LumosRequest.Method.POST, payload,
			success => {
				logs.Clear();
			},
			error => {
				LumosUnity.Debug.LogWarning("Log messages not sent. Will try again at next timer interval.");
			}
		);
	}

	/// <summary>
	/// Appends the given messages to the list of log messages to skip.
	/// Supply the entire string or just the beginning of one.
	/// </summary>
	/// <param name="messages">The messages to ignore.</param>
	public static void AddMessagesToIgnore (params string[] messages)
	{
		toIgnore.AddRange(messages);
	}
}
