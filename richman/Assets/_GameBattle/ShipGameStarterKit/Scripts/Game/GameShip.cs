using UnityEngine;

[AddComponentMenu("Game/Ship")]
public class GameShip : GameUnit
{
	// Units per second
	public float maxMovementSpeed = 7f;

	// Degrees per second
	public float maxTurningSpeed = 60f;

	// Same as hull damage, but for sails
	public float sailDamageReduction = 0f;

	// Current and maximum sail health
	public Vector2 sailHealth = new Vector2(100f, 100f);
	
	private GameObject mNextShip;
	public GameObject NextShip { get { return mNextShip; }}
	Vector3 mLastPos;
	Vector3 mVelocity;

	/// <summary>
	/// Calculated movement speed depends on the current condition of the sails.
	/// </summary>

	public float movementSpeed { get { return maxMovementSpeed * sailHealth.x / sailHealth.y; } }

	/// <summary>
	/// Calculated turning speed depends on the current condition of the hull.
	/// </summary>

	public float turningSpeed { get { return maxTurningSpeed * health.x / health.y; } }

	/// <summary>
	/// Current velocity in units per second.
	/// </summary>

	public Vector3 velocity { get { return mVelocity; } }

	/// <summary>
	/// Helper function that finds the ship stats script that contains the specified child in its transform hierarchy.
	/// </summary>

	new static public GameShip Find (Transform trans)
	{
		while (trans != null)
		{
			GameShip stats = trans.GetComponent<GameShip>();
			if (stats != null) return stats;
			trans = trans.parent;
		}
		return null;
	}

	/// <summary>
	/// Cache some values.
	/// </summary>

	protected override void OnStart ()
	{
		mLastPos = mTrans.position;
	}

	/// <summary>
	/// Apply the specified amount of damage to the ship's sails.
	/// </summary>

	public float ApplyDamageToSails (float val)
	{
		if (val < 0f) val = 0f;
		val *= (1.0f - sailDamageReduction);
		val = Mathf.Min(sailHealth.x, val);
		sailHealth.x -= val;
		return val;
	}

	/// <summary>
	/// Calculate the ship's velocity.
	/// </summary>

	void LateUpdate ()
	{
		Vector3 pos = mTrans.position;
		mVelocity = (pos - mLastPos) * (1.0f / Time.deltaTime);
		mLastPos = pos;
	}
}