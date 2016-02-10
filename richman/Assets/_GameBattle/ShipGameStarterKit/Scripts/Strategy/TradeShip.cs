using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Strategy/Trade Ship")]
public class TradeShip : MonoBehaviour
{
	[System.Serializable]
	public class CargoEntry
	{
		public int id = 0;
		public int amount = 0;
		public Town owner = null;
	}

	public float			speed		= 0f;
	public float			distance	= 0f;
	public TradeRoute 		tradeRoute 	= null;
	public AvailableShips.Template 	prefab 		= null;

	public List<CargoEntry> cargo = new List<CargoEntry>();

	Transform	mTrans;
	Vector2 	mOffset;
	Vector3 	mTargetPos;
	Quaternion 	mTargetRot;
	float 		mStartTime 	= Time.time + 2f;
	float 		mNextUpkeep = Time.time + 1f;
	Town		mLastTown	= null;
	
	int	mCurrentRevenue = 0;
	int	mCurrentUpkeep	= 0;
	int mLastRevenue 	= 0;
	int mLastUpkeep 	= 0;
	float mNextWeek 	= Time.time + 60f;
	
	/// <summary>
	/// Returns the amount of cargo currently in the ship's cargo hold.
	/// </summary>
	
	public int cargoWeight
	{
		get
		{
			int amount = 0;
			foreach (CargoEntry ent in cargo) amount += ent.amount;
			return amount;
		}
	}
	
	/// <summary>
	/// Returns the current cargo allowance of the ship.
	/// </summary>
	
	public int cargoAllowance
	{
		get
		{
			return Mathf.Max(0, prefab.cargo - cargoWeight);
		}
	}
	
	/// <summary>
	/// Position the ship at the beginning of the trade route.
	/// </summary>
	
	void OnEnable()
	{
		mTrans = transform;
		mTargetPos = mTrans.position;
		mTargetRot = mTrans.rotation;
		mOffset.x = Random.Range(0.0f, 10.0f);
		mOffset.y = Random.Range(0.0f, 10.0f);
	}
	
	/// <summary>
	/// Show a useful tooltip regarding the ship's weekly income.
	/// </summary>
	
	void OnMouseEnter()
	{
		int profit = mLastRevenue - mLastUpkeep;
		if (profit == 0) profit = mCurrentRevenue - mCurrentUpkeep;
		
		if (profit < 0)
		{
			ScrollingCombatText.Print(gameObject, "Weekly Loss: $" + profit, Color.red);
		}
		else
		{
			ScrollingCombatText.Print(gameObject, "Weekly Profit: $" + profit,
				(profit < prefab.price / 10) ? Color.yellow : Color.green);
		}
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	
	void Update()
	{
		// Update the last week's statistics if it's time
		if (mNextWeek < Time.time)
		{
			mNextWeek = Time.time + 60f;

			mLastRevenue = mCurrentRevenue;
			mLastUpkeep  = mCurrentUpkeep;
			
			mCurrentRevenue = 0;
			mCurrentUpkeep 	= 0;
		}
		
		// Ship upkeep cost
		if (mNextUpkeep < Time.time)
		{
			mNextUpkeep = Time.time + 1f;
			int upkeep = Mathf.RoundToInt(prefab.price * 0.01f);
			mCurrentUpkeep += upkeep;
			Config.Instance.gold -= upkeep;
		}
		
		// If this is a brand new ship, dock it at the first town
		if (mLastTown == null) DockShip(tradeRoute.town0);
		
		// Calculate the bobble rotation
		Vector3 rot = new Vector3( Mathf.Sin(mOffset.x + Time.time * 0.7326f) * 0.75f, 0f,
			Mathf.Sin(mOffset.y + Time.time * 1.2265f) * 1.5f );
		Quaternion bobble = Quaternion.Euler(rot);
		
		if (tradeRoute == null)
		{
			mTrans.rotation = bobble;
		}
		else
		{
			// If it's time to start moving the ship, do that
			if (mStartTime < Time.time)
			{
				// Ships's maximum speed in units per second
				float maxSpeed = prefab.speed * 0.1f;

				// Ships should start with the speed of 0 and accelerate gradually
				float acceleration = 0.05f * prefab.acceleration * Time.deltaTime;
				
				// Adjust the traveling speed
				speed = Mathf.Min(speed + acceleration, maxSpeed);
				
				// Distance the ship has traveled since it left the dock
				distance += speed * Time.deltaTime;
				
				// Sampling factor in 0-1 range
				float length = tradeRoute.length;
				float factor = Mathf.Clamp01(distance / length);
				
				Vector3 nextPos;
				
				if (mLastTown == tradeRoute.town0)
				{
					// Traveling from Town0 to Town1
					float time 	= Interpolation.Linear(0f, length, factor);
					mTargetPos 	= tradeRoute.normalizedPath.Sample(time, SplineV.SampleType.Linear);
					nextPos 	= tradeRoute.normalizedPath.Sample(time + 1f, SplineV.SampleType.Linear);

					if (factor == 1f) DockShip(tradeRoute.town1);
				}
				else
				{
					// Traveling from Town1 to Town0
					float time 	= Interpolation.Linear(length, 0f, factor);
					mTargetPos 	= tradeRoute.normalizedPath.Sample(time, SplineV.SampleType.Linear);
					nextPos 	= tradeRoute.normalizedPath.Sample(time - 1f, SplineV.SampleType.Linear);
					
					if (factor == 1f) DockShip(tradeRoute.town0);
				}
				
				// Calculate the rotation
				Vector3 diff = nextPos - mTargetPos;
				if (diff.magnitude > 0.01f) mTargetRot = Quaternion.LookRotation(diff);
			}
			else
			{
				speed 	 = 0f;
				distance = 0f;
			}

			// Update the position
			{
				float factor = Time.deltaTime * 5.0f;
				mTrans.position = Vector3.Lerp(mTrans.position, mTargetPos, factor);
				mTrans.rotation = Quaternion.Slerp(mTrans.rotation, bobble * mTargetRot, factor);
			}
		}
	}
	
	/// <summary>
	/// Dock the ship at the specified town.
	/// </summary>
	
	void DockShip (Town town)
	{
		int gold = SellCargo(town);
		
		if (gold > 0)
		{
			mCurrentRevenue += gold;
			Config.Instance.gold += gold;
			ScrollingCombatText.Print(gameObject, "$" + gold, (gold < prefab.price / 10) ? Color.yellow : Color.green);
		}
		LoadCargo(town);

		mStartTime = Time.time + 2f;
		mLastTown = town;
	}
	
	/// <summary>
	/// Sell all of the ship's cargo at the specified town.
	/// </summary>
	
	int SellCargo (Town town)
	{
		int profit = 0;
		
		for (int i = cargo.Count; i > 0; )
		{
			CargoEntry ent = cargo[--i];

			if (ent.owner != town)
			{
				Town.ResourceEntry res = town.resources[ent.id];
				
				if (res.production < 0)
				{
					int available = ent.amount;
					int demand = -Mathf.RoundToInt(res.warehouse);
					
					// Remove this cargo entry from the ship's hold
					cargo.RemoveAt(i);

					// Move the ship's contents into the warehouse
					res.warehouse += available;
					
					// Now it's time to calculate how much the town will pay for these goods
					int amount = Mathf.Max(0, demand + (res.production * 3) / 2);
					amount = Mathf.Min(available, amount);
					
					// High demand
					if (amount > 0)
					{
						profit += amount * 7;
						demand -= amount;
						available -= amount;
					}
					
					// Medium demand
					if (available > 0 && demand > 0)
					{
						amount = Mathf.Min(available, demand);
						profit += amount * 5;
						demand -= amount;
						available -= amount;
					}
					
					// Low demand
					if (available > 0)
					{
						profit += available * 2;
					}
				}
			}
		}
		return profit;
	}
	
	/// <summary>
	/// Load the town's exported cargo onto the ship.
	/// </summary>
	
	void LoadCargo (Town town)
	{
		int available = cargoAllowance;
		int stock = 0;
		
		// Add up the town's exported stockpiles
		foreach (TradeRoute.Item item in tradeRoute.items)
		{
			if (item.town == town)
			{
				Town.ResourceEntry res = town.resources[item.id];
				int amount = Mathf.RoundToInt(res.warehouse);
				stock += amount;
			}
		}
		
		// If we can carry something, let's load it onto the ship
		if (stock > 0 && available > 0)
		{
			// We want to evenly load all available goods
			float factor = Mathf.Min((float)available / stock, 1f);
			
			foreach (TradeRoute.Item item in tradeRoute.items)
			{
				if (item.town == town)
				{
					Town.ResourceEntry res = town.resources[item.id];
					int amount = Mathf.Min(available, Mathf.RoundToInt(res.warehouse * factor));
					
					if (amount > 0)
					{
						CargoEntry ce = new CargoEntry();
						ce.id = item.id;
						ce.amount = amount;
						ce.owner = town;
						res.warehouse -= amount;
						available -= amount;
						cargo.Add(ce);
						
						// If we reach the limit, we want to end the loading process
						if (available == 0) break;
					}
				}
			}
		}
	}
}