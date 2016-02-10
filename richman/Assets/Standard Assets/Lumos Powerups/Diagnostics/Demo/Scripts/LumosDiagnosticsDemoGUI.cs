using UnityEngine;
using System.Collections;

public class LumosDiagnosticsDemoGUI : MonoBehaviour {
	
	const string logsTooltip = "Calls several function in Debug, all of which are captured and recorded to the Lumos website.";
	const string feedbackTooltip = "Opens the default Feedback GUI window provided by Lumos.";
	const string customFeedbackTooltip = "Opens a custom made Feedback GUI window.";
	enum GUIState { Base, DefaultFeedback, CustomFeedback };
	GUIState state;
	
	Rect windowRect;
	string email = "email@example.com";
	string message = "More awesome please!";
	string category = "Feature Request";
	
	const string apiKeyMissing = "Your Lumos API key has not been set. Please go to Unity's preferences -> Lumos Tab, and enter your game's API key from the Lumos Website.";
	string errorMessage = "";
	
	void Awake () 
	{
		CheckLumosInstall();
		LumosFeedbackGUI.windowClosed += DefaultFeedbackGUIClosed;
	}
	
	void CheckLumosInstall ()
	{
		var credentials = LumosCredentials.Load();
		
		if (credentials.apiKey.Length != 36) {
			errorMessage = apiKeyMissing;
		}
	}
	
	void DefaultFeedbackGUIClosed ()
	{
		state = GUIState.Base;
	}
	
	void OnGUI ()
	{
		GUILayout.Label(errorMessage);
		
		switch (state) {
			case GUIState.CustomFeedback:
				CustomFeedbackGUI();
				break;
			case GUIState.DefaultFeedback:
				// Show nothing, LumosFeedbackGUI is displaying for us
				break;
			default:
				BaseGUI();
				break;
		}
	}
	
	void BaseGUI ()
	{
		if (GUILayout.Button(new GUIContent("Record Logs", logsTooltip))) {
			LumosDiagnosticsDemo.RecordLogs();
		}
		
		if (GUILayout.Button(new GUIContent("Show Feedback Window", feedbackTooltip))) {
			state = GUIState.DefaultFeedback;
			LumosFeedbackGUI.ShowDialog();
		}
		
		if (GUILayout.Button(new GUIContent("Show Custom Feedback Window", customFeedbackTooltip))) {
			state = GUIState.CustomFeedback;
			
			var margin = 10;
			windowRect = new Rect(margin, margin, Screen.width - (2 * margin), Screen.height - (2 * margin));
		}
	}
	
	void CustomFeedbackGUI ()
	{
		GUILayout.Window(1337, windowRect, DisplayWindow, "Give Feedback");
	}
	
	/// <summary>
	/// Displays the window.
	/// </summary>
	/// <param name="windowID">The window's ID.</param>
	void DisplayWindow (int windowId)
	{
		GUILayout.BeginHorizontal();
			// Email 
			GUILayout.Label("Email (optional)");
			email = GUILayout.TextField(email, GUILayout.Width(150));
		
			GUILayout.FlexibleSpace();
			
			// Feedback Type
			GUILayout.Label("Type");
			category = GUILayout.TextField(category, GUILayout.Width(150));
		GUILayout.EndHorizontal();

		// Feedback Message
		message = GUILayout.TextArea(message, GUILayout.MinHeight(150));
		
		// Cancel and Send buttons
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Cancel")) {
				state = GUIState.Base;
			}

			if (GUILayout.Button("Send")) {
				Debug.Log("Sending feedback...");
				LumosFeedback.Record(message, email, category);
				state = GUIState.Base;
			}
		GUILayout.EndHorizontal();
	}
}
