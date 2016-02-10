using UnityEngine;
using System.Collections;

public class simple_ship_drive : MonoBehaviour
{
		public GameObject water_particles;
		public bool waterSplat = false;
		private bool systemplay = false;
		private ParticleSystem t;
		// Use this for initialization
		void Start ()
		{
				t = water_particles.GetComponent<ParticleSystem> ();
				WaterAnimation (false);
		}

		public void WaterAnimation (bool e)
		{
				
				if (e && !waterSplat) {
						t.Play (true);
						waterSplat = true;
				} 

				if (!e && waterSplat) {
						t.Stop (true);
						waterSplat = false;
				}
		}
		// Update is called once per frame
		void Update ()
		{
//				if (waterSplat && !systemplay) {
//						WaterAnimation (true);
//				} 
//				if (!waterSplat && systemplay) {
//						WaterAnimation (false);
//				
//				}
		}
}
