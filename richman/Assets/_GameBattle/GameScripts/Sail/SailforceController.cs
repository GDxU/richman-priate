//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using System;

public class SailforceController : AIControllerBase
{
	// Data
	public float ForceChangeStep = 0.33f;
	public float StepCount = 4f;
	public GameObject ForceTarget;
	
	private float mCurrentForceTarget;
	private float mMaxForce;
	private float mCurrentForce;
	private SailforceUIAction mUIAction;
	
	void Start()
	{
		mMaxForce = ForceChangeStep * StepCount;
		mCurrentForceTarget = 0f;
		mUIAction = gameObject.GetComponent<SailforceUIAction>();
		if( mUIAction == null ) 
			mUIAction = gameObject.AddComponent<SailforceUIAction>();
		mUIAction.mForeChangeCB = ForceChangeCallback;
	}
	
	void SetForceTarget(GameObject target)
	{
		ForceTarget = target;
	}
	
	protected override void OnFrameUpdate()
    {
		mCurrentForce = Mathf.Lerp(mCurrentForce, mCurrentForceTarget, 0.005f);
		if( ForceTarget != null ) {
			GlobalMethods.SendMessage(ForceTarget, "SetForce", mCurrentForce);
		}
    }
	
	private void ForceChangeCallback()
	{
		mCurrentForceTarget += ForceChangeStep;
		mCurrentForceTarget = mCurrentForceTarget % mMaxForce;
	}
	
	public override void DisableController()
	{
		base.DisableController();
		if(mUIAction != null) mUIAction.EnableAction();
	}
	
	public override void EnableController()
	{
		base.EnableController();
		if(mUIAction != null) mUIAction.DisableAction();
	}
}

