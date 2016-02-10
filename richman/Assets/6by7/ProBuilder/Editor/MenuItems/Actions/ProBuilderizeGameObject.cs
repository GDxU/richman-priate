using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ProBuilder2.Common;

namespace ProBuilder2.Actions
{
public class ProBuilderizeMesh : Editor
{
	[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Actions/ProBuilderize Selection")]
	public static void MenuProBuilderizeObjects()
	{
		foreach(Transform t in Selection.transforms)
		{
			if(t.GetComponent<MeshFilter>())
			{
				pb_Object pb = ProBuilderize(t);
				if(pb.GetComponent<MeshCollider>())	
					DestroyImmediate(pb.GetComponent<MeshCollider>());
			}
		}
	}

	public static pb_Object ProBuilderize(Transform t)
	{
		Mesh m = t.GetComponent<MeshFilter>().sharedMesh;

		// pb_Face[] faces = new pb_Face[m.triangles.Length/3];
		List<pb_Face> faces = new List<pb_Face>();

		for(int n = 0; n < m.subMeshCount; n++)
		{
			int[] tris = m.GetTriangles(n);
			for(int i = 0; i < tris.Length; i+=3)
			{
				int index = -1;
				for(int j = 0; j < faces.Count; j++)
				{
					if(	faces[j].distinctIndices.Contains(tris[i+0]) ||
						faces[j].distinctIndices.Contains(tris[i+1]) ||
						faces[j].distinctIndices.Contains(tris[i+2]))
					{
						index = j;
						break;
					}
				}
				
				if(index > -1)
				{
					int len = faces[index].indices.Length;
					int[] arr = new int[len + 3];
					System.Array.Copy(faces[index].indices, 0, arr, 0, len);
					arr[len+0] = tris[i+0];
					arr[len+1] = tris[i+1];
					arr[len+2] = tris[i+2];
					faces[index].SetIndices(arr);
					faces[index].RebuildCaches();
				}
				else
				{
					faces.Add( 
						new pb_Face(
							new int[3] {
								tris[i+0],
								tris[i+1],
								tris[i+2]
								},
							t.GetComponent<MeshRenderer>().sharedMaterials[n],
							new pb_UV(),
							0,
							-1,
							Color.white
						));					
				}
			}
		}

		t.gameObject.SetActive(false);
		pb_Object pb = ProBuilder.CreateObjectWithVerticesFaces(m.vertices, faces.ToArray());
		pb.SetName("FrankenMesh");
		pb.Refresh();
		pb_Editor_Utility.SetEntityType(ProBuilder.EntityType.Detail, pb.gameObject);
		
		GameObject go = pb.gameObject;

		go.transform.position = t.position;
		go.transform.localRotation = t.localRotation;
		go.transform.localScale = t.localScale;
		pb.FreezeScaleTransform();
		return pb;
	}
}
}