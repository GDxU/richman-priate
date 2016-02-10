//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using UnityEngine;
using System.Collections;

public class ActionBase : MonoBehaviour
{
	private bool mEnableAction = true;
    public virtual void Init()
    {
    }
	
	public virtual void EnableAction()
	{
		mEnableAction = true;
	}

	public virtual void DisableAction()
	{
		mEnableAction = false;
	}

	void Update()
    {
		if( mEnableAction ) OnFrameUpdate();
    }

    protected virtual void OnFrameUpdate()
    {
    }
}