/// <summary>
/// Used to create a message. 
/// </summary>
public interface IMessage
{
    /// <summary>
    /// A string of message, it is the id of the message.
    /// </summary>
    string message
    {
        get;
        set;
    }

    /// <summary>
    /// An argument that was also used to identify which caller should 
    /// be notified when message comes.Only those listener who has the
    /// same argument registered will be called.
    /// </summary>
    object argument
    {
        get;
        set;
    }

    /// <summary>
    /// Callback results, can be anything.
    /// </summary>
    object result
    {
        get;
        set;
    }
}
