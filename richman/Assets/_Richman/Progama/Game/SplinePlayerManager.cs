using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SplineCity))]
public class SplinePlayerManager : MonoBehaviour
{
		public LayerMask groundonlymask;
		public static SplinePlayerManager Instance;
		// Use this for initialization
		[SerializeField]
		private float
				_nodeStationAllowDistance = .005f;

		public float nodeStationAllowDistance {
				set {
						this._nodeStationAllowDistance = value;
				}
		}

		void Start ()
		{
				Instance = this;
				//StartCoroutine (setup ());
		}

		public void rez_init_player (RichChar  per, int stepID)
		{
		
				try {
						if (per == null)
								throw new System.ArgumentNullException ("per");
						//RichChar per = a as RichChar;
						if (GameObject.Find (per.playername) == null) {
								//if (per.stand_at_prop_id == 0)
								//		throw new UnityException ("the object current step is not set! please refer to the character setup on :" + per.playername);
								//Property loc = getPropertyById (per.stand_at_prop_id);
								///if (loc == null)
								///		throw new UnityException ("cannot find the location by the property ID please go check it. Maybe this can help: set each player's stand_at_prop_id at 1.");
								//Vector3 pos = loc.getStepLocation ();
								if (per.prefab == null)
										throw new UnityException ("the object prefab is not set! please refer to the character setup on :" + per.playername);
								//per.currentstep = proertyIDtoIndex (per.stand_at_prop_id);
								GameObject MACHINE = (GameObject)Instantiate (per.prefab) as GameObject;
								MACHINE.name = per.playername;
								CharacterControllerLogic control = MACHINE.GetComponentInChildren<CharacterControllerLogic> () as CharacterControllerLogic;
								control.FansCam = Camera.main.gameObject.GetComponent<FantasticCamera> () as FantasticCamera;
								//per.position = pos;
								//fcharacter_now.transform.position = pos;
								per.character_stage = MACHINE;
								per.setGroundMask (groundonlymask);
						}
				} catch (UnityException e) {
						Debug.LogError (e.ToString ());
				}
		}

		public void initAll (RichChar camfocus)
		{
				foreach (GameObject snapob in GameObject.FindGameObjectsWithTag("Player")) {
						//snapob.SetActive (true);
						RichmanAnimator h = snapob.GetComponent<RichmanAnimator> ();
						h.enabled = true;
						h.stop_scope_allowance = _nodeStationAllowDistance;
						h.init (GetComponent<SplineCity> ().getSplineLoaded ());
				}
				//focus to this player
				CharacterControllerLogic u = camfocus.character_stage.GetComponentInChildren<CharacterControllerLogic> () as CharacterControllerLogic;
				u.triggerFocusMain ();
		}
}
