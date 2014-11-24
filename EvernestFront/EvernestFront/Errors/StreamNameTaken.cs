
namespace EvernestFront.Errors
{
    public class StreamNameTaken : FrontError
    {
        public string StreamName { get; private set; }

        internal StreamNameTaken(string name)
        {
            StreamName = name;
        }

    }
}
