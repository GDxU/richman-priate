using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Strategy/Trade Route")]
public class TradeRoute : MonoBehaviour
{
	[System.Serializable]
	public class Item
	{
		public int 	id  	= 0;	// Resource's ID
		public Town town 	= null;	// Town that owns this resource
	}
	
	static public List<TradeRoute> list = new List<TradeRoute>();
	static public float globalAlpha = 1f;
	
	// The two connected towns
	public Town town0 = null;
	public Town town1 = null;
	
	// Controls whether this trade route is visible
	public float targetAlpha = 1.0f;
	
	// Texture used to draw the path
	public Texture2D texture = null;
	
	// Shared material
	private static Material mMat = null;
	
	// Spline created with the points above
	private SplineV 		mOriginal	 	= new SplineV();
	private SplineV			mNormalized		= new SplineV();
	private bool 			mRebuild 		= false;
	private Mesh 			mMesh 			= null;
	private MeshFilter		mFilter			= null;
	private MeshRenderer	mRen			= null;
	private float			mAlpha			= 0f;
	private float			mLength			= 0f;
	private Vector3			mTooltipPos;
	
	/// <summary>
	/// List of traded items
	/// </summary>

	public List<Item> items = new List<Item>();
	
	/// <summary>
	/// List of all ships assigned to this trade route
	/// </summary>

	public List<AvailableShips.Owned> ships = new List<AvailableShips.Owned>();
	
	/// <summary>
	/// Read-only access to connected towns
	/// </summary>

	public SplineV path { get { return mOriginal; } }
	
	/// <summary>
	/// Gets the normalized spline path.
	/// </summary>
	
	public SplineV normalizedPath { get { return mNormalized; } }
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="TradeRoute"/> is valid.
	/// </summary>
	
	public bool isValid { get { return (town0 != null) && (town1 != null); } }
	
	/// <summary>
	/// Gets the length of the trade route.
	/// </summary>
	
	public float length { get { return isValid ? mLength : 0f; } }
	
	/// <summary>
	/// Sample the trade route spline at the specified time.
	/// </summary>
	
	public Vector3 Sample (float time) { return mOriginal.Sample(time, SplineV.SampleType.Spline); }
	
	/// <summary>
	/// Connect the specified town.
	/// </summary>
	
	public bool Connect (Town town)
	{
		if (town0 == null)
		{
			town0 = town;
			Add(town.anchor);
		}
		else if (town1 == null && town0 != town)
		{
			Add(town.anchor);
			town1 = town;
		}
		return (town1 != null);
	}
	
	/// <summary>
	/// Adds a new point to the trade route.
	/// </summary>
	
	public void Add (Vector3 v)
	{
		if (town0 != null && town1 == null)
		{
			v.y = 0.05f;
			
			if (mOriginal.isValid)
			{
				mOriginal.AddKey(mOriginal.endTime + (v - mOriginal.end).magnitude, v);
			}
			else
			{
				mOriginal.AddKey(0.0f, v);
			}
			mRebuild = true;
		}
	}
	
	/// <summary>
	/// Removes the last added trade route point.
	/// </summary>
	
	public bool UndoAdd()
	{
		if (mOriginal.isValid)
		{
			mOriginal.list.RemoveAt(mOriginal.list.Count - 1);
			mRebuild = true;
			return true;
		}
		return false;
	}
	
	/// <summary>
	/// Copies the trade route path from the specified trade route.
	/// </summary>
	
	public void CopyPath (SplineV sp)
	{
		mRebuild = true;
		mOriginal.Clear();
		foreach (SplineV.CtrlPoint cp in sp.list) mOriginal.AddKey(cp.mTime, cp.mVal);
	}
	
	/// <summary>
	/// Returns the town connected to the specified town if it's in this trade route.
	/// </summary>
	
	public Town GetConnectedTown (Town town)
	{
		if (town0 == town) return town1;
		if (town1 == town) return town0;
		return null;
	}
	
	/// <summary>
	/// Gets the traded resource's owner, or null if it's not being traded.
	/// </summary>
	
	public Town GetExportedResourceOwner (int id)
	{
		foreach (Item item in items)
		{
			if (item.id == id)
			{
				return item.town;
			}
		}
		return null;
	}
	
	/// <summary>
	/// Sets the specified town's resource as being traded via the trade route.
	/// </summary>
	
	public void SetExportedResource (Town owner, int id)
	{
		foreach (Item item in items)
		{
			if (item.id == id)
			{
				if (owner == null)
				{
					items.Remove(item);
					item.town.resources[id].beingTraded = false;
				}
				else item.town = owner;
				return;
			}
		}
		
		if (owner != null)
		{
			Item item = new Item();
			item.town = owner;
			item.id = id;
			items.Add(item);
			owner.resources[id].beingTraded = true;
		}
	}
	
	/// <summary>
	/// Assigns the specified ship to this trade route.
	/// </summary>

	public void AssignShip (AvailableShips.Owned ship)
	{
		if (ship.tradeRoute != this)
		{
			if (ship.tradeRoute != null) ship.tradeRoute.UnassignShip(ship);
			ship.tradeRoute = this;
			ships.Add(ship);

			if (ship.prefab != null)
			{
				Vector3 start = mOriginal.Sample(0f, SplineV.SampleType.Linear);
				Vector3 next  = mOriginal.Sample(1f, SplineV.SampleType.Linear);
				GameObject go = Instantiate(ship.prefab.prefab, start, Quaternion.LookRotation(next - start)) as GameObject;

				if (go != null)
				{
					// Replace the possible multiple colliders with a single one residing at root
					Collider[] cols = go.GetComponentsInChildren<Collider>();

					if (cols.Length > 1)
					{
						foreach (Collider col in cols) Destroy(col);
						go.AddComponent<BoxCollider>();
					}

					ship.asset = go;
					go.AddComponent<Highlightable>();
					TradeShip script = go.AddComponent<TradeShip>();
					script.tradeRoute = this;
					script.prefab = ship.prefab;
					script.speed = ship.prefab.speed;
				}
			}
		}
	}
	
	/// <summary>
	/// Unassigns the specified ship from this trade route.
	/// </summary>

	public void UnassignShip (AvailableShips.Owned ship)
	{
		if (ship.tradeRoute == this)
		{
			ships.Remove(ship);
			ship.tradeRoute = null;

			if (ship.asset != null)
			{
				Object.Destroy(ship.asset);
				ship.asset = null;
			}
		}
	}
	
	/// <summary>
	/// Adds this trade route to the list.
	/// </summary>
	
	void OnEnable()
	{
		list.Add(this);
		name = "Trade Route " + GetInstanceID();
	}
	
	/// <summary>
	/// Removes this trade route from the list.
	/// </summary>
	
	void OnDisable()
	{
		list.Remove(this);
		
		if (mFilter != null)
		{
			Object.Destroy(mFilter);
			mFilter = null;
		}
		
		if (mMesh != null)
		{
			Object.Destroy(mMesh);
			mMesh = null;
		}
		
		mRebuild = true;
		if (Config.Instance != null) Config.Instance.onGUI.Remove(DrawGUI);
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	
	void Start()
	{
		Config.Instance.onGUI.Add(DrawGUI);
	}
	
	/// <summary>
	/// Draw the trade route's name.
	/// </summary>
	
	void DrawGUI()
	{
		if (town0 != null && town1 != null && mAlpha > 0.001f)
		{
			Vector3 pos = UI.GetScreenPos(mTooltipPos);
			
			if (UI.IsVisible(pos) && pos.z < 100f)
			{
				Rect rect = new Rect(pos.x - 150f, pos.y - 20f, 300f, 40f);
				bool hover = rect.Contains(UI.GetMousePos());
				targetAlpha = hover ? 1f : 0.5f;
				
				UI.SetAlpha(mAlpha);
				
				UI.DrawTitle(new Rect(pos.x - 150f, pos.y - 20f, 300f, 20f),
					town0.name + " - " + town1.name, Config.Instance.infoStyle);
				
				//UI.DrawTitle(new Rect(pos.x - 150f, pos.y, 300f, 20f),
				//	Mathf.RoundToInt(mLength) + " miles, " + (GetIncomePerTurn() * 6) + " gold/day",
				//	Config.Instance.infoStyle);
				
				UI.RestoreAlpha();
			}
		}
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	
	void Update()
	{
		bool wasVisible = mAlpha > 0.001f;
		float factor = Mathf.Min(1f, Time.deltaTime * 5f);
		mAlpha = Mathf.Lerp(mAlpha, targetAlpha * globalAlpha, factor);
		
		if (mAlpha > 0.001f)
		{
			if (mRebuild)
			{
				if (mOriginal.isValid)
				{
					mRebuild = false;
					
					if (mMesh == null)
					{
						mMesh = new Mesh();
						mMesh.name = "Trade Route " + GetInstanceID();
					}
					
					if (mFilter == null)
					{
						mFilter = gameObject.AddComponent<MeshFilter>();
						mFilter.mesh = mMesh;
					}
					
					if (mMat == null)
					{
						Shader shader = Shader.Find("Transparent/Diffuse");
						mMat = new Material(shader);
						mMat.name = "Trade Route";
						mMat.mainTexture = texture;
					}
					
					if (mRen == null)
					{
						mRen = gameObject.AddComponent<MeshRenderer>();
						mRen.material = mMat;
						mRen.castShadows = false;
						mRen.receiveShadows = false;
					}
				
					// Find the center of the spline
					Vector3 center = Vector3.zero;
					foreach (SplineV.CtrlPoint cp in mOriginal.list) center += cp.mVal;
					center *= 1.0f / mOriginal.list.Count;
					
					// Reposition the trade route
					transform.position = center;
					
					// Re-create the mesh
					mLength = mOriginal.GetMagnitude();
					int subdivisions = Mathf.RoundToInt(mLength);
					CreateMesh(mOriginal, -center, 0.15f, subdivisions);
					
					// Create the normalized path that will be used for sampling
					mNormalized = SplineV.Normalize(mOriginal, subdivisions);
					mLength = mNormalized.GetMagnitude();
				}
				else if (mMesh != null)
				{
					mMesh.Clear();
					if (mRen != null) mRen.enabled = false;
				}
			}
			
			// Update the material color
			if (mRen != null)
			{
				mRen.enabled = true;
				mRen.material.color = isValid ? new Color(1f, 1f, 1f, mAlpha * 0.5f) : new Color(1f, 1f, 1f, mAlpha * 0.25f);
			}
		}
		else if (wasVisible)
		{
			mAlpha = 0f;
			mRen.enabled = false;
		}
	}
	
	/// <summary>
	/// Creates a trade route mesh.
	/// </summary>
	
	void CreateMesh (SplineV initial, Vector3 offset, float width, int subdivisions)
	{
		mMesh.Clear();
		if (initial.list.Count < 2) return;
		
		SplineV spline = SplineV.Normalize(initial, subdivisions * 4);
		
		float start = spline.startTime;
		float length = spline.endTime - start;
		
		// We will need the spline's center for tooltip purposes
		mTooltipPos = spline.Sample(start + length * 0.5f, SplineV.SampleType.Spline);
		
		List<Vector3> v = new List<Vector3>();
		List<Vector3> n = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> faces = new List<int>();
		
		++subdivisions;
		
		for (int i = 0; i < subdivisions; ++i)
		{
			float f0 = (float)(i - 1) / subdivisions;
			float f1 = (float)(i    ) / subdivisions;
			float f2 = (float)(i + 1) / subdivisions;
			float f3 = (float)(i + 2) / subdivisions;
			
			Vector3 s0 = spline.Sample(start + f0 * length, SplineV.SampleType.Linear);
			Vector3 s1 = spline.Sample(start + f1 * length, SplineV.SampleType.Linear);
			Vector3 s2 = spline.Sample(start + f2 * length, SplineV.SampleType.Linear);
			Vector3 s3 = spline.Sample(start + f3 * length, SplineV.SampleType.Linear);
			
			Vector3 dir0 = (s2 - s0).normalized;
			Vector3 dir1 = (s3 - s1).normalized;
			
			// Cross(dir, up)
			Vector3 tan0 = new Vector3(-dir0.z, 0f, dir0.x);
			Vector3 tan1 = new Vector3(-dir1.z, 0f, dir1.x);
			
			tan0 *= width;
			tan1 *= width;
			
			Vector3 v0 = s1 - tan0;
			Vector3 v1 = s2 - tan1;
			Vector3 v2 = s2 + tan1;
			Vector3 v3 = s1 + tan0;
			
			v.Add(offset + v1);
			n.Add(Vector3.up);
			uv.Add(new Vector2(1.0f, f2));
			
			v.Add(offset + v0);
			n.Add(Vector3.up);
			uv.Add(new Vector2(1.0f, f1));
			
			v.Add(offset + v3);
			n.Add(Vector3.up);
			uv.Add(new Vector2(0.0f, f1));
			
			v.Add(offset + v2);
			n.Add(Vector3.up);
			uv.Add(new Vector2(0.0f, f2));
		}
		
		for (int i = 0; i < v.Count; i += 4)
		{
			faces.Add(i);
			faces.Add(i+1);
			faces.Add(i+2);
			faces.Add(i+2);
			faces.Add(i+3);
			faces.Add(i);
		}
		
		// Assign the mesh data
		mMesh.vertices = v.ToArray();
		mMesh.normals = n.ToArray();
		mMesh.uv = uv.ToArray();
		mMesh.triangles = faces.ToArray();
	}
	
	/// <summary>
	/// Finds the next trade route connected to the specified town.
	/// </summary>
	
	static public TradeRoute FindNext (TradeRoute tradeRoute, Town town, bool reverse)
	{
		bool found = false;
		TradeRoute first = null;
		TradeRoute last = null;
		
		foreach (TradeRoute tr in TradeRoute.list)
		{
			if (tr == tradeRoute)
			{
				// Now that we've found the current node, if we're going in reverse, we can use the last node
				if (reverse && last != null) return last;

				// Remember that we've found the current node
				found = true;
			}
			else if (tr.GetConnectedTown(town) != null)
			{
				// If the current node has already been found and we're going in order, we're done
				if (found && !reverse) return tr;

				// Remember this node
				if (first == null) first = tr;
				last = tr;
			}
		}
		
		// If we were going in reverse, just return the last available node
		if (reverse) return (last == null) ? tradeRoute : last;

		// Going in order? Just return the first node.
		return (first == null) ? tradeRoute : first;
	}
}