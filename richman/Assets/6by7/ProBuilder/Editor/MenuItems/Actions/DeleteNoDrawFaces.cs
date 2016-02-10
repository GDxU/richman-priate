using UnityEngine;
using UnityEditor;
using System.Collections;

public class DeleteNoDrawFaces : Editor
{
	[MenuItem("Tools/ProBuilder/Actions/Delete All NoDraw Faces")]
	public static void MenuDeleteNoDrawFaces()
	{
		foreach(pb_Object pb in pbUtil.GetComponents<pb_Object>(Selection.transforms))
		{
			pbUndo.RecordObject(pb, "Delete NoDraw Faces");
			foreach(pb_Face face in pb.faces)
				if(face.material != null && face.material.name == "NoDraw")
					pb.DeleteFace(face);
		}

	}
}
