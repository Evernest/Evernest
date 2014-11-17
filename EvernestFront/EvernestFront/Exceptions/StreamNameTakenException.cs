
namespace EvernestFront.Exceptions
{
    public class StreamNameTakenException : FrontException
    {
        public string StreamName { get; private set; }
        /// <summary>
        /// Constructor for StreamNameTakenException.
        /// </summary>
        /// <param name="name"></param>
        internal StreamNameTakenException(string name)
        {
            StreamName = name;
        }

    }
}
