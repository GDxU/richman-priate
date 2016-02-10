using UnityEngine;

[AddComponentMenu("Exploration/Ship Bobble")]
public class ShipBobble : MonoBehaviour
{
	public ShipController control;

	Transform mTrans;
	Vector3 mOffset;
	Vector2 mTime;

	void Start ()
	{
		mTrans = transform;
		mOffset.x = Random.Range(0.0f, 10.0f);
		mOffset.y = Random.Range(0.0f, 10.0f);
	}

	void Update ()
	{
		float strength = 1f + ((control != null) ? control.speed : 0f);
		float delta = Time.deltaTime * strength;

		mTime.x += delta * 0.7326f;
		mTime.y += delta * 1.2265f;

		// Calculate the bobble rotation
		Vector3 rot = new Vector3(
			Mathf.Sin(mOffset.x + mTime.x) * 0.75f * strength, 0f,
			Mathf.Sin(mOffset.y + mTime.y) * 1.5f  * strength);
		mTrans.localRotation = Quaternion.Euler(rot);
	}
}