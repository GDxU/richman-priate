using UnityEngine;
using System.Collections.Generic;

public class Messages
{
    public const string CreateItem = "create_item";
    public const string DestoryItem = "destory_item";
    public const string ItemCreated = "item_created";
    public const string Gesture = "gesture";

    public const string MMForceController_Channel = "MMForceController";
    public const string MMForceController_MoveLeft = "Left";
    public const string MMForceController_MoveRight = "Right";
    public const string MMForceController_MoveForward = "Forward";
    public const string MMForceController_Jump = "Jump";
    public const string MMForceController_SuperJump = "SuperJump";
    //public const string MMForceController_PauseAndPlay = "PauseAndPlay";
    public const string MMForceController_Boost = "Boost";
    public const string MMForceController_GameOver = "GameOver";
	
	public const string MMAI_MoveFinish = "MovingFinished";
	public const string MMGUI_Message = "MMGUI";
	
	public const string MMGame_Quit = "gamequit";
	public const string MMGame_Start = "gamestart";
	public const string MMGame_Run = "gamerun";
	public const string MMGame_Launched = "gamelaunched";
	public const string MMGame_ShowMainUI = "showMainUI";
    public const string MMGame_FinishGame = "GameFinish";
	
	public const string MMCamera_Switch = "switchCamera";
	public const string MMSelect_Next = "selectNext";
	public const string MMSelect_Previous = "selectPrevious";
}

/// <summary>
/// A message queue who transfers messages
/// </summary>
public class MsgQueue
{
    /// <summary>
    /// Listener function delegate. Accept IMessage as argument.
    /// </summary>
    /// <param name="val">the value of IMessage</param>
    /// <returns>true: means continue to listen. false: not listen anymore.</returns>
    public delegate bool Callback(IMessage val);

    public MsgQueue(MsgQueueDictionary.MsgQueueID msgId)
    {
        if(mMessageListeners == null)
            mMessageListeners = new Dictionary<string, List<Listener>>();
    }

    /// <summary>
    /// An internal message class who implemented an Imessage interface.
    /// </summary>
    private class Message : IMessage
    {
        public Message(string msg, object arg)
        {
            message = msg;
            argument = arg;
            result = null;
        }
        public string message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }

        public object argument
        {
            get { return mArg; }
            set { mArg = value; }
        }

        public object result
        {
            get { return mResult; }
            set { mResult = value; }
        }
        private string mMessage;
        private object mArg;
        private object mResult;
    }

    /// <summary>
    /// An internal Listener class that has listener callback function and message.
    /// </summary>
    private class Listener
    {
        public Listener(Callback func, string message, object arg)
        {
            mMessage = new Message(message, arg);
			mCallback = func;
        }
        public IMessage mMessage;
        public Callback mCallback = null;
    }
	
	private Dictionary<string, List<Listener>> mMessageListeners = null;
    //private List<KeyValuePair<string, object>> mMessageQueue = new List<KeyValuePair<string, object>>();

    /// <summary>
    /// Add a listener without arguments. It will get called any time when same message id be submitted.
    /// </summary>
    /// <param name="message">message id string</param>
    /// <param name="func">callback listener</param>
    public void AddListener(string message, Callback func)
    {
        AddListener(message, func, null);
    }

    /// <summary>
    /// Add a listener with arguments. It will get called only when message has the same id and argument.
    /// </summary>
    /// <param name="message">message id string</param>
    /// <param name="func">callback listener</param>
    /// <param name="arg">argument from caller</param>
    public void AddListener(string message, Callback func, object arg)
    {
        Listener listener = new Listener(func, message, arg);
        if (!mMessageListeners.ContainsKey(message))
            mMessageListeners[message] = new List<Listener>();
        mMessageListeners[message].Add(listener);
    }

    /// <summary>
    /// remove listener of message without argument.
    /// </summary>
    /// <param name="message">message id string</param>
    /// <param name="func">callback listener</param>
    public void RemoveListener(string message, Callback func)
    {
        RemoveListener(message, func, null);
    }

    /// <summary>
    /// remove listener of message with argument.
    /// </summary>
    /// <param name="message">message id string</param>
    /// <param name="func">callback listener</param>
    /// <param name="arg">argument from listener</param>
    public void RemoveListener(string message, Callback func, object arg)
    {
        if (mMessageListeners.ContainsKey(message))
        {
            mMessageListeners[message].RemoveAll(x => x.mCallback == func && x.mMessage.argument == arg);
        }
    }

    /// <summary>
    /// submit a message
    /// </summary>
    /// <param name="message">message id string</param>
    /// <param name="dvalue">message value</param>
    public void SendMessage(string message, object dvalue)
    {
        if (mMessageListeners.ContainsKey(message))
        {
			SendMessage(message, dvalue, null);
        }
    }

    /// <summary>
    /// submit a message with caller's argument.
    /// </summary>
    /// <param name="message">message id string</param>
    /// <param name="dvalue">message value</param>
    /// <param name="argument">caller's argument</param>
    public void SendMessage(string message, object dvalue, object argument)
    {
        if (mMessageListeners.ContainsKey(message))
        {
            List<Listener> list = mMessageListeners[message];
			for (int i = 0; i < list.Count;)
            {
                Listener listener = list[i];
				listener.mMessage.result = dvalue;
				if( listener.mCallback != null)
				{
					if( listener.mMessage.argument == null)
					{
						listener.mMessage.argument = argument;
						if(!listener.mCallback(listener.mMessage))
							list.Remove(listener);
						else
						{
							++i;
							listener.mMessage.argument = null;
						}
					}
					else if( listener.mMessage.argument == argument)
					{
					   	if(!listener.mCallback(listener.mMessage))
							list.Remove(listener);
						else
							++i;
					}
					else
					{
						++i;
					}
				}
				else
				{
					++i;
					DebugExt.LogWarning("MsgQueue Listener lose callback funcition");
				}
            }
        }
    }
}
