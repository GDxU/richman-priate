//============================================================================================================
// Weili Zhi Copy right reserved.
//============================================================================================================
/// BattleShipOrbit
//============================================================================================================
// Created on 18/7/2012 9:06:52 AM by Weili Zhi
//============================================================================================================
using System;
using UnityEngine;

public class BattleShipOrbit : ActionBase
{
	public ShipController control;
	public float MaxRange = 6.0f;
	public float sensitivity = 1f;
	public Vector2 horizontalTiltRange = new Vector2(-20f, 20f);

	Transform mTrans;
	Vector2 mInput;
	Vector2 mOffset;
	private GameUnit mMyUnit;
			
	void Start ()
	{
		mTrans = transform;
		mMyUnit = GameUnit.Find(transform);
		if( mMyUnit == null ) DebugExt.LogError("Cannot find GameUnit of " + transform.name);
	}

	protected override void OnFrameUpdate ()
	{
		if (control != null)
		{
			float multiplier = Time.deltaTime * sensitivity;
			
			/*bool mouseInput = Input.GetMouseButton(0);

			// Automatically show the cursor
			if (!Application.isEditor && Input.GetMouseButtonUp(0)) Screen.showCursor = true;

			if (mouseInput)
			{
				// Mouse input
				mInput.x = Input.GetAxis("Mouse X");
				mInput.y = Input.GetAxis("Mouse Y");
				multiplier *= 300f;
			}
			else
			{
				// Joystick input
				//mInput.x = Input.GetAxis("View X");
				//mInput.y = Input.GetAxis("View Y");
				//multiplier *= 75f;
			}*/
			GameUnit enemyUnit = GameUnit.Find(mMyUnit, 20.0f, new GameUnit.UnitType[1] {GameUnit.UnitType.Pirate});
			if( mMyUnit != null && enemyUnit != null ) {
				Vector3 dir = enemyUnit.transform.position - mMyUnit.transform.position;
				//float angle = Vector3.Angle(dir, mMyUnit.transform.forward);
				float angle = ToolCalculations.Angle(dir, mMyUnit.transform.forward);
				mOffset.x = angle;
				//mOffset.y = angle.z;
			}
			/*if (mouseInput || mInput.sqrMagnitude > 0.001f)
			{
				mOffset.x += mInput.x * multiplier;
				mOffset.y += mInput.y * multiplier;

				// Limit the angles
				mOffset.x = Tools.WrapAngle(mOffset.x);
				mOffset.y = Mathf.Clamp(mOffset.y, horizontalTiltRange.x, horizontalTiltRange.y);

				// Automatically hide the cursor
				if (mouseInput && !Application.isEditor && mOffset.magnitude > 10f) Screen.showCursor = false;
			}
			else if (Mathf.Abs(mOffset.x) < 35f)
			{
				// No key pressed and the camera has not been moved much -- slowly interpolate the offset back to 0
				float factor = Time.deltaTime * control.speed * 4f;
				mOffset.x = Mathf.Lerp(mOffset.x, 0f, factor);
				mOffset.y = Mathf.Lerp(mOffset.y, 0f, factor);
			}
			 */
			// Calculate the rotation and wrap it around
			Quaternion targetRot = Quaternion.Euler(-mOffset.y, mOffset.x, 0f);

			// Interpolate the rotation for smoother results
			mTrans.localRotation = Quaternion.Slerp(mTrans.localRotation,
				targetRot, Mathf.Clamp01(Time.deltaTime * 10f));
			
		}
	}
}