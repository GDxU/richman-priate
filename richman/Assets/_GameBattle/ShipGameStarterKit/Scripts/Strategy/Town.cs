using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Strategy/Town")]
public class Town : MonoBehaviour
{
	static public List<Town> list = new List<Town>();
	
	[System.Serializable]
	public class ResourceEntry
	{
		public int production = 0;
		public float warehouse = 0;
		public bool beingTraded = false;
	}
	
	// Currently selected trade route
	TradeRoute mSelectedRoute = null;
	
	// Private variables
	bool 	mShowInfo 		= false;
	float 	mAlpha 			= 0.0f;
	float 	mNextUpdate 	= 0f;
	float 	mUpdateInterval = 1f;
	float 	mUpdateFactor 	= 1f / 10f;
	float 	mSpoilRate 		= 0.975f;
	Vector3	mAnchor;
	bool	mAnchorFound	= false;
	
	// Starting resources, will be removed
	public int wheat 	= 0;
	public int food		= 0;
	public int wood		= 0;
	public int coal		= 0;
	public int ore		= 0;
	public int iron		= 0;
	public int tools	= 0;
	
	// Town's available resources
	List<ResourceEntry> mResources = new List<ResourceEntry>();

	// When we're dragging a resource, we need to know what it is we're dragging and from which town
	static int mDragResource = -1;
	static Town mDragTown = null;
	static AvailableShips.Template mDragPrefab = null;
	static AvailableShips.Owned mDragShip = null;
	
	/// <summary>
	/// Gets the town's resources.
	/// </summary>
	
	public List<ResourceEntry> resources
	{
		get
		{
			if (mResources.Count == 0) InitResources();
			return mResources;
		}
	}
	
	/// <summary>
	/// Gets the town's anchor point.
	/// </summary>
	
	public Vector3 anchor
	{
		get
		{
			if (!mAnchorFound)
			{
				mAnchorFound = true;
				Transform child = transform.Find("Anchor");
				mAnchor = (child == null) ? transform.position : child.position;
			}
			return mAnchor;
		}
	}
	
	/// <summary>
	/// Whether the town's detailed information is currently shown.
	/// Will automatically focus the camera on a child named 'Viewpoint'
	/// </summary>
	
	public bool showInfo
	{
		get
		{
			return mShowInfo;
		}
		set
		{
			if (mShowInfo != value)
			{
				mShowInfo = value;
				Transform view = mShowInfo ? transform.FindChild("Viewpoint") : null;
				StrategicCamera.viewpoint = view;
			}
		}
	}
	
	/// <summary>
	/// Gets the town resource of specified index corresponding to TownResources' list.
	/// </summary>
	
	int GetStartingResource (int index)
	{
		switch (index)
		{
			case 0: return wheat;
			case 1: return food;
			case 2: return wood;
			case 3: return coal;
			case 4: return ore;
			case 5: return iron;
			case 6: return tools;
		}
		return wheat;
	}
	
	/// <summary>
	/// Initialize the resources.
	/// </summary>
	
	void InitResources()
	{
		if (mResources.Count == 0)
		{
			for (int i = 0; i < TownResources.Instance.list.Count; ++i)
			{
				ResourceEntry entry = new ResourceEntry();
				entry.production = GetStartingResource(i);
				entry.warehouse = 0;
				mResources.Add(entry);
			}
		}
	}
	
	/// <summary>
	/// Finds the town the specified game object belongs to.
	/// </summary>
	
	public static Town Find (GameObject go)
	{
		while (go != null)
		{
			Town town = go.GetComponent<Town>();
			if (town != null) return town;
			Transform trans = go.transform.parent;
			if (trans == null) break;
			go = trans.gameObject;
		}
		return null;
	}

	/// <summary>
	/// Notification of a trade route being removed.
	/// </summary>
	
	public void DisconnectTradeRoute (TradeRoute tr) { if (tr == mSelectedRoute) mSelectedRoute = null; }
	
	/// <summary>
	/// Cancel the icon dragging operation.
	/// </summary>
	
	void CancelDrag()
	{
		mDragShip 		= null;
		mDragPrefab 	= null;
		mDragResource 	= -1;
		mDragTown 		= null;
	}
	
	/// <summary>
	/// Add this town to the list.
	/// </summary>
	
	void OnEnable() { list.Add(this); }
	
	/// <summary>
	/// Remove this town from the list.
	/// </summary>
	
	void OnDisable() { list.Remove(this); }
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	
	void Start()
	{
		InitResources();
		Config.Instance.onGUI.Add(DrawGUI);
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	
	void Update()
	{
		if (mNextUpdate < Time.time)
		{
			mNextUpdate = Time.time + mUpdateInterval;
			
			foreach (ResourceEntry res in resources)
			{
				// Reduce the stored or demanded goods
				res.warehouse *= mSpoilRate;
				
				// If the resource is being traded, adjust it
				if (res.beingTraded || res.production < 0)
				{
					res.warehouse += res.production * mUpdateFactor;
				}
			}
		}
	}
	
	/// <summary>
	/// Draw the user interface for the town.
	/// </summary>
	
	void DrawGUI()
	{
		float alpha = showInfo ? 1.0f : 0.0f;
		mAlpha = Mathf.Lerp(mAlpha, alpha, Time.deltaTime * (showInfo ? 4.0f : 2.0f));

		if (mAlpha > 0.001f)
		{
			// Block camera control
			if (showInfo) StrategicCamera.allowInput = false;
			
			UI.SetAlpha(mAlpha);
			{
				// Automatically choose the first available trade route
				if (mSelectedRoute == null || (mSelectedRoute.town0 != this && mSelectedRoute.town1 != this))
				{
					mSelectedRoute = null;
					
					foreach (TradeRoute tr in TradeRoute.list)
					{
						Town town = tr.GetConnectedTown(this);
	
						if (town != null)
						{
							mSelectedRoute = tr;
							break;
						}
					}
				}
				
				// Draw the town's UI
				DrawTownUI();
				
				// If we have a trade route we can draw the trade UI
				if (mSelectedRoute != null) DrawTradeUI();

				// Draw the ship-related UI
				DrawBuildShipsUI();
				DrawFreeShipsUI();
				
				// Cancel all dragging operations on mouse up
				if (Input.GetMouseButtonUp(0)) CancelDrag();
				
				// Draw the dragged resource
				DrawIconUI();
				
				// Draw the exit button
				if (GUI.Button(new Rect(Screen.width * 0.5f - 75.0f,
					Screen.height - 45.0f, 150.0f, 40.0f), "Return to Game",
					Config.Instance.skin.button))
				{
					showInfo = false;
				}
			}
			UI.RestoreAlpha();
		}
	}
	
	/// <summary>
	/// Draws the UI for the current town.
	/// </summary>
	
	void DrawTownUI()
	{
		Vector2 mousePos = UI.GetMousePos();
		Rect rect = new Rect(Screen.width * 0.5f + 130f, Screen.height * 0.5f - 260f, 292f, 270f);
		
		// Allow the opposite town's resources to be dragged straight to this window for simplicity's sake
		if (mDragResource != -1 && Input.GetMouseButtonUp(0) && rect.Contains(mousePos))
		{
			if (mDragTown == this) mDragTown = null;
			mSelectedRoute.SetExportedResource(mDragTown, mDragResource);
			CancelDrag();
		}
		
		// Draw the resources
		Rect inner = UI.DrawWindow(rect, name);
		DrawResources(this, inner);
		
		inner.y += inner.height - 40f;
		inner.height = 40f;

		if (GUI.Button(inner, "Establish a New Trade Route", Config.Instance.skin.button))
		{
			showInfo = false;
			TradeRouteCreator.Instance.StartNewTradeRoute(this);
		}

		if (mSelectedRoute != null)
		{
			rect = new Rect(rect.x, Screen.height * 0.5f + 40f, 292f, 225f);
			
			// Allow main town's resources to be dragged straight to this window for simplicity's sake
			if (mDragResource != -1 && Input.GetMouseButtonUp(0) && rect.Contains(mousePos))
			{
				if (mDragTown != this) mDragTown = null;
				mSelectedRoute.SetExportedResource(mDragTown, mDragResource);
				CancelDrag();
			}
			
			Town town = mSelectedRoute.GetConnectedTown(this);
			inner = UI.DrawWindow(rect, town.name);
			
			// See if we have more than one trade route
			TradeRoute next = TradeRoute.FindNext(mSelectedRoute, this, true);
			
			// Draw trade route navigation buttons
			if (next != mSelectedRoute)
			{
				if (GUI.Button(new Rect(rect.x + 20f, rect.y - 18f, 40f, 40f), "<<", Config.Instance.skin.button))
				{
					mSelectedRoute = next;
				}
				else if (GUI.Button(new Rect(rect.x + rect.width - 60f, rect.y - 18f, 40f, 40f), ">>", Config.Instance.skin.button))
				{
					mSelectedRoute = TradeRoute.FindNext(mSelectedRoute, this, false);
				}
			}
			DrawResources(town, inner);
		}
	}
	
	/// <summary>
	/// Draws the town's resources.
	/// </summary>
	
	void DrawResources (Town town, Rect rect)
	{
		Vector2 size = TownResources.Instance.GetIconSize();

		rect = UI.Bevel(rect, 8.0f);
		float offset = 60.0f;
		
		UI.DrawTitle(new Rect(rect.x, rect.y - 5.0f, rect.width, 40.0f), "Surplus", Config.Instance.infoStyle);
		rect.y += 30.0f;
		rect.height -= 30.0f;
		DrawResources(town, rect, size, true);

		rect.y += offset;
		rect.height -= offset;
		
		UI.DrawTitle(new Rect(rect.x, rect.y - 5.0f, rect.width, 40.0f), "Demand", Config.Instance.infoStyle);
		rect.y += 30.0f;
		rect.height -= 30.0f;
		DrawResources(town, rect, size, false);
	}
	
	/// <summary>
	/// Draws the town's resources.
	/// </summary>
	
	void DrawResources (Town town, Rect rect, Vector2 size, bool surplus)
	{
		// Draw the resource grid
		List<Vector2> grid = UI.DrawGrid(rect, size.x, size.y, 4f, 4f, 4f, 4);

		int index = 0;
		Town otherTown = (mSelectedRoute == null) ? null : mSelectedRoute.GetConnectedTown(town);
		
		foreach (Vector2 pos in grid)
		{
			if (index >= town.mResources.Count) break;
			
			// Run through all resources until we find the one we can draw
			for (; index < town.mResources.Count; ++index)
			{
				ResourceEntry ent = town.mResources[index];
				
				if ((surplus && ent.production > 0) || (!surplus && ent.production < 0))
				{
					float alpha = 1f;
					Rect iconRect = new Rect(pos.x, pos.y, size.x, size.y);

					// Positive resources that the city is producing can be dragged
					bool canDrag = (ent.production > 0);
					
					// Don't allow dragging of resources which the other town doesn't actually need
					if (mSelectedRoute == null)
					{
						canDrag = false;
					}
					else if (otherTown != null)
					{
						// If the other town doesn't need this resource, don't allow it to be dragged
						if (otherTown.resources[index].production >= 0)
						{
							canDrag = false;
						}
					}
					
					// If the resource cannot be dragged and it's in the surplus section, make it transparent
					if (!canDrag && surplus) alpha = 0.5f;
					
					// If we're currently dragging this resource, make it transparent
					if (mDragResource == index) alpha = 0.5f;
				
					// Allow dragging of positive production resources
					if (canDrag && mSelectedRoute != null && Input.GetMouseButtonDown(0) && UI.ContainsMouse(iconRect))
					{
						mDragResource = index;
						mDragTown = town;
					}
					
					TownResource tr = TownResources.Instance.Get(index);

					if (tr.icon != null)
					{
						Color prev = GUI.color;
						GUI.color = new Color(1f, 1f, 1f, prev.a * alpha);
						UI.DrawTexture(pos.x, pos.y, tr.icon);
						GUI.color = prev;
					}
					Tooltip.AddArea(iconRect, DrawTooltip, tr);
					
					// Stockpile amount caption (bottom-right)
					{
						int amount = Mathf.RoundToInt(ent.warehouse);
						Color color = Color.white;
						if (amount > 0) color = Color.green;
						else if (amount < 0) color = Color.red;
						UI.DrawLabel(iconRect, amount.ToString(), Config.Instance.infoStyle, color, Color.black, true);
					}
					
					// Production caption (top-left)
					{
						int amount = ent.production;
						Color color = Color.white;
						if (amount > 0) color = Color.green;
						else if (amount < 0) color = Color.red;
	
						TextAnchor anchor = Config.Instance.infoStyle.alignment;
						Config.Instance.infoStyle.alignment = TextAnchor.UpperLeft;
						UI.DrawLabel(iconRect, (amount < 0) ? amount.ToString() : ("+" + amount),
							Config.Instance.descStyle, color, new Color(0f, 0f, 0f, 0.5f), true);
						Config.Instance.infoStyle.alignment = anchor;
					}
					++index;
					break;
				}
			}
		}
	}
	
	/// <summary>
	/// Draws the traded resources.
	/// </summary>
	
	void DrawTradedResourcesUI (List<Vector2> grid, Town town)
	{
		int index = 0;

		foreach (TradeRoute.Item item in mSelectedRoute.items)
		{
			if (item.town == town)
			{
				Vector2 v = grid[index];
				TownResource tr = TownResources.Instance.list[ item.id ];
				if (tr.icon != null) UI.DrawTexture(v.x, v.y, tr.icon);
				Rect iconRect = new Rect(v.x, v.y, 50f, 50f);
				
				// It should be possible to drag these icons
				if (Input.GetMouseButtonDown(0) && iconRect.Contains(UI.GetMousePos()))
				{
					CancelDrag();
					mDragResource = item.id;
					mDragTown = item.town;
				}
				
				if (++index >= grid.Count) break;
			}
		}
	}
	
	/// <summary>
	/// Draw trade-related UI.
	/// </summary>
	
	void DrawTradeUI()
	{
		Vector2 mousePos = UI.GetMousePos();
		
		// Imports and exports for this town
		Town opposite = mSelectedRoute.GetConnectedTown(this);
		
		// Draw the trade window
		Rect rect = new Rect(Screen.width * 0.5f - 75f, Screen.height * 0.5f - 270f, 150f, 512f);
		
		// Allow resources to be dropped here
		if (mDragResource != -1 && Input.GetMouseButtonUp(0) && rect.Contains(mousePos))
		{
			mSelectedRoute.SetExportedResource(mDragTown, mDragResource);
			CancelDrag();
		}
		// Allow ships to be dropped here
		else if (mDragShip != null && Input.GetMouseButtonUp(0) && rect.Contains(mousePos))
		{
			mSelectedRoute.AssignShip(mDragShip);
			CancelDrag();
		}
		
		rect = UI.DrawPanel(rect);
		Rect titleRect = new Rect(rect.x, rect.y - 25.0f, rect.width, 40.0f);
		UI.DrawPanel(titleRect);
		UI.DrawTitle(titleRect, "Trade", Config.Instance.headerStyle);
		rect = UI.Bevel(rect, 8f);
		
		// Main town imports from the connected city
		{
			UI.DrawTitle(new Rect(rect.x, rect.y, rect.width, 30f), "Imports", Config.Instance.infoStyle);
			
			rect.y += 30f;
			rect.height -= 30f;
			
			// Draw the arrow
			UI.DrawTexture(rect.x + rect.width - 70f + Mathf.Sin(Time.time * 5f) * 10f,
				rect.y + rect.width * 0.5f - Icons.Instance.arrowRight.height * 0.5f, Icons.Instance.arrowRight);
			
			// Draw the grid for icons
			bool tint = (mDragResource != -1 && mDragTown != null && mDragTown != this);
			Color prev = GUI.color;
			GUI.color = tint ? new Color(0f, 1f, 0f, prev.a) : prev;
			List<Vector2> grid = UI.DrawGrid(rect, 50f, 50f, 4f, 4f, 4f, 4);
			GUI.color = prev;
			
			// Draw the imported resources
			DrawTradedResourcesUI(grid, opposite);
			
			rect.y += rect.width;
			rect.height -= rect.width;
		}
		
		// Assigned ships
		{
			UI.DrawTitle(new Rect(rect.x, rect.y, rect.width, 30f), "Ship Capacity", Config.Instance.infoStyle);
			
			rect.y += 30f;
			rect.height -= 30f;
			
			// Draw the grid, coloring it green if we're dragging an assignable ship
			Color prev = GUI.color;
			GUI.color = (mDragShip == null) ? prev : new Color(0f, 1f, 0f, prev.a);
			List<Vector2> grid = UI.DrawGrid(rect, 50f, 50f, 4f, 4f, 4f, 4);
			GUI.color = prev;
			
			rect.y += rect.width;
			rect.height -= rect.width;

			int count = 0;

			// Draw all assigned ships' icons
			foreach (AvailableShips.Owned ship in AvailableShips.Instance.list)
			{
				if (ship.tradeRoute == mSelectedRoute)
				{
					Vector2 pos = grid[count++];
					
					Rect shipRect = new Rect(pos.x, pos.y, 50f, 50f);
			
					if (ship.prefab.icon != null)
					{
						UI.DrawTexture(pos.x, pos.y, ship.prefab.icon);
					}
					
					UI.DrawLabel(shipRect, ship.prefab.cargo.ToString(), Config.Instance.infoStyle,
						Color.white, Color.black, true);
					
					// Allow dragging of this icon
					if (Input.GetMouseButtonDown(0) && shipRect.Contains(mousePos))
					{
						CancelDrag();
						mDragShip = ship;
					}
				}
			}
		}
		
		// Main city exports to the connected city
		{
			UI.DrawTitle(new Rect(rect.x, rect.y, rect.width, 30f), "Exports", Config.Instance.infoStyle);
			
			rect.y += 30f;
			rect.height -= 30f;
			
			// Draw the arrow
			UI.DrawTexture(rect.x + rect.width - 70f + Mathf.Sin(Time.time * 5f) * 10f,
				rect.y + rect.width * 0.5f - Icons.Instance.arrowRight.height * 0.5f, Icons.Instance.arrowRight);
			
			// Draw the grid for icons
			bool tint = (mDragResource != -1 && mDragTown == this);
			Color prev = GUI.color;
			GUI.color = tint ? new Color(0f, 1f, 0f, prev.a) : prev;
			List<Vector2> grid = UI.DrawGrid(rect, 50f, 50f, 4f, 4f, 4f, 4);
			GUI.color = prev;
			
			// Draw the imported resources
			DrawTradedResourcesUI(grid, this);
		}
	}
	
	/// <summary>
	/// Draw the ship-building UI.
	/// </summary>
	
	void DrawBuildShipsUI()
	{
		Vector2 mousePos = UI.GetMousePos();
		Rect rect = new Rect(Screen.width * 0.5f - 425f, Screen.height * 0.5f - 210f, 292f, 166f);
		rect = UI.DrawWindow(rect, "Build Ships");
		
		// Allow ships to be sold by dragging them here
		if (mDragShip != null && Input.GetMouseButtonUp(0) && rect.Contains(mousePos))
		{
			if (mSelectedRoute != null) mSelectedRoute.UnassignShip(mDragShip);
			AvailableShips.Instance.list.Remove(mDragShip);
			Config.Instance.gold += mDragShip.prefab.price;
			CancelDrag();
		}
		
		// Draw the grid, colored green if we're dragging a sellable ship
		rect = UI.Bevel(rect, 8f);
		Color prev = GUI.color;
		GUI.color = mDragShip == null ? prev : new Color(0f, 1f, 0f, prev.a);
		List<Vector2> grid = UI.DrawGrid(rect, 50f, 50f, 4f, 4f, 4f, 8);
		GUI.color = prev;
		
		int max = Mathf.Min(grid.Count, AvailableShips.Instance.prefabs.Count);
		
		// Draw all ships that we can currently build
		for (int i = 0; i < max; ++i)
		{
			Vector3 pos = grid[i];
			AvailableShips.Template ship = AvailableShips.Instance.prefabs[i];
			Rect shipRect = new Rect(pos.x, pos.y, 50f, 50f);
			
			if (ship.icon != null)
			{
				UI.DrawTexture(pos.x, pos.y, ship.icon);
			}
			
			// Price caption
			UI.DrawLabel(shipRect, "$" + ship.price, Config.Instance.descStyle,
				Config.Instance.gold < ship.price ? Color.red : Color.white, Color.black, true);
			
			// Cargo caption
			UI.DrawLabel(shipRect, ship.cargo.ToString(), Config.Instance.infoStyle,
				Color.white, Color.black, true);
			
			// Allow dragging of this icon
			if (Config.Instance.gold >= ship.price &&
				Input.GetMouseButtonDown(0) &&
				shipRect.Contains(mousePos))
			{
				CancelDrag();
				mDragPrefab = ship;
			}
			
			// TODO: Ship tooltips
			//Tooltip.AddArea(shipRect, OnShipTooltip, ship);
		}
	}
	
	/// <summary>
	/// Draw the available ships window.
	/// </summary>
	
	void DrawFreeShipsUI()
	{
		Vector2 mousePos = UI.GetMousePos();
		Rect rect = new Rect(Screen.width * 0.5f - 425f, Screen.height * 0.5f + 20f, 292f, 166f);
		rect = UI.DrawWindow(rect, "Owned Ships");
		rect = UI.Bevel(rect, 8f);
		
		// Draw the grid, coloring it green if we're dragging something useful
		Color prev = GUI.color;
		GUI.color = (mDragShip == null && mDragPrefab == null) ? prev : new Color(0f, 1f, 0f, prev.a);
		List<Vector2> grid = UI.DrawGrid(rect, 50f, 50f, 4f, 4f, 4f, 8);
		GUI.color = prev;
		
		// Drop functionality for purchased ships
		if (Input.GetMouseButtonUp(0) && rect.Contains(mousePos))
		{
			if (mDragPrefab != null)
			{
				if (Config.Instance.gold >= mDragPrefab.price)
				{
					Config.Instance.gold -= mDragPrefab.price;
					
					// Add a new ship to the list of owned ships
					AvailableShips.Owned ship = new AvailableShips.Owned();
					ship.prefab = mDragPrefab;
					AvailableShips.Instance.list.Add(ship);
				}
			}
			else if (mDragShip != null)
			{
				// Clear the ship's trade route
				mSelectedRoute.UnassignShip(mDragShip);
			}
			CancelDrag();
		}
		
		int count = 0;
		
		// Run through all owned ships and display the free ones
		foreach (AvailableShips.Owned ship in AvailableShips.Instance.list)
		{
			if (ship.tradeRoute == null)
			{
				if (count >= grid.Count) break;
				
				Vector3 pos = grid[count++];
				Rect shipRect = new Rect(pos.x, pos.y, 50f, 50f);
				
				if (ship.prefab.icon != null)
				{
					UI.DrawTexture(pos.x, pos.y, ship.prefab.icon);
				}
				
				// Allow dragging of this icon
				if (Input.GetMouseButtonDown(0) && shipRect.Contains(mousePos))
				{
					CancelDrag();
					mDragShip = ship;
				}
			}
		}
	}
	
	/// <summary>
	/// Draws the dragged resource icon.
	/// </summary>
	
	void DrawIconUI()
	{
		if (mDragResource != -1)
		{
			TownResource tr = TownResources.Instance.Get(mDragResource);

			if (tr.icon != null)
			{
				Vector2 mouse = UI.GetMousePos();
				UI.DrawTexture(mouse.x - tr.icon.width * 0.5f,
					mouse.y - tr.icon.height * 0.5f, tr.icon);
			}
		}
		else
		{
			Texture2D icon = null;
			if (mDragPrefab != null) icon = mDragPrefab.icon;
			else if (mDragShip != null) icon = mDragShip.prefab.icon;
			
			if (icon != null)
			{
				// Draw the dragged icon
				Vector2 mouse = UI.GetMousePos();
				UI.DrawTexture(mouse.x - icon.width * 0.5f,
					mouse.y - icon.height * 0.5f, icon);
			}
		}
	}
	
	/// <summary>
	/// Tooltip callback function.
	/// </summary>
	
	void DrawTooltip (Vector2 pos, object param)	
	{
		TownResource tr = param as TownResource;
		
		if (tr != null)
		{
			Rect rect = new Rect(pos.x - 100.0f, pos.y + 10.0f, 200.0f, 150.0f);
			rect = UI.DrawPanel(rect);
	
			GUILayout.BeginArea(rect);
			UI.DrawTitle(tr.name, Config.Instance.headerStyle);
			GUILayout.Label(tr.description, Config.Instance.skin.label);
			GUILayout.EndArea();
		}
	}
}