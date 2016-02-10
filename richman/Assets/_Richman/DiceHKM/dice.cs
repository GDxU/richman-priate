using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class dice
{
		public dice (string e, GameObject dicePrefab, dicecontrol controller)
		{
				gameoname = e;
				this.dicePrefab = dicePrefab;
				this.control = controller;
				ID = GetHashCode ();
		}

		private int ID;
		private float angluar_push_base = 40f;
		private dicecontrol control;
		private GameObject watched, dicePrefab;
		private string gameoname;

		public int watch ()
		{
				try {
						bool fact1 = Mathf.Abs (watched.rigidbody.angularVelocity.magnitude) < 0.001f;
						bool fact2 = Mathf.Abs (watched.rigidbody.velocity.magnitude) < 0.001f;
						if (fact1 && fact2) {
								List<float> d = new List<float> ();
								foreach (Vector3 f in dicecontrol.faces) {
										float a = Vector3.Angle (watched.transform.up, f);
										d.Add (a);
								}
								float dk = d.Min ();
								int index = d.IndexOf (dk);
								result_number = dicecontrol.facePrepresetation [index];
								d.Clear ();
								//	Debug.Log ("get the min at " + index + " result face: " + result_number);
								//control.report_result (getDiceResult, this);
								return ID;
						}

				} catch (UnityException e) {
						Debug.Log ("error from e " + e + " result");
				}
				return -1;
		}

		public int result_number;

		public void throwup (Vector3 startfrom)
		{
				float Y = Random.Range (0f, 340f);
				Vector3 offset_y = new Vector3 (0, 1f, 0);
				Vector3 p = offset_y + startfrom + Quaternion.Euler (0, Y, 0) * Vector3.forward * 0.5f + Vector3.up * 0.5f;
				try {
						watched = GameObject.Find (gameoname);
						if (watched == null) {
								watched = GameObject.Instantiate (dicePrefab, p, Quaternion.identity)  as GameObject;
								watched.name = gameoname;
								watched.tag = "dice";
						} 
						watched.transform.position = p;


						watched.rigidbody.AddForce (Vector3.up * (Random.Range (1, 3) * 5 + 20), ForceMode.Impulse);
						watched.rigidbody.angularVelocity = new Vector3 (angluar_push_base + Random.Range (0.2f, 1f) * 180f, angluar_push_base + Random.Range (0.2f, 1f) * 190f, angluar_push_base + Random.Range (0.2f, 0.9f) * 110f);
				} catch (UnityException e) {
						Debug.Log ("error from e " + e + " result");
				}
		}

		public void remove ()
		{
				GameObject.Destroy (watched);	
		}
}

