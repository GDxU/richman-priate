//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using LitJson;
using System.Collections;

public class LevelController : AIControllerBase
{
	public GameObject PlayerShip;
	private LevelData mLevelData;
    void Awake()
	{
		// Add Data and Actions.
		string path = Application.dataPath + "/" + Settings.LevelDataPath;
		mLevelData = JsonExtend.Load<LevelData>(path);
		
	}
	void Start()
	{
		GlobalMethods.SendMessage(GlobalModuleController.MessageTarget, ModuleNames.Map, false); // Todo: temp
		GlobalMethods.SendMessage(gameObject, LevelData.WelcomeEvent);
	}
	
	void TryStrategyMode()
	{
		GlobalMethods.SendMessage(GlobalModuleController.MessageTarget, ModuleNames.Map, false);
	}
	
	void TryExplorationMode()
	{
		GlobalMethods.SendMessage(GlobalModuleController.MessageTarget, ModuleNames.Sail, true);
	}
	
	void TryBattleMode()
	{
		GlobalMethods.SendMessage(GlobalModuleController.MessageTarget, ModuleNames.Battle, true);
	}
}