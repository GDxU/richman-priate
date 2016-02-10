/// Copyright (c) 2013 Rebel Hippo Inc. All rights reserved.

using System;
using System.Text;

namespace LumosUnity
{
	/// <summary>
	/// Wrapper for UnityEngine.Debug
	/// Allows Lumos to record debug messages that won't be picked up by the
	/// Diagnostics powerup.
	/// </summary>
	public class Debug
	{
		public const string prefix = "[Lumos] ";

		/// <summary>
		/// Records a debug message.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="alwaysShow">When true, always prints the message to console.</param>
		public static void Log (object message, bool alwaysShow = false)
		{
			LogMessage(UnityEngine.Debug.Log, message, alwaysShow);
		}

		/// <summary>
		/// Records a warning.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="alwaysShow">When true, always prints the message to console.</param>
		public static void LogWarning (object message, bool alwaysShow = false)
		{
			LogMessage(UnityEngine.Debug.LogWarning, message, alwaysShow);
		}

		/// <summary>
		/// Records an error.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="alwaysShow">When true, always prints the message to console.</param>
		public static void LogError (object message, bool alwaysShow = false)
		{
			LogMessage(UnityEngine.Debug.LogError, message, alwaysShow);
		}

		/// <summary>
		/// Records a message.
		/// </summary>
		/// <param name="logger">Function to send the message to.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="alwaysShow">When true, always prints the message to console.</param>
		static void LogMessage (Action<object> logger, object message, bool alwaysShow)
		{
			if (alwaysShow || Lumos.debug) {
				logger(prefix + message);
			}
		}
	}
}
