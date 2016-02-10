using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Game/Reposition Water")]
public class RepositionWater : MonoBehaviour
{
	Transform mTrans;
	Transform mCamTrans;

	void Start ()
	{
		mTrans = transform;
		mCamTrans = Camera.main.transform;
	}

	void LateUpdate()
	{
		if (mCamTrans != null)
		{
			Vector3 pos = mCamTrans.position;
			pos.y = 0.0f;

			if (mTrans.position != pos)
			{
				mTrans.rotation = Quaternion.identity;
				mTrans.position = pos;
			}
		}
	}
}