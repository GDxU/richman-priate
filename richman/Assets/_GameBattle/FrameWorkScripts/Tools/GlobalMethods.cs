using System;
using UnityEngine;

public static class GlobalMethods
{
	public static void SendMessage(GameObject target, string eventName)	
	{
		//Debug.Log("Send Message " + eventName + " to " + target.name);
		target.SendMessage(eventName);
	}
	
	public static void SendMessage(GameObject target, string eventName, object arg)	
	{
		//Debug.Log("Send Message " + eventName + " to " + target.name);
		target.SendMessage(eventName, arg);
	}
}