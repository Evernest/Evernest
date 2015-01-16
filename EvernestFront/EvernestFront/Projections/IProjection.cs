using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Projections
{
    internal interface IProjection
    {

        void OnSystemEvent(ISystemEvent systemEvent);
    }
}
