//============================================================================================================
// (c) Ganz  -  This code is exclusively the property of Ganz and all rights
// therein, including without limitation copyrights and trade secrets, are
// reserved by Ganz.  Any reproduction, disclosure, distribution or 
// unauthorized use is strictly prohibited.
//============================================================================================================
/// Debug lines
//============================================================================================================
// Created on 5/13/2011 10:20:45 AM by HaoH
//============================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class DebugLine
{
    // The width of the line.
    private const float lineWidth = 0.15f;

    // The shader to render the line.
    private const string lineShader = "Diffuse";

    // The name of the debug line object.
    private const string lineObjectName = "DebugLine";

    // The list of line renderers.
    private static List<GameObject> mLineRenderers = new List<GameObject>();

    /// <summary>
    /// Draw a line of points.
    /// </summary>
    /// <param name="points">The list of points.</param>
    public static void DrawLine(List<Vector3> points)
    {
        DrawLine(points.ToArray());
    }

    /// <summary>
    /// Draw a line over the points.
    /// </summary>
    /// <param name="points">The points to add.</param>
    public static void DrawLine(Vector3[] points)
    {
        GameObject obj = CreateLineRenderer();
        LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();

        lineRenderer.SetVertexCount(points.Length);
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }

        mLineRenderers.Add(obj);
    }

    public static void DrawRect(Vector3 min, Vector3 max)
    {
        Vector3 leftTop = min;
        Vector3 leftBtm = new Vector3(min.x, min.y, max.z);
        Vector3 rightBtm = max;
        Vector3 rightTop = new Vector3(max.x, min.y, min.z);

        Vector3[] points = { leftTop, leftBtm, rightBtm, rightTop, leftTop };
        DebugLine.DrawLine(points);
    }

    /// <summary>
    /// Draw a line from start position to end position.
    /// </summary>
    /// <param name="startPosition">The start position.</param>
    /// <param name="endPosition">The end position.</param>
    public static void DrawLine(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject obj = CreateLineRenderer();
        LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();

        lineRenderer.SetVertexCount(2);

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        mLineRenderers.Add(obj);
    }

    /// <summary>
    /// Destroy all the lines.
    /// </summary>
    public static void DestroyLines()
    {
        foreach (GameObject obj in mLineRenderers)
        {
            UnityEngine.Object.DestroyImmediate(obj);
        }

        mLineRenderers.Clear();
    }

    /// <summary>
    /// Create the line renderer.
    /// </summary>
    /// <returns>The line renderer to use.</returns>
    private static GameObject CreateLineRenderer()
    {
        GameObject obj = new GameObject(lineObjectName);
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.SetWidth(lineWidth, lineWidth);
        lineRenderer.material = new Material(Shader.Find(lineShader));
        lineRenderer.useWorldSpace = true;
        lineRenderer.castShadows = false;
        lineRenderer.receiveShadows = false;
        return obj;
    }
}