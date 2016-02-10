//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// Action Base
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//===========================================================================================================	=
using UnityEngine;
using System;
using System.Collections.Generic;

public class SailCameraController : AIControllerBase
{
	private List<GameObject> mSailCameraObjects = new List<GameObject>();
	void Start()
	{
		Transform tf = transform.FindChild("Steer");
		if( tf != null && tf.gameObject != null ) mSailCameraObjects.Add(tf.gameObject);
	}
	
	public override void DisableController()
	{
		base.DisableController();
		foreach(GameObject go in mSailCameraObjects) {
			GlobalMethods.SendMessage(go, "DisableController");
		}
		Camera camera = gameObject.GetComponent<Camera>();
		camera.enabled = false;
	}
	
	public override void EnableController()
	{
		base.EnableController();
		foreach(GameObject go in mSailCameraObjects) {
			GlobalMethods.SendMessage(go, "EnableController");
		}
		Camera camera = gameObject.GetComponent<Camera>();
		camera.enabled = true;
	}
}