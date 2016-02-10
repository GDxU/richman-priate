using System;
using UnityEngine;
using System.Collections;

namespace TouchInputManagerBackend
{
    public abstract class TouchGUIBase : TouchBase
    {
        #region PublicTweakableMembers

        public int basePartLayer = 0;
        public int topPartLayer = 0;

        public TouchGUITexture basePart;
        public TouchGUITexture topPart;

        public enum RecenterMethod
        {
            DontRecenter,
            Snap,
            Smooth
        }

        public RecenterMethod recenterBaseOnRelease;
        public float recenterBaseMethodValue;
        public float scale;
        #endregion

        protected Vector2 _baseScreenSize { get; private set; }
        protected bool isUsingEffect { get; private set; }

        protected void Start()
        {
            InitializeParts();

			if(basePart.texture != null)
            	_baseScreenSize = new Vector2(basePart.texture.width / 2, basePart.texture.height/2);
			else if(topPart.texture != null)
				_baseScreenSize = new Vector2(topPart.texture.width / 2, topPart.texture.height/2);

            isUsingEffect = GetComponent<TouchAnimation>() != null;
        }

        protected void Update()
        {
            PositionBase();
            PositionTop();
            if (!isUsingEffect)
            {
                RescalePart(basePart, scale);
                RescalePart(topPart, scale);
            }
        }

        protected void OnGUI()
        {
            GUI.depth = basePartLayer;
            basePart.Draw();
            GUI.depth = topPartLayer;
            topPart.Draw();
        }

        /// <summary>
        /// Positions the base GUITexture. Override to do something other than snap to finger
        /// </summary>
        protected virtual void PositionBase()
        {
            if (tracker.AnyLatchedFingers)
                basePart.transform.position = new Vector3(tracker.FirstInitialScreenPosition.x, tracker.FirstInitialScreenPosition.y, basePart.transform.position.z);
            else
            {
                switch (recenterBaseOnRelease)
                {
                    case RecenterMethod.Snap:
                        CenterInRegion(basePart.transform,false);
                        break;
                    case RecenterMethod.Smooth:
                        {
                            Vector2 center = CenterOfRegion(false);
                            basePart.transform.position = Vector3.Lerp(basePart.transform.position, new Vector3(center.x, center.y, basePart.transform.position.z), recenterBaseMethodValue);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Positions the Top GUITexture. Override to do something other than snap to finger
        /// </summary>
        protected virtual void PositionTop()
        {
            topPart.transform.position = new Vector3(basePart.transform.position.x, basePart.transform.position.y, topPart.transform.position.z);
        }
    
        /// <summary>
        /// Positions GUI elements to their start positions
        /// </summary>
        public virtual void InitializeParts()
        {
            basePart.transform.position = Vector3.forward * basePartLayer;// *depth;
            topPart.transform.localPosition = Vector3.forward * topPartLayer;// *(depth + 1);
            RescalePart(basePart, scale);
            RescalePart(topPart, scale);

            CenterInRegion(basePart.transform);
            CenterInRegion(topPart.transform);
        }

        /// <summary>
        /// Sets GUI element scale
        /// </summary>
        public static void RescalePart(TouchGUITexture toScale, float scaleTo)
        {
			if (toScale.texture == null)
								return;

            toScale.transform.localScale = new Vector2(scaleTo, scaleTo / ((float)toScale.texture.width / (float)toScale.texture.height)); 
        }
	
        /// <summary>
        /// Position a transform to the center of region, in screen space (Use for GUI elements)
        /// </summary>
        protected void CenterInRegion(Transform t, bool ignoreOffset = false)
        {
            Vector2 center = CenterOfRegion(ignoreOffset);
            t.position = new Vector3(center.x, center.y, t.position.z);
        }

        /// <summary>
        /// Returns a vector2 representing center of region in screenspace
        /// </summary>
        protected Vector2 CenterOfRegion(bool ignoreOffset = false)
        {
            if (ignoreOffset)
                return new Vector2(tracker.activeRegion.x + (tracker.activeRegion.width / 2),
                                   tracker.activeRegion.y + (tracker.activeRegion.height / 2));
            else
                return new Vector2(tracker.activeRegion.x + ((tracker.activeRegion.width / 2) + tracker.centeredPositionOffset.x),
                                   tracker.activeRegion.y + ((tracker.activeRegion.height / 2) + tracker.centeredPositionOffset.y));
        }
    }
}