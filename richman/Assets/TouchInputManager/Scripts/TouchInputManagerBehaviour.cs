using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TouchInputManagerBackend
{
    [AddComponentMenu("")]
    public class TouchInputManagerBehaviour : MonoBehaviour
    {
	
        public List<TouchInputLayout> prefabTouchInputLayouts;
        private List<int> r_allTouches;
        private Dictionary<string, TouchInputLayout> r_allTouchInputLayout;	
	
        private int trackedTouchCount {
            get
            {
                return r_allTouches.Count;
            }
        }
	
        public void Load()
        {
			name = name.Replace("(Clone)", "");

			gameObject.hideFlags = HideFlags.HideInHierarchy;

            Input.multiTouchEnabled = true;
            r_allTouchInputLayout = new Dictionary<string, TouchInputLayout>();
            r_allTouches = new List<int>();

            foreach(TouchInputLayout til in prefabTouchInputLayouts)
            {
                if(r_allTouchInputLayout.ContainsKey(til.touchLayoutID) == false)
                {
                    GameObject instantiatedObj = Instantiate(til.gameObject) as GameObject;
                    TouchInputLayout tilInstantiated = instantiatedObj.GetComponent<TouchInputLayout>();
                    r_allTouchInputLayout.Add(til.touchLayoutID,tilInstantiated);
                    r_allTouchInputLayout[til.touchLayoutID].transform.parent = transform;
				
                }
                else
                    Debug.LogError("Layout ID is not unique: " + til.touchLayoutID);
            }
        }

        public void RenderLayoutAll(bool render)
        {
            foreach (TouchInputLayout til in r_allTouchInputLayout.Values)
            {
                til.ShouldRender(render);
            }
        }

        public void PassInputToLayoutAll(bool passInput)
        {
            foreach (TouchInputLayout til in r_allTouchInputLayout.Values)
            {
                til.ShouldReceiveInput(passInput);
            }
        }

        public void RenderLayout(LayoutID ID, bool render)
        {
            RenderLayout(render, FindLayoutByID(ID.ToString()));
        }

        private void RenderLayout(bool render, TouchInputLayout til)
        {
            if (til != null)
            {
                til.ShouldRender(render);
                return;
            }
        }

        public void PassInputToLayout(LayoutID ID, bool passInput)
        {
            PassInputToLayout(passInput, FindLayoutByID(ID.ToString()));
        }

        private void PassInputToLayout(bool passInput, TouchInputLayout til)
        {
            if (til != null)
            {
                til.ShouldReceiveInput(passInput);
                return;
            }
        }

        public void ChangeAnimationPreset(string presetID, InputID inputID, LayoutID layoutID)
        {
            TouchInputLayout til = FindLayoutByID(layoutID.ToString());
            til.ChangeAnimationPreset(presetID, inputID.ToString());
        }

        public string GetCurrentAnimationPreset(InputID inputID, LayoutID layoutID)
        {
            TouchInputLayout til = FindLayoutByID(layoutID.ToString());
            return til.GetCurrentAnimationPreset(inputID.ToString());
        }

        public Vector2 GetJoystick(InputID joystickID, LayoutID layoutID)
        {
            return GetJoystick(joystickID, layoutID, false);
        }

        public Vector2 GetJoystick(InputID joystickID, LayoutID layoutID, bool normalized)
        {
            TouchInputLayout til = FindLayoutByID(layoutID.ToString());
            return til.GetJoystick(joystickID.ToString(), normalized);
        }

        public bool GetButton(InputID buttonID, LayoutID layoutID)
        {
            TouchInputLayout til = FindLayoutByID(layoutID.ToString());
            return til.GetButton(buttonID.ToString());
        }

        public bool GetButtonDown(InputID buttonID, LayoutID layoutID)
        {
            TouchInputLayout til = FindLayoutByID(layoutID.ToString());
            return til.GetButtonDown(buttonID.ToString());
        }

        public bool GetButtonUp(InputID buttonID, LayoutID layoutID)
        {
            TouchInputLayout til = FindLayoutByID(layoutID.ToString());
            return til.GetButtonUp(buttonID.ToString());
        }
	
        void Update()
        {
            foreach(TouchInputLayout til in r_allTouchInputLayout.Values)
            {
                til.ResetPress();
            }

#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (!r_allTouches.Contains(0))
                {
                    AddTouch(0);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                RemoveTouch(0);
            }


            if (Input.GetMouseButtonDown(1))
            {
                if (!r_allTouches.Contains(1))
                {
                    AddTouch(1);
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                RemoveTouch(1);
            }

            int touchCount = Input.GetMouseButton(0) ? (Input.GetMouseButton(1) ? 2 : 1) : (Input.GetMouseButton(1) ? 1 : 0);
            if (touchCount != trackedTouchCount)
            {
                RemoveAndUnlatchAll();
            }
#else
            foreach (Touch t in Input.touches)
		{
			if(t.phase == TouchPhase.Began)
			{
				if(!r_allTouches.Contains(t.fingerId))
				{
					AddTouch(t.fingerId);
				}
			}
		}

        if (Input.touchCount != trackedTouchCount)
		{
			RemoveAndUnlatchAll();
		}

        for (int i = 0; i < trackedTouchCount; i++)
		{                
		   if (Input.GetTouch(r_allTouches[i]).phase == TouchPhase.Ended || Input.GetTouch(r_allTouches[i]).phase == TouchPhase.Canceled)
		   {
		       RemoveTouch(r_allTouches[i]);
		       i--;
		   }
		}
#endif

        }
	
        private void AddTouch(int t)
        {
            r_allTouches.Add(t);
		
            foreach(TouchInputLayout til in r_allTouchInputLayout.Values)
            {
                til.AddTouch(t);
            }
        }
	
        private void RemoveTouch(int t)
        {
            r_allTouches.Remove(t);
		
            foreach(TouchInputLayout til in r_allTouchInputLayout.Values)
            {
                til.RemoveTouch(t);
            }
        }
	
        private void RemoveAndUnlatchAll()
        {
            foreach(TouchInputLayout til in r_allTouchInputLayout.Values)
            {
                til.RemoveAndUnlatchAll();
            }
		
            r_allTouches.Clear();
        }

        private TouchInputLayout FindLayoutByID(string ID)
        {
            if(r_allTouchInputLayout.ContainsKey(ID))
            {
                TouchInputLayout til = r_allTouchInputLayout[ID];
                return til;
            }
            else
            {
                Debug.LogWarning("Layout doesn't exist: " + ID);
                return null;
            }
        }


    }


#if (!UNITY_IPHONE && !UNITY_ANDROID) ||  UNITY_EDITOR

        public struct TouchEditor
        {
            public Vector2 position;
        }

#endif
}
