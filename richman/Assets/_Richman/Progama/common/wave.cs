using UnityEngine;
using System.Collections;

public class wave : MonoBehaviour
{
		public float amplitudeX = 0f;
		public float amplitudeY = 1.0f;
		public float omegaX = 0f;
		public float omegaY = 0.50f;
		public float index;
		private Vector3 currentpos;

		void Start ()
		{
				currentpos = transform.position;
		}

		public void Update ()
		{
				index += Time.deltaTime;
				float x = amplitudeX * Mathf.Cos (omegaX * index);
				float y = Mathf.Abs (amplitudeY * Mathf.Sin (omegaY * index));
				transform.localPosition = new Vector3 (currentpos.x + x, currentpos.y + y, currentpos.z);
		}
}
