using UnityEngine;
using System.Collections;

public class qd_DecalDebug : MonoBehaviour {

	void OnDrawGizmos()
	{
		Mesh m = GetComponent<MeshFilter>().sharedMesh;

		for(int i = 0; i < m.normals.Length; i++)
		{
			Gizmos.DrawLine( transform.TransformPoint(m.vertices[i]), transform.TransformPoint(m.vertices[i]) + transform.TransformDirection(m.normals[i]) * .2f);
		}
	}
}
