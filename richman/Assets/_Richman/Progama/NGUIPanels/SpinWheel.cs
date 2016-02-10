using UnityEngine;
using System.Collections;

public class SpinWheel: MonoBehaviour
{

		public Camera Cam;
		private Vector3 currentLoc;
		private float dotUp;
		private float dotRight;
		private Vector3 Right;
		private Vector3 Up;
		private bool calculation_enter;
		private Vector2 momentum;

		void Start ()
		{
				//Set initial up and right directions for dot product calc
				Right = transform.right;
				Up = transform.up;
				calculation_enter = false;
				//		UICamera.OnCustomInput.Combine();
		}

		void OnDrawGizmos ()
		{


		}

		void OnDrawGizmosSelected ()
		{
				if (Cam != null) {
						Gizmos.color = Color.blue;
						//Gizmos.DrawLine (Cam.transform.position, UICamera.currentTouch.pos);
						Gizmos.DrawRay (transform.position, currentLoc);
				}
		}

		public void OnPress ()
		{
				//	Ray ray = Cam.ScreenPointToRay (UICamera.currentTouch.pos);
				//	RaycastHit hit;
				//	if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << 0)) {
				//hit.point
				Vector3 touchpos = UICamera.currentTouch.pos;
				currentLoc = transform.InverseTransformPoint (touchpos);
				currentLoc.z = 0;
				currentLoc = transform.TransformPoint (currentLoc);
				//initialize dot products
				dotUp = Vector3.Dot (Up.normalized, (currentLoc - transform.position).normalized);
				dotRight = Vector3.Dot (Right.normalized, (currentLoc - transform.position).normalized);
				//	}
				calculation_enter = true;
				Debug.Log ("OnPress casted");

		}

		public void Release ()
		{
				calculation_enter = false;	
				if (rigidbody.angularVelocity.magnitude < 5) {
						//	renderer.sharedMaterial = slowWheelMaterial;
					
				} else {	
						//	renderer.sharedMaterial = fastWheelMaterial;
						
					
				}
				//transform.rotation.ToAngleAxis (out  angle, out axis);
				//vangle = rigidbody.angularVelocity.magnitude;
				//float aar = Mathf.Pow
				float aar = Mathf.Sqrt (Mathf.Pow (momentum.x, 2) + Mathf.Pow (momentum.y, 2));
				rigidbody.AddRelativeTorque (0f, 0f, aar);
				Debug.Log ("show move");

		}

		private float angle = 0.0F, vangle = 0f;
		private Vector3 axis = Vector3.zero, touchpos;

		public void OnDrag ()
		{
				if (calculation_enter) {
						touchpos = UICamera.currentTouch.pos;
				}
		}

		private void cal ()
		{
				Vector3 newLoc = transform.InverseTransformPoint (touchpos);
				newLoc.z = 0;
				newLoc = transform.TransformPoint (newLoc);
				//Calculate the dot product between initial state and current mouse location
				float newDotUp = Vector3.Dot (Up.normalized, (newLoc - transform.position).normalized);
				float newDotRight = Vector3.Dot (Right.normalized, (newLoc - transform.position).normalized);
				//calculate angle between previous mouse location and current mouse location
				float ang = Vector3.Angle (currentLoc - transform.position, newLoc - transform.position);
				//Determine which direction to rotate.  Based off of previous and current dot products.
				if (newDotUp >= 0) {
						if (dotUp >= 0) {
								if (newDotRight > dotRight) {
										transform.Rotate (0, 0, -ang);
								} else
										transform.Rotate (0, 0, ang);
						} else {
								if (newDotRight > dotRight) {
										transform.Rotate (0, 0, ang);
								} else
										transform.Rotate (0, 0, -ang);
						}
				} else {
						if (dotUp < 0) {
								if (newDotRight > dotRight) {
										transform.Rotate (0, 0, ang);
								} else
										transform.Rotate (0, 0, -ang);
						} else {
								if (newDotRight > dotRight) {
										transform.Rotate (0, 0, -ang);
								} else
										transform.Rotate (0, 0, ang);
						}
				}
		
				//Reset previous information
				currentLoc = newLoc;
				momentum.x = newDotUp - dotUp;
				momentum.y = newDotRight - dotRight;
				dotUp = newDotUp;
				dotRight = newDotRight;
			
				//	Debug.Log (dotUp.ToString ());
				//}
				//transform.rigidbody.angularVelocity (new Vector3 (0, 0, ang));
	
		}

		private bool InputDTouchUp ()
		{
				return Input.GetMouseButtonUp (0);
		}

		void Update ()
		{
				
				if (InputDTouchUp () && calculation_enter) {
						Release ();
				} else {
						if (calculation_enter) {
								cal ();
						}
						
				}

				
				
		}
//		
//		public static double getAngle (double vx, double vy)
//		{
//				return Math.toDegrees (Math.atan2 (vy, vx));
//		}
//		
//		public static double getVelocityWithAngle (double vx, double vy)
//		{
//				return Math.sqrt (Math.pow (vx, 2) + Math.pow (vy, 2));
//		}
//		
//		public static void angleVelocityToXYVelocity (double angle, double velocity)
//		{
//				double vx = Math.cos (Math.toRadians (angle)) * velocity;
//				double vy = Math.sqrt (Math.pow (velocity, 2) - Math.pow (vx, 2));
//			
//				Debug.Log ("vx: " + vx + " vy: " + vy);
//		}

}