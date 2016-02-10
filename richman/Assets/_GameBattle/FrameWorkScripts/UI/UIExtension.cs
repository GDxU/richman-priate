///UI extension class 
/// Hao Han
using System.Collections.Generic;
using UnityEngine;

class UIExtension : UI
{
    //Print FPS
    private static float prevTime_Test = 0;
    private static float totalFPS_Test = 0;
    private static float totalFrame_Test = 0;
    private static float avgFps_Test;
    private const int AvgTime = 1;  //avg fps/per sec;
    public static void PrintFPS(Rect rect, float frameRate)
    {
        float scale = Time.time - prevTime_Test;
        totalFrame_Test++;
        totalFPS_Test += frameRate;

        if (scale > AvgTime)
        {

            avgFps_Test = totalFPS_Test / totalFrame_Test;

            //Debug.Log(scale + "     " + totalFPS_Test + " avg fps = " + avgFps_Test);

            //reset it;
            totalFPS_Test = 0;
            prevTime_Test = Time.time;
            totalFrame_Test = 0;
        }

        Color color = Color.black;

        if (avgFps_Test < 15)
            color = Color.red;
        else if (avgFps_Test > 29)
            color = Color.blue;

        GUI.color = color;
        GUI.Label(rect, avgFps_Test.ToString());    //avg fps;

        //realtime fps;
        color = Color.black;
        if (frameRate < 15)
            color = Color.red;
        else if (frameRate > 29)
            color = Color.blue;
        GUI.color = color;
        Rect rectForRealtime = new Rect(rect.xMax + 20, rect.y, rect.width, rect.height);
        GUI.Label(rectForRealtime, frameRate.ToString());
    }
    /// <summary>
    /// Draws a grid of boxes and returns their positions.
    /// </summary>
    public static List<Rect> DrawAutoGrids(Rect rect, float gridWidth, float gridHeight, GUIStyle style)
    {
        List<Rect> rectGrids = new List<Rect>();

        float width = rect.width;
        float height = rect.height;

        //x, y grid count;
        int xCount = Mathf.FloorToInt(width/gridWidth);
        int yCount = Mathf.FloorToInt(height/gridHeight);

        //x,y gaps 
        float xTotalGaps = width - xCount*gridWidth;
        float yTotalGaps = height - yCount*gridHeight;

        //gap count;
        float xGap = xTotalGaps/(xCount+1);
        float yGap = yTotalGaps/(yCount+1);

        Vector2 topLeft = new Vector2(rect.x + xGap, rect.y + yGap);    //topleft to draw 1st grid;
        

        int id = 0;
        for (int i = 0; i < xCount; i++ )
        {
            for(int j = 0; j < yCount; j++)
            {
                
                float x = topLeft.x + (i* xGap) + (i * gridWidth);
                float y = topLeft.y + (j * yGap) + (j * gridHeight);
                Rect gridRect = new Rect(x, y, gridWidth, gridHeight);

                rectGrids.Add(gridRect);
                GUI.Box(gridRect, (++id).ToString(), style);
            }
        }
        return rectGrids;
    }

    public static List<Rect> DrawAutoGrids(Rect rect, float gridWidth, float gridHeight)
    {
        GUIStyle style = MyGUIBehavior.Instance.skin.box;   //style
        return DrawAutoGrids(rect, gridWidth, gridHeight, style);
    }
}