
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Test for menu;
/// </summary>
public class MyGUITest
{
    private static MyGUIBehavior myGuiBehavior;
    private static void AttachMyGUIBehavior()
    {
        GameObject gameObject = GameObject.Find("_Config");
        myGuiBehavior = gameObject.GetComponent<MyGUIBehavior>();
        if (myGuiBehavior == null)
            myGuiBehavior = gameObject.AddComponent<MyGUIBehavior>();
    }

    public delegate void OnGUICallback();

    private static int layerID = 0;

    [MenuItem("Debug/GUITest")]
    static void GUITest()
    {
        AttachMyGUIBehavior();
        GUITest myGUI = new GUITest();
        myGUI.AddGUICall(DrawWindow2, MyGUI.GUICallPriority.top);
        myGUI.AddGUICall(DrawGrids, MyGUI.GUICallPriority.top);
        myGUI.AddGUICall(DrawWindow3, MyGUI.GUICallPriority.top);
        myGUI.AddGUICall(DrawWindow4, MyGUI.GUICallPriority.top);
        myGUI.AddGUICall(myGUI.DrawWindow, MyGUI.GUICallPriority.background);

        MyGUIBehavior.Instance.AddMyGUI(layerID++, myGUI);

        myGUI.AddGUICall(myGUI.PrintFPS, MyGUI.GUICallPriority.top);
    }
    
    [MenuItem("Debug/RemoveGUITest")]
    static void RemoveGUITest()
    {
        AttachMyGUIBehavior();

        for(int i = 0; i < layerID; i++)
        {
            MyGUI myGUI = MyGUIBehavior.Instance.GetMyGUI(i);
            if (myGUI == null)
                continue;

            if(myGUI is GUITest)
            {
                GUITest guiTest = (GUITest) myGUI;
                myGUI.RemoveGUICall(guiTest.DrawWindow, MyGUI.GUICallPriority.background);
            }
        }
    }

    [MenuItem("Debug/RemoveDrawWindow2")]
    static void RemoveDrawWindow2()
    {
        AttachMyGUIBehavior();

        for (int i = 0; i < layerID; i++)
        {
            MyGUI myGUI = MyGUIBehavior.Instance.GetMyGUI(i);
            if(myGUI == null)
                continue;
            
            myGUI.RemoveGUICall(DrawWindow2, MyGUI.GUICallPriority.top);
            myGUI.RemoveGUICall(DrawWindow3, MyGUI.GUICallPriority.top);
            myGUI.RemoveGUICall(DrawWindow4, MyGUI.GUICallPriority.top);
        }
    }

    private static Rect window2 = new Rect(Screen.width * 0.5f - 100f, Screen.height * 0.5f - 120f, 300f, 240f);
    private static Rect window3 = new Rect(Screen.width * 0.5f + 100f, Screen.height * 0.5f + 120f, 150f, 150f);
    private static Rect window4 = new Rect(10f, 10f, 190f, 190f);
    private static Rect window5 = new Rect();

    public static void DrawWindow2()
    {
        UI.DrawWindow(window2, "DrawWindow2");
    }

    public static void DrawWindow3()
    {
        UI.DrawWindow(window3, "Hao DrawWindow3");
    }

    public static void DrawWindow4()
    {
        UI.DrawWindow(window4, "Hao DrawWindow4");
    }

    public static void DrawGrids()
    {
        DebugExt.Log("----? draw grids??");
        // Draw the resource grid
        //List<Vector2> grid = UI.DrawGrid(window2, 50f, 50f, 4f, 4f, 4f, 8);
        Rect rect = window2;//new Rect(window2.x + 3, window2.height + 3, window2.width - 6, window2.height - 6);
        rect.x += 6;
        rect.y += 6;
        rect.width -= 8;
        rect.height -= 8;
        
        GUI.Box(rect, "", MyGUIBehavior.Instance.skin.button);

        //List<Rect> rectGrids = UIExtension.DrawAutoGrids(rect, 50f, 30f);
        List<Rect> rectGrids = UIExtension.DrawAutoGrids(rect, 60f, 60f);
    }
}

/// <summary>
/// Test for MyGUI;
/// </summary>
public class GUITest : MyGUI
{
    public void DrawWindow()
    {
        DebugExt.Log("Hao Test  --------> DrawWindow()");
        Rect rect = UI.DrawWindow(new Rect(Screen.width * 0.5f - 200f, Screen.height * 0.5f - 220f, 400f, 440f), "Hao TTTTT");
    }

    public void PrintFPS()
    {
        UIExtension.PrintFPS(new Rect(Screen.width - 55f, 10f, 45f, 60f), 30);
    }
}