using UnityEngine;
using System.Collections;

public class LumosAnalyticsDemo : MonoBehaviour {
	
	// Use this for initialization
	void Awake () 
	{				
		// Lumos must initialize before events can be properly recorded.
		// This is quick and won't cause issues for most games so long as the Lumos prefab is in your initial scene. 
		// This is a method to notify you when Lumos has finished initializing. 
		Lumos.OnReady += StartRecordingEvents;
	}
	
	static void StartRecordingEvents ()
	{
		// A basic event
		// Every time this is called, the "hits" value will increment for this event
		// Event IDs are created on-the-fly in your code and DO NOT need to be setup on the Lumos website first
		LumosAnalytics.RecordEvent("lumos-is-ready");
		
		// Any events recorded after this point in execution should record
	}
	
	// An example of an event being recorded with a value. 
	// The value is used in a number of ways to show you interesting statistics 
	// On the Lumos website you can see it's average, sum, and more
	public static void ExampleEventWithValue (string eventID, float eventValue)
	{
		LumosAnalytics.RecordEvent(eventID, eventValue);
	}
	
	// An example of an event being recorded with a custom category
	// Events have a default category called 'default' that is used when you do not supply one
	// There is also an option in LumosAnalytics.cs to use scene names as categories
	public static void ExampleEventWithCategory (string category, string eventID)
	{
		LumosAnalytics.RecordEvent(category, eventID);
	}
	
	// An example of a unique event
	// Unique events are not repeatable per player
	// This is great if you want to know information such as how many players have completed a certain level
	// as opposed to how many times a level has been completed
	public static void ExampleUniqueEvent (string eventID)
	{
		LumosAnalytics.RecordEvent(eventID, false);
	}
}
