using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Strategy/Trade Route Creator")]
public class TradeRouteCreator : MonoBehaviour
{
	[HideInInspector]
	public static TradeRouteCreator Instance = null;
	
	private GameObject 		mGO 		= null;
	private TradeRoute 		mRoute 		= null;
	private bool 			mIsValid 	= false;
	private Vector3 		mTargetPos;
	private Town 			mTargetTown = null;
	private ArrowProjector 	mProj 		= null;
	private string			mTooltip;
	
	public Texture2D pathTexture = null;
	
	/// <summary>
	/// Gets a value indicating whether the <see cref="TradeRouteCreator"/> is active.
	/// </summary>
	
	public bool isActive { get { return mGO != null; } }
	
	void OnEnable()
	{
		Instance = this;
		mProj = gameObject.GetComponent<ArrowProjector>();
		if (mProj == null) mProj = gameObject.AddComponent<ArrowProjector>();
	}
	
	void OnDisable()
	{
		if (Instance == this) Instance = null;
	}
	
	void Start()
	{
		Config.Instance.onGUI.Add(DrawGUI);
	}
	
	void DrawGUI()
	{
		if (StrategicCamera.viewpoint == null)
		{
			if (isActive)
			{
				if (mRoute.town0 == null)
				{
					UI.DrawTitle(new Rect(0f, 0f, Screen.width, 35f),
						"Start by selecting the starting town.", Config.Instance.infoStyle);
				}
				else if (mRoute.town1 == null)
				{
					UI.DrawTitle(new Rect(0f, 0f, Screen.width, 35f),
						"Draw a path by left-clicking. Right-click to undo.", Config.Instance.infoStyle);
				}
				
				if (mProj != null && !string.IsNullOrEmpty(mTooltip))
				{
					Vector2 pos = UI.GetScreenPos(mProj.transform.position);
					UI.DrawTitle(new Rect(pos.x - 100f, pos.y - 17f, 200f, 35f),
						mTooltip, Config.Instance.infoStyle);
				}
			}
		}
	}
	
	/// <summary>
	/// Starts a new trade route.
	/// </summary>
	
	public TradeRoute StartNewTradeRoute (Town town)
	{
		if (mGO == null)
		{
			mGO = new GameObject("Trade Route Creator");
			mRoute = mGO.AddComponent<TradeRoute>();
			mRoute.texture = pathTexture;
		}
		
		if (town != null)
		{
			mRoute.Connect(town);
		}
		return mRoute;
	}
	
	/// <summary>
	/// Gets the town under the mouse cursor.
	/// </summary>
	
	Town GetTownUnderMouse()
	{
		RaycastHit hitInfo;
	
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 300.0f))
		{
			return Town.Find(hitInfo.collider.gameObject);
		}
		return null;
	}
	
	/// <summary>
	/// Gets the water surface target position under the mouse.
	/// </summary>
	
	Vector3 GetMouseTargetPos (Town town)
	{
		if (town == null) return Game.GetMouseWaterPosition();
		Transform trans = town.transform.FindChild("Anchor");
		return (trans != null) ? trans.position : town.transform.position;
	}
	
	/// <summary>
	/// Determines whether the current route placement is valid.
	/// </summary>
	
	bool IsPlacementValid (Vector3 target)
	{
		mTooltip = string.Empty;
					
		if (mRoute != null && mRoute.path.list.Count > 0)
		{
			Vector3 start = mRoute.path.list[mRoute.path.list.Count - 1].mVal;

			RaycastHit hit;
			Vector3 dir = target - start;
			float dist = dir.magnitude;
			
			if (dist < 5.0f)
			{
				mTooltip = "Too short!";
				return false;
			}
			dir *= 1.0f / dist;
			
			// Allow only up to 90 degree turns
			if (mRoute.path.length > 1)
			{
				Vector3 prev = mRoute.path.list[mRoute.path.list.Count - 2].mVal;
				Vector3 prevDir = (start - prev).normalized;

				if (Vector3.Dot(dir, prevDir) < 0f)
				{
					mTooltip = "Tight turn!";
					return false;
				}
			}
			
			// Collide only with the terrain
			if (Physics.SphereCast(start, 1f, dir, out hit, dist, 1 << 10))
			{
				mTooltip = "Too shallow!";
				return false;
			}
			
			// Display some useful information
			mTooltip = Mathf.RoundToInt(dist) + " miles";
		}
		return true;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	
	void Update()
	{
		// The camera should not respond to clicks while we are creating a trade route -- let us handle that
		if (isActive)
		{
			StrategicCamera.onClick = OnClick;
		
			// Update the target town and position
			mTargetTown = GetTownUnderMouse();
			mTargetPos = GetMouseTargetPos(mTargetTown);

			// Validate the trade route segment
			mIsValid = IsPlacementValid(mTargetPos);
		}	
		
		// Reposition the projector
		if (mProj != null)
		{
			if (isActive && mRoute != null && mRoute.path.list.Count > 0)
			{
				mProj.enabled = true;
				mProj.originPos = mRoute.path.list[mRoute.path.list.Count - 1].mVal;
				mProj.targetPos = mTargetPos;
				mProj.color = mIsValid ? Color.white : Color.red;
			}
			else mProj.enabled = false;
		}
	}
	
	/// <summary>
	/// Callback triggered when the player clicks on something.
	/// </summary>
	
	void OnClick (int button)
	{
		if (button == 0)
		{
			if (mIsValid)
			{
				if (mTargetTown != null)
				{
					if (mRoute.Connect(mTargetTown))
					{
						// See if there is a duplicate route
						foreach (TradeRoute tr in TradeRoute.list)
						{
							if (tr != mRoute && tr.GetConnectedTown(mRoute.town0) == mRoute.town1)
							{
								// Copy the trade path
								tr.CopyPath(mRoute.path);
								
								// Ensure that no towns are still referencing this trade route
								foreach (Town t in Town.list) t.DisconnectTradeRoute(tr);
								Object.Destroy(mGO);
								break;
							}
						}
						
						// This route has now been created
						mIsValid = false;
						mTargetTown = null;
						mRoute = null;
						mGO = null;
					}
				}
				else
				{
					mRoute.Add(mTargetPos);
				}
			}
		}
		else if (!mRoute.UndoAdd())
		{
			foreach (Town t in Town.list) t.DisconnectTradeRoute(mRoute);
			Object.Destroy(mGO);
			mIsValid = false;
			mTargetTown = null;
			mRoute = null;
			mGO = null;
		}
	}
}