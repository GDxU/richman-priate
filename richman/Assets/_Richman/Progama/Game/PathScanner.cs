using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathScanner : MonoBehaviour
{
		public bool debug = false;
		public GameObject allproperties, spotlight, buildingDust, rezIsland;
		public string name_of_the_property_holder = "propertyholder";
		public List<Property> bigList = new List<Property> ();
		public LayerMask ground_only_mask;
		public float[] fix_price_inflation_factor;
		public float[] revenue_factor;
		public float buildingDustScale = 0.5f;
		public List<BuildingDescripter> building_desc;
		public List<Station> mstation;
		private FollowTrackingCamera component_camera_tracker;

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
				GameObject ro = GameObject.Find (name_of_the_property_holder);
				if (ro == null) {
						ro = GameObject.Instantiate (allproperties, Vector3.zero, Quaternion.identity) as GameObject;
						ro.name = name_of_the_property_holder;
				}
				component_camera_tracker = GetComponent<FollowTrackingCamera> ();
				int endof = 50, station_c = 0;
				bigList.Clear ();
				for (int i=0; i<endof; i++) {
						string objectname = name_of_the_property_holder + "/" + getnum (i);
						//Debug.Log ("objectname to show:" + objectname);
						GameObject holder = GameObject.Find (objectname);
						if (holder != null) {
								//Debug.Log ("objectname found:" + objectname);
								Property pr = new Property ();
								pr.event_pass = Property.passing_event.NOTHING;
								pr.name = "P:" + holder.name;
								pr.id = i;
								pr.fix_price_inflation_factor = Random.Range (1.0f, 2.0f);
								pr.revenue_factor = Random.Range (0.9f, 1.5f);
								//setup and create the default location place;
								pr.setHolder (debug, holder, getGroundPosition (holder.transform.position), rezIsland);
								Station install_station = mstation.Find (x => x.position_step == station_c);
								if (install_station != null) {
										pr.event_pass = install_station.event_pass;
										pr.blocktype = install_station.blocktype;
										pr.name = "Special:" + install_station.name;
										pr.id = install_station.id;
										pr.owned_by_id = -2;
										pr.fix_price_inflation_factor = -1;
										pr.revenue_factor = -1;
										pr.setSpecialBuilding (install_station, Vector3.zero);
								}
								bigList.Add (pr);
								station_c++;
								yield return new WaitForEndOfFrame ();
						}
				}
		}

		public void scan ()
		{
				StartCoroutine (doscan ());
		}

		public void rez_init_player (RichChar  per)
		{
 
				try {
						if (per == null)
								throw new System.ArgumentNullException ("per");
						//RichChar per = a as RichChar;
						if (GameObject.Find (per.playername) == null) {
								if (per.stand_at_prop_id == 0)
										throw new UnityException ("the object current step is not set! please refer to the character setup on :" + per.playername);
								Property loc = getPropertyById (per.stand_at_prop_id);
								if (loc == null)
										throw new UnityException ("cannot find the location by the property ID please go check it. Maybe this can help: set each player's stand_at_prop_id at 1.");
								Vector3 pos = loc.getStepLocation ();
								if (per.prefab == null)
										throw new UnityException ("the object prefab is not set! please refer to the character setup on :" + per.playername);
								per.currentstep = proertyIDtoIndex (per.stand_at_prop_id);
								GameObject fcharacter_now = (GameObject)Instantiate (per.prefab) as GameObject;
								fcharacter_now.name = per.playername;
								per.position = pos;
								fcharacter_now.transform.position = pos;
								per.character_stage = fcharacter_now;
								per.setGroundMask (ground_only_mask);
						}
				} catch (UnityException e) {
						Debug.LogError (e.ToString ());
				}
		}

		private float dist = 10.1f;
		public float LandAboveFromGround = 0.1f;

		private Vector3 getGroundPosition (Vector3 e)
		{

				RaycastHit hit;
				Vector3 dir = -Vector3.up;
				Vector3 from = new Vector3 (e.x, 10f, e.z);
				Debug.DrawRay (from, dir * dist, Color.yellow);
        
				Vector3 h = new Vector3 ();
				if (Physics.Raycast (from, dir, out hit, dist, ground_only_mask)) {
						h = hit.collider.gameObject.transform.position;
				}
				return new Vector3 (e.x, h.y + LandAboveFromGround, e.z);
		}

		// learn from http://msdn.microsoft.com/en-us/library/x0b5b5bc%28v=vs.110%29.aspx
		public Property getPropertyById (int n_id)
		{
				return bigList.Find (r => r.id == n_id);
		}

		public int proertyIDtoIndex (int n_id)
		{
				Property epro = bigList.Find (r => r.id == n_id);
				return bigList.IndexOf (epro);
		}

		public Property getPropertyByIndex (int n)
		{
				return bigList [n];
		}

		public bool isCurrentStepTheLastIndex (int linear_step_id)
		{
				Property p = bigList [linear_step_id];
				return bigList [bigList.Count - 1].id == p.id;
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

		private GameObject findHouseModelBylocType (Property.locType kConsant)
		{
				BuildingDescripter p = building_desc.Find (r => r.typebuilding == kConsant);
				return p.givePreFab ();
		}

		public IEnumerable<Property> getPropertyListByOwner (RichChar person)
		{
				return	bigList.Where (building => building.owned_by_id == person.owner_id);
		}
}
