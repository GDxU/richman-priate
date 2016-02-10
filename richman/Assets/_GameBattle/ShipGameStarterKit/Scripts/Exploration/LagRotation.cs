using UnityEngine;

[AddComponentMenu("Exploration/Lag Rotation")]
public class LagRotationExploration : MonoBehaviour
{
	public float speed = 10f;
	
	Transform mTrans;
	Transform mParent;
	Quaternion mRelative;
	Quaternion mParentRot;
	
	void Start()
	{
		mTrans = transform;
		mParent = mTrans.parent;
		mRelative = mTrans.localRotation;
		mParentRot = mParent.rotation;
	}
	
	void LateUpdate()
	{
		mParentRot = Quaternion.Slerp(mParentRot, mParent.rotation, Time.deltaTime * speed);
		mTrans.rotation = mParentRot * mRelative;
	}
}