using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class TouchInputManagerEditor_GUISidebar : TouchInputManagerEditor_GUIWidget
{
	private Action _OnGUI;
	private Action _OnUpdate;
	
	private bool expanding = false;
	private float _widthToExpandTo = 0;
	private float _expandSpeed = 1;//1 to disable;
	private Vector2 _scrollPosition;
    public float _width = 200;
    private float _dragStartPosition = 0;
    private float _widthStartOfDrag = 0;
    private float _currentDragPos = 0;
    private bool _isBeingDragged = false;
    public bool _clickedAway = false;
	
	private float _minWidth, _maxWidth;
	
	private GUIStyle _style,padding;
	
	public TouchInputManagerEditor_GUISidebar(float minWidth, float maxWidth, GUIStyle style,Action OnGUI,Action OnUpdate, Action repaintDelegate) : base(repaintDelegate)
	{
		_style = style;
		padding = new GUIStyle();
		padding.padding = new RectOffset(5,5,0,0);
		_OnGUI = OnGUI;
		_OnUpdate = OnUpdate;
		SetMinMaxWidth(minWidth,maxWidth);
	}
	 
	public void Expand()
	{
		expanding = true;
		_widthToExpandTo = _width;
		_width = 0;
	}
	
	public void SetMinMaxWidth(float minWidth, float maxWidth) 
	{
		_minWidth = minWidth; 
		_maxWidth = maxWidth;
		_width = minWidth;
	}
	
	// Update is called once per frame
	public void Update () 
	{
		if (_isBeingDragged)
        {
			expanding = false;
            _width = _widthStartOfDrag - (_dragStartPosition - _currentDragPos);
            Repaint();
        }
		else if(expanding)
		{
			_width = Mathf.Lerp(_width,_widthToExpandTo,_expandSpeed);
			if(_width > _widthToExpandTo - 0.01f)
			{
				expanding = false;
				_width = _widthToExpandTo;
			}
			Repaint();
		}
		
		
		if(!expanding)
        	_width = Mathf.Clamp(_width, _minWidth, _maxWidth);

		if(_OnUpdate != null)
			_OnUpdate();
	}
	
	public void OnGUI(float leftOffset = 0, float heightOffset = 0, float height = -1)
	{
		if(height<0)
			height = Screen.height;
		
        Rect dragPos = new Rect(leftOffset + _width - 5, 0, 10, heightOffset + height);
        EditorGUIUtility.AddCursorRect(dragPos, MouseCursor.ResizeHorizontal);
        if (Event.current != null)
        {
            if (Event.current.isMouse)
            {
                _currentDragPos = Event.current.mousePosition.x;
            }

            if (Event.current.rawType == EventType.MouseDown && dragPos.Contains(Event.current.mousePosition))
            {
                if (!_isBeingDragged)
                {
                    _dragStartPosition = Event.current.mousePosition.x;
                    _widthStartOfDrag = _width;
                }
                _isBeingDragged = true;
                Event.current.Use();
            }
            else if (_isBeingDragged && Event.current.rawType == EventType.mouseUp)
            {
                _isBeingDragged = false;
            }


            if (Event.current.rawType == EventType.MouseDown && _clickedAway == false)
            {
                _clickedAway = true;
            }
        } 

        EditorGUILayout.BeginVertical(_style, GUILayout.Width(_width), GUILayout.Height(height));
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
			
		EditorGUILayout.BeginVertical(padding);
		if(_OnGUI != null)
			_OnGUI();
		EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
	}
	
}
