using TouchInputManagerBackend;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Linq;

namespace TouchInputManagerBackendEditor
{
    public class TouchInputManagerInputEditorPreview : TouchInputManagerEditor_GUIWidget {
	
        private TouchBase _activeBase = null;
        private TouchAnimationPreset presetToShow = null;
        private TouchAnimationPreset.TouchAnimationPresetSnapshot animSnapshotBase;
        private TouchAnimationPreset.TouchAnimationPresetSnapshot animSnapshotTop;
	
        private Vector2 _dragStartPosition = Vector2.zero;
        private Vector2 _dragStartPositionScreenSpace = Vector2.zero;
        private Vector2 _currentDragPos = Vector2.zero;
        private Vector2 _currentDragPosScreenSpace = Vector2.zero;
        private bool _isBeingDragged = false;
	
        Vector2 basePos, topPos;
	
        GUIStyle animationLabelStyle;
	
        public TouchInputManagerInputEditorPreview(GUIStyle _animationLabelStyle, Action repaintAction) : base(repaintAction)
        {
            animationLabelStyle = _animationLabelStyle;
        }
	
        public void Load(TouchBase tb)
        {
            _activeBase = tb;
            TouchAnimation ta = tb.GetComponent<TouchAnimation>();
            if(ta != null)
                SetPreset(ta.allPresets.First(a => a.presetID == ta.initialPreset));
            else
            {
                presetToShow = null;
                ResetPos();
            }
        }
	
        public string ActivePreset()
        {
            return presetToShow == null ? null : presetToShow.presetID;
        }
	
        public void SetPreset(TouchAnimationPreset p)
        {
            TouchGUIBase b = (_activeBase as TouchGUIBase);
            if(b == null)
                return;
		
            presetToShow = p;
            animSnapshotBase.color = presetToShow.unlatchedAnimationBase.transitionColor;
            animSnapshotTop.color =  presetToShow.unlatchedAnimationTop.transitionColor;
		
            animSnapshotBase.size =  presetToShow.unlatchedAnimationBase.changeToSize;
            animSnapshotTop.size =  presetToShow.unlatchedAnimationTop.changeToSize;
		
            animSnapshotBase.texture =  presetToShow.unlatchedAnimationBase.changeToTexture;
            animSnapshotTop.texture =  presetToShow.unlatchedAnimationTop.changeToTexture;
		
            ResetPos();
        }
	
        public void ClearPreset()
        {
            if(presetToShow == null)
                return;
		
            presetToShow = null;
            ResetPos();
        }
	

        public void Update()
        {
            if (_isBeingDragged)
            {
                basePos = _dragStartPositionScreenSpace;
                Repaint();
            }
            else
            {
                TouchGUIBase t = _activeBase as TouchGUIBase;
                if(t!=null)
                {
                    switch(t.recenterBaseOnRelease)
                    {
                        case TouchGUIBase.RecenterMethod.Snap:
                            if(basePos != new Vector2(0.5f,0.5f))
                            {
                                ResetPos();
                                Repaint();
                            }
                            break;
                        case TouchGUIBase.RecenterMethod.Smooth:
                            if(Vector2.Distance(basePos,new Vector2(0.5f,0.5f)) > 0.01f)
                            {
                                basePos = Vector3.Lerp(basePos,new Vector2(0.5f,0.5f),t.recenterBaseMethodValue);
                                Repaint();
                            }
                            break;
                    }
                }
			
		
            }
	
            if(presetToShow != null)
            {
                presetToShow.UpdateAnimationSnapshots(_isBeingDragged,ref animSnapshotBase,ref animSnapshotTop);
                Repaint();
            }
        }

        public void OnGUI(Rect drawArea)
        {
            GUI.BeginGroup(drawArea);

		
            Rect dragPos = new Rect(0,0,drawArea.width,drawArea.height);
            if (Event.current != null)
            {
                if (Event.current.isMouse)
                {
                    _currentDragPos = Event.current.mousePosition;
                    _currentDragPosScreenSpace = new Vector2(_currentDragPos.x/drawArea.width,_currentDragPos.y/drawArea.height);
                }

                if (Event.current.rawType == EventType.MouseDown && dragPos.Contains(Event.current.mousePosition))
                {
                    if (!_isBeingDragged)
                    {
                        _dragStartPosition = Event.current.mousePosition;
                        _dragStartPositionScreenSpace =  new Vector2(_dragStartPosition.x/drawArea.width,_dragStartPosition.y/drawArea.height);
                    }
                    _isBeingDragged = true;
                    Event.current.Use();
                }
                else if (_isBeingDragged && Event.current.rawType == EventType.mouseUp)
                {
                    _isBeingDragged = false;
                    Repaint();
                }
            } 

		
            if(_activeBase == null)
            {
                GUI.EndGroup();
                return;
            }
		
            TouchGUIBase b = _activeBase as TouchGUIBase;
            if(b == null)
            {
                GUI.EndGroup();
                return;
            }
		
            if(b is TouchButton)
            {
                topPos = basePos;
            }
            else if(b is TouchJoystick)
            {
                if(_isBeingDragged)
                {
                    TouchJoystick j = b as TouchJoystick;
					if(j.topPart.texture != null)
					{
						Vector2 textureSize = j.basePart.texture == null ? new Vector2(j.topPart.texture.width,j.topPart.texture.height) : new Vector2(j.basePart.texture.width,j.basePart.texture.height);

	                    Vector2 touchPos =  _currentDragPosScreenSpace;
	                    Vector2 firstInitScreenPos = _dragStartPositionScreenSpace;
	                    float scaleX = Mathf.Clamp(Mathf.Abs(touchPos.x - firstInitScreenPos.x), 0, (presetToShow == null ? b.scale : animSnapshotBase.size) / 2);
						float scaleY = Mathf.Clamp(Mathf.Abs(touchPos.y - firstInitScreenPos.y), 0, ((presetToShow == null ? b.scale : animSnapshotBase.size) / 2 / ((float)textureSize.x / (float)textureSize.y)) / (drawArea.height / drawArea.width));
	                    Vector2 input = (_currentDragPosScreenSpace - _dragStartPositionScreenSpace);
	                    input = new Vector2(j.disableX ? 0 : input.x, j.disableY ? 0 : input.y).normalized;
	                    topPos = new Vector2(firstInitScreenPos.x + (((input.x * scaleX))), firstInitScreenPos.y + (((input.y * scaleY))));
					}
                }
                else
                    topPos = basePos;
            }
		
            if(presetToShow == null)
            {
			
                if(b.topPartLayer > b.basePartLayer)
                {
                    DrawTex(basePos, b.scale, b.basePart.texture, drawArea, b.basePart.color);
                    DrawTex(topPos, b.scale, b.topPart.texture, drawArea, b.topPart.color);
                }
                else
                {
                    DrawTex(topPos, b.scale, b.topPart.texture, drawArea, b.topPart.color);
                    DrawTex(basePos, b.scale, b.basePart.texture, drawArea, b.basePart.color);
                }
				
            }
            else
            {
                if(b.topPartLayer > b.basePartLayer)
                {
                    DrawTex(basePos,animSnapshotBase.size,animSnapshotBase.texture,drawArea,animSnapshotBase.color);
                    DrawTex(topPos,animSnapshotTop.size,animSnapshotTop.texture,drawArea,animSnapshotTop.color);
                }
                else
                {
                    DrawTex(topPos,animSnapshotTop.size,animSnapshotTop.texture,drawArea,animSnapshotTop.color);
                    DrawTex(basePos,animSnapshotBase.size,animSnapshotBase.texture,drawArea,animSnapshotBase.color);
                }
            }

            if (presetToShow != null)
            {
                GUI.Label(new Rect(10, 10, drawArea.width - 20, 15), "Animation: " + presetToShow.presetID, animationLabelStyle);
            }
		

            GUI.EndGroup();

        }
	
        public void DrawTex(Vector2 pos, float scale, Texture t, Rect drawArea, Color c)
        {
	    	if (t == null)
								return;

            GUI.color = c;
            Vector2 basePosNonScreenSpace = new Vector2(pos.x * drawArea.width,pos.y * drawArea.height);
            float baseWidthPixels = scale * drawArea.width;
            float baseHeightPixels = baseWidthPixels / ((float)t.width / (float)t.height);

            Rect baseDrawArea = new Rect(basePosNonScreenSpace.x - (baseWidthPixels / 2), basePosNonScreenSpace.y - (baseHeightPixels / 2), baseWidthPixels, baseHeightPixels);
            GUI.DrawTexture(baseDrawArea,t);

            GUI.color = Color.white;
        }
	
        private void ResetPos()
        {
            basePos = new Vector2(0.5f,0.5f);
            topPos = new Vector2(0.5f,0.5f);
        }
	
    }
}