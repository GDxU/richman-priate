using System.Collections.Generic;
using UnityEngine;

public class DebugExt
{

    /// <summary>
    /// Log a string message to the UnityEngine debug. 
    /// </summary>
    public static void Log(string text)
    {
        UnityEngine.Debug.Log(text);
    }
	
	/// <summary>
    /// Log two objects error out to the UnityEngine debug. 
    /// </summary>
    public static void Log(object msg, UnityEngine.Object obj)
    {
        UnityEngine.Debug.Log(msg, obj);
    }

    /// <summary>
    /// Log a string error out to the UnityEngine debug. 
    /// </summary>
    public static void LogError(string text)
    {
        UnityEngine.Debug.LogError(text);
    }
	
	/// <summary>
    /// Log a object error out to the UnityEngine debug. 
    /// </summary>
    public static void LogError(object obj)
    {
        UnityEngine.Debug.LogError(obj);
    }
	
	/// <summary>
    /// Log two objects error out to the UnityEngine debug. 
    /// </summary>
    public static void LogError(object msg, UnityEngine.Object obj)
    {
        UnityEngine.Debug.LogError(msg, obj);
    }
	
    /// <summary>
    /// Log a string warning out to the UnityEngine debug. 
    /// </summary>
    public static void LogWarning(string text)
    {
        UnityEngine.Debug.LogWarning(text);
    }
	
	/// <summary>
    /// Log a string warning out to the UnityEngine debug. 
    /// </summary>
    public static void LogWarning(object obj)
    {
        UnityEngine.Debug.LogWarning(obj.ToString());
    }
	
	/// <summary>
    /// Log two objects warning out to the UnityEngine debug. 
    /// </summary>
    public static void LogWarning(object msg, UnityEngine.Object obj)
    {
        UnityEngine.Debug.LogWarning(msg, obj);
    }
	
	/// <summary>
    /// Log a string warning out to the UnityEngine debug. 
    /// </summary>
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        UnityEngine.Debug.DrawLine(start, end, color);
    }

    public static void PrintList<T>(ICollection<T> list, string preStr)
    {
        string str = preStr;

        foreach (T node in list)
        {
            str += " ";
            str += node.ToString();
            str += " -> ";
        }

        Log(str);
    }


    /// <summary>
    /// Create debug object;
    /// </summary>
    public static GameObject CreateObject(Vector3 leftBottomPos, float scale, Color color, string name,
                                    int layerMask, PrimitiveType objectType, 
                                    GameObject rootObject, bool hasCollider)
    {
        GameObject obj = GameObject.CreatePrimitive(objectType);

        Vector3 centerPos = new Vector3(leftBottomPos.x + (scale / 2), leftBottomPos.y, leftBottomPos.z + (scale / 2));
        obj.transform.position = centerPos;

        obj.renderer.material.color = color;
        obj.layer = layerMask;
        obj.name = name;

        obj.transform.localScale = new Vector3(scale, scale, scale);

        if(rootObject)
            obj.transform.parent = rootObject.transform;
        
        if(!hasCollider)
            UnityEngine.Object.DestroyImmediate(obj.collider);

        return obj;
    }
}