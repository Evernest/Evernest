using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.Projections
{
    /// <summary>
    /// A projection is a set of data which is updated exclusively by receiving system events.
    /// </summary>
    internal interface IProjection
    {
        /// <summary>
        /// Upadtes the projection to take the system event into account.
        /// </summary>
        /// <param name="systemEvent"></param>
        void OnSystemEvent(ISystemEvent systemEvent);
    }
}
