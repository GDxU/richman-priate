using UnityEngine;

[AddComponentMenu("Exploration/Ship Tilt")]
public class ShipTilt : MonoBehaviour
{
	public ShipController control;
	public float degrees = 12f;

	Transform mTrans;

	void Start ()
	{
		mTrans = transform;
	}

	void Update()
	{
		if (control != null)
		{
			mTrans.localRotation = Quaternion.Euler(0f, 0f, control.steering * degrees);
		}
	}
}