using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.Projections
{
    internal interface IProjection
    {

        void OnSystemEvent(ISystemEvent systemEvent);
    }
}
