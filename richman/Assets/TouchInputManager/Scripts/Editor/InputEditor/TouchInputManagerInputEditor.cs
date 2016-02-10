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
    public class TouchInputManagerInputEditor : TouchInputManagerEditorBase{
	    
        private TouchInputManagerInputEditorPreview _gamePreview;
        private TouchInputManagerEditor_GUISidebar listOfTouchInputs;
        private TouchInputManagerEditor_GUISidebar currentSelectedTouchBaseSettings;
	
        private List<TouchBase> inputs = new List<TouchBase>();
	
        internal class SelectedBase
        {
            public TouchBase _touchBase = null;
            public string name = null;
            public Texture baseImg, topImg;
            public TouchInputManagerEditor_GUISidebar selectedMiscPanel = null;
            public TouchAnimationPreset _touchAnimationActivePreset = null;
		
            public void Select(TouchBase toSelect)
            {
                GUI.FocusControl("");

                selectedMiscPanel = null;
                _touchAnimationActivePreset = null;
                _touchBase = toSelect;
                baseImg = null;
                topImg = null;
			
                name = "";
                if(toSelect != null )
                {
                    name = _touchBase.gameObject.name;
                    if(toSelect is TouchGUIBase) 
                    {
					
                        baseImg = ((TouchGUIBase)toSelect).basePart.texture;
                        topImg = ((TouchGUIBase)toSelect).topPart.texture;
                    }
                } 
            }
        }
        private SelectedBase _selectedTouchBase = new SelectedBase();

        //TODO rename everything in the code to Touch Input / Touch Input Editor
        [MenuItem("Window/Touch Input Manager/Touch Input")]
        static void GetWindow()
        {
            GetWindow<TouchInputManagerInputEditor>("Touch Input");
        }
	
        protected override void InitializeStyles()
        {
            base.InitializeStyles();
            listOfTouchInputs = new TouchInputManagerEditor_GUISidebar(150,400,_styleDarkWithBorderBG,GUI_ListOfTouchInputs,null,Repaint);
            listOfTouchInputs.Expand();
            currentSelectedTouchBaseSettings = new TouchInputManagerEditor_GUISidebar(250,400,_styleDarkWithBorderBG,GUI_EditPanel,null,Repaint);

            //TODO clean this shit up, create a class that holds on styles/etc and pass that around
            GUIStyle miniDarkBG = new GUIStyle(EditorStyles.miniLabel);
            miniDarkBG.normal.background = _styleOldSelectedBG.normal.background;
		
            _gamePreview = new TouchInputManagerInputEditorPreview(miniDarkBG,Repaint);
        }
	
        protected new void OnEnable()
        {
            base.OnEnable();
            RefreshListOfInputs();
        }
	
        protected void Update()
        {
            if(initialized == false || listOfTouchInputs == null)
            {
                initialized = false;
                return;
            } 
		
            if(_selectedTouchBase._touchBase == null)
            {
                ClearSelection();
            }
		
            listOfTouchInputs.Update();
            currentSelectedTouchBaseSettings.Update();
            if(_selectedTouchBase != null && _selectedTouchBase.selectedMiscPanel != null)
                _selectedTouchBase.selectedMiscPanel.Update();
		
            _gamePreview.Update();
        }
	
        protected void OnProjectChange()
        {
            RefreshListOfInputs();
            Repaint();
        }
	
        protected new void OnGUI()
        {
            base.OnGUI();
            if(initialized == false)
                return;
		
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
		
            if (GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GenericMenuCreateAll();
            }
		
            GUILayout.FlexibleSpace();
        
            EditorGUILayout.EndHorizontal();
		
            EditorGUILayout.BeginHorizontal();
		
            listOfTouchInputs.OnGUI(0,0,position.height - 15);
		
            if(_selectedTouchBase._touchBase != null)
            {
                currentSelectedTouchBaseSettings.OnGUI(GUILayoutUtility.GetLastRect().xMax,0,position.height - 15);
            }
		
            if(_selectedTouchBase != null && _selectedTouchBase.selectedMiscPanel != null)
                _selectedTouchBase.selectedMiscPanel.OnGUI(GUILayoutUtility.GetLastRect().xMax,0,position.height - 15);
		
            Rect lastRec = GUILayoutUtility.GetLastRect();
		
            EditorGUILayout.EndHorizontal();
		
            _gamePreview.OnGUI(new Rect(
                                   lastRec.x + lastRec.width,
                                   lastRec.y,
                                   Mathf.Clamp(position.width-(lastRec.x + lastRec.width),0,1000000), Mathf.Clamp(lastRec.height,0,1000000)));
        }
		
        protected void GUI_ListOfTouchInputs()
        {
            if (inputs.Count == 0)
            {
                EditorGUILayout.LabelField("Click create to begin!");
            }
            else
            {
                GUIContent layouts = new GUIContent();
			
                layouts.text = "Joysticks";
                layouts.image = _joystickIcon;
                GUILayout.Label(layouts,EditorStyles.boldLabel);
			
                foreach(TouchBase input in inputs.Where(a => a is TouchJoystick).OrderBy(a => a.name))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    layouts.text = input.name;
                    layouts.image = _joystickIcon;
                    GUILayout.Label(layouts, _selectedTouchBase._touchBase == input ? (listOfTouchInputs._clickedAway ? _styleOldSelectedBG : _styleSelectedBG) : EditorStyles.label);
                    GUILayout.EndHorizontal();
				
                    if (Event.current.type == EventType.mouseDown && 
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        if(_selectedTouchBase._touchBase != input)
                        {
                            listOfTouchInputs._clickedAway = false;
                            if (_selectedTouchBase._touchBase == null)
                                currentSelectedTouchBaseSettings.Expand();

                            _selectedTouchBase.Select(input);
                            _gamePreview.Load(_selectedTouchBase._touchBase);
                        }
					
					
					
                        Event.current.Use();
                    }
                    else if (Event.current.type == EventType.mouseUp && 
                             GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.button == 1)
                    {
                        GenericMenuCreateDelete(true, false, false);

                        Event.current.Use();
                    }

                    if(Event.current.type == EventType.MouseDrag && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        if(DragAndDrop.objectReferences.Length == 0)
                        {
                            DragAndDrop.PrepareStartDrag();

                            DragAndDrop.objectReferences = new UnityEngine.Object[] { null };

                            DragAndDrop.SetGenericData("TouchInputManagerInputDrag", new TouchInputManagerInputDrag_Wrapper(input));

                            DragAndDrop.StartDrag("Input Editor Drag");

                            Event.current.Use();

                        }
                    }
				
                }
			
                layouts.text = "Buttons";
                layouts.image = _buttonIcon;
                GUILayout.Label(layouts,EditorStyles.boldLabel);
			
                foreach(TouchBase input in inputs.Where(a => a is TouchButton).OrderBy(a => a.name))
                {
				
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    layouts.text = input.name;
                    layouts.image = _buttonIcon;
                    GUILayout.Label(layouts, _selectedTouchBase._touchBase == input ? (listOfTouchInputs._clickedAway ? _styleOldSelectedBG : _styleSelectedBG) : EditorStyles.label);
                    GUILayout.EndHorizontal();
                
                    if (_selectedTouchBase._touchBase != input && 
                        Event.current.type == EventType.mouseDown && 
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        listOfTouchInputs._clickedAway = false;
                        if(_selectedTouchBase._touchBase == null)
                            currentSelectedTouchBaseSettings.Expand();
					
                        _selectedTouchBase.Select(input);
                        _gamePreview.Load(_selectedTouchBase._touchBase);
					
                        Event.current.Use();
                    }
                    else if (Event.current.type == EventType.mouseUp &&
                             GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.button == 1)
                    {
                        GenericMenuCreateDelete(false, true, false);

                        Event.current.Use();
                    }

                    if(Event.current.type == EventType.MouseDrag && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        if (DragAndDrop.objectReferences.Length == 0)
                        {
                            DragAndDrop.PrepareStartDrag();

                            DragAndDrop.objectReferences = new UnityEngine.Object[] { null };

                            DragAndDrop.SetGenericData("TouchInputManagerInputDrag", new TouchInputManagerInputDrag_Wrapper(input));

                            DragAndDrop.StartDrag("Input Editor Drag");

                            Event.current.Use();

                        }
                    }
                }
			
                layouts.text = "Zones";
                layouts.image = _layoutIcon;
                GUILayout.Label(layouts,EditorStyles.boldLabel);
			
                foreach(TouchBase input in inputs.Where(a => a is TouchZone).OrderBy(a => a.name))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    layouts.text = input.name;
                    layouts.image = _layoutIcon;
                    GUILayout.Label(layouts, _selectedTouchBase._touchBase == input ? (listOfTouchInputs._clickedAway ? _styleOldSelectedBG : _styleSelectedBG) : EditorStyles.label);
                    GUILayout.EndHorizontal();
				
                    if (_selectedTouchBase._touchBase != input && 
                        Event.current.type == EventType.mouseDown && 
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        listOfTouchInputs._clickedAway = false;
                        if(_selectedTouchBase._touchBase == null)
                            currentSelectedTouchBaseSettings.Expand();
					
                        _selectedTouchBase.Select(input);
                        _gamePreview.Load(_selectedTouchBase._touchBase);
					
                        Event.current.Use();
                    }
                    else if (Event.current.type == EventType.mouseUp &&
                             GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.button == 1)
                    {
                        GenericMenuCreateDelete(false, false, true);

                        Event.current.Use();
                    }
				
                    if(Event.current.type == EventType.MouseDrag && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        if (DragAndDrop.objectReferences.Length == 0)
                        {
                            DragAndDrop.PrepareStartDrag();

                            DragAndDrop.objectReferences = new UnityEngine.Object[] { null };

                            DragAndDrop.SetGenericData("TouchInputManagerInputDrag", new TouchInputManagerInputDrag_Wrapper(input));

                            DragAndDrop.StartDrag("Input Editor Drag");

                            Event.current.Use();

                        }
                    }

                }
            }
        }
	
        protected void GUI_EditPanel()
        {
		
            GUILayout.Label("Name: ",EditorStyles.boldLabel);
            _selectedTouchBase.name = EditorGUILayout.TextField(_selectedTouchBase.name, EditorStyles.textField);

		
            if (GUILayout.Button("Rename", EditorStyles.miniButton))
            {
                if(_selectedTouchBase.name != AssetDatabase.GetAssetPath(_selectedTouchBase._touchBase))
                {
                    string result = "e";
                    _selectedTouchBase.name = _selectedTouchBase.name.Trim();
                    if (_selectedTouchBase.name.Length != 0)
                    {
                        result = AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_selectedTouchBase._touchBase), _selectedTouchBase.name);
                        AssetDatabase.Refresh();
                    }

                    if (result != "")
                    {
                        EditorUtility.DisplayDialog("Error", "The joystick must have a name that is unique and contain atleast 1 character.", "Ok");
                        _selectedTouchBase.name = _selectedTouchBase._touchBase.name;
                    }
                    else
                    {
                        _selectedTouchBase._touchBase.gameObject.name = _selectedTouchBase.name;
                        EditorUtility.SetDirty(_selectedTouchBase._touchBase);
                    }
                }
			
                RefreshListOfInputs();
			
                LooseFocus();
            }
		
            TouchZone z = _selectedTouchBase._touchBase as TouchZone;
            if(z != null)
                GUI_EditPanel_Zone(z);
		
            TouchJoystick j = _selectedTouchBase._touchBase as TouchJoystick;
            if(j != null)
            {
                GUI_EditPanel_Joystick(j);
                GUI_EditPanel_GUIBase(j);
            }
		
            TouchButton b = _selectedTouchBase._touchBase as TouchButton;
            if(b != null)
            {
                GUI_EditPanel_Button(b);
                GUI_EditPanel_GUIBase(b);
            }

        }
	
        protected void GUI_EditPanel_Zone(TouchZone z)
        {
            if(z == null)
                return;
		
            GUILayout.Label("Layers to test against: ",EditorStyles.boldLabel);
		
            LayerMask newMask = z.layersToTouch;
		
            bool prevStatus = z.layersToTouch == 0;
		
            GUILayout.BeginHorizontal();
            bool currStatus = EditorGUILayout.Toggle("Nothing",prevStatus,EditorStyles.toggle);
            GUILayout.EndHorizontal();
		
            if(prevStatus != currStatus && prevStatus == false)
            {
                newMask = 0;
            }
		
            prevStatus = z.layersToTouch == -1;
            GUILayout.BeginHorizontal();
            currStatus = EditorGUILayout.Toggle("Everything",prevStatus,EditorStyles.toggle);
            GUILayout.EndHorizontal();
            if(prevStatus != currStatus && prevStatus == false)
            {
                newMask = -1;
            }
		
		
            for (int i=0;i<32;i++) {
	 
                string layerName = LayerMask.LayerToName (i);
	 	
                if (layerName != "") {
                    prevStatus = (z.layersToTouch == (z.layersToTouch | (1 << i)) || (z.layersToTouch == -1));
                    GUILayout.BeginHorizontal();
                    currStatus = EditorGUILayout.Toggle(layerName,prevStatus,EditorStyles.toggle);
                    GUILayout.EndHorizontal();
                    if(prevStatus != currStatus)
                    {
                        if(prevStatus == false)
                        {
                            newMask = newMask | (1 << i);
                        }
                        else
                        {
                            if(z.layersToTouch == -1)
                            {
                                newMask = 1 << i;
                                newMask = ~newMask;
                            }
                            else
                            {
                                newMask = newMask & ~(1 << i);;
                            }
                        }
                    }
                }
            }
		
            if(newMask != z.layersToTouch)
            {
                z.layersToTouch = newMask;
                EditorUtility.SetDirty(z);
            }
		
        }
	
        protected void GUI_EditPanel_Button(TouchButton b)
        {
		
        }
	
        protected void GUI_EditPanel_Joystick(TouchJoystick j)
        {
            GUILayout.Label("Joystick:",EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Lock X: ",EditorStyles.miniLabel);
            bool oldLockX = j.disableX;
            oldLockX = EditorGUILayout.Toggle(oldLockX);
            if(oldLockX != j.disableX)
            {
                j.disableX = oldLockX;
                EditorUtility.SetDirty(j);
            }
		
            GUILayout.Label("Lock Y: ",EditorStyles.miniLabel);
            bool oldLockY = j.disableY;
            oldLockY = EditorGUILayout.Toggle(oldLockY);
            if(oldLockY != j.disableY)
            {
                j.disableY = oldLockY;
                EditorUtility.SetDirty(j);
            }
            GUILayout.EndHorizontal();
			
        }
	
        protected void GUI_EditPanel_GUIBase(TouchGUIBase t)
        {
            if(t == null)
                return;
		
		
            bool hasAnimation = t.GetComponent<TouchAnimation>() != null;

            if (!hasAnimation)
            {

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Base ", EditorStyles.boldLabel);

                    _selectedTouchBase.baseImg =
                        EditorGUILayout.ObjectField(_selectedTouchBase.baseImg, typeof (Texture), false,
                                                    GUILayout.Height(currentSelectedTouchBaseSettings._width/2)) as Texture;
                    if (_selectedTouchBase.baseImg != t.basePart.texture)
                    {
                        t.basePart.texture = _selectedTouchBase.baseImg;
                        /*t.basePart.pixelInset = new Rect(t.basePart.texture.width/-2, t.basePart.texture.height/-2,
                                                     t.basePart.texture.width, t.basePart.texture.height);*/
                        EditorUtility.SetDirty(t.basePart);
                    }


                    GUILayout.Label("Layer", EditorStyles.miniLabel);
                    int newLayer = EditorGUILayout.IntField(t.basePartLayer);
                    if (newLayer != t.basePartLayer)
                    {
                        t.basePartLayer = newLayer;
                        EditorUtility.SetDirty(t);
                    }

                    GUILayout.Label("Color", EditorStyles.miniLabel);
                    Color newColor = EditorGUILayout.ColorField(t.basePart.color);
                    if (newColor != t.basePart.color)
                    {
                        t.basePart.color = newColor;
                        EditorUtility.SetDirty(t.basePart.gameObject);
                    }

                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Top ", EditorStyles.boldLabel);
                    _selectedTouchBase.topImg =
                        EditorGUILayout.ObjectField(_selectedTouchBase.topImg, typeof (Texture), false,
                                                    GUILayout.Height(currentSelectedTouchBaseSettings._width/2)) as Texture;
                    if (_selectedTouchBase.topImg != t.topPart.texture)
                    {
                        t.topPart.texture = _selectedTouchBase.topImg;
                        /*t.topPart.pixelInset = new Rect(t.topPart.texture.width/-2, t.topPart.texture.height/-2,
                                                    t.topPart.texture.width, t.topPart.texture.height);*/
                        EditorUtility.SetDirty(t.topPart);
                    }


                    GUILayout.Label("Layer", EditorStyles.miniLabel);
                    int newLayer = EditorGUILayout.IntField(t.topPartLayer);
                    if (newLayer != t.topPartLayer)
                    {
                        t.topPartLayer = newLayer;
                        EditorUtility.SetDirty(t);
                    }


                    GUILayout.Label("Color", EditorStyles.miniLabel);
                    Color newColor = EditorGUILayout.ColorField(t.topPart.color);
                    if (newColor != t.topPart.color)
                    {
                        t.topPart.color = newColor;
                        EditorUtility.SetDirty(t.topPart.gameObject);
                    }


                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

            
                GUILayout.Label("Size on screen: ", EditorStyles.boldLabel);
                float newScale = EditorGUILayout.Slider(t.scale, 0f, 1f);
                if (newScale != t.scale)
                {
                    t.scale = newScale;
                    EditorUtility.SetDirty(t);
                }
            }

            GUILayout.Label("Recenter on Release: ", EditorStyles.boldLabel);
            TouchGUIBase.RecenterMethod newR =
                (TouchGUIBase.RecenterMethod)EditorGUILayout.EnumPopup(t.recenterBaseOnRelease);
            if (newR != t.recenterBaseOnRelease)
            {
                t.recenterBaseOnRelease = newR;
                if (t.recenterBaseOnRelease != TouchGUIBase.RecenterMethod.Smooth)
                    t.recenterBaseMethodValue = 0;

                EditorUtility.SetDirty(t);
            }
            if (t.recenterBaseOnRelease == TouchGUIBase.RecenterMethod.Smooth)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Speed: ", EditorStyles.miniLabel);
                float oldSpeed = EditorGUILayout.Slider(t.recenterBaseMethodValue, 0f, 1f);
                GUILayout.EndHorizontal();
                if (oldSpeed != t.recenterBaseMethodValue)
                {
                    t.recenterBaseMethodValue = oldSpeed;
                    EditorUtility.SetDirty(t);
                }
            }



            GUILayout.Label("Animation: ",EditorStyles.boldLabel);
            if(!hasAnimation)
            {
                GUILayout.Label("No Animation component.",EditorStyles.miniLabel);
                if(GUILayout.Button("Add Animation component",EditorStyles.miniButton))
                {
                    TouchAnimation ta = t.gameObject.AddComponent<TouchAnimation>();
                    ta.initialPreset = "Default";
                    ta.allPresets.Add(Aimation_GenerateDefaultAnimPreset("Default",t));
                    _gamePreview.SetPreset(ta.allPresets.First(a => a.presetID == ta.initialPreset));
				
                    EditorUtility.SetDirty(t.gameObject);
                }
            }
            else
            {
                TouchAnimation ta = t.gameObject.GetComponent<TouchAnimation>();
			
                if(ta.allPresets.GroupBy(a => a.presetID).Any( a => a.Count() > 1))
                {
                    GUILayout.Space(5);
                    EditorGUILayout.HelpBox("Presets must have unique names.", MessageType.Error);
                    GUILayout.Space(5);
                }
                /*else
				EditorGUILayout.HelpBox("Animations will ignore size and texture settings above.",MessageType.Info);*/
			
                GUILayout.BeginHorizontal();
                GUILayout.Label("Initial Preset: ",EditorStyles.miniLabel);
			
                int currentIndex = ta.allPresets.IndexOf (ta.allPresets.First(a => a.presetID == ta.initialPreset));
                string[] popUpVals = new string[ta.allPresets.Count];
                int ii = 0;
                ta.allPresets.ForEach(a => {
                                               popUpVals[ii] = a.presetID; ii++;
                });
			
                int newIndex = EditorGUILayout.Popup(currentIndex,popUpVals);
	
                if(newIndex != currentIndex)
                {
                    ta.initialPreset  = popUpVals[newIndex];
                    EditorUtility.SetDirty(ta);
                }
			
                GUILayout.EndHorizontal();
			
			
                for (int i = 0; i < ta.allPresets.Count; i++)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent( (ta.allPresets[i].presetID), _animationIcon), EditorStyles.label);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
	            
                    if (GUILayout.Button("Animate", EditorStyles.miniButtonLeft))
                    {
                        _selectedTouchBase._touchAnimationActivePreset = ta.allPresets[i];
					
                        if(_selectedTouchBase.selectedMiscPanel == null)
                        {
                            _selectedTouchBase.selectedMiscPanel = new TouchInputManagerEditor_GUISidebar(340,500,_styleDarkWithBorderBG,GUI_EditPanel_Animation,null,Repaint);
                            _selectedTouchBase.selectedMiscPanel.Expand();
                        }
                        else
                        {
                            float w = _selectedTouchBase.selectedMiscPanel._width;
                            _selectedTouchBase.selectedMiscPanel = new TouchInputManagerEditor_GUISidebar(340,500,_styleDarkWithBorderBG,GUI_EditPanel_Animation,null,Repaint);
                            _selectedTouchBase.selectedMiscPanel._width = w;
                        }	
					
                        _gamePreview.SetPreset(ta.allPresets[i]);
                    }
				
                    if (GUILayout.Button("Delete", EditorStyles.miniButtonRight))
                    {
                        if(ta.allPresets.Count == 1)
                        {
                            EditorUtility.DisplayDialog("Last Preset","You must have atleast one preset.","Ok");
                        }
                        else
                        {
						
                            if(EditorUtility.DisplayDialog("Are you sure?","You are about to delete preset '"+ta.allPresets[i].presetID +"'.","Ok","Cancel"))
                            {
                                if(_selectedTouchBase._touchAnimationActivePreset != null && _selectedTouchBase._touchAnimationActivePreset.presetID == ta.allPresets[i].presetID)
                                {
                                    _selectedTouchBase._touchAnimationActivePreset = null;
                                }
							
                                if(ta.allPresets[i].presetID == ta.initialPreset)
                                {
                                    ta.initialPreset = (i == 0) ? ta.allPresets[1].presetID : ta.allPresets[0].presetID;
                                }
							
                                if(ta.allPresets[i].presetID == _gamePreview.ActivePreset())
                                    _gamePreview.SetPreset(ta.allPresets.First(a => a.presetID == ta.initialPreset));
								
                                LooseFocus(); 
                                ta.allPresets.RemoveAt(i);
                                i--;
                                EditorUtility.SetDirty(ta);
                            }
                        }
                    }
	
                    GUILayout.EndHorizontal();
                }

			
                GUILayout.Space(5);
			
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Preset", EditorStyles.miniButton))
                {
                    string toAdd = "NewAnim_1";
                    int counter = 1;
				
                    while(ta.allPresets.Any(a => a.presetID == toAdd))
                    {
                        counter++;
                        toAdd = "NewAnim_"+counter;
                    }
				
                    ta.allPresets.Add(Aimation_GenerateDefaultAnimPreset(toAdd,t));
				
                    EditorUtility.SetDirty(t);
                    LooseFocus();
                }
			
                GUILayout.EndHorizontal();
			
                GUILayout.Space(15);
                if(GUILayout.Button("Remove animation component",EditorStyles.miniButton))
                {
                    if(EditorUtility.DisplayDialog("Are you sure?","This will destroy all the animations.","Ok","Cancel"))
                    {
                        _selectedTouchBase._touchAnimationActivePreset = null;
                        _gamePreview.ClearPreset();
					
                        Component.DestroyImmediate(t.gameObject.GetComponent<TouchAnimation>(),true);
                        EditorUtility.SetDirty(t.gameObject);
                    }
                }
            }
		
        }
	
        private void GUI_EditPanel_Animation()
        {
            TouchAnimationPreset ps = _selectedTouchBase._touchAnimationActivePreset;
            if(ps == null)
            {
                _selectedTouchBase.selectedMiscPanel = null;
                return;
            }
            bool dirty = false;
            bool closed = false;

            GUILayout.BeginHorizontal();

            GUILayout.Label("Animation Preset: ", EditorStyles.boldLabel);

            if(GUILayout.Button("Close", EditorStyles.miniButton))
            {
                closed = true;
            }

            GUILayout.EndHorizontal();

            string oldPresetID = ps.presetID;

            ps.presetID = EditorGUILayout.TextField(ps.presetID, EditorStyles.textField);


            if (oldPresetID != ps.presetID)
            {
                TouchAnimation ta = _selectedTouchBase._touchBase.GetComponent<TouchAnimation>();
                if (ta.initialPreset == oldPresetID)
                    ta.initialPreset = ps.presetID;

                dirty = true;
            }
		
		
            GUILayout.BeginHorizontal();
		
            GUILayout.BeginVertical(_styleJustRightBorderBG);
            GUI_EditPanel_Animation_DoAnimationInfoLabels();
            GUILayout.EndVertical();
		
				
            GUILayout.BeginVertical(_styleJustRightBorderBG);
            GUILayout.Label("Base (Up)",EditorStyles.boldLabel);
            if(GUI_EditPanel_Animation_DoAnimationInfoEditor(ps.unlatchedAnimationBase))
                dirty = true;
            GUILayout.EndVertical();
		
            GUILayout.BeginVertical();
            GUILayout.Label("Base (Down)",EditorStyles.boldLabel);
            if(GUI_EditPanel_Animation_DoAnimationInfoEditor(ps.latchedAnimationBase))
                dirty = true;
            GUILayout.EndVertical();

		
            GUILayout.EndHorizontal();
				
            GUILayout.Space(20);
		
            GUILayout.BeginHorizontal();
		
            GUILayout.BeginVertical(_styleJustRightBorderBG);
            GUI_EditPanel_Animation_DoAnimationInfoLabels();
            GUILayout.EndVertical();
		
				
            GUILayout.BeginVertical(_styleJustRightBorderBG);
            GUILayout.Label("Top (Up)",EditorStyles.boldLabel);
            if(GUI_EditPanel_Animation_DoAnimationInfoEditor(ps.unlatchedAnimationTop))
                dirty = true;
            GUILayout.EndVertical();
		
            GUILayout.BeginVertical();
            GUILayout.Label("Top (Down)",EditorStyles.boldLabel);
            if(GUI_EditPanel_Animation_DoAnimationInfoEditor(ps.latchedAnimationTop))
                dirty = true;
            GUILayout.EndVertical();

		
            GUILayout.EndHorizontal();
		
            if(dirty)
            {
                EditorUtility.SetDirty(_selectedTouchBase._touchBase.GetComponent<TouchAnimation>());
                _gamePreview.SetPreset(ps);
            }

            if(closed)
            {
                _selectedTouchBase._touchAnimationActivePreset = null;
            }
				
        }
        private void GUI_EditPanel_Animation_DoAnimationInfoLabels()
        {
            GUILayout.Space(23);
            GUILayout.Label("Texture: ",EditorStyles.boldLabel);
            GUILayout.Space((_selectedTouchBase.selectedMiscPanel._width/3) - 14);
            GUILayout.Label("Animate Size: ",EditorStyles.boldLabel);
            GUILayout.Label("Animate Color: ",EditorStyles.boldLabel);
            GUILayout.Label("Transition: ",EditorStyles.boldLabel);
            GUILayout.Space(20);
        }
        private bool GUI_EditPanel_Animation_DoAnimationInfoEditor(TouchAnimationInformation t)
        {
            bool dirty = false;
            Texture oldTex = t.changeToTexture;
            oldTex = EditorGUILayout.ObjectField(oldTex,typeof(Texture),false,GUILayout.Height(_selectedTouchBase.selectedMiscPanel._width/3)) as Texture;
            if(oldTex != t.changeToTexture)
            {
                t.changeToTexture = oldTex;
                dirty = true;
            }
		
            GUILayout.BeginHorizontal();
			
            
            float oldSize = Mathf.Clamp01( EditorGUILayout.FloatField(t.changeToSize));
            if(oldSize != t.changeToSize)
            {
                t.changeToSize = oldSize;
                dirty = true;
            }
			
            GUILayout.EndHorizontal();
		
            GUILayout.BeginHorizontal();
			
            
            Color oldColor = EditorGUILayout.ColorField(t.transitionColor);
            if(oldColor != t.transitionColor)
            {
                t.transitionColor = oldColor;
                dirty = true;
            }
			
            GUILayout.EndHorizontal();
		
            GUILayout.Space(5);
		
		
            TouchAnimationInformation.TransitionMethod newR = (TouchAnimationInformation.TransitionMethod)EditorGUILayout.EnumPopup(t.transitionMethod);
            if(newR != t.transitionMethod)
            {
                t.transitionMethod = newR;
                if(t.transitionMethod != TouchAnimationInformation.TransitionMethod.Smooth)
                    t.transitionMethodValue = 0;
			 
                dirty = true;
            }
            GUI.enabled =t.transitionMethod == TouchAnimationInformation.TransitionMethod.Smooth;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed: ",EditorStyles.miniLabel);
            float oldSpeed = Mathf.Clamp01(EditorGUILayout.FloatField(t.transitionMethodValue));
            GUILayout.EndHorizontal();
            if(oldSpeed != t.transitionMethodValue)
            {
                t.transitionMethodValue = oldSpeed;
                dirty = true;
            }
            GUI.enabled  = true;
		
		
            return dirty;
        }
        private TouchAnimationPreset Aimation_GenerateDefaultAnimPreset(string presetName, TouchGUIBase t)
        {
            TouchAnimationPreset newAnimPreset = new TouchAnimationPreset{ presetID = presetName };
            newAnimPreset.latchedAnimationTop = new TouchAnimationInformation(t.topPart);
            newAnimPreset.unlatchedAnimationTop = new TouchAnimationInformation(t.topPart);
            newAnimPreset.latchedAnimationBase = new TouchAnimationInformation(t.basePart);
            newAnimPreset.unlatchedAnimationBase = new TouchAnimationInformation(t.basePart);

            return newAnimPreset;
        }
	
	
        private void GenericMenuCreateAll()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Joystick"), false, NewJoystick); 
            menu.AddItem(new GUIContent("Button"), false, NewButton); 
            menu.AddItem(new GUIContent("Zone"), false, NewZone); 

            menu.ShowAsContext();
            Event.current.Use();
        }
	
        private void GenericMenuCreateDelete(bool showJoystick, bool showButton, bool showZone)
        {
            GenericMenu menu = new GenericMenu();

            if(showJoystick) menu.AddItem(new GUIContent("New Joystick"), false, NewJoystick); 
            if(showButton) menu.AddItem(new GUIContent("New Button"), false, NewButton); 
            if(showZone) menu.AddItem(new GUIContent("New Zone"), false, NewZone); 
            menu.AddItem(new GUIContent("Duplicate"), false, DuplicateSelected); 
            menu.AddItem(new GUIContent("Delete"), false, DeleteSelected); 

            menu.ShowAsContext();
            Event.current.Use();
        }
	
        private void NewJoystick()
        {
            AssetDatabase.CopyAsset("Assets/TouchInputManager/Prefabs/Joysticks/Template/Joystick.prefab",AssetDatabase.GenerateUniqueAssetPath("Assets/TouchInputManager/Prefabs/Joysticks/Joystick.prefab"));
            AssetDatabase.Refresh();
		
            RefreshListOfInputs();
        }
	
        private void NewButton()
        {
            AssetDatabase.CopyAsset("Assets/TouchInputManager/Prefabs/Buttons/Template/Button.prefab",AssetDatabase.GenerateUniqueAssetPath("Assets/TouchInputManager/Prefabs/Buttons/Button.prefab"));
            AssetDatabase.Refresh();
		
            RefreshListOfInputs();
        }
	
        private void NewZone()
        {
            AssetDatabase.CopyAsset("Assets/TouchInputManager/Prefabs/Zones/Template/Zone.prefab",AssetDatabase.GenerateUniqueAssetPath("Assets/TouchInputManager/Prefabs/Zones/Zone.prefab"));
            AssetDatabase.Refresh();
		
            RefreshListOfInputs();
        }
	
        private void DuplicateSelected()
        {
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(_selectedTouchBase._touchBase),AssetDatabase.GenerateUniqueAssetPath(AssetDatabase.GetAssetPath(_selectedTouchBase._touchBase)));
            AssetDatabase.Refresh();
		
            RefreshListOfInputs();
        }

        private void ClearSelection()
        {
            _selectedTouchBase.Select(null);
            _gamePreview.ClearPreset();
        }

        private void DeleteSelected()
        {
            if(!EditorUtility.DisplayDialog("Are you sure?","This cannot be undone!","Yes","No"))
                return;
		
            if(_selectedTouchBase != null)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_selectedTouchBase._touchBase));
                ClearSelection();
                RefreshListOfInputs();
            }
        }
	
        private void RefreshListOfInputs()
        {
            inputs.Clear();
		
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/TouchInputManager/Prefabs/Joysticks");
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info) 
            {
                TouchBase tb = AssetDatabase.LoadAssetAtPath("Assets/TouchInputManager/Prefabs/Joysticks/"+f.Name,typeof(TouchBase)) as TouchBase;
                if(tb != null)
                {
                    inputs.Add(tb);
                }
            }
		
            dir = new DirectoryInfo(Application.dataPath + "/TouchInputManager/Prefabs/Buttons");
            info = dir.GetFiles("*.*");
            foreach (FileInfo f in info) 
            {
                TouchBase tb = AssetDatabase.LoadAssetAtPath("Assets/TouchInputManager/Prefabs/Buttons/"+f.Name,typeof(TouchBase)) as TouchBase;
                if(tb != null)
                {
                    inputs.Add(tb);
                }
            }
		
            dir = new DirectoryInfo(Application.dataPath + "/TouchInputManager/Prefabs/Zones");
            info = dir.GetFiles("*.*");
            foreach (FileInfo f in info) 
            {
                TouchBase tb = AssetDatabase.LoadAssetAtPath("Assets/TouchInputManager/Prefabs/Zones/"+f.Name,typeof(TouchBase)) as TouchBase;
                if(tb != null)
                {
                    inputs.Add(tb);
                }
            }
		
            inputs.OrderBy(a => a is TouchJoystick).OrderBy(a => a is TouchButton).OrderBy(a => a is TouchZone);
        }
	
    }
}