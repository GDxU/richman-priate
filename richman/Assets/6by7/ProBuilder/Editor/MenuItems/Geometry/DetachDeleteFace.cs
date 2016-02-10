using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ProBuilder2.Actions
{
	public class DetachDeleteFace : Editor
	{
		[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Geometry/Detach Face(s)", false, pb_Constant.MENU_GEOMETRY + pb_Constant.MENU_GEOMETRY_FACE + 4)]
		public static void MenuDetachFace()
		{
			pb_Object[] pbSelection = pbUtil.GetComponents<pb_Object>(Selection.transforms);

			pbUndo.RecordObjects(pbSelection as Object[], "Detach Face(s)");

			foreach(pb_Object pb in pbSelection)
			{
				foreach(pb_Face face in pb.SelectedFaces)
					pb.DetachFace(face);
				
				pb.Refresh();
				pb.GenerateUV2(true);
			}
		}

		[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Geometry/Delete Face (Backspace)", false, pb_Constant.MENU_GEOMETRY + pb_Constant.MENU_GEOMETRY_FACE + 5)]
		public static void MenuDeleteFace()
		{
			pb_Object[] pbSelection = pbUtil.GetComponents<pb_Object>(Selection.transforms);

			pbUndo.RecordObjects(pbSelection as Object[], "Delete Face(s)");

			foreach(pb_Object pb in pbSelection)
			{
				pb.DeleteFaces(pb.SelectedFaces);
				pb_Editor.instance.ClearFaceSelection();
				pb_Editor.instance.UpdateSelection();
				pb.Refresh();
				pb.GenerateUV2(true);
			}
		}
	}
}