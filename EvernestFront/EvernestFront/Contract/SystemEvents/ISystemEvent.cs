namespace EvernestFront.Contract.SystemEvents
{
    /// <summary>
    /// A system event represents an administrative action (user/stream/source creation or deletion for instance) that has already occured.
    /// It is sent to the projections so that they are updated, and stored in a system event stream so that the system state can be rebuilt after a shutdown.
    /// This empty interface is implemented by all system events for polymorphism purposes.
    /// </summary>
    internal interface ISystemEvent
    {
    }
}
