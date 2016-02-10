using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]
public class Spinner : MonoBehaviour
{
		int numberAverages = 3, touchCount = 0;
		public Camera input_camera;
		private Quaternion originalRotation ;
		private Quaternion offsetRotation ;
		private Vector3 touchPosition, touchpos;
		// Make sure there is always a Rigidbody
		private bool calculation_enter;

		void Awake ()
		{
				calculation_enter = false;
				numberAverages = Mathf.Clamp (numberAverages, 1, numberAverages);
		
		}

		public void OnPress ()
		{
				touchpos = UICamera.currentTouch.pos;
				calculation_enter = true;
		}

		public void OnDrag ()
		{
				touchpos = UICamera.currentTouch.pos;
				//calculation_enter = true;
		}

		public void OnRelease ()
		{
				touchpos = UICamera.currentTouch.pos;
				calculation_enter = false;
		}

		void Update ()
		{
				RaycastHit hit;
				Vector3 dir;
				if (calculation_enter) {    
						rigidbody.angularVelocity = Vector3.zero;
			
						// Get movement of the finger since last frame
						//original line since that cannot be complied	Vector2 touchPosition = iPhoneInput.GetTouch (0).position;
						Vector2 touchPosition = new Vector3 ();
						// Record initial variables
						if (Physics.Raycast (input_camera.ScreenPointToRay (touchPosition), out hit)) {
								originalRotation = transform.rotation;
								dir = hit.point - transform.position;
								offsetRotation = Quaternion.Inverse (Quaternion.LookRotation (dir));
								Spin (dir);
						}
				}
		}
	
		private void Spin (Vector3 dir)
		{
				RaycastHit hit;
				List<Vector3> previousDirList = new List<Vector3> ();
				Vector3 currentDir = new Vector3 ();
				Vector2 touchPositio = new Vector2 ();
				bool touched = false;
		
				// Initialize previous dir list
				for (int i  = 0; i < numberAverages; i++) {
						previousDirList.Add (currentDir);
				}
				currentDir = dir;
				//touchPosition = iPhoneInput.GetTouch (0).position;
				while (calculation_enter && Physics.Raycast (input_camera.ScreenPointToRay(touchpos),out hit)) {
						//	touchPosition = iPhoneInput.GetTouch (0).position;
						// Remove first element of the array
						previousDirList.RemoveAt (0);
						// Add current dir to the end
						previousDirList.Add (currentDir);
						currentDir = hit.point - transform.position;
			
						transform.rotation = Quaternion.LookRotation (currentDir) * offsetRotation * originalRotation;
						//yield;
				}
		
				// User let go of the mouse so make the object spin on its own
				Vector3 avgPreviousDir = Vector3.zero;
				foreach (Vector3 d in previousDirList) {
						avgPreviousDir = avgPreviousDir + d;
				}
				avgPreviousDir /= numberAverages;
				Kick (currentDir, avgPreviousDir);
		}
	
		private void Kick (Vector3 r2, Vector3 r1)
		{
				Vector3 linearVelocity;
				Vector3 angVelocity;
		
				// Calculate the angular velocity:  omega = r x v / r^2
				linearVelocity = (r2 - r1) / Time.deltaTime;
				rigidbody.angularVelocity = Vector3.Cross (r2, linearVelocity) / r2.sqrMagnitude;
		
		}
}