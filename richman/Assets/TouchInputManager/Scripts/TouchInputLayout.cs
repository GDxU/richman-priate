using System;
using UnityEngine;
using System.Collections.Generic;

namespace TouchInputManagerBackend
{
    [AddComponentMenu("")]
    public class TouchInputLayout : MonoBehaviour {
	
        [System.Serializable]
        public class TrackerPrefabBasePrefabPair
        {
            public TouchTracker _tracker;
            public TouchBase _base;
        }
	
        public string touchLayoutID;
        public List<TrackerPrefabBasePrefabPair> trackerPrefabBasePrefab = new List<TrackerPrefabBasePrefabPair>();
	
        private Dictionary<string, TouchJoystick> r_allJoysticks = new Dictionary<string, TouchJoystick>();
        private Dictionary<string, TouchButton> r_allButtons = new Dictionary<string, TouchButton>();
        private Dictionary<string, TouchZone> r_allZones = new Dictionary<string, TouchZone>();

        private List<TouchGUITexture> r_allTextures = new List<TouchGUITexture>();
        private bool _isRendering = true;
        public bool IsRendering { get { return _isRendering; }}
        private bool _isReceivingInput = true;
        public bool IsReceivingInput { get { return _isReceivingInput; } }

        public Vector2 GetJoystick(string joystickID)
        {
            return GetJoystick(FindJoystickByID(joystickID, false), true);
        }

        public Vector2 GetJoystick(string joystickID, bool normalized)
        {
            return GetJoystick(FindJoystickByID(joystickID, false), normalized);
        }

        public Vector2 GetJoystick(TouchJoystick tjs)
        {
            return GetJoystick(tjs, true);
        }

        public Vector2 GetJoystick(TouchJoystick tjs, bool normalized)
        {
            if (tjs == null)
                return Vector2.zero;

            if (normalized)
            {
                return tjs.GetInputNormalized();
            }
            else
            {
                return tjs.GetInput();
            }
        }

        public bool GetButton(string buttonID)
        {
            return GetButton(FindButtonByID(buttonID, false));
        }

        public bool GetButton(TouchButton tb)
        {
            if (tb == null)
                return false;

            return tb.GetButton();
        }

        public bool GetButtonDown(string buttonID)
        {
            return GetButtonDown(FindButtonByID(buttonID, false));
        }

        public bool GetButtonDown(TouchButton tb)
        {
            if (tb == null)
                return false;

            return tb.GetButtonDown();
        }

        public bool GetButtonUp(string buttonID)
        {
            return GetButtonUp(FindButtonByID(buttonID, false));
        }

        public bool GetButtonUp(TouchButton tb)
        {
            if (tb == null)
                return false;

            return tb.GetButtonUp();
        }

        public bool AddTouch(int t)
        {
            if (!_isReceivingInput)
                return false;

            foreach(TouchJoystick tjs in r_allJoysticks.Values)
            {
                if(tjs.tracker.IsWithinRegion(t) && tjs.tracker.CanLatchTouch())
                {
                    tjs.tracker.LatchFinger(t);
                    return true;
                }
            }

            foreach (TouchButton tb in r_allButtons.Values)
            {
                if (tb.tracker.IsWithinRegion(t) && tb.tracker.CanLatchTouch())
                {
                    tb.tracker.LatchFinger(t);
                    return true;
                }
            }

            foreach (TouchZone tz in r_allZones.Values)
            {
                if (tz.tracker.IsWithinRegion(t) && tz.tracker.CanLatchTouch())
                {
                    tz.tracker.LatchFinger(t);
                    return true;
                }
            }
		
            return false;
        }
	
        public bool RemoveTouch(int t)
        {
            foreach(TouchJoystick tjs in r_allJoysticks.Values)
            {
                if(tjs.tracker.IsLatchedTo(t))
                {
                    tjs.tracker.UnlatchFinger(t);
                    return true;
                }
            }
		
            foreach(TouchButton tb in r_allButtons.Values)
            {
                if(tb.tracker.IsLatchedTo(t))
                {
                    tb.tracker.UnlatchFinger(t);
                    return true;
                }
            }

            foreach (TouchZone tz in r_allZones.Values)
            {
                if (tz.tracker.IsLatchedTo(t))
                {
                    tz.tracker.UnlatchFinger(t);
                    return true;
                }
            }

            return false;
        }
	
        public void RemoveAndUnlatchAll()
        {
            foreach(TouchJoystick tjs in r_allJoysticks.Values)
            {
                tjs.tracker.ResetTouch();
            }

            foreach (TouchButton tb in r_allButtons.Values)
            {
                tb.tracker.ResetTouch();
            }

            foreach (TouchZone tz in r_allZones.Values)
            {
                tz.tracker.ResetTouch();
            }
        }
	
        public void ResetPress()
        {
            foreach(TouchButton tb in r_allButtons.Values)
            {
                tb.ResetPressed();
            }
        }

        public void ShouldRender(bool render)
        {
            //teeheee
            OnToggle(render, ref _isRendering, () =>
                                                   {
                                                       foreach (TouchGUITexture texture in r_allTextures)
                                                       {
                                                           texture.enabled = IsRendering;
                                                       }
                                                   });
        }

        public void ShouldReceiveInput(bool receiveInput)
        {
            OnToggle(receiveInput,ref _isReceivingInput, RemoveAndUnlatchAll);
        }

        public void ChangeAnimationPreset(string presetID, string inputID)
        {
            if (r_allButtons.ContainsKey(inputID))
            {
                TouchButton b = r_allButtons[inputID];
                TouchAnimation bAnim = b.GetComponent<TouchAnimation>();
                if (bAnim != null)
                {
                    bAnim.SwapToPreset(presetID);
                }
                else
                {
                    Debug.LogError("Button ID '" + inputID + "' does not have animations, but you are trying to load an animation preset.");
                }
            }
            else if (r_allJoysticks.ContainsKey(inputID))
            {
                TouchJoystick j = r_allJoysticks[inputID];
                TouchAnimation jAnim = j.GetComponent<TouchAnimation>();
                if (jAnim != null)
                {
                    jAnim.SwapToPreset(presetID);
                }
                else
                {
                    Debug.LogError("Joystick ID '" + inputID + "' does not have animations, but you are trying to load an animation preset.");
                }
            }
            else
            {
                Debug.LogError("Layout ID '" + touchLayoutID + "' does not contain an input device with ID '" + inputID);
            }
        }

        public string GetCurrentAnimationPreset(string inputID)
        {
            if (r_allButtons.ContainsKey(inputID))
            {
                TouchButton b = r_allButtons[inputID];
                TouchAnimation bAnim = b.GetComponent<TouchAnimation>();
                if (bAnim != null)
                {
                    return bAnim.CurrentPreset();
                }
                else
                {
                    Debug.LogError("Button ID '" + inputID + "' does not have animations, but you are trying to load an animation preset.");
                    return "";
                }
            }
            else if (r_allJoysticks.ContainsKey(inputID))
            {
                TouchJoystick j = r_allJoysticks[inputID];
                TouchAnimation jAnim = j.GetComponent<TouchAnimation>();
                if (jAnim != null)
                {
                    return jAnim.CurrentPreset();
                }
                else
                {
                    Debug.LogError("Joystick ID '" + inputID + "' does not have animations, but you are trying to load an animation preset.");
                    return "";
                }
            }
            else
            {
                Debug.LogError("Layout ID '" + touchLayoutID + "' does not contain an input device with ID '" + inputID);
                return "";
            }
        }
        
        
        private void OnToggle(bool toggle, ref bool testAgainst, Action toDo)
        {
            if (toggle == testAgainst)
                return;

            testAgainst = toggle;
            toDo();
        }

        void Awake()
        {
            name = name.Replace("(Clone)", "");
		
            foreach(TrackerPrefabBasePrefabPair tpbp in trackerPrefabBasePrefab)
            {
                TouchTracker tt = tpbp._tracker;
			
                TouchBase tb = Instantiate(tpbp._base) as TouchBase;
                tb.tracker = tt;
                tb.Initialize();
			
                tb.transform.parent = transform;
                tt.transform.parent = tb.transform;
			
                if(tb is TouchJoystick)
                {
                    if(FindJoystickByID(tt._id,true) == null)
                    {
                        r_allJoysticks.Add(tt._id,tb as TouchJoystick);
                    }
                    else
                        Debug.LogError("Joystick ID '" + tt._id + "' is not unique in layout '" + name + "' and so will be ignored.");
                }
                else if(tb is TouchButton)
                {
                    if(FindButtonByID(tt._id,true) == null)
                    {
                        r_allButtons.Add(tt._id,tb as TouchButton);
                    }
                    else
                        Debug.LogError("Button ID '" + tt._id + "' is not unique in layout '" + name + "' and so will be ignored.");
                }
                else if(tb is TouchZone)
                {
                    if(FindZoneByID(tt._id,true) == null)
                    {
                        r_allZones.Add(tt._id,tb as TouchZone);
                    }
                    else
                        Debug.LogError("TouchZone ID '" + tt._id + "' is not unique in layout '" + name + "' and so will be ignored.");
                }
            }
		
            r_allTextures.AddRange(GetComponentsInChildren<TouchGUITexture>());
            ShouldRender(false);
            ShouldReceiveInput(false);
        }

        private TouchJoystick FindJoystickByID(string ID, bool suppressWarnings)
        {
            if(r_allJoysticks.ContainsKey(ID))
                return r_allJoysticks[ID];
            else
            {
                if(!suppressWarnings)
                    Debug.LogWarning("Joystick doesn't exist: " + ID);
                return null;
            }
			
        }

        private TouchButton FindButtonByID(string ID, bool suppressWarnings)
        {
            if (r_allButtons.ContainsKey(ID))
                return r_allButtons[ID];
            else
            {
                if (!suppressWarnings)
                    Debug.LogWarning("Button doesn't exist: " + ID);
                return null;
            }
        }

        private TouchZone FindZoneByID(string ID, bool suppressWarnings)
        {
            if (r_allZones.ContainsKey(ID))
                return r_allZones[ID];
            else
            {
                if (!suppressWarnings)
                    Debug.LogWarning("Button doesn't exist: " + ID);
                return null;
            }
        }
    }
}