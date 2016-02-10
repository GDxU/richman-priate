using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SplineCity : MonoBehaviour
{
		public bool debug = false, useExistingTagPlayer = false;
		public GameObject buildingDust, rezIsland;
		[SerializeField]
		private GameObject
				loadSplinePrefab;
		[SerializeField]
		private int
				totalnodes;
		private Spline loadSpline;
		public Vector2 fixPriceInflationRange, revenueRange;
		public float buildingDustScale = 0.5f, seaLevel = 0.01f, building_offset = 0.1f, land_distance_from_node = 2f;
		public List<Property>
				bigList = new List<Property> ();
		public List<BuildingDescripter>
				building_desc;
		public List<Station>
				mstation;
		// Use this for initialization
		void Start ()
		{
				scan ();
		}

		public Spline getSplineLoaded ()
		{
				return loadSpline;
		}

		private string getnum (int n)
		{
				if (n < 10) {
						return "0" + n;
				} else {
						return n + "";
				}
		}
	
		private IEnumerator doscan ()
		{
				GameObject loadSplineb = Instantiate (loadSplinePrefab) as GameObject;
				GameObject holder = gameObject;
				int i = 0, station_id = 0;
				loadSpline = loadSplineb.GetComponent<Spline> () as Spline; 
				loadSplineb.name = "game trail preference";
				totalnodes = loadSpline.SplineNodes.Length;
				//foreach (SplineNode L in loadSpline.SplineNodes) {
				//L.Position;
				//}
//		GameObject ro = GameObject.Find (name_of_the_property_holder);
//				if (ro == null) {
//						ro = GameObject.Instantiate (allproperties, Vector3.zero, Quaternion.identity) as GameObject;
//						ro.name = name_of_the_property_holder;
//				}
				//int endof = 50, station_c = 0;
				bigList.Clear ();
				//for (int i=0; i<totalnodes; i++) {
				foreach (SplineNode L in loadSpline.SplineNodes) {
						//string objectname = name_of_the_property_holder + "/" + getnum (i);
						//Debug.Log ("objectname to show:" + objectname);
						//GameObject holder = GameObject.Find (objectname);
						//if (holder != null) {
						//Debug.Log ("objectname found:" + objectname);
						Property pr = new Property ();
						pr.event_pass = Property.passing_event.NOTHING;
						pr.name = "P:" + getnum (i);
						pr.id = i;
						pr.fix_price_inflation_factor = Random.Range (fixPriceInflationRange.x, fixPriceInflationRange.y);
						pr.revenue_factor = Random.Range (revenueRange.x, revenueRange.y);
						//setup and create the default location place;
						GameObject hholder = new GameObject ();
						hholder.transform.position = L.Position;
						hholder.transform.parent = holder.transform;
						pr.setHolderSpline (debug, hholder, rezIsland, splineSidelocation (i, L.Position));
						Station install_station = mstation.Find (x => x.position_step == station_id);
						if (install_station != null) {
								pr.event_pass = install_station.event_pass;
								pr.blocktype = install_station.blocktype;
								pr.name = "Special:" + install_station.name;
								pr.id = install_station.id;
								pr.owned_by_id = -2;
								pr.fix_price_inflation_factor = -1;
								pr.revenue_factor = -1;
								pr.setSpecialBuilding (install_station, Vector3.up * building_offset);
						}
						hholder.name = pr.name;
						bigList.Add (pr);
						station_id++;
						i++;
						yield return new WaitForEndOfFrame ();
						//}
				}

				//players initalization for demo
				if (useExistingTagPlayer) {
						Debug.Log ("useExistingTagPlayer");
						foreach (GameObject snapob in GameObject.FindGameObjectsWithTag("Player")) {
								snapob.SetActive (true);
								RichmanAnimator h = snapob.GetComponent<RichmanAnimator> ();
								h.enabled = true;
								h.init (loadSpline);
								h.setDemo (true);
								//Debug.Log ("enabled");
								yield return new WaitForSeconds (0.5f);
						}
						yield return new WaitForSeconds (0.5f);
//						if (demoCC.Instance != null) {
//							demoCC.Instance.initss ();
//						}
				}
		}

		private Vector3 splineSidelocation (int i, Vector3 p)
		{
				float t = (float)i / (float)totalnodes;
				//footStepPos = groundPos + Quaternion.Euler (0, Y, 0) * Vector3.forward * 1.5f;
				//p.TransformedNormal * asond
				//p.Parameters
				Quaternion pu = loadSpline.GetOrientationOnSpline (t);
				Vector3 lop1 = p + pu * Vector3.right * land_distance_from_node;
				Vector3 lop = new Vector3 (lop1.x, p.y + seaLevel, lop1.z);
				return lop;
		}

		public Property getbyIdProp (int pID)
		{
				return bigList [pID];
		}

		public void scan ()
		{
				StartCoroutine (doscan ());
		}
		// Update is called once per frame
		void Update ()
		{
	
		}

		public void RezBuilding (Property prop, Property.locType t, float estTime, System.Action continue_action)
		{
		
				Vector3 pso = prop.getHolderPropertyTargetPosition ();
				Vector3 from = new Vector3 (pso.x, pso.y - 3.0f, pso.z);
				GameObject dust_particles = Instantiate (buildingDust, pso, Quaternion.Euler (new Vector3 (90f, 0f, 0f))) as GameObject;
				dust_particles.SetActive (true);
				dust_particles.transform.localScale = new Vector3 (buildingDustScale, buildingDustScale, buildingDustScale);
				Transform land = prop.getHolderPropertyTargetTransform ();
				BuildingDescripter p = building_desc.Find (r => r.typebuilding == t);
				GameObject build_block = Instantiate (p.givePreFab (), from, Quaternion.identity) as GameObject;
				//GameObject lightpost = Instantiate (spotlight, pso, Quaternion.identity) as GameObject;
				build_block.name = p.building_name;
				//lightpost.transform.parent = build_block.transform;
				build_block.transform.parent = land;
				//lightpost.transform.parent = build_block.tranform;
				prop.blocktype = p.typebuilding;
				//lightpost.transform.LookAt (build_block.transform);
				prop.updateNewProperty (build_block, estTime, continue_action);
		}

		//private RichChar search_player;

		public List<Property> getPropertyListByOwner (RichChar w)
		{
				//Debug.Log (w.owner_id);
				return	bigList.FindAll (x => x.owned_by_id == w.owner_id);
		}

}
