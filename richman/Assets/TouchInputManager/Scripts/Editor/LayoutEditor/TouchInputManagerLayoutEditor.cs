using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using TouchInputManagerBackend;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TouchInputManagerBackendEditor
{
    public class TouchInputManagerLayoutEditor : TouchInputManagerEditorBase {
	
        private TouchInputManagerEditor_GUISidebar layoutSidebar;
        private TouchInputManagerEditor_GUISidebar currentSelectedLayout;
        private TouchInputManagerLayoutPreview layoutPreview;
	
        private List<TouchInputLayout> _allLayouts = new List<TouchInputLayout>();

        private bool compileChangeUpdate = false;
        internal class SelectedLayout
        {
            public TouchInputLayout _layout = null;
            public TouchBase _activeBase = null;
            public TouchTracker _activeTracker = null;
            public string renameNameTemp;
            public void Select(TouchInputLayout toSelect)
            {
                GUI.FocusControl("");
                _activeTracker = null;
                _activeBase = null;
                _layout = toSelect;
                if(toSelect != null)
                {
                    renameNameTemp = _layout.name;
                }
            }
        }

        private readonly SelectedLayout _selectedLayout = new SelectedLayout();
	

        [MenuItem("Window/Touch Input Manager/Touch Layout")]
        static void GetWindow()
        {
            GetWindow<TouchInputManagerLayoutEditor>("Touch Layout");
        }
	
		
        protected override void InitializeStyles()
        {
            base.InitializeStyles();
            layoutSidebar = new TouchInputManagerEditor_GUISidebar(150,400,_styleDarkWithBorderBG,GUI_LayoutList,null,Repaint);
            currentSelectedLayout = new TouchInputManagerEditor_GUISidebar(250,400,_styleDarkWithBorderBG,GUI_SelectedLayout,null,Repaint);
		
            //TODO clean this shit up, create a class that holds on styles/etc and pass that around
            GUIStyle miniDarkBG = new GUIStyle(EditorStyles.miniLabel);
            miniDarkBG.normal.background = _styleOldSelectedBG.normal.background;
		
            layoutPreview = new TouchInputManagerLayoutPreview(_styleSelectedBG,_styleDarkNoBorderBG,miniDarkBG,Repaint);
		
            RefreshList();
        }
	
        protected new void OnEnable()
        {
            base.OnEnable();
            RefreshList();
        }
	
        protected void OnProjectChange()
        {
            RefreshList();
            Repaint();
        }

        public void OnInspectorUpdate()
        {
            Repaint(); 
        }

	
        protected void Update()
        {
            if (initialized == false || layoutSidebar == null)
            {
                initialized = false;
                return;
            }

            if (compileChangeUpdate != EditorApplication.isCompiling)
            {
                Repaint();
                compileChangeUpdate = EditorApplication.isCompiling;
            }

            layoutSidebar.Update();
         
            //incase we delete a joystick etc

            if (_selectedLayout._layout != null) 
                currentSelectedLayout.Update();

            if (_selectedLayout._layout != null)
            { 
                if (layoutPreview.Update())
                    EditorUtility.SetDirty(_selectedLayout._layout);
                if(layoutPreview.trackerBeingModified != null)
                {
                    _selectedLayout._activeTracker = layoutPreview.trackerBeingModified._tracker;
                    _selectedLayout._activeBase = layoutPreview.trackerBeingModified._base;
                }
            }
        }

        protected new void OnGUI()
        {
            if (_selectedLayout._layout != null)
            {
                Rect dragAcceptArea = new Rect(layoutSidebar._width + currentSelectedLayout._width, 0, position.width - (layoutSidebar._width + currentSelectedLayout._width), position.height - 16);
                if (Event.current.type == EventType.dragUpdated || Event.current.type == EventType.dragPerform || Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                {
                    if (dragAcceptArea.Contains(Event.current.mousePosition))
                    {
                        TouchInputManagerInputDrag_Wrapper wrapper = DragAndDrop.GetGenericData("TouchInputManagerInputDrag") as TouchInputManagerInputDrag_Wrapper;
                        if(wrapper != null)
                        {
                            TouchBase b = (wrapper).Value as TouchBase;
                            if (b != null)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                                if (Event.current.type == EventType.DragPerform)
                                {
                                    DragAndDrop.AcceptDrag();
                                    Event.current.Use();
                                    AddNewInput(b);
                                }
                            }
                        }
                    
                    }
                }
            }


            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
		
            if (GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GenericMenuCreateAll();
            }
		
            GUILayout.FlexibleSpace();

            bool idNeedRebuild = (InputIDNeedsRebuilding() || LayoutIDNeedsRebuilding());
            if (EditorApplication.isCompiling)
                GUI.enabled = false;

            if (GUILayout.Button(new GUIContent(EditorApplication.isCompiling || AnyIDErrors() ? EditorApplication.isCompiling ? "Compiling..." : "Errors exist" : idNeedRebuild ? "Build ID's" : "Build ID's", EditorApplication.isCompiling || AnyIDErrors() || idNeedRebuild ? _errorIcon : _noErrorIcon), EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                BuildIDs();
            }
         
            GUI.enabled = true;
            

            EditorGUILayout.EndHorizontal();
		
            base.OnGUI();
            if(initialized == false)
                return;
		
            EditorGUILayout.BeginHorizontal();
            layoutSidebar.OnGUI(0,0,position.height-16);
		
            if(_selectedLayout._layout != null)
                currentSelectedLayout.OnGUI(layoutSidebar._width,0,position.height-16);
		
            EditorGUILayout.EndHorizontal();
		
		
            if(_selectedLayout._layout != null)
                layoutPreview.OnGUI(new Rect(layoutSidebar._width+currentSelectedLayout._width,0,position.width-(layoutSidebar._width+currentSelectedLayout._width),position.height-16),_selectedLayout._layout.trackerPrefabBasePrefab);

		
	
        }
	
			
        private void GUI_LayoutList()
        {
            if (_allLayouts.Count == 0)
            {
                EditorGUILayout.LabelField("Click create to begin!");
            }
            else
            {
                GUIContent layouts = new GUIContent();
			
                layouts.text = "All Layouts";
                layouts.image = _allLayoutsIcon;
                GUILayout.Label(layouts,EditorStyles.boldLabel);
			
                foreach(TouchInputLayout layout in _allLayouts.OrderBy(a => a.name))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    layouts.text = layout.name;
                    layouts.image = _layoutIcon;
                    GUILayout.Label(layouts, _selectedLayout._layout == layout ? (layoutSidebar._clickedAway ? _styleOldSelectedBG : _styleSelectedBG) : EditorStyles.label);
                    GUILayout.EndHorizontal();
				
                    if (Event.current.type == EventType.mouseDown && 
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        layoutSidebar._clickedAway = false;
					
                        _selectedLayout.Select(layout);
					
                        if (Event.current.button == 1)
                        {
                            GenericMenuCreateDelete();
                        }
					
					
                        Event.current.Use();
                    }

                }
            }
			
        }
	
        private void GUI_SelectedLayout()
        {
            GUILayout.Label("Name: ",EditorStyles.boldLabel);
		
            _selectedLayout.renameNameTemp = EditorGUILayout.TextField(_selectedLayout.renameNameTemp, EditorStyles.textField);

		
            if (GUILayout.Button("Rename", EditorStyles.miniButton))
            {
                string result = "e";
                _selectedLayout.renameNameTemp = _selectedLayout.renameNameTemp.Trim();
                if (_selectedLayout.renameNameTemp.Length != 0)
                {
                    result = AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_selectedLayout._layout), _selectedLayout.renameNameTemp);
                    AssetDatabase.Refresh();
                }

                if (result != "")
                {
                    EditorUtility.DisplayDialog("Error", "The layout must have a name that is unique, contain atleast 1 character and does not contain symbols", "Ok");
                    _selectedLayout.renameNameTemp = _selectedLayout._layout.name;
                }
                else
                {
                    _selectedLayout._layout.gameObject.name = _selectedLayout.renameNameTemp;
                    EditorUtility.SetDirty(_selectedLayout._layout);
                }
			
                RefreshList();
			
                LooseFocus();
            }


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Layout ID: ", !IsLayoutIDUnique(_selectedLayout._layout.touchLayoutID) || _selectedLayout._layout.touchLayoutID.Length == 0 ? _errorIcon : _noErrorIcon), EditorStyles.label);

            string newlayoutID = _selectedLayout._layout.touchLayoutID;

            newlayoutID = EditorGUILayout.TextField(newlayoutID, EditorStyles.textField);
            newlayoutID = Regex.Replace(newlayoutID, @"[^a-zA-Z0-9]", "");

            if (newlayoutID != _selectedLayout._layout.touchLayoutID)
            {
                _selectedLayout._layout.touchLayoutID = newlayoutID;
                EditorUtility.SetDirty(_selectedLayout._layout);
            }

            EditorGUILayout.EndHorizontal();

		
            EditorGUILayout.BeginVertical();
            {
                int count = 0;
			
                GUILayout.Label(new GUIContent("Joysticks",_joystickIcon),EditorStyles.boldLabel);
                foreach(TouchInputLayout.TrackerPrefabBasePrefabPair b in _selectedLayout._layout.trackerPrefabBasePrefab.OrderBy(a => a._base.name).Where(a => a._base is TouchJoystick))
                {
                    count++;

                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label(new GUIContent(b._base.name, _joystickIcon), layoutPreview.trackerBeingModified != null && layoutPreview.trackerBeingModified._tracker == b._tracker ? _styleSelectedBG : EditorStyles.label);
                    EditorGUILayout.EndHorizontal();

                
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(32);

                    GUILayout.Label(new GUIContent("ID: ", !IsInputIDUnique(b._tracker._id, _selectedLayout._layout) || b._tracker._id.Length == 0 ? _errorIcon : _noErrorIcon), EditorStyles.label);

                    string newID = b._tracker._id;
                
                    newID = EditorGUILayout.TextField(newID, EditorStyles.textField);
                    newID = Regex.Replace(newID, @"[^a-zA-Z0-9]", "");

                    if(newID != b._tracker._id)
                    {
                        b._tracker._id = newID;
                        EditorUtility.SetDirty(b._tracker);
                    }

                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(32);
                
                    if (GUILayout.Button("Remove", EditorStyles.miniButton))
                    {
                        layoutPreview.trackerBeingModified = b;
                        DeleteSelectedInput();
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    if (Event.current.type == EventType.mouseDown &&
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        layoutPreview.trackerBeingModified = b;
                        Event.current.Use();
                    }

                
                }
                if(count == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label("None");
                    EditorGUILayout.EndHorizontal();
                }
                count = 0;
			
                GUILayout.Label(new GUIContent("Buttons",_buttonIcon),EditorStyles.boldLabel);
                foreach(TouchInputLayout.TrackerPrefabBasePrefabPair b in _selectedLayout._layout.trackerPrefabBasePrefab.OrderBy(a => a._base.name).Where(a => a._base is TouchButton))
                {
                    count++;

                
                
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label(new GUIContent(b._base.name, _buttonIcon), layoutPreview.trackerBeingModified != null && layoutPreview.trackerBeingModified._tracker == b._tracker ? _styleSelectedBG : EditorStyles.label);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(32);
                    GUILayout.Label(new GUIContent("ID: ", !IsInputIDUnique(b._tracker._id, _selectedLayout._layout) || b._tracker._id.Length == 0 ? _errorIcon : _noErrorIcon), EditorStyles.label);

                    string newID = b._tracker._id;

                    newID = EditorGUILayout.TextField(newID, EditorStyles.textField);
                    newID = Regex.Replace(newID, @"[^a-zA-Z0-9]", "");

                    if (newID != b._tracker._id)
                    {
                        b._tracker._id = newID;
                        EditorUtility.SetDirty(b._tracker);
                    } 
                
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(32);

                    if (GUILayout.Button("Remove", EditorStyles.miniButton))
                    {
                        layoutPreview.trackerBeingModified = b;
                        DeleteSelectedInput();
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    if (Event.current.type == EventType.mouseDown &&
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        layoutPreview.trackerBeingModified = b;
                        Event.current.Use();
                    }

                }
                if(count == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label("None");
                    EditorGUILayout.EndHorizontal();
                }
                count = 0;
			
                GUILayout.Label(new GUIContent("Zones",_layoutIcon),EditorStyles.boldLabel);
                foreach(TouchInputLayout.TrackerPrefabBasePrefabPair b in _selectedLayout._layout.trackerPrefabBasePrefab.OrderBy(a => a._base.name).Where(a => a._base is TouchZone))
                {
                    count++;
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label(new GUIContent(b._base.name, _layoutIcon), layoutPreview.trackerBeingModified != null && layoutPreview.trackerBeingModified._tracker == b._tracker ? _styleSelectedBG : EditorStyles.label);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(32);
                    GUILayout.Label(new GUIContent("ID: ", !IsInputIDUnique(b._tracker._id,_selectedLayout._layout) || b._tracker._id.Length == 0 ? _errorIcon : _noErrorIcon), EditorStyles.label);

                    string newID = b._tracker._id;

                    newID = EditorGUILayout.TextField(newID, EditorStyles.textField);
                    newID = Regex.Replace(newID, @"[^a-zA-Z0-9]", "");

                    if (newID != b._tracker._id)
                    {
                        b._tracker._id = newID;
                        EditorUtility.SetDirty(b._tracker);
                    } 
                
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(32);

                    if (GUILayout.Button("Remove", EditorStyles.miniButton))
                    {
                        layoutPreview.trackerBeingModified = b;
                        DeleteSelectedInput();
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    if (Event.current.type == EventType.mouseDown &&
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        layoutPreview.trackerBeingModified = b;
                        Event.current.Use();
                    }
                }
                if(count == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label("None");
                    EditorGUILayout.EndHorizontal();
                }
                count = 0;
            }
            EditorGUILayout.EndVertical();
		
		
        }
        protected void NewLayout()
        {
            AssetDatabase.CopyAsset("Assets/TouchInputManager/Prefabs/Layouts/Template/Layout.prefab",AssetDatabase.GenerateUniqueAssetPath("Assets/TouchInputManager/Prefabs/Layouts/Layout.prefab"));
            AssetDatabase.Refresh();
		
            RefreshList();
        }
	
        private void GenericMenuCreateAll()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Layout"), false, NewLayout); 

            menu.ShowAsContext();
            Event.current.Use();
        }
	
		
        private void GenericMenuCreateDelete()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("New Layout"), false, NewLayout); 
            menu.AddItem(new GUIContent("Duplicate"), false, DuplicateSelected); 
            menu.AddItem(new GUIContent("Delete"), false, DeleteSelected); 

            menu.ShowAsContext();
            Event.current.Use();
        }

        private void MenuRemoveInput()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Remove from layout"), false, DeleteSelectedInput);

            menu.ShowAsContext();
            Event.current.Use();
        }
	
        private void DuplicateSelected()
        {
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(_selectedLayout._layout),AssetDatabase.GenerateUniqueAssetPath(AssetDatabase.GetAssetPath(_selectedLayout._layout)));
            AssetDatabase.Refresh();
		
            RefreshList();
        }
	
        private void DeleteSelected()
        {
            if(!EditorUtility.DisplayDialog("Are you sure?","This cannot be undone!","Yes","No"))
                return;
		
            if(_selectedLayout._layout != null)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_selectedLayout._layout));
                _selectedLayout.Select(null);
                RefreshList();
            }
        }

        protected void AddNewInput (TouchBase b)
        {
            if(_selectedLayout._layout == null)
                return;
		
            TouchTracker tc = _selectedLayout._layout.gameObject.AddComponent<TouchTracker>();
            tc.activeRegion = new Rect(0.25f,0.25f,0.5f,0.5f);
            tc._id = "";

            _selectedLayout._layout.trackerPrefabBasePrefab.Add(new TouchInputLayout.TrackerPrefabBasePrefabPair{ _tracker = tc, _base = b});
            EditorUtility.SetDirty(_selectedLayout._layout.gameObject);
            EditorUtility.SetDirty(_selectedLayout._layout);
            EditorUtility.SetDirty(tc);
        }
	
        protected void DeleteInput(TouchBase b)
        {
            if(_selectedLayout._layout == null)
                return;
		
            TouchInputLayout.TrackerPrefabBasePrefabPair p = _selectedLayout._layout.trackerPrefabBasePrefab.First(a => a._base == b);
            if(p != null)
            {
                _selectedLayout._layout.trackerPrefabBasePrefab.Remove(p);
                DestroyImmediate(p._tracker, true);
                EditorUtility.SetDirty(_selectedLayout._layout.gameObject);
                EditorUtility.SetDirty(_selectedLayout._layout);
            }
        }

        protected void DeleteSelectedInput()
        {
            if (_selectedLayout._layout == null)
                return;

            TouchInputLayout.TrackerPrefabBasePrefabPair p = layoutPreview.trackerBeingModified;
            if (p != null)
            {
                _selectedLayout._layout.trackerPrefabBasePrefab.Remove(p);
                DestroyImmediate(p._tracker, true);
                EditorUtility.SetDirty(_selectedLayout._layout.gameObject);
                EditorUtility.SetDirty(_selectedLayout._layout);
            }
        }

        private bool IsInputIDUnique(string id, TouchInputLayout layout)
        {
            if (layout == null)
                return false;

            int count = 0;
            foreach (TouchInputLayout.TrackerPrefabBasePrefabPair trackerPrefabBasePrefabPair in layout.trackerPrefabBasePrefab)
            {
                if (trackerPrefabBasePrefabPair._tracker._id == id)
                {
                    count++;
                    if (count > 1)
                        return false;
                }
            }
            return true;
        }

        private bool IsLayoutIDUnique(string id)
        {
            int count = 0;
            foreach (TouchInputLayout layout in _allLayouts)
            {
                if (layout.touchLayoutID == id)
                {
                    count++;
                    if (count > 1)
                        return false;
                }
            }
            return true;
        }

        private bool LayoutIDNeedsRebuilding()
        {
            string[] existingIDs = Enum.GetNames(typeof(LayoutID));

            foreach (TouchInputLayout touchInputLayout in _allLayouts)
            {
                if (!existingIDs.Contains(touchInputLayout.touchLayoutID))
                    return true;
            }

            return false;
        }

        private bool InputIDNeedsRebuilding()
        {
            string[] existingIDs = Enum.GetNames(typeof(InputID));

            foreach (TouchInputLayout touchInputLayout in _allLayouts)
            {
                foreach (TouchInputLayout.TrackerPrefabBasePrefabPair trackerPrefabBasePrefabPair in touchInputLayout.trackerPrefabBasePrefab)
                {
                    if (!existingIDs.Contains(trackerPrefabBasePrefabPair._tracker._id))
                        return true;
                }
            }

            return false;
        }

        private bool AnyIDErrors()
        {
            foreach (var touchInputLayout in _allLayouts)
            {
                if (touchInputLayout.touchLayoutID.Length == 0 || !IsLayoutIDUnique(touchInputLayout.touchLayoutID))
                    return true;

                foreach (var trackerPrefabBasePrefabPair in touchInputLayout.trackerPrefabBasePrefab)
                {
                    if (!IsInputIDUnique(trackerPrefabBasePrefabPair._tracker._id, touchInputLayout) || trackerPrefabBasePrefabPair._tracker._id.Length == 0)
                        return true;
                }
            }

            return false;
        }

        private void BuildIDs()
        {
            CodeTypeDeclaration layoutIDEnum = new CodeTypeDeclaration("LayoutID");
            layoutIDEnum.IsEnum = true;
            CodeTypeDeclaration inputIDEnum = new CodeTypeDeclaration("InputID");
            inputIDEnum.IsEnum = true;
            int inputIDValues = 0;
            int layoutIDValues = 0;

            List<string> addedInputIDs = new List<string>(); //these only have to be unique between layouts, so ID's might repeat

            foreach (var touchInputLayout in _allLayouts)
            {
                CodeMemberField enumField = new CodeMemberField("LayoutID", touchInputLayout.touchLayoutID);
                enumField.InitExpression = new CodePrimitiveExpression(layoutIDValues);
                layoutIDValues++;
                layoutIDEnum.Members.Add(enumField);

                foreach (var trackerPrefabBasePrefabPair in touchInputLayout.trackerPrefabBasePrefab)
                {
                    if(addedInputIDs.Contains(trackerPrefabBasePrefabPair._tracker._id))
                        continue;

                    addedInputIDs.Add(trackerPrefabBasePrefabPair._tracker._id);
                    // Creates the enum member
                    CodeMemberField enumFieldInput = new CodeMemberField("InputID", trackerPrefabBasePrefabPair._tracker._id);
                    enumFieldInput.InitExpression = new CodePrimitiveExpression(inputIDValues);
                    inputIDValues++;
                    inputIDEnum.Members.Add(enumFieldInput);
                }
            }

            CodeNamespace tim = new CodeNamespace("UnityEditor");
            tim.Types.Add(layoutIDEnum);
            tim.Types.Add(inputIDEnum);

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.BlankLinesBetweenMembers = false;

            using (StreamWriter sourceWriter = new StreamWriter("Assets/TouchInputManager/Scripts/TouchIdentifiersTemp.cs"))
            {
                provider.GenerateCodeFromType(layoutIDEnum, sourceWriter, options);
                provider.GenerateCodeFromType(inputIDEnum, sourceWriter, options);
                sourceWriter.Close();
                CompilerResults r = provider.CompileAssemblyFromFile(new CompilerParameters(), Path.GetFullPath("Assets/TouchInputManager/Scripts/TouchIdentifiersTemp.cs"));

                if (r.Errors.Count != 0)
                {
                    EditorUtility.DisplayDialog("Error",
                                                "An ID either starts with a number, contains a symbol/whitespace or is not unique - this is not allowed.",
                                                "Ok");
                    File.Delete("Assets/TouchInputManager/Scripts/TouchIdentifiersTemp.cs");
                    return;
                }

                //its worked, correct the enums on all of our touch sticks/buttons/layouts

                AssetDatabase.Refresh();
                AssetDatabase.DeleteAsset("Assets/TouchInputManager/Scripts/TouchIdentifiers.cs");
                AssetDatabase.CopyAsset("Assets/TouchInputManager/Scripts/TouchIdentifiersTemp.cs",
                                        "Assets/TouchInputManager/Scripts/TouchIdentifiers.cs");
                AssetDatabase.DeleteAsset("Assets/TouchInputManager/Scripts/TouchIdentifiersTemp.cs");
                AssetDatabase.Refresh();
                return;
            }
        }

        private void RefreshList() 
        {
            if(_allLayouts == null)
                _allLayouts = new List<TouchInputLayout>();
            _allLayouts.Clear();
				
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/TouchInputManager/Prefabs/Layouts");
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info) 
            {
                TouchInputLayout tl = AssetDatabase.LoadAssetAtPath("Assets/TouchInputManager/Prefabs/Layouts/"+f.Name,typeof(TouchInputLayout)) as TouchInputLayout;
                if(tl != null)
                {
                    if(tl.trackerPrefabBasePrefab == null)
                        tl.trackerPrefabBasePrefab = new List<TouchInputLayout.TrackerPrefabBasePrefabPair>();

                    for(int i = tl.trackerPrefabBasePrefab.Count-1; i >= 0; i--)
                    {
                        if(tl.trackerPrefabBasePrefab[i]._base == null || tl.trackerPrefabBasePrefab[i]._tracker == null)
                        {
                            if (tl.trackerPrefabBasePrefab[i]._base == null && tl.trackerPrefabBasePrefab[i]._tracker != null)
                            {
                                DestroyImmediate(tl.trackerPrefabBasePrefab[i]._tracker, true);    
                            }

                            tl.trackerPrefabBasePrefab.RemoveAt(i);
                        }
                        
                    }
                    _allLayouts.Add(tl);
                }
            }

            TouchInputManagerBehaviour tim = AssetDatabase.LoadAssetAtPath("Assets/TouchInputManager/Resources/TouchInputManager/TouchInputManager.prefab", typeof(TouchInputManagerBehaviour)) as TouchInputManagerBehaviour;
            if(tim.prefabTouchInputLayouts == null)
                tim.prefabTouchInputLayouts = new List<TouchInputLayout>();
            else 
                tim.prefabTouchInputLayouts.Clear();

            tim.prefabTouchInputLayouts.AddRange(_allLayouts);
            EditorUtility.SetDirty(tim);
            EditorUtility.SetDirty(tim.gameObject);
        }
	
    }
}