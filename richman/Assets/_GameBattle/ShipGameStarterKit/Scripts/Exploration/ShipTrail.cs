using UnityEngine;

[RequireComponent(typeof(ImprovedTrail))]
[AddComponentMenu("Exploration/Ship Trail")]
public class ShipTrail : MonoBehaviour
{
	public ShipController control;

	ImprovedTrail mTrail;

	void Start ()
	{
		mTrail = GetComponent<ImprovedTrail>();
	}

	void Update ()
	{
		if (control != null)
		{
			mTrail.alpha = control.speed;
		}
	}
}