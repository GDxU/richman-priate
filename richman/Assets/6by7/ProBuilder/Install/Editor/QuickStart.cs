#if UNITY_EDITOR

#if UNITY_4_3 || UNITY_4_3_0 || UNITY_4_3_1 || UNITY_4_3_2 || UNITY_4_3_3 || UNITY_4_3_4 || UNITY_4_3_5 || UNITY_4_3_6 || UNITY_4_3_7 || UNITY_4_3_8 || UNITY_4_3_9 || UNITY_4_4 || UNITY_4_4_0 || UNITY_4_4_1 || UNITY_4_4_2 || UNITY_4_4_3 || UNITY_4_4_4 || UNITY_4_4_5 || UNITY_4_4_6 || UNITY_4_4_7 || UNITY_4_4_8 || UNITY_4_4_9 || UNITY_4_5 || UNITY_4_5_0 || UNITY_4_5_1 || UNITY_4_5_2 || UNITY_4_5_3 || UNITY_4_5_4 || UNITY_4_5_5 || UNITY_4_5_6 || UNITY_4_5_7 || UNITY_4_5_8 || UNITY_4_5_9 || UNITY_4_6 || UNITY_4_6_0 || UNITY_4_6_1 || UNITY_4_6_2 || UNITY_4_6_3 || UNITY_4_6_4 || UNITY_4_6_5 || UNITY_4_6_6 || UNITY_4_6_7 || UNITY_4_6_8 || UNITY_4_6_9 || UNITY_4_7 || UNITY_4_7_0 || UNITY_4_7_1 || UNITY_4_7_2 || UNITY_4_7_3 || UNITY_4_7_4 || UNITY_4_7_5 || UNITY_4_7_6 || UNITY_4_7_7 || UNITY_4_7_8 || UNITY_4_7_9 || UNITY_4_8 || UNITY_4_8_0 || UNITY_4_8_1 || UNITY_4_8_2 || UNITY_4_8_3 || UNITY_4_8_4 || UNITY_4_8_5 || UNITY_4_8_6 || UNITY_4_8_7 || UNITY_4_8_8 || UNITY_4_8_9
#define UNITY_4_3
#endif

using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

/**
 *	\brief 
 */
public class QuickStart : EditorWindow
{
	#if PROTOTYPE
	const string PRODUCT_NAME = "Prototype";
	const string INSTALL_PATH = "Assets/6by7/Prototype/Install/";
	#else
	const string PRODUCT_NAME = "ProBuilder";
	const string INSTALL_PATH = "Assets/6by7/ProBuilder/Install/";
	#endif

	static string[] FILES_TO_DELETE = new string[]
	{
		"VertexColorInterface.cs",
		"MirrorTool.cs"
	};

	[MenuItem("Tools/" + PRODUCT_NAME + "/Run Update or Install", false, 1)]
	public static void InstallUpdate()
	{
		#if PROTOTYPE
		EditorWindow.GetWindow<QuickStart>(true, "Prototype Install", true).ShowUtility();
		#else
		EditorWindow.GetWindow<QuickStart>(true, "ProBuilder Install", true).ShowUtility();
		#endif
	}

	int currentRevision = -1;
	int packNo;
	#if PROTOTYPE
	const string PACKNAME = "Prototype";
	#else
	const string PACKNAME = "ProBuilder";
	#endif

	private void OnEnable()
	{
		currentRevision = GetCurrentVersion();
		foundPacks = GetProBuilderPacks();

		if(install == InstallType.Release)
			selectedPack = GetHighestVersion_Release(foundPacks);
		else
			selectedPack = GetHighestVersion_Source(foundPacks);
	}

	public enum InstallType
	{
		Release,
		#if !PROTOTYPE
		Source
		#endif
	}

	private InstallType install;
	private GUIStyle headerStyle = new GUIStyle();
	private Vector2 scroll = Vector2.zero;
	private string[] foundPacks = new string[0]{};
	private int  selectedPack = -1;
	private Rect scrollBox = new Rect(0,0,0,0);
	int scrollMaxHeight = 0;
	private void OnGUI()
	{
		Color oldBGC = GUI.backgroundColor;

		if(EditorGUIUtility.isProSkin)
			headerStyle.normal.textColor = new Color(1f, 1f, 1f, .8f);
		else
			headerStyle.normal.textColor = Color.black;

		headerStyle.alignment = TextAnchor.MiddleCenter;
		headerStyle.fontSize = 14;
		headerStyle.fontStyle = FontStyle.Bold;

		GUILayout.Label(PACKNAME + " Install Tool", headerStyle);

		GUILayout.Space(10);

		GUI.color = Color.white;
		
		GUILayout.BeginHorizontal();

			GUILayout.BeginVertical();
				GUILayout.Label("Current Install: " + ((currentRevision > 0) ? currentRevision.ToString() : 
					PACKNAME + " Not Found" ));
				GUILayout.Label("Newest Available: r" + packNo);
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
				GUI.backgroundColor = Color.green;
				if(GUILayout.Button("Install", GUILayout.MinHeight(28)))
				{
					if(EditorUtility.DisplayDialog("Install " + PACKNAME + " Update", "Install " + Path.GetFileNameWithoutExtension(foundPacks[selectedPack]) + "\n\nWarning!  Back up your project!",
						"Run Update or Install", "Cancel"))
					{
						LoadPack(foundPacks[selectedPack]);
						this.Close();
					}
				}
				GUI.backgroundColor = Color.white;

			GUILayout.EndVertical();

		GUILayout.EndHorizontal();

		TextAnchor ta = GUI.skin.box.alignment;
		GUI.skin.box.alignment = TextAnchor.MiddleLeft;
		Color oldBoxTC = GUI.skin.box.normal.textColor;

		if(EditorGUIUtility.isProSkin)
			GUI.skin.box.normal.textColor = new Color(1f, 1f, 1f, .65f);

		GUILayout.Space(4);
			switch(install)
			{
				case InstallType.Release:
					GUILayout.Box("Release is the standard installation type.  It provides pre-compiled " + PACKNAME + " libraries instead of source code, meaning Unity doesn't need to compile extra files.\n\n*Note that all " + PACKNAME + " `Actions` and example scene files are still provided as source code.");
					break;
				#if !PROTOTYPE
				case InstallType.Source:
					GUILayout.Box(PACKNAME + " will be installed with full source code.  Note that you will need to remove any previous "+ PACKNAME + " \"Release\" installations prior to installing a Source version.  This is not recommended for users upgrading from a prior installation, as you *will* lose all prior-built " + PACKNAME + " objects.");
					break;
				#endif
			}
		GUILayout.Space(4);
		GUI.skin.box.alignment = ta;
		GUI.skin.box.normal.textColor = oldBoxTC;

		EditorGUI.BeginChangeCheck();
		
		GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Install Type");
			install = (InstallType)EditorGUILayout.EnumPopup(install);
		GUILayout.EndHorizontal();

		if( EditorGUI.EndChangeCheck() )
		switch(install)
		{
			case InstallType.Release:
				selectedPack = GetHighestVersion_Release(foundPacks);
				break;

			#if !PROTOTYPE
			case InstallType.Source:
				selectedPack = GetHighestVersion_Source(foundPacks);
				break;
			#endif
		}

		GUILayout.Label("Available " + PACKNAME + " Packs", EditorStyles.boldLabel);

		GUIStyle labelStyle = GUIStyle.none;

		if(EditorGUIUtility.isProSkin)
			labelStyle.normal.textColor = new Color(1f, 1f, 1f, .8f);

		labelStyle.normal.background = EditorGUIUtility.whiteTexture;
		labelStyle.alignment = TextAnchor.MiddleLeft;
		int CELL_HEIGHT = 18;
		labelStyle.contentOffset = new Vector2(2, 0f);
		labelStyle.margin = new RectOffset(6, 0, 0, 0);

		Rect r = GUILayoutUtility.GetLastRect();

		if(Event.current.type == EventType.Repaint)
		{
			scrollBox = new Rect(r.x, r.y+17, Screen.width-9, Screen.height-r.y-22);
			scrollMaxHeight = (int)scrollBox.height-2;
		}

		GUI.Box(scrollBox, "");

		scroll = EditorGUILayout.BeginScrollView(scroll, false, false, GUILayout.MinWidth(Screen.width-6),
			GUILayout.MaxHeight(scrollMaxHeight));

		Color odd = EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f, .1f) : new Color(.2f, .2f, .2f, .1f);
		Color even = EditorGUIUtility.isProSkin ? new Color(.2f, .2f, .2f, .6f) : new Color(.2f, .2f, .2f, .05f);

		int i = 0;
		for(int n = 0; n < foundPacks.Length; n++)
		{
			if(n == selectedPack) {
				GUI.backgroundColor = new Color(0.23f, .49f, .89f, 1f);
					Color oc = labelStyle.normal.textColor;
					labelStyle.normal.textColor = Color.white;
					GUILayout.Box(Path.GetFileName(foundPacks[n]), labelStyle, GUILayout.MinHeight(CELL_HEIGHT), GUILayout.MaxHeight(CELL_HEIGHT));
					labelStyle.normal.textColor = oc;
				GUI.backgroundColor = Color.white;
			}
			else
			{
				switch(install)
				{
					case InstallType.Release:
						if(foundPacks[n].Contains("-source"))
							continue;
						break;
					
					#if !PROTOTYPE
					case InstallType.Source:
						if(foundPacks[n].Contains("-unity"))
							continue;
						break;
					#endif
				}

				GUI.backgroundColor = i++ % 2 == 0 ? odd : even;
				if(GUILayout.Button(Path.GetFileName(foundPacks[n]), labelStyle, GUILayout.MinHeight(CELL_HEIGHT), GUILayout.MaxHeight(CELL_HEIGHT)))
					selectedPack = n;
				GUI.backgroundColor = Color.clear;
			}
		}

		EditorGUILayout.EndScrollView();
		labelStyle.normal.background = null;
		GUI.backgroundColor = oldBGC;
	}

#region Load Package

	private int GetHighestVersion_Source(string[] packs)
	{
		// sort out non ProBuilder2v- packages
		int highestVersion = 0;
		int index = 0;
		for(int i = 0; i < packs.Length; i++) 
		{
			if(packs[i].Contains("ProBuilder2-v") && packs[i].Contains("-source"))
			{ 
				string pattern = @"[v0-9]{4,6}";
				MatchCollection matches = Regex.Matches(packs[i], pattern, RegexOptions.IgnorePatternWhitespace);

				int revision = -1;
				foreach(Match m in matches)
					revision = int.Parse(m.ToString().Replace("v", ""));
				
				if(revision < 1)
					continue;

				if(revision > highestVersion) {
					highestVersion = revision;
					index = i;
				}
			}
		}
		packNo = highestVersion;
		return index;
	}

	private int GetHighestVersion_Release(string[] packs)
	{
		// sort out non ProBuilder2v- packages
		int highestVersion = 0;
		int index = 0;
		
		#if PROTOTYPE
		string packageName = "Prototype-v";
		#else
		string packageName = "ProBuilder2-v";
		#endif

		#if UNITY_4_3
		string uVersion = "-unity43";
		#else
		string uVersion = "-unity35";
		#endif

		for(int i = 0; i < packs.Length; i++)
		{
			if(packs[i].Contains(packageName) && !packs[i].Contains("-source") && packs[i].Contains(uVersion))
			{
				// probably not necessary here, but regular expressions are super cool.  
				string pattern = @"[v0-9]{4,6}";
				MatchCollection matches = Regex.Matches(packs[i], pattern, RegexOptions.IgnorePatternWhitespace);

				int revision = -1;
				foreach(Match m in matches)
					revision = int.Parse(m.ToString().Replace("v", ""));
				
				if(revision < 1)
					continue;

				if(revision > highestVersion) {
					highestVersion = revision;
					index = i;
				}
			}
		}
		packNo = highestVersion;
		return index;
	}

	private string[] GetProBuilderPacks()
	{
		string[] allFiles = Directory.GetFiles(INSTALL_PATH, "*.*", SearchOption.AllDirectories);
		
		#if UNITY_4_3
		string[] allPackages = System.Array.FindAll(allFiles, name => (name.EndsWith(".unitypackage") && !name.Contains("unity35")));
		#else
		string[] allPackages = System.Array.FindAll(allFiles, name => (name.EndsWith(".unitypackage") && !name.Contains("unity43")));
		#endif

		return allPackages;
	}

	public static void LoadPack(string pb_path)
	{
		RemoveOutdatedFiles();

		// remove old GUI path
		string oldGUIPath = "Assets/6by7/Shared/GUI";
		
		DeleteDirectory(oldGUIPath);
	
		#if !UNITY_STANDALONE_OSX && !UNITY_IPHONE
		pb_path = pb_path.Replace("\\", "/");
		#endif		

		if(pb_path == "") return;
		
		AssetDatabase.ImportPackage(pb_path, false);
	}
#endregion

#region File Utility

	public static void RemoveOutdatedFiles()
	{
		foreach(string str in FILES_TO_DELETE)
		{
			string[] files = System.IO.Directory.GetFiles("Assets/", str, System.IO.SearchOption.AllDirectories);
			foreach(string file in files)
			{
				System.IO.File.Delete(file);
				if(System.IO.File.Exists(file+".meta"))
					System.IO.File.Delete(file + ".meta");		
			}

			// AssetDatabase.Refresh();
		}
	}

	public static bool ProBuilderExists()
	{
		string[] allFiles = Directory.GetFiles("Assets/", "*.*", SearchOption.AllDirectories);
		string[] allLibs = System.Array.FindAll(allFiles, name => name.EndsWith(".dll"));

		for(int i = 0; i < allLibs.Length; i++)
			if(allLibs[i].Contains("ProBuilder"))
				return true;
		return false;
	}

	public static void DeleteDirectory(string path)
	{
		if(!Directory.Exists(path))
			return;

		string[] files = Directory.GetFiles(path);
		string[] dirs = Directory.GetDirectories(path);

		foreach (string file in files)
		{
			File.SetAttributes(file, FileAttributes.Normal);
			File.Delete(file);
		}

		foreach (string dir in dirs)
		{
			DeleteDirectory(dir);
		}

		Directory.Delete(path, false);

		if(File.Exists(path+".meta"))
			File.Delete(path+".meta");
	}
#endregion

#region ProBuilder Utillity
	
	// todo - replace this with something less stupid.
	private int GetCurrentVersion()
	{		
		string[] files = System.IO.Directory.GetFiles("Assets/", "pb_About*", System.IO.SearchOption.AllDirectories);
		string aboutPath = "";
		foreach(string file in files)
		{
			if(file.Contains("pb_About"))
			{
				aboutPath = file;	
				break;
			}	
		}

		if (aboutPath != "")
		{
			string[] str = File.ReadAllLines(aboutPath);
			foreach(string s in str)
			{
				if(s.Contains("REVISION_NO"))
				{
					string rev = Regex.Match(s, @"\d+").Value;
					int i; 
					if (int.TryParse(rev, out i) )
						return i;
				}
			}
		}
		return -1;
	}
#endregion
}

class ProBuilderInstallPostProcessor : AssetPostprocessor 
{
	#if !PROTOTYPE
	const string PACKNAME = "ProBuilder";
	#else
	const string PACKNAME = "Prototype";
	#endif

	static void OnPostprocessAllAssets (
		string[] importedAssets,
		string[] deletedAssets,
		string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		foreach(var str in importedAssets)
		{
			// Detect a new unity package
			if(str.Contains(".unitypackage") && str.Contains(PACKNAME))
			{
				QuickStart.InstallUpdate();
				break;
			}

			// An upgrade has taken place
			if(str.Contains("ProBuilderCore.dll"))
			{
				System.Type upgradeUtil = System.Type.GetType("pb_Upgrade_Utility");
				MethodInfo onUpgrade = upgradeUtil.GetMethod("OnProBuilderUpgrade");
				onUpgrade.Invoke(null, null);
				break;
			}
		}
	}

	private static string[] GetScenes()
	{
		string[] allFiles = Directory.GetFiles("Assets/", "*.*", SearchOption.AllDirectories);
		string[] allScenes = System.Array.FindAll(allFiles, name => name.EndsWith(".unity"));
		return allScenes;
	}

}

#endif