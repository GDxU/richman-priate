//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using System;

public class SailforceUIAction : ActionBase
{
	private Rect mUIArea;
	public delegate void ForceChangeCallback();
	public ForceChangeCallback mForeChangeCB;
	
	void Start()
	{
		mUIArea = new Rect(Screen.width - 200f, Screen.height - 200f, 180f, 180f);
	}
	
	void OnGUI()
	{
		if( GUI.Button( mUIArea, "Change Speed")) {
			if( mForeChangeCB != null ) 
				mForeChangeCB();
		}
	}
}

