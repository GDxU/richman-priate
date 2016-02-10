using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TouchInputManagerBackend
{
    [AddComponentMenu("")]
    public class TouchAnimation : MonoBehaviour
    {
        public string initialPreset;
        public List<TouchAnimationPreset> allPresets = new List<TouchAnimationPreset>();
        private TouchAnimationPreset _activePreset = null;
        private TouchGUIBase _touchGUIBaseOld;

        void Start()
        {
            _touchGUIBaseOld = GetComponent<TouchGUIBase>();
            VerifyPresets();
            if (allPresets.Count == 0)
            {
                Destroy(this);
                return;
            }

            if (string.IsNullOrEmpty(initialPreset))
            {
                initialPreset = allPresets[0].presetID;
            }

            SwapToPreset(initialPreset);
            _activePreset.SnapToAnimation(_touchGUIBaseOld);
        }

        void Update()
        {
            _activePreset.UpdateAnimations(_touchGUIBaseOld);
        }

        public void SwapToPreset(string id)
        {
            if(allPresets.Count(a => a.presetID == id) == 0)
                Debug.LogError("Animation preset does not exist: " + id);
            else
            {
                TouchAnimationPreset swapTo = allPresets.First(a => a.presetID == id);
                _activePreset = swapTo;
            }

        } 

        public void VerifyPresets()
        {
            allPresets.ForEach(a => a.VerifyAnimations(_touchGUIBaseOld));
        }

        public string CurrentPreset()
        {
            if (_activePreset != null)
                return _activePreset.presetID;
            else return "";
        }
    }


    [System.Serializable]
    public class TouchAnimationPreset
    {
        public string presetID = "";
        public TouchAnimationInformation unlatchedAnimationTop = new TouchAnimationInformation();
        public TouchAnimationInformation unlatchedAnimationBase = new TouchAnimationInformation();
        public TouchAnimationInformation latchedAnimationTop = new TouchAnimationInformation();
        public TouchAnimationInformation latchedAnimationBase = new TouchAnimationInformation();

        public struct TouchAnimationPresetSnapshot
        {
            public Texture texture;
            public float size;
            public Color color;
        }

        public void VerifyAnimations(TouchGUIBase touchGUIBaseOld)
        {
            if (unlatchedAnimationTop == null)
                unlatchedAnimationTop = new TouchAnimationInformation();
            unlatchedAnimationTop.Verify(touchGUIBaseOld);

            if (unlatchedAnimationBase == null)
                unlatchedAnimationBase = new TouchAnimationInformation();
            unlatchedAnimationBase.Verify(touchGUIBaseOld);

            if (latchedAnimationTop == null)
                latchedAnimationTop = new TouchAnimationInformation();
            latchedAnimationTop.Verify(touchGUIBaseOld);

            if (latchedAnimationBase == null)
                latchedAnimationBase = new TouchAnimationInformation();
            latchedAnimationBase.Verify(touchGUIBaseOld);
        }

        public void SnapToAnimation(TouchGUIBase touchGUIBaseOld)
        {
            LerpAnimationInformation(touchGUIBaseOld.basePart, latchedAnimationBase, unlatchedAnimationBase, 1);
            LerpAnimationInformation(touchGUIBaseOld.topPart, latchedAnimationTop, unlatchedAnimationTop, 1);
        }

        public void UpdateAnimations(TouchGUIBase touchGUIBaseOld)
        {
            if (touchGUIBaseOld.tracker.AnyLatchedFingers)
            {
                switch (latchedAnimationBase.transitionMethod)
                {
                    case TouchAnimationInformation.TransitionMethod.Snap:
                        LerpAnimationInformation(touchGUIBaseOld.basePart, unlatchedAnimationBase, latchedAnimationBase, 1);
                        break;
                    case TouchAnimationInformation.TransitionMethod.Smooth:
                        LerpAnimationInformation(touchGUIBaseOld.basePart, new TouchAnimationInformation(touchGUIBaseOld.basePart), latchedAnimationBase, latchedAnimationBase.transitionMethodValue);
                        break;
                }

                switch (latchedAnimationTop.transitionMethod)
                {
                    case TouchAnimationInformation.TransitionMethod.Snap:
                        LerpAnimationInformation(touchGUIBaseOld.topPart, unlatchedAnimationTop, latchedAnimationTop, 1);
                        break;
                    case TouchAnimationInformation.TransitionMethod.Smooth:
                        LerpAnimationInformation(touchGUIBaseOld.topPart, new TouchAnimationInformation(touchGUIBaseOld.topPart), latchedAnimationTop, latchedAnimationTop.transitionMethodValue);
                        break;
                }
            }
            else
            {
                switch (unlatchedAnimationBase.transitionMethod)
                {
                    case TouchAnimationInformation.TransitionMethod.Snap:
                        LerpAnimationInformation(touchGUIBaseOld.basePart, latchedAnimationBase, unlatchedAnimationBase, 1);
                        break;
                    case TouchAnimationInformation.TransitionMethod.Smooth:
                        LerpAnimationInformation(touchGUIBaseOld.basePart, new TouchAnimationInformation(touchGUIBaseOld.basePart), unlatchedAnimationBase, unlatchedAnimationBase.transitionMethodValue);
                        break;
                }

                switch (unlatchedAnimationTop.transitionMethod)
                {
                    case TouchAnimationInformation.TransitionMethod.Snap:
                        LerpAnimationInformation(touchGUIBaseOld.topPart, latchedAnimationTop, unlatchedAnimationTop, 1);
                        break;
                    case TouchAnimationInformation.TransitionMethod.Smooth:
                        LerpAnimationInformation(touchGUIBaseOld.topPart, new TouchAnimationInformation(touchGUIBaseOld.topPart), unlatchedAnimationTop, unlatchedAnimationTop.transitionMethodValue);
                        break;
                }
            }
        }

        public void UpdateAnimationSnapshots(bool isLatched, ref TouchAnimationPresetSnapshot baseSnapshot, ref TouchAnimationPresetSnapshot topSnapshot)
        {
            if (isLatched)
            {
                switch (latchedAnimationBase.transitionMethod)
                {
                    case TouchAnimationInformation.TransitionMethod.Snap:
                        baseSnapshot = LerpAnimationSnapshotInformation(baseSnapshot, latchedAnimationBase, 1);
                        break;
                    case TouchAnimationInformation.TransitionMethod.Smooth:
                        baseSnapshot = LerpAnimationSnapshotInformation(baseSnapshot, latchedAnimationBase, latchedAnimationBase.transitionMethodValue);
                        break;
                }

                switch (latchedAnimationTop.transitionMethod)
                {
                    case TouchAnimationInformation.TransitionMethod.Snap:
                        topSnapshot = LerpAnimationSnapshotInformation(topSnapshot, latchedAnimationTop, 1);
                        break;
                    case TouchAnimationInformation.TransitionMethod.Smooth:
                        topSnapshot = LerpAnimationSnapshotInformation(topSnapshot, latchedAnimationTop, latchedAnimationTop.transitionMethodValue);
                        break;
                }
            }
            else
            {
                switch (unlatchedAnimationBase.transitionMethod)
                {
                    case TouchAnimationInformation.TransitionMethod.Snap:
                        baseSnapshot = LerpAnimationSnapshotInformation(baseSnapshot, unlatchedAnimationBase, 1);
                        break;
                    case TouchAnimationInformation.TransitionMethod.Smooth:
                        baseSnapshot = LerpAnimationSnapshotInformation(baseSnapshot, unlatchedAnimationBase, unlatchedAnimationBase.transitionMethodValue);
                        break;
                }

                switch (unlatchedAnimationTop.transitionMethod)
                {
                    case TouchAnimationInformation.TransitionMethod.Snap:
                        topSnapshot = LerpAnimationSnapshotInformation(topSnapshot, unlatchedAnimationTop, 1);
                        break;
                    case TouchAnimationInformation.TransitionMethod.Smooth:
                        topSnapshot = LerpAnimationSnapshotInformation(topSnapshot, unlatchedAnimationTop, unlatchedAnimationTop.transitionMethodValue);
                        break;
                }
            }
        }

        private void LerpAnimationInformation(TouchGUITexture target, TouchAnimationInformation from, TouchAnimationInformation to, float t)
        {
            target.color = Color.Lerp(from.transitionColor, to.transitionColor, t);

            target.texture = to.changeToTexture;

            float newSize = Mathf.Lerp(from.changeToSize, to.changeToSize, t);
            TouchGUIBase.RescalePart(target, newSize);
        }

        private TouchAnimationPresetSnapshot LerpAnimationSnapshotInformation(TouchAnimationPresetSnapshot target, TouchAnimationInformation to, float t)
        {
            target.color = Color.Lerp(target.color, to.transitionColor, t);

            target.texture = to.changeToTexture;

            target.size = Mathf.Lerp(target.size, to.changeToSize, t);

            return target;
        }
    }

    [System.Serializable]
    public class TouchAnimationInformation
    {

        public enum TransitionMethod
        {
            Snap,
            Smooth,
        }

        public TransitionMethod transitionMethod = TransitionMethod.Snap;
        public float transitionMethodValue;

        public Color transitionColor;

        public Texture changeToTexture;

        public float changeToSize;

        public TouchAnimationInformation()
        { }

        public TouchAnimationInformation(TouchGUITexture from)
        {
            transitionColor = from.color;
            changeToTexture = from.texture;
            changeToSize = from.transform.localScale.x;
        }

        public void Verify(TouchGUIBase touchGUIBaseOld)
        {
			///we now allow no textures.. guess there is nothing else left to verify.. xD
            /*if (changeToTexture == null)
                changeToTexture = touchGUIBaseOld.topPart.texture;*/
        }
    }


}
