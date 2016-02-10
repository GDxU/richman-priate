using UnityEngine;
using UnityEditor;
using System.Collections;

public class pb_About : EditorWindow 
{
	public const string RELEASE_VERSION = "2.2.2f1";
	const int REVISION_NO = 1776;
	const string BUILD_DATE = "02-09-2014";
	const string VELOCIRAPTOR_RISK = "Electrifying Eagle";

	[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/About", false, 0)]
	public static void VersionInfo()
	{
		#if PROTOTYPE
		EditorWindow.GetWindow<pb_About>(true, "About Prototype", true);
		#else
		EditorWindow.GetWindow<pb_About>(true, "About ProBuilder", true);
		#endif
	}

	Vector2 SCREEN_SIZE = new Vector2(400f, 180f);
	const int LABEL_WIDTH = 200;
	public void OnGUI()
	{
		this.minSize = SCREEN_SIZE;
		this.maxSize = SCREEN_SIZE;

		GUIStyle headerStyle = GUIStyle.none;

		if(EditorGUIUtility.isProSkin)
			headerStyle.normal.textColor = new Color(1f, 1f, 1f, .8f);

		headerStyle.alignment = TextAnchor.MiddleCenter;
		// headerStyle.contentOffset = new Vector2(4f, 0f);
		headerStyle.fontSize = 14;
		headerStyle.fontStyle = FontStyle.Bold;

		#if PROTOTYPE
		GUILayout.Label("Prototype " + RELEASE_VERSION, headerStyle);
		#else
		GUILayout.Label("ProBuilder " + RELEASE_VERSION, headerStyle);
		#endif
		
		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
			GUILayout.Label("Version Number: ", EditorStyles.boldLabel, GUILayout.MinWidth(LABEL_WIDTH), GUILayout.MaxWidth(LABEL_WIDTH));
			GUILayout.Label(RELEASE_VERSION);
		GUILayout.EndHorizontal();

		GUILayout.Space(5);

		GUILayout.BeginHorizontal();
			GUILayout.Label("Revision Number: ", EditorStyles.boldLabel, GUILayout.MinWidth(LABEL_WIDTH), GUILayout.MaxWidth(LABEL_WIDTH));
			GUILayout.Label( REVISION_NO.ToString() );
		GUILayout.EndHorizontal();

		GUILayout.Space(5);

		GUILayout.BeginHorizontal();
			GUILayout.Label("Build Date: ", EditorStyles.boldLabel, GUILayout.MinWidth(LABEL_WIDTH), GUILayout.MaxWidth(LABEL_WIDTH));
			GUILayout.Label( BUILD_DATE);
		GUILayout.EndHorizontal();

		GUILayout.Space(5);

		GUILayout.BeginHorizontal();
			GUILayout.Label("Code Name: ", EditorStyles.boldLabel, GUILayout.MinWidth(LABEL_WIDTH), GUILayout.MaxWidth(LABEL_WIDTH));
			GUILayout.Label( VELOCIRAPTOR_RISK);
		GUILayout.EndHorizontal();

		GUILayout.Space(5);

		GUILayout.BeginHorizontal();
			GUILayout.Label("Support Email: ", EditorStyles.boldLabel, GUILayout.MinWidth(LABEL_WIDTH), GUILayout.MaxWidth(LABEL_WIDTH));
			GUILayout.Label("contact@sixbysevenstudio.com");
		GUILayout.EndHorizontal();

		GUILayout.Space(7);
		
		if(GUILayout.Button("Visit the Six By Seven Forums", GUILayout.MinHeight(26)))
			Application.OpenURL("http://www.sixbysevenstudio.com/forum/");
	}
}
