using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Game/Unit")]
public class GameUnit : MonoBehaviour
{
	public enum UnitType {
		Business,
		Pirate,
		RoyalNavy
	}
	// List of all in-game units
	static List<GameUnit> mList;
	static Dictionary<UnitType, List<GameUnit>> mAllListDict = new Dictionary<UnitType, List<GameUnit>>();
	
	public UnitType MyType;

	// Animation to trigger when the unit gets destroyed
	public Animation destroyAnimation;

	// In percent: 0.3 means that only 70% damage will be applied
	public float damageReduction = 0f;

	// Current and maximum hull health
	public Vector2 health = new Vector2(100f, 100f);

	// Cache the transform for speed
	protected Transform mTrans;

	// Flag gets set to 'true' once the unit gets destroyed. Used to despawn the unit.
	bool mDestroyed = false;

	// List of all colliders belonging to this unit
	public Collider[] colliders;

	/// <summary>
	/// Helper function that finds the unit that contains the specified child in its transform hierarchy.
	/// </summary>

	static public GameUnit Find (Transform trans)
	{
		while (trans != null)
		{
			GameUnit unit = trans.GetComponent<GameUnit>();
			if (unit != null) return unit;
			trans = trans.parent;
		}
		return null;
	}

	/// <summary>
	/// Find the unit that's easiest to aim at given the specified direction.
	/// </summary>

	static public GameUnit Find (GameUnit myUnit, Vector3 dir, float maxRange, float maxAngle, UnitType [] enemyType)
	{
		GameUnit bestUnit = null;

		if (myUnit != null)
		{
			float bestValue = 0f;
			Vector3 pos = myUnit.transform.position;
			List<GameUnit> list = new List<GameUnit>();
			foreach(UnitType type in enemyType)
			{
				list.AddRange(mAllListDict[type]);
			}
			foreach (GameUnit unit in list)
			{
				if (unit == myUnit || unit == null || unit.mTrans == null) continue;

				// If the unit is too far, move on to the next
				Vector3 unitDir = unit.mTrans.position - pos;
				float distance = unitDir.magnitude;
				if (distance > maxRange || distance < 0.01f) continue;

				// Normalize the distance and determine the dot product
				if (distance != 0f) unitDir *= 1.0f / distance;

				// Calculate the angle
				float angle = Vector3.Angle(dir, unitDir);

				// The angle must be within the sensor threshold
				if (angle < maxAngle)
				{
					// Calculate the value of this target
					float val = (maxRange - distance) / maxRange * (1f - angle / maxAngle);

					if (val > bestValue)
					{
						bestValue = val;
						bestUnit = unit;
					}
				}
			}
		}
		return bestUnit;
	}
	
	/// <summary>
	/// Find the closest enemy unit.
	/// </summary>

	static public GameUnit Find (GameUnit myUnit, float maxRange, UnitType [] types)
	{
		GameUnit bestUnit = null;

		if (myUnit != null)
		{
			Vector3 pos = myUnit.transform.position;
			List<GameUnit> list = new List<GameUnit>();
			foreach(UnitType type in types)
			{
				list.AddRange(mAllListDict[type]);
			}
			float closest = float.MaxValue;
			foreach (GameUnit unit in list)
			{
				if (unit == myUnit || unit == null || unit.mTrans == null) continue;

				// If the unit is too far, move on to the next
				Vector3 unitDir = unit.mTrans.position - pos;
				float distance = unitDir.magnitude;
				if (distance > maxRange || distance < 0.01f) continue;

				if( distance < closest ) {
					closest = distance;
					bestUnit = unit;
				}
			}
		}
		return bestUnit;
	}

	/// <summary>
	/// Add this unit to the list of in-game units.
	/// </summary>

	void OnEnable () { mList.Add(this); }

	/// <summary>
	/// Remove this unit from the list.
	/// </summary>

	void OnDisable () { mList.Remove(this); }
	
	void Awake()
	{
		if( mAllListDict.ContainsKey(MyType)) {
			mList = mAllListDict[MyType];
		}
		else {
			mList = new List<GameUnit>();
			mAllListDict.Add(MyType, mList);
		}
	}
	
	/// <summary>
	/// Remember all colliders belonging to this unit.
	/// </summary>
	/// 

	void Start ()
	{
		mTrans = transform;
		colliders = GetComponentsInChildren<Collider>();
		OnStart();
	}

	/// <summary>
	/// Returns 'true' if the unit has the specified transform in its hierarchy.
	/// </summary>

	public bool IsParentOf (Transform t)
	{
		while (t != null)
		{
			if (t == mTrans) return true;
			t = t.parent;
		}
		return false;
	}

	/// <summary>
	/// Apply the specified amount of damage to the unit. Returns the actual amount of damage inflicted.
	/// </summary>

	public float ApplyDamage (float val, GameObject go)
	{
		if (mDestroyed) return 0f;
		if (val < 0f) val = 0f;
		val *= (1.0f - damageReduction);
		val = Mathf.Min(health.x, val);
		health.x -= val;

		if (health.x == 0f)
		{
			// The ship can now be considered destroyed
			mDestroyed = true;

			// Play the death animation, if any
			if (destroyAnimation != null) destroyAnimation.Play();

			// Notify all attached scripts that the ship has been destroyed by something
			gameObject.SendMessage("OnDestroyedBy", go, SendMessageOptions.DontRequireReceiver);
		}
		return val;
	}

	/// <summary>
	/// Wait for the animation to stop playing
	/// </summary>

	void Update ()
	{
		if (mDestroyed && (destroyAnimation == null || !destroyAnimation.isPlaying))
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Virtual Start() functionality, if desired.
	/// </summary>

	protected virtual void OnStart () { }
}