using System;
using UnityEngine;

public class DragMessage
{
    public int Index = 0;
    public Vector2 StartPosition = Vector2.zero;
    public Vector2 EndPosition = Vector2.zero;
    public Vector2 DeltaPosition = Vector2.zero;
    public float DeltaTime = 0f;
}

public class MouseDrag : MonoBehaviour
{
    public GameObject MessageTarget = null;
    public int MouseButton = 0;
    private bool mStartDrag = false;
    private Vector2 mPositionRec = Vector2.zero;
    private Rect mDragArea;
    public Rect DragArea { set { mDragArea = value; } }

    void Start()
    {
        if (mDragArea.width == 0.0f)
        {
            mDragArea = new Rect(0f, 0f, Screen.width, Screen.height);
        }

        if (MessageTarget == null) MessageTarget = gameObject;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(MouseButton))
        {
            if( !mStartDrag)
            {
                Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                if (mDragArea.Contains(mousePos))
                {
                    mStartDrag = true;
                    mPositionRec = mousePos;
                    GlobalMethods.SendMessage(MessageTarget, "DragStart");
                }
            }
        }
        else if (Input.GetMouseButtonUp(MouseButton))
        {
            if (mStartDrag)
            {
                Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                DragMessage msg = new DragMessage();
                msg.Index = MouseButton;
                msg.StartPosition = mPositionRec;
                msg.EndPosition = mousePos;
                msg.DeltaPosition = mousePos - mPositionRec;
                msg.DeltaTime = Time.deltaTime;
                mStartDrag = false;
                mPositionRec = Vector2.zero;
                if (MessageTarget != null)
                {
                    GlobalMethods.SendMessage(MessageTarget, "DragMove", msg);
                    GlobalMethods.SendMessage(MessageTarget, "DragEnd");
                }
            }
        }
        else if (mStartDrag)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (mDragArea.Contains(mousePos))
            {
                if (mousePos != mPositionRec)
                {
                    DragMessage msg = new DragMessage();
                    msg.Index = MouseButton;
                    msg.StartPosition = mPositionRec;
                    msg.EndPosition = mousePos;
                    msg.DeltaPosition = mousePos - mPositionRec;
                    msg.DeltaTime = Time.deltaTime;
                    mPositionRec = mousePos;
                    if (MessageTarget != null) GlobalMethods.SendMessage(MessageTarget, "DragMove", msg);
                }
            }
            else
            {
                mStartDrag = false;
                DragMessage msg = new DragMessage();
                msg.Index = MouseButton;
                msg.StartPosition = mPositionRec;
                msg.EndPosition = mousePos;
                msg.DeltaPosition = mousePos - mPositionRec;
                msg.DeltaTime = Time.deltaTime;
                mPositionRec = Vector2.zero;
                if (MessageTarget != null)
                {
                    GlobalMethods.SendMessage(MessageTarget, "DragMove", msg);
                    GlobalMethods.SendMessage(MessageTarget, "DragEnd");
                }
            }
        }
	}
}

