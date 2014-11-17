
namespace EvernestFront.Errors
{
    public class StreamNameTaken : FrontError
    {
        public string StreamName { get; private set; }
        /// <summary>
        /// Constructor for StreamNameTakenException.
        /// </summary>
        /// <param name="name"></param>
        internal StreamNameTaken(string name)
        {
            StreamName = name;
        }

    }
}
