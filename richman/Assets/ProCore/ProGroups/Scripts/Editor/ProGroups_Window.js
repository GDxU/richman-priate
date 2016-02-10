//#pragma strict


class ProGroups_Window extends EditorWindow 
{
	static var window : EditorWindow;

    @MenuItem("Tools/ProGroups/Open ProGroups Window (v1.02)")
    static function Init()
	{
		window = GetWindow(ProGroups_Window);
		window.Show();
		window.title = "ProGroups v1.0.2";
    }
	
	@MenuItem("Tools/ProGroups/New Group From Selection %g")
	static function NewGroupFromSelection()
	{
		groupContainer.NewGroup(Selection.gameObjects, newGroupName);
		window.Repaint();
	}
	//---

	var needsConnect : boolean = true;
	static var groupContainer : GroupContainer;
	
	var guiPath : String = "Assets/ProCore/ProGroups/GUI/";
	
	var icon_Rect : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_Rect.png", typeof(Texture2D));
	var icon_SnowFlake : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_SnowFlake.png", typeof(Texture2D));
	var icon_Eye : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_Eye.png", typeof(Texture2D));
	var icon_Select : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_Select.png", typeof(Texture2D));
	var icon_UpdateGroup : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_UpdateGroup.png", typeof(Texture2D));
	var icon_MoveUp : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_MoveUp.png", typeof(Texture2D));
	var icon_Delete : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_Delete.png", typeof(Texture2D));
	var icon_Gear : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_Gear.png", typeof(Texture2D));
	var icon_Add : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_Add.png", typeof(Texture2D));
	var icon_MultiPlus : Texture2D = Resources.LoadAssetAtPath(guiPath+"ProGroupsIcons_MultiPlus.png", typeof(Texture2D));
	
	var freezeIcon : Texture2D;
	var visIcon : Texture2D;
	
	var scrollPos : Vector2;
	var modMode : boolean = false;
	static var newGroupName : String = "New Group";
	
	function OnEnable()
	{
		needsConnect = true;
	}
	
	function OnDisable()
	{
		needsConnect = true;
	}
	
	function Connect()
	{
		if(groupContainer == null)
		{
			if(GameObject.Find("_GroupContainer"))
			{
				groupContainer = GameObject.Find("_GroupContainer").GetComponent(GroupContainer);
			}
			else
			{
				var groupObj = new GameObject();
				groupObj.name = "_GroupContainer";
				groupObj.AddComponent("GroupContainer");
				groupContainer = groupObj.GetComponent(GroupContainer);
				
				var newHideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
				groupObj.hideFlags = newHideFlags;
			}
		}
		needsConnect = false;
		
		//This is a little hacky, but works fine, and solves the window nullRef error when exiting Play Mode
		var windowMe = GetWindow(ProGroups_Window);
		windowMe.Repaint();
	}
	
	//GUI
	function OnGUI() 
	{
		if(!groupContainer)
		{
			Connect();
		}
		//make the bleeping buttons behave...
		var iconButtonStyle : GUIStyle = new GUIStyle(GUI.skin.button);
		iconButtonStyle.stretchWidth = false;
		iconButtonStyle.margin = RectOffset(2,2,1,1);
		iconButtonStyle.padding.left = 1;
		iconButtonStyle.padding.right = 1;
		iconButtonStyle.normal.background = null;
		iconButtonStyle.hover.background = null;
		iconButtonStyle.active.background = null;
		iconButtonStyle.focused.background = null;
		iconButtonStyle.onNormal.background = null;
		iconButtonStyle.onHover.background = null;
		iconButtonStyle.onActive.background = null;
		iconButtonStyle.onFocused.background = null;
		//
		
		
		EditorGUILayout.BeginHorizontal();
		if(!modMode)
		{
			newGroupName = EditorGUILayout.TextField(newGroupName);
			if(newGroupName == "")
				newGroupName = "New Group";
			
			if(GUILayout.Button(GUIContent(icon_Add, "Create New Group from Selected Objects"), iconButtonStyle))
			{
				GUIUtility.keyboardControl = 0;
				NewGroupFromSelection();
				window.Repaint();
			}
			
			if(GUILayout.Button(GUIContent(icon_Gear, "Modify Groups"), iconButtonStyle))
			{
				modMode = true;
			}
		}
		else
		{
			if(GUILayout.Button("Done"))
			{
				modMode = false;
				modBtnText = "Modify";
			}
		}
		//
		EditorGUILayout.EndHorizontal();
		
		if(groupContainer.sceneGroups != null)
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			EditorGUILayout.BeginVertical();
			var i : int = 0;
			for(var theGroup : Group in groupContainer.sceneGroups)
			{
				
				if(theGroup.frozen)
					freezeIcon = icon_SnowFlake;
				else
					freezeIcon = icon_Rect;
					
				if(theGroup.hidden)
					visIcon = icon_Rect;
				else
					visIcon = icon_Eye;
				
				
				EditorGUILayout.BeginHorizontal();
				//var vertPos : int = (t_height*(i))+(vertOffset*(i+2))+initialVertOffset;
				
				if(modMode)
				{					
					//move up
					if(i != 0)
					{
						if(GUILayout.Button(GUIContent(icon_MoveUp, "Move Group Up"), iconButtonStyle))
						{
							GUIUtility.keyboardControl = 0;
							groupContainer.MoveGroupUp(i);
							window.Repaint();
						}
					}
					
					//delete
					if(GUILayout.Button(GUIContent(icon_Delete, "Remove Group (NOT Objects!)"), iconButtonStyle))
					{
						if(EditorUtility.DisplayDialog("Remove this Group?", "All objects from the group will become visible and un-frozen. No objects will be deleted.", "Confirm", "Cancel"))
						{
							GUIUtility.keyboardControl = 0;
							groupContainer.RemoveGroup(i);
							window.Repaint();
						}
					}
					
					//add selected to group
					if(GUILayout.Button(GUIContent(icon_MultiPlus, "Add Selected Objects to this Group"), iconButtonStyle))
					{
						GUIUtility.keyboardControl = 0;
						AddSelectedToGroup(theGroup);
						window.Repaint();
					}
					
					//update
					if(GUILayout.Button(GUIContent(icon_UpdateGroup, "Rebuild Group from Selection"), iconButtonStyle))
					{
						if(EditorUtility.DisplayDialog("Replace Objects in the Group with Selected Objects", "Note: all objects from the old group will become visible and un-frozen.", "Confirm", "Cancel"))
						{
							GUIUtility.keyboardControl = 0;
							groupContainer.UpdateGroup(theGroup, Selection.gameObjects);
							if(window != null)
								window.Repaint();
						}
					}
					
					//group name
					theGroup.name = EditorGUILayout.TextField(theGroup.name);
				}
				else
				{
					//select
					if(GUILayout.Button(GUIContent(icon_Select, "Select Group Objects"), iconButtonStyle))
					{
						GUIUtility.keyboardControl = 0;
						SelectGroup(theGroup);
					}
					
					//vis toggle
					if(GUILayout.Button(visIcon, iconButtonStyle))
					{
						if(Event.current.alt)
						{
							groupContainer.Isolate(i);
						}
						else
						{
							groupContainer.ToggleVis(theGroup);
						}
						
						GUIUtility.keyboardControl = 0;
					}
					
					//freeze toggle
					if(GUILayout.Button(freezeIcon, iconButtonStyle))
					{
						GUIUtility.keyboardControl = 0;
						groupContainer.ToggleFreeze(theGroup);
					}
					
					//group name
					GUILayout.Label(theGroup.name);
				}

				i++;
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}
	}
	
	function SelectGroup(theGroup : Group)
	{
		var foundIssue : boolean = false;
		var unitythree : boolean = false;
		var continueCheck : boolean = true;
		var continueSelect : boolean = true;
		for(var theObject : GameObject in theGroup.objects)
		{
			if(continueCheck)
			{
				#if UNITY_3_5
				unitythree = true;
				if(!theObject.active || theObject.hideFlags == HideFlags.NotEditable)
					foundIssue = true;
				#endif
				
				if(!unitythree)
				{
					if(!theObject.activeSelf || theObject.hideFlags == HideFlags.NotEditable)
						foundIssue = true;
				}
				
				if(foundIssue)
				{
					continueCheck = false;
					if(!EditorUtility.DisplayDialog("Warning! Some Objects in this Group are Frozen or Hidden.", "You should probably un-hide and un-freeze the objects in this group first", "Select Anyway", "Cancel Selection"))
					{
						continueSelect = false;
						Selection.objects = new UnityEngine.Object[0];
					}
				}
			}
		}
		
		if(continueSelect)
		{
			var tempSelectionArray = new Array();
			for(var theObject : GameObject in theGroup.objects)
			{
				tempSelectionArray.Add(theObject);
			}
			Selection.objects = tempSelectionArray.ToBuiltin(GameObject);
		}
	}
	
	function AddSelectedToGroup(theGroup : Group)
	{
		var tempSelectionArray = new Array();
		for(var theObject : GameObject in theGroup.objects)
		{
			tempSelectionArray.Add(theObject);
		}
		for(var theObject : GameObject in Selection.gameObjects)
		{
			tempSelectionArray.Add(theObject);
		}
		Selection.objects = tempSelectionArray.ToBuiltin(GameObject);
		
		groupContainer.UpdateGroup(theGroup, Selection.gameObjects);
	}
}
