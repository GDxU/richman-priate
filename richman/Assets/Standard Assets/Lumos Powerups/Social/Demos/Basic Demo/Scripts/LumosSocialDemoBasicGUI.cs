using UnityEngine;
using System;
using System.Collections;

public class LumosSocialDemoBasicGUI : MonoBehaviour 
{
	enum UI { None, Menu };
	UI currentUI;
	
	// Use this for initialization
	void Start () 
	{
		LumosSocialGUI.ShowWindow(LumosGUIWindow.Login);
	}
	
	void OnGUI ()
	{
		if (LumosSocialGUI.isShowing()) {
			return;
		}
		
		switch (currentUI) {
			case UI.Menu:
				DisplayMenu();
				break;
			default:
				DisplayMenuButton();
				break;
		}
	}
	
	void DisplayMenuButton ()
	{	
		if (GUILayout.Button("Menu")) {
			currentUI = UI.Menu;
		}
	}
	
	void DisplayMenu ()
	{
		if (Social.localUser == null || !Social.localUser.authenticated) {
			if (GUILayout.Button("Login/Register")) {
				LumosSocialGUI.ShowWindow(LumosGUIWindow.Login);
			}
		} else {
			if (GUILayout.Button("Achievements")) {
				LumosSocialGUI.ShowWindow(LumosGUIWindow.Achievements);
			}
			
			if (GUILayout.Button("Leaderboards")) {
				LumosSocialGUI.ShowWindow(LumosGUIWindow.Leaderboards);
			}
			
			if (GUILayout.Button("Profile")) {
				LumosSocialGUI.ShowWindow(LumosGUIWindow.Profile);
			}
			
			if (GUILayout.Button("Settings")) {
				LumosSocialGUI.ShowWindow(LumosGUIWindow.Settings);
			}
			
			if (GUILayout.Button("Close")) {
				currentUI = UI.None;
			}
		}
	}
}
