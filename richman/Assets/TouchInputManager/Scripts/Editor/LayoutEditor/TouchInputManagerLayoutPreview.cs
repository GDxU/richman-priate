using TouchInputManagerBackend;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TouchInputManagerBackendEditor
{
    public class TouchInputManagerLayoutPreview : TouchInputManagerEditor_GUIWidget
    {

        public TouchInputLayout.TrackerPrefabBasePrefabPair trackerBeingModified = null;
        private GUIStyle _styleSelectedBG, _styleDarkNoBorderBG, _previewerBoxStyle;
	
        private bool _isResizing;
        private bool _isDragging;
        private Vector2 _modifyStartPos;
        private Vector2 _currentPos;
        private Rect _rectStartOfDrag;
        private ResizeSide _resizeSide;
	
        internal enum ResizeSide
        {
            Left,
            Right,
            Top,
            Bottom
        }
	
        public TouchInputManagerLayoutPreview(GUIStyle styleSelectedBG, GUIStyle styleDarkNoBorderBG, GUIStyle previewerBoxStyle,Action repaintAction) : base(repaintAction)
        {
            _styleSelectedBG = styleSelectedBG;
            _styleDarkNoBorderBG = styleDarkNoBorderBG;
            _previewerBoxStyle = previewerBoxStyle;
        }
	
        public bool Update()
        {
            bool dirty = false;
		
            if (trackerBeingModified != null && _isDragging)
            {
                Vector2 moveAmount = _modifyStartPos - _currentPos;

                moveAmount.x = Mathf.Clamp(-moveAmount.x, -(_rectStartOfDrag.x), 1 - _rectStartOfDrag.xMax);
                moveAmount.y = Mathf.Clamp(moveAmount.y, -(_rectStartOfDrag.y), 1 - _rectStartOfDrag.yMax);
			
                trackerBeingModified._tracker.activeRegion.xMin = _rectStartOfDrag.xMin + moveAmount.x;
                trackerBeingModified._tracker.activeRegion.width = _rectStartOfDrag.width;
                trackerBeingModified._tracker.activeRegion.yMin = _rectStartOfDrag.yMin + moveAmount.y;
                trackerBeingModified._tracker.activeRegion.height = _rectStartOfDrag.height;
                Repaint();
                dirty = true;
            }
		
            if (trackerBeingModified != null && _isResizing)
            {
                Vector2 resizeAmount = _modifyStartPos - _currentPos;

                switch (_resizeSide)
                {
                    case ResizeSide.Left:
                        trackerBeingModified._tracker.activeRegion.x = Mathf.Clamp(_rectStartOfDrag.x - resizeAmount.x
                                                                                   , 0,
                                                                                   _rectStartOfDrag.x + _rectStartOfDrag.width - 0.05f);

                        trackerBeingModified._tracker.activeRegion.width = _rectStartOfDrag.width -
                                                                           (trackerBeingModified._tracker.activeRegion.x -
                                                                            _rectStartOfDrag.x);
                        break;
                    case ResizeSide.Right:
                        trackerBeingModified._tracker.activeRegion.width =
                            Mathf.Clamp(_rectStartOfDrag.width - (resizeAmount.x), 0.05f, 1 - _rectStartOfDrag.x);
                        break;
                    case ResizeSide.Bottom:
                        trackerBeingModified._tracker.activeRegion.y = Mathf.Clamp(_rectStartOfDrag.y + resizeAmount.y
                                                                                   , 0,
                                                                                   _rectStartOfDrag.y + _rectStartOfDrag.height - 0.05f);

                        trackerBeingModified._tracker.activeRegion.height = _rectStartOfDrag.height -
                                                                            (trackerBeingModified._tracker.activeRegion.y -
                                                                             _rectStartOfDrag.y);
                        break;
                    case ResizeSide.Top:
                        trackerBeingModified._tracker.activeRegion.height =
                            Mathf.Clamp(_rectStartOfDrag.height + (resizeAmount.y), 0.05f, 1 - _rectStartOfDrag.y);
                        break;
                }

                Repaint();
                dirty = true;
            }
		
		
            return dirty;
        }
	
        public void OnGUI(Rect drawArea, List<TouchInputLayout.TrackerPrefabBasePrefabPair> trackers)
        {
            if(trackers == null || trackers.Count == 0)
            {
                GUILayout.BeginArea(new Rect(drawArea.xMin + 10, drawArea.yMin, drawArea.width - 20, drawArea.height));
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox("Drag and drop Joysticks/Buttons/Zones from the Input Editor here", MessageType.Info);
                GUILayout.EndArea();
            }
            foreach (TouchInputLayout.TrackerPrefabBasePrefabPair pairs in trackers)
            {
                float posStartX, posWidth, posStartY, posHeight;
                float width = drawArea.width;
                float height = drawArea.height;

                posStartX = width * pairs._tracker.activeRegion.x;
                posWidth = width * pairs._tracker.activeRegion.width;

                posStartY = height * (pairs._tracker.activeRegion.y);
                posHeight = height * (pairs._tracker.activeRegion.height);
                posStartY = (height - posStartY - posHeight);

                posStartX += drawArea.x;
                posStartY += 16;

                Rect drawPos = new Rect(posStartX, posStartY, posWidth, posHeight);
			
                if(trackerBeingModified == null)
                    GUILayout.BeginArea(drawPos, _styleDarkNoBorderBG);
                else
                    GUILayout.BeginArea(drawPos, trackerBeingModified._tracker == pairs._tracker ? _styleSelectedBG : _styleDarkNoBorderBG);

                GUI.Label(new Rect(10, 10, drawPos.width - 20, 15), pairs._base.name + " | ID: " + pairs._tracker._id, _previewerBoxStyle);

                if(pairs._base is TouchGUIBase)
                {
                    TouchGUIBase b = pairs._base as TouchGUIBase;

                    DrawTex(drawPos, 0.5f, b.basePart.texture, b.basePart.color * new Color(0.2f, 0.2f, 0.2f));
                    DrawTex(drawPos, 0.5f, b.topPart.texture, b.topPart.color * new Color(0.2f, 0.2f, 0.2f));

                    GUI.color = Color.white;
                }

                GUILayout.EndArea();

                ResizableDraggableJoystick(pairs, drawPos, drawArea);


            }
        }


        void ResizableDraggableJoystick(TouchInputLayout.TrackerPrefabBasePrefabPair toResize, Rect drawnArea, Rect fullArea)
        {
            Rect resizePosLeft = new Rect(drawnArea.xMin, drawnArea.yMin + 8, 8, drawnArea.height - 16);
            EditorGUIUtility.AddCursorRect(resizePosLeft, MouseCursor.ResizeHorizontal);

            Rect resizePosRight = new Rect(drawnArea.xMax - 8, drawnArea.yMin + 8, 8, drawnArea.height - 16);
            EditorGUIUtility.AddCursorRect(resizePosRight, MouseCursor.ResizeHorizontal);

            Rect resizePosTop = new Rect(drawnArea.xMin + 8, drawnArea.yMin, drawnArea.width - 16, 8);
            EditorGUIUtility.AddCursorRect(resizePosTop, MouseCursor.ResizeVertical);

            Rect resizePosBottom = new Rect(drawnArea.xMin + 8, drawnArea.yMax - 8, drawnArea.width - 16, 8);
            EditorGUIUtility.AddCursorRect(resizePosBottom, MouseCursor.ResizeVertical);

            Rect dragZone = new Rect(drawnArea.xMin + 10, drawnArea.yMin + 10, drawnArea.width - 20, 15);
            EditorGUIUtility.AddCursorRect(dragZone, MouseCursor.MoveArrow);

            if (Event.current != null)
            {
                if (Event.current.isMouse)
                {
                    _currentPos = Event.current.mousePosition;
                    _currentPos.x -= fullArea.x;
                    _currentPos.x /= (fullArea.width);
                    _currentPos.y -= (16);
                    _currentPos.y /= (fullArea.height - (16));
                }

                if (Event.current.rawType == EventType.MouseDown)
                {
                    if (dragZone.Contains(Event.current.mousePosition))
                    {
                        trackerBeingModified = toResize;
                        StartJoystickMove(toResize,_currentPos);
                        Event.current.Use();
                    }
                    else if (resizePosLeft.Contains(Event.current.mousePosition))
                    {
                        trackerBeingModified = toResize;
                        StartJoystickResize(toResize,_currentPos,ResizeSide.Left);
                        Event.current.Use();
                    }
                    else if (resizePosRight.Contains(Event.current.mousePosition))
                    {
                        trackerBeingModified = toResize;
                        StartJoystickResize(toResize,_currentPos,ResizeSide.Right);
                        Event.current.Use();
                    }
                    else if (resizePosTop.Contains(Event.current.mousePosition))
                    {
                        trackerBeingModified = toResize;
                        StartJoystickResize(toResize,_currentPos,ResizeSide.Top);
                        Event.current.Use();
                    }
                    else if (resizePosBottom.Contains(Event.current.mousePosition))
                    {
                        trackerBeingModified = toResize;
                        StartJoystickResize(toResize,_currentPos,ResizeSide.Bottom);
                        Event.current.Use();
                    }

                }
                else if (Event.current.rawType == EventType.mouseUp && trackerBeingModified != null)
                {
                    _isResizing = false;
                    _isDragging = false;
                    trackerBeingModified = null;
                    Event.current.Use();
                }

            }
        }
	

        void StartJoystickResize( TouchInputLayout.TrackerPrefabBasePrefabPair tracker, Vector2 currentPos, ResizeSide side)
        {
            if (!_isResizing)
            {
                _modifyStartPos = currentPos;

                _rectStartOfDrag = tracker._tracker.activeRegion;
                _resizeSide = side;
                trackerBeingModified = tracker;
            }

            _isResizing = true;
        }

        void StartJoystickMove(TouchInputLayout.TrackerPrefabBasePrefabPair tracker, Vector2 currentPos)
        {
            if(!_isDragging)
            {
                _modifyStartPos = currentPos;
                _rectStartOfDrag = tracker._tracker.activeRegion;
                trackerBeingModified = tracker;
            }

            _isDragging = true;
        }

        public void DrawTex(Rect drawPos, float scale, Texture t, Color c)
        {
            if(t == null)
                return;

            GUI.color = c * new Color(0.25f, 0.25f, 0.25f, 0.25f);
            GUI.DrawTexture(
                new Rect((drawPos.width / 2) - (t.width / 2 * scale),
                         (drawPos.height / 2) - (t.height / 2 * scale) + 10, t.width * scale,
                         t.height * scale), t);

        }

    }
}