using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Exploration/Active Camera")]
public class GameCamera : MonoBehaviour
{
	static List<Transform> mTargets = new List<Transform>();
	static float mAlpha;
	static GameCamera mInstance = null;
	static public Vector3 direction = Vector3.forward;
	static public Vector3 flatDirection = Vector3.forward;

	public float interpolationTime = 0f;

	Transform mTrans;
	Vector3 mPos;
	Quaternion mRot;

	/// <summary>
	/// Target the camera is following.
	/// </summary>

	static public Transform target
	{
		get
		{
			return (mTargets.Count == 0) ? null : mTargets[mTargets.Count - 1];
		}
		set
		{
			mTargets.Clear();
			if (value != null) mTargets.Add(value);
			mAlpha = 0f;
		}
	}

	/// <summary>
	/// Add a new target to the top of the list.
	/// </summary>

	static public void AddTarget (Transform t)
	{
		if (t != null)
		{
			mTargets.Remove(t);
			mTargets.Add(t);
			mAlpha = 0f;
		}
	}

	/// <summary>
	/// Remove the specified target from the list.
	/// </summary>

	static public void RemoveTarget (Transform t)
	{
		if (t != null)
		{
			if (target == t) mAlpha = 0f;
			mTargets.Remove(t);
		}
	}

	/// <summary>
	/// Detach the camera from the current parent.
	/// </summary>

	static public void DetachFromParent ()
	{
		if (mInstance != null && mInstance.mTrans.parent != null)
		{
			mInstance.mTrans.parent = null;
		}
	}

	/// <summary>
	/// Detach the camera from the specified parent.
	/// </summary>

	static public bool DetachFromParent (Transform t)
	{
		if (mInstance != null && ToolCalculations.IsChild(t, mInstance.mTrans))
		{
			mInstance.mTrans.parent = null;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Keep a singleton reference.
	/// </summary>

	void Awake () { mInstance = this; }

	/// <summary>
	/// Cache the transform.
	/// </summary>

	void Start () { mTrans = transform; }

	/// <summary>
	/// Interpolate the position.
	/// </summary>

	void LateUpdate ()
	{
		Transform t = target;

		if (t == null)
		{
			mTrans.parent = null;
		}
		else if (mAlpha < 1f)
		{
			// Start of the interpolation process -- record the position and rotation
			if (mAlpha == 0f)
			{
				mTrans.parent = null;
				mPos = mTrans.position;
				mRot = mTrans.rotation;
			}

			// Advance the alpha
			if (interpolationTime > 0f) mAlpha += Time.deltaTime / interpolationTime;
			else mAlpha = 1f;

			if (mAlpha < 1f)
			{
				// Interpolation process continues
				mTrans.position = Vector3.Lerp(mPos, t.position, mAlpha);
				mTrans.rotation = Quaternion.Slerp(mRot, t.rotation, mAlpha);
			}
			else
			{
				// Interpolation finished -- parent the camera to the target and assume its orientation
				mTrans.parent = t;
				mTrans.position = t.position;
				mTrans.rotation = t.rotation;
			}
		}

		// Update the directional and flat directional vectors
		direction = mTrans.rotation * Vector3.forward;
		flatDirection = direction;
		flatDirection.y = 0f;
		flatDirection.Normalize();
	}
	
	void OnDestroy()
	{
		mInstance = null;
	}
}