using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MyGUIBehavior, just OnGUI() from Unity;
/// </summary>
class MyGUIBehavior : MonoBehaviour
{
		public static MyGUIBehavior Instance = null;
		public GUISkin skin = null;
		public Font townNameFont = null;
		public Texture2D townNameBackground = null;
		public Texture2D windowBackground = null;
		public Texture2D windowBorder = null;
		public int windowPadding = 7;
		public Color affordableText = new Color (32.0f / 255.0f, 121.0f / 255.0f, 32.0f / 255.0f, 1.0f);
		public Color notAffordableText = new Color (168.0f / 255.0f, 43.0f / 255.0f, 0.0f, 1.0f);
		public GUIStyle headerStyle = null;
		public GUIStyle descStyle = null;
		public GUIStyle infoStyle = null;
		private static SortedList<int, MyGUI> mMyGUIList;    //maintance a sorted list to draw;
		private static List<int> mRemoveGUI_LayerID_List;   //save idex of GUI which need to be removed (Auto clean)
		void Awake ()
		{
				Instance = this;
				//SetDefaultMyGUI();

				mMyGUIList = new SortedList<int, MyGUI> ();
				mRemoveGUI_LayerID_List = new List<int> ();

				SetDefaultValue ();
		}

		/// <summary>
		/// set default value;
		/// </summary>
		private void SetDefaultValue ()
		{
				skin = Config.Instance.skin;
				townNameFont = Config.Instance.townNameFont;
				townNameBackground = Config.Instance.townNameBackground;
				windowBackground = Config.Instance.windowBackground;
				windowBorder = Config.Instance.windowBorder;


				headerStyle = Config.Instance.headerStyle;
				descStyle = Config.Instance.descStyle;
				infoStyle = Config.Instance.infoStyle;
		}

		/*
    public MyGUI SetDefaultMyGUI()
    {
        GUISkin defaultGUISkin = GUISkin.CreateInstance("GUISkin") as GUISkin;
        defaultGUISkin.name = "DefaultGUISkin";
        defaultGUISkin.box.normal.background = null;//Config.Instance.windowBorder;
        defaultGUISkin.box.border.left = windowPadding;
        defaultGUISkin.box.border.right = windowPadding;
        defaultGUISkin.box.border.top = windowPadding;
        defaultGUISkin.box.border.bottom = windowPadding;


        DefaultMyGUI = new MyGUI(defaultGUISkin);
    }

     * */

		void OnGUI ()
		{
				if (Instance == null)
						return;

				foreach (KeyValuePair<int, MyGUI> keyValuePair in mMyGUIList) {
						MyGUI myGui = keyValuePair.Value;

						if ((myGui.TopGUICalls.Count == 0) && (myGui.BackgourndGUICalls.Count == 0)) {
								mRemoveGUI_LayerID_List.Add (keyValuePair.Key);
								continue;
						}
            
						foreach (var backgourndGuiCall in myGui.BackgourndGUICalls) {
								backgourndGuiCall ();
						}

						foreach (var topGuiCall in myGui.TopGUICalls) {
								topGuiCall ();
						}
				}
        
				RemoveUnuseMyGUI (); //remove unuse myGUI which is no callback stacks;
		}

		/// <summary>
		/// Remove unuse MyGUI from list;
		/// </summary>
		private void RemoveUnuseMyGUI ()
		{
				foreach (int key in mRemoveGUI_LayerID_List) {
						mMyGUIList.Remove (key);
						DebugExt.Log (" Remove Unuse MyGUI: layerID = " + key);
				}
				mRemoveGUI_LayerID_List.Clear ();
		}

		/// <summary>
		/// Add MyGUI into list;
		/// </summary>
		/// <param name="layerID"></param>
		/// <param name="myGui"></param>
		public void AddMyGUI (int layerID, MyGUI myGui)
		{
				mMyGUIList.Add (layerID, myGui);
		}

		/// <summary>
		/// Get MyGUI by layerID;
		/// </summary>
		public MyGUI GetMyGUI (int layerID)
		{
				MyGUI myGui;
				mMyGUIList.TryGetValue (layerID, out myGui);
				return myGui;
		}
}

