//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using System;

public class KeyBoardController : AIControllerBase
{
	GameObject KeyPowerTarget = null;
	void Start() {
		if( KeyPowerTarget == null ) {
			KeyPowerTarget = gameObject;
		}
	}
	
	protected override void OnFrameUpdate ()
	{
		if( KeyPowerTarget != null ) {
			float x = Input.GetAxis("Horizontal");
			GlobalMethods.SendMessage(KeyPowerTarget, "KeyBoardDrive", x);
		}
	}
}