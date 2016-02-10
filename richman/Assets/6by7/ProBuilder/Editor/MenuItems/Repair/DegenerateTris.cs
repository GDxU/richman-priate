using UnityEditor;
using UnityEngine;
using System.Collections;
using ProBuilder2.MeshOperations;

namespace ProBuilder2.Actions
{
	public class DegenerateTris : Editor
	{
		[MenuItem("Tools/ProBuilder/Repair/Remove Degenerate Triangles")]
		public static void MenuRemoveDegenerateTriangles()
		{
			foreach(pb_Object pb in pbUtil.GetComponents<pb_Object>(Selection.transforms))
				pb.RemoveDegenerateTriangles();
		}
	}
}