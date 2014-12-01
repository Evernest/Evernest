
namespace EvernestFront.Errors
{
    public class EventStreamNameTaken : FrontError
    {
        public string StreamName { get; private set; }

        internal EventStreamNameTaken(string name)
        {
            StreamName = name;
        }

    }
}
