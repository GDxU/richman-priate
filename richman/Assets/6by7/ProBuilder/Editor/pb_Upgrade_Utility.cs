using UnityEngine;
using UnityEditor;
using System.Collections;
using ProBuilder2.Actions;

/**
 *	\brief Write any post-install code here.
 *	OnProBuilderUpgrade will be called whenever a ProBuilder upgrade script is run.
 */
public class pb_Upgrade_Utility : Editor
{
	public static void OnProBuilderUpgrade()
	{		
		pb_Editor_Utility.ForceRefresh(false);	// meshes need to be refreshed for some reason that I cannot recall.
		ProBuilderMenuItems.ForceCloseEditor();	// force close editor, which allows the new GUI to load resources.
		
		try {
			UpgradeSceneCollisions();				// r961+ uses Unity's component system for colliders
		} catch(System.Exception e) {
			Debug.LogWarning("ProBuilder failed upgrading scene collisions.\n" + e.ToString());
		}

		try {
			NoDrawFix.FixNoDraw();
		} catch(System.Exception e) {
			Debug.LogWarning("ProBuilder failed resetting GameObject flags.  Try manually running Repair/Fix GameObject Flags.\n" + e.ToString());
		}

		try {
			foreach(pb_Object pb in FindObjectsOfType(typeof(pb_Object)))
				foreach(pb_Face face in pb.faces)
				{
					if(face.colors == null)
						pb.SetFaceColor(face, Color.white);
				}
		} catch(System.Exception e) {
			Debug.LogWarning("Failed repairing vertex colors.\n" + e.ToString());
		}
	}

	public static void UpgradeSceneCollisions()
	{
		pb_Entity[] objs = (pb_Entity[])GameObject.FindObjectsOfType(typeof(pb_Entity));
		foreach(pb_Entity ent in objs)
			ent.GenerateCollisions();
	}

	// for most users this should not be necessary
	// [MenuItem("Tools/ProBuilder/Actions/Refresh Project Collisions")] 
	public static void UpgradeSceneCollisionsProjectWide()
	{
		string[] allFiles = System.IO.Directory.GetFiles("Assets/", "*.*", System.IO.SearchOption.AllDirectories);
		string[] allScenes = System.Array.FindAll(allFiles, name => name.EndsWith(".unity"));
		string currentScene = EditorApplication.currentScene;
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		int objCount = 0;
		
		foreach(string cheese in allScenes)
		{
			EditorApplication.OpenScene(cheese);
			pb_Entity[] objs = (pb_Entity[])GameObject.FindObjectsOfType(typeof(pb_Entity));
			foreach(pb_Entity ent in objs)
				ent.GenerateCollisions();
			objCount += objs.Length;
			EditorApplication.SaveScene(cheese, false);
		}

		EditorApplication.OpenScene(currentScene);

		EditorUtility.DisplayDialog("Update Collisions", "Collisions successfully refreshed!\n\nRefreshed " + objCount + " ProBuilder objects across " + allScenes.Length + " scenes.", "Okay");

	}
}
