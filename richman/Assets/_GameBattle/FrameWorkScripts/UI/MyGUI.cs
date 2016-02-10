using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// maintain list for GUICalls, all gui should be inherated from this class;
/// </summary>
public abstract class MyGUI
{
    /*
    //GUISkin
    private GUISkin mGUISkin = null;
    public GUISkin GUISkin { get { return mGUISkin; } }

    //GUIContent
    private GUIContent mGuiContent = null;
    public GUIContent GUIContent { get { return mGuiContent; } set { mGuiContent = value; } }
    
    private Color mOriginalColor;
     */
    
    public enum GUICallPriority { top, background,}
    public delegate void OnGUICallback();
    
    //2 thread of GUICalls
    private List<OnGUICallback> onTopGUICalls = new List<OnGUICallback>();
    public List<OnGUICallback> TopGUICalls { get { return onTopGUICalls; } }
    private List<OnGUICallback> onBackgroundGUICalls = new List<OnGUICallback>();
    public List<OnGUICallback> BackgourndGUICalls { get { return onBackgroundGUICalls; } }
    
    /*
    public MyGUI(GUISkin guiSkin)
    {
        if(guiSkin == null)
        {
            setDefault();
        }
        else
        {
            mGUISkin = guiSkin;
        }
    }

    public MyGUI()
    {
        setDefault();
    }

    private void setDefault()
    {
        mGUISkin = MyGUIBehavior.Instance.skin;
        //mGuiContent = MyGUIBehavior.Instance.mGuiContent;
        //mOriginalColor = MyGUIBehavior.DefaultMyGUI.mOriginalColor;
    }
     */

    /// <summary>
    /// Add GUICall
    /// </summary>
    /// <param name="guiCallback"></param>
    /// <param name="callPriority"></param>
    public  void AddGUICall(OnGUICallback guiCallback, GUICallPriority callPriority)
    {
        switch (callPriority)
        {
            case GUICallPriority.top:
                AddTopGUICall(guiCallback);
                break;
            case GUICallPriority.background:
                AddBackgroundGUICall(guiCallback);
                break;
            default:
                AddTopGUICall(guiCallback);
                break;
        }
    }

    protected void AddTopGUICall(OnGUICallback guiCallback)
    {
        onTopGUICalls.Add(guiCallback);
    }

    protected void AddBackgroundGUICall(OnGUICallback guiCallback)
    {
        onBackgroundGUICalls.Add(guiCallback);
    }

    /// <summary>
    /// Remove GUI call
    /// </summary>
    /// <param name="guiCallback"></param>
    /// <param name="callPriority"></param>
    public void RemoveGUICall(OnGUICallback guiCallback, GUICallPriority callPriority)
    {
        DebugExt.Log("  RemoveGUICall in myGUI - " + this.GetHashCode());
        switch (callPriority)
        {
            case GUICallPriority.top:
                RemoveTopGUICall(guiCallback);
                break;
            case GUICallPriority.background:
                RemoveBackgroundGUICall(guiCallback);
                break;
            default:
                RemoveTopGUICall(guiCallback);
                break;
        }
    }

    protected void RemoveTopGUICall(OnGUICallback guiCallback)
    {
        onTopGUICalls.Remove(guiCallback);
    }

    protected void RemoveBackgroundGUICall(OnGUICallback guiCallback)
    {
        onBackgroundGUICalls.Remove(guiCallback);
    }
}