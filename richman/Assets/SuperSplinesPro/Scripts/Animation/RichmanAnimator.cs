using UnityEngine;
using System.Collections;

public class RichmanAnimator : MonoBehaviour
{
		private Spline spline;
		public WrapMode wrapMode = WrapMode.Clamp;
		public float 
				rotationOffset = .009f, 
				stop_scope_allowance = 1000f,
				passedTime = 0f,
				internalSpeed = 0f,
				speed = 0.0158f, 
				acceleration = 0.5f, 
				deceleration = 0.8f;
		//private double segment;
		[SerializeField]
		private bool
				drive = false, continueDriveDemo = false;
		[SerializeField]
		private int
				target_forward_stops = 0, cache_forward = 0;
		private int
				RunningIndex, totalStops, EndIndex, adjustedRunningIndex, stopID;
		//public GameObject water_particles;
		private bool hasParticleSystem, waterSplat = false;
		public ParticleSystem particelSysBack;

		public void setDemo (bool r)
		{
				continueDriveDemo = r;
				drive = r;
		}
		// Use this for initialization
		void Start ()
		{
				stopID = 0;
				RunningIndex = 1;
		}

		private float WrapValue (float v, float start, float end, WrapMode wMode)
		{
				switch (wMode) {
				case WrapMode.Clamp:
				case WrapMode.ClampForever:
						return Mathf.Clamp (v, start, end);
				case WrapMode.Default:
				case WrapMode.Loop:
						return Mathf.Repeat (v, end - start) + start;
				case WrapMode.PingPong:
						return Mathf.PingPong (v, end - start) + start;
				default:
						return v;
				}
		}

		private void WaterAnimation (bool e)
		{
		
				if (e && !waterSplat) {
						particelSysBack.Play (true);
						waterSplat = true;
				}
				if (!e && waterSplat) {
						particelSysBack.Stop (true);
						waterSplat = false;
				}
		}

		private void process_input_forward_steps ()
		{
				if (!drive) {
						if (cache_forward > 0) {
								target_forward_stops = (RunningIndex + cache_forward) % totalStops;
								drive = true;
								//RunningIndex = RunningIndex + target_forward_stops;
								cache_forward = 0;
						}
				}
		}

		//private Vector3 targetNode;

		private void trigger_location_event ()
		{
				//RunningIndex = RunningIndex % totalStops;
				//	segment = (float)RunningIndex / (float)totalStops;
				adjustedRunningIndex = RunningIndex % totalStops;
				Vector3 p1 = spline.SplineNodes [adjustedRunningIndex].Position;
				Vector3 p2 = spline.GetPositionOnSpline (WrapValue (passedTime + rotationOffset, 0f, 1f, wrapMode));
				float delta = Mathf.Abs (Vector3.Distance (p1, p2));
				//targetNode = p1;
			
				if (stopID != adjustedRunningIndex && 
						delta < stop_scope_allowance) {
						stopID = adjustedRunningIndex;
						RunningIndex++;
						//RunningIndex = RunningIndex > EndIndex ? 0 : RunningIndex;
						if (!continueDriveDemo) {
								if (target_forward_stops == stopID) {
										drive = false;
										boardcastSignal ("complete_location_from_forward");
								} else {
										boardcastSignal ("arrive_at_location_jit");
								}
						}

						Debug.DrawLine (p2, p1, Color.red);
				} else {

						Debug.DrawLine (p2, p1, Color.yellow);
					
						//"arrive_at_location_jit
				}
				//if (stopID == adjustedRunningIndex)
				//				RunningIndex++;
		}

		public void BoatRunTrigger (bool t)
		{
				drive = t;
		}

		private void boardcastSignal (string signal)
		{
				if (gameEngine.Instance != null) {
						gameEngine.Instance.eventStepTrigger (GetHashCode (), stopID, signal);
						
				}
		}
		//move forward from the dice index reading...
		public void move (int input)
		{
				cache_forward = input;
		}

//		public void OnDrawGizmosSelected ()
//		{
//				// Draw a yellow sphere at the transform's position
//				Gizmos.color = Color.yellow;
//				Gizmos.DrawSphere (targetNode, stop_scope_allowance);
//		}

		public void init (Spline va)
		{
				spline = va;
				RunningIndex = 1;
				totalStops = va.SplineNodes.Length;
				EndIndex = totalStops - 1;
				//	segment = (float)RunningIndex / (float)totalStops;
				hasParticleSystem = particelSysBack != null ? true : false;
				WaterAnimation (false);
				trigger_location_event ();
		}
		// Update is called once per frame
		void Update ()
		{
				process_input_forward_steps ();
				passedTime += Time.deltaTime * internalSpeed;
				float clampedParam = WrapValue (passedTime, 0f, 1f, wrapMode);
				transform.rotation = spline.GetOrientationOnSpline (WrapValue (passedTime + rotationOffset, 0f, 1f, wrapMode));
				transform.position = spline.GetPositionOnSpline (clampedParam) - transform.right * spline.GetCustomValueOnSpline (clampedParam) * .5f;
				if (drive) {
						
						internalSpeed = Mathf.Lerp (internalSpeed, speed, Time.deltaTime * acceleration);
						
						trigger_location_event ();
						if (hasParticleSystem) {
								//t = water_particles.GetComponent<ParticleSystem> ();
								WaterAnimation (true);
						}
						//spline.GetClosestPointParam(transform.position,clampedParam
				} else {
						if (internalSpeed > 0.001f) {
								internalSpeed = Mathf.Lerp (internalSpeed, 0.0f, Time.deltaTime * deceleration);
						} else {
								internalSpeed = 0f;
						}
						if (hasParticleSystem) {
								WaterAnimation (false);
						}
				}
		}
}
