using UnityEngine;

[AddComponentMenu("Exploration/Ship Camera")]
public class ShipCamera : MonoBehaviour
{
	public ShipController control;
	public AnimationCurve distance;
	public AnimationCurve angle;

	Transform mTrans;

	void Start ()
	{
		mTrans = transform;
	}

	void Update ()
	{
		if (control != null)
		{
			float speed = control.speed;
			Quaternion rot = Quaternion.Euler(angle.Evaluate(speed), 0f, 0f);
			mTrans.localPosition = rot * Vector3.back * distance.Evaluate(speed);
			mTrans.localRotation = rot;
		}
	}
}