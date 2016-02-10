using UnityEngine;
using System.Collections;

public class rotateP : MonoBehaviour
{

		public float speed = 30.0f;
		private Vector3 pivot = new Vector3 (0f, 50f, 0f);
	
		void Update ()
		{
				transform.RotateAround (pivot, Vector3.up, speed * Time.deltaTime);
		}
}
