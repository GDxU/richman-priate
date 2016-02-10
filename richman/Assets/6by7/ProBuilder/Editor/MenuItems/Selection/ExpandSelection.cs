using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using ProBuilder2.EditorEnum;

public class ExpandSelection : Editor
{
	[MenuItem("Tools/ProBuilder/Selection/Grow Selection &g", false, pb_Constant.MENU_SELECTION + 1)]
	public static void MenuGrowSelection()
	{
		foreach(pb_Object pb in pbUtil.GetComponents<pb_Object>(Selection.transforms))
		{
			switch(pb_Editor.instance.selectionMode)
			{
				case SelectMode.Vertex:
					pb.SetSelectedEdges(pbMeshUtils.GetConnectedEdges(pb, pb.SelectedTriangles));
					break;
				
				case SelectMode.Face:
				case SelectMode.Edge:
					List<pb_Face>[][] all = pbMeshUtils.GetConnectedFacesJagged(pb, pb.SelectedFaces);
					List<pb_Face> f = new List<pb_Face>();
					foreach(List<pb_Face>[] fds in all)
						foreach(List<pb_Face> lf in fds)
							f.AddRange(lf);
					f = f.Distinct().ToList();

					pb.SetSelectedFaces(f.ToArray());

					break;
			}
		}
		pb_Editor.instance.UpdateSelection();
	}
}
