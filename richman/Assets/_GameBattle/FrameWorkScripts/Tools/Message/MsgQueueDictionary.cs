//============================================================================================================
// (c) Ganz  -  This code is exclusively the property of Ganz and all rights
// therein, including without limitation copyrights and trade secrets, are
// reserved by Ganz.  Any reproduction, disclosure, distribution or 
// unauthorized use is strictly prohibited.
//============================================================================================================
/// Add and record all msgqueues.
//============================================================================================================
// Created on 6/27/2011 2:26:06 PM by haoh
//============================================================================================================

using System.Collections.Generic;
using UnityEngine;

public static class MsgQueueDictionary
{
    //MsgQueueID for all msg;
    //Keep less msgQueue instances
    public enum MsgQueueID
    {
        Global_MsgQueue,
        AssetBundleInternal_MsgQueue,
        MMMainPlayer_MsgQuque,
        MMTestGUI_MsgQuque,
		MMGUI_MsgQueue
    }

    //MsgQueueDictionary
    private static Dictionary<MsgQueueID, MsgQueue> mMsgQueueDictionary = new Dictionary<MsgQueueID, MsgQueue>();

    //Global Msgqueue
    public static MsgQueue GlobalMsgQueue
    {
        get
        {
            return mGlobalMsgQueue;
        }
    }
    private static MsgQueue mGlobalMsgQueue = null;

    /// <summary>
    /// constructor
    /// </summary>
    static MsgQueueDictionary()
    {
        mGlobalMsgQueue = AddMsgQueue(MsgQueueID.Global_MsgQueue);        //Global MsgQueue
    }

    /// <summary>
    /// Add a msgqueue, if it exists, return it, otherwise, add a new one;
    /// </summary>
    public static MsgQueue AddMsgQueue(MsgQueueID msgQueueId)
    {
        if(mMsgQueueDictionary == null)
            mMsgQueueDictionary = new Dictionary<MsgQueueID, MsgQueue>();

        if (mMsgQueueDictionary.ContainsKey(msgQueueId))
            return mMsgQueueDictionary[msgQueueId];
        
        //Add msgqueue
        MsgQueue msgQueue = new MsgQueue(msgQueueId);
        mMsgQueueDictionary.Add(msgQueueId, msgQueue);

        return msgQueue;
    }
}