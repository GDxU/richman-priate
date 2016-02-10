using UnityEngine;
using System.Collections;

// A simple GUI to allow for experimentation with Lumos Analytics
public class LumosAnalyticsDemoGUI : MonoBehaviour {
	
	string eventValue = "0";
	string category = "my-custom-category";
	string eventName = "my-unique-event";
	string errorMessage = "";
	const string apiKeyMissing = "Your Lumos API key has not been set. Please go to Unity's preferences -> Lumos Tab, and enter your game's API key from the Lumos Website.";
	float baseWidth;
	
	void Awake () 
	{
		CheckLumosInstall();
	}
	
	void CheckLumosInstall ()
	{
		var credentials = LumosCredentials.Load();
		
		if (credentials.apiKey.Length != 36) {
			errorMessage = apiKeyMissing;
		}
	}
	
	void Update ()
	{
		// Used to make the UI look nicer
		baseWidth = Screen.width / 4;
	}
	
	void OnGUI ()
	{		
		GUILayout.Label(errorMessage);
		GUILayout.Space(20);
		
		// Event with value
		GUILayout.BeginHorizontal();
			GUILayout.Label("Event Value (float)", GUILayout.Width(baseWidth));
			eventValue = GUILayout.TextField(eventValue, GUILayout.Width(baseWidth));
			
			if (GUILayout.Button("Event + Value", GUILayout.Width(baseWidth))) {
				Debug.Log("Sending event with value...");
				var eventAsFloat = float.Parse(eventValue);
				LumosAnalytics.RecordEvent("event-with-value", eventAsFloat);
			}
		GUILayout.EndHorizontal();
		
		GUILayout.Space(20);
		
		// Event with custom category
		GUILayout.BeginHorizontal();
			GUILayout.Label("Custom Category", GUILayout.Width(baseWidth));
			category = GUILayout.TextField(category, GUILayout.Width(baseWidth));
		
			if (GUILayout.Button("Event + Category", GUILayout.Width(baseWidth))) {
				Debug.Log("Sending event with custom category...");
				LumosAnalytics.RecordEvent(category, "event-with-custom-category");
			}
		GUILayout.EndHorizontal();
		
		GUILayout.Space(20);
		
		// Unique event
		GUILayout.BeginHorizontal();
			GUILayout.Label("Unique Event (per user)", GUILayout.Width(baseWidth));
			eventName = GUILayout.TextField(eventName, GUILayout.Width(baseWidth));
		
			if (GUILayout.Button("Unique Event", GUILayout.Width(baseWidth))) {
				Debug.Log("Sending unique event...");
				LumosAnalytics.RecordEvent(eventName, false);
			}
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
	}
}
