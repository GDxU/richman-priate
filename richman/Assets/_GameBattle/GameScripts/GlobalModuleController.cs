//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using System;
using System.Collections.Generic;

public class GlobalModuleController : AIControllerBase
{
	
	public GameObject [] SailModule;
	public GameObject [] BattleModule;
	
	private static GameObject mControllerGameObject;
	public static GameObject MessageTarget { get { return mControllerGameObject; } }
	
	void Awake()
	{
		mControllerGameObject = gameObject;
	}
	
	public void Map(bool enable)
	{
		// Map mode needs Camark to decide.
		EnableSail(false);
		EnableBattle(false);
	}
	
	public void Sail(bool enable)
	{
		GameCamera.DetachFromParent();
		if( enabled) {
			EnableBattle(false);
			EnableSail(true);
		}
		else {
			EnableSail(false);
			EnableBattle(true);
		}
	}
	
	public void Battle(bool enable)
	{
		GameCamera.DetachFromParent();
		if( enabled) {
			EnableSail(false);
			EnableBattle(true);
		}
		else {
			EnableBattle(false);
			EnableSail(true);
		}
	}
	
	private void EnableSail(bool enable)
	{
		if( enable ) {
			foreach( GameObject go in SailModule)
			{
				//go.SetActiveRecursively(true);
				GlobalMethods.SendMessage( go, "EnableController");
			}
		}
		else {
			foreach( GameObject go in SailModule)
			{
				//go.SetActiveRecursively(false);
				GlobalMethods.SendMessage( go, "DisableController");
			}
		}
	}
	
	private void EnableBattle(bool enable)
	{
		if( enable ) {
			foreach( GameObject go in BattleModule)
			{
				//go.SetActiveRecursively(true);
				GlobalMethods.SendMessage( go, "EnableController");
			}
		}
		else {
			foreach( GameObject go in BattleModule)
			{
				//go.SetActiveRecursively(false);
				GlobalMethods.SendMessage( go, "DisableController");
			}
		}
	}
}

