//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using System.Collections;

public class WelcomeAction : ActionBase
{
	public GUISkin skin	= null;
	private bool mShowMe = false;
	
	public void DisplayWelcome ()
	{
		mShowMe = true;
	}
	
	void OnGUI() {
		if (!mShowMe) return;
		
		Rect rect = UI.DrawWindow(new Rect(Screen.width * 0.5f - 200f, Screen.height * 0.5f - 220f, 400f, 440f), "Welcome");

		GUILayout.BeginArea(rect);
		{
			GUILayout.Space(10f);

			if (Application.isEditor)
			{
				GUILayout.Label("Thank you for buying the Ship Game Starter Kit!\n\n" +
					"We hope it will help you with your own game ambitions. If you have any suggestions, comments, feature or even new starter kit requests, " +
					"please don't hesitate to contact us via support@tasharen.com.\n\nWe hope your game will be a stellar success!", skin.label);
			}
			else
			{
				GUILayout.Label("Welcome to the Ship Game Starter Kit!\n\n" +
					"You will want to start by selecting your first town and creating a trade route that links it to another town (you can read the instructions below).\n\n" +
					"At any time press F5 to switch between Strategy and Exploration modes.\n\n" +
					"May this rough prototype help you with your own game ambitions!", skin.label);
			}
			
			if (GUI.Button(new Rect(100f, rect.height - 120f, 200f, 30f), "Try Battle Mode", skin.button))
			{
				mShowMe = false;
				GlobalMethods.SendMessage(gameObject, "TryBattleMode");
			}
			
			if (GUI.Button(new Rect(100f, rect.height - 80f, 200f, 30f), "Try Strategy Mode", skin.button))
			{
				mShowMe = false;
				GlobalMethods.SendMessage(gameObject, "TryStrategyMode");
			}

			if (GUI.Button(new Rect(100f, rect.height - 40f, 200f, 30f), "Try Exploration Mode", skin.button))
			{
				mShowMe = false;
				GlobalMethods.SendMessage(gameObject, "TryExplorationMode");
			}
		}
		GUILayout.EndArea();
	}
}