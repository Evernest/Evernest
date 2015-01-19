namespace EvernestBack
{
    /// <summary>The IAgent interface represents a transaction with a Stream. It contains a message and a unique ID.</summary>
    public interface IAgent
    {
        string Message { get; }
        long RequestID { get; }
    }
}
