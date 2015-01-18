
namespace EvernestFront.Contract.SystemEvent
{
    //TODO: add contract as soon as FrontError becomes an enum
    class InvalidCommandSystemEvent : ISystemEvent
    {
        internal FrontError Error { get; private set; }

        internal InvalidCommandSystemEvent(FrontError error)
        {
            Error = error;
        }
    }
}
