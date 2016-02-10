using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class diceCon : MonoBehaviour
{
		private List<int> dicelist = new List<int> ();
		private List<int> result = new List<int> ();
		//  private gameEngine ge = gameEngine.Instance;
		public  GameObject dicePrefab;
		private  GameObject  watched;
		public bool resultDebug = false;
		public int total_dices = 1, sum_results = 0, debugDice = 1;
		private bool operation_ongoing = false;
		public float waiting = 3.4f;
		private animationaudio E;
		// Use this for initialization
		void Start ()
		{

		}

		public void addDiceResult (int hash, int res)
		{
				Debug.Log ("add new-" + hash);
				if (dicelist.Contains (hash)) {
                    
						result.Add (res);
						sum_results += res;
						if (dicelist.Count == result.Count) {
								StartCoroutine (dice_display_flow ());
						}
				}
                
		}

		private IEnumerator dice_display_flow ()
		{
				GameObject[] plist = GameObject.FindGameObjectsWithTag ("dice");
				//gameEngine.Instance.getCam ().ToFocusDice (plist [0].transform);
				yield return new WaitForSeconds (waiting);
				//gameEngine.Instance.getCam ().SnapFocus (gameEngine.Instance.CurrentPlayer ().character_stage.transform);
				foreach (GameObject d in plist) {
						GameObject.Destroy (d, Random.Range (0.3f, 3.1f));
				}
				operation_ongoing = false;
				dicelist.Clear ();
				result.Clear ();
				if (resultDebug && debugDice <= 0)
						Debug.Log ("debug dice cannot be zero!");
				gameEngine.Instance.CurrentPlayer ().move (resultDebug ? debugDice : sum_results, PlayerData.direction.FORWARD);
				sum_results = 0;
		}

		private IEnumerator rezDice ()
		{
				Vector3 startfrom = gameEngine.Instance.CurrentPlayer ().character_stage.transform.position;
				for (int i = 0; i<total_dices; i++) {
						string gameoname = "d" + i;
                
						watched = GameObject.Find (gameoname);
						if (watched == null) {
								watched = GameObject.Instantiate (dicePrefab, startfrom, Quaternion.identity)  as GameObject;
								watched.name = gameoname;
								watched.tag = "dice";
						} 
						Vector3 orbitpos = startfrom + Quaternion.Euler (0, Random.Range (0, 360f), 0) * Vector3.forward * 1.1f;
						watched.transform.position = orbitpos;
                
						E = watched.GetComponentInChildren<animationaudio> ();
						yield return new WaitForFixedUpdate ();
						if (E != null) {
								E.throwDice ();
								dicelist.Add (watched.GetHashCode ());
						}
				}
		}

		public void throw_dice ()
		{
				if (!operation_ongoing)
						operation_ongoing = true;
				else
						return;
				if (total_dices < 1) {
						Debug.LogError ("dice number has to be more than 0");
						return;
				}
				StartCoroutine (rezDice ());
		}
}
