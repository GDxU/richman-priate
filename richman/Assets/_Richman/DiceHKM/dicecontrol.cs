using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dicecontrol : MonoBehaviour
{
		public  GameObject dicePrefab;
		private gameEngine ge;
		private bool operation_ongoing = false;
		private List<dice> dicelist = new List<dice> ();
		public int total_dices;
		private Dictionary<int,int> result = new Dictionary<int,int> ();
		public static Vector3[] faces = new Vector3[] {
				Vector3.up,
				-Vector3.up,
				Vector3.right,
				-Vector3.right,
				Vector3.forward,
				-Vector3.forward
		};
		public static  int[] facePrepresetation = new int[6]{
			3,6,2,5,1,4
		};

		// Use this for initialization
		void Start ()
		{
				ge = GetComponent<gameEngine> ();
		}


		// Update is called once per frame
		void Update ()
		{
	
				if (dicelist.Count == 0 || !operation_ongoing)
						return;
				if (result.Count != total_dices) {
						try {
							
								foreach (dice c in dicelist) {
										int hash = c.watch ();
										if (hash != -1) {
												result [hash] = c.result_number;
												//Debug.Log ("instance: " + hash + " result: " + c.result_number);
										}
								}
								
						} catch (UnityException e) {
								Debug.Log ("error from Update " + e + " result");
						}
				} else {
						operation_ongoing = false;
						int total_dice_result = 0;
						foreach (KeyValuePair<int, int> entry in result) {
								total_dice_result += entry.Value;
						}
						Debug.Log ("Update goes up now with total: " + total_dice_result);
						ge.CurrentPlayer ().move (total_dice_result, PlayerData.direction.FORWARD);
						StartCoroutine (removeaction ());
					
				}
		}

		private IEnumerator removeaction ()
		{
				yield return new WaitForSeconds (3.4f);
				foreach (dice d in dicelist) {
						d.remove ();
				}
				result.Clear ();
				dicelist.Clear ();
				
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
				result.Clear ();
				Vector3 startfrom = ge.CurrentPlayer ().character_stage.transform.position;
				for (int i = 0; i<total_dices; i++) {
						dice d = new dice ("dice_" + i, dicePrefab, this);
						d.throwup (startfrom);
						dicelist.Add (d);
				}
			
		}
		
		
}
