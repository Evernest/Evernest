
namespace EvernestFront
{
    public class StreamInfo
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Count { get; private set; }
        public int LastEventId { get; private set; }

        internal StreamInfo(int id, string name, int count, int lastEventId)
        {
            Id = id;
            Name = name;
            Count = count;
            LastEventId = lastEventId;
        }

        
    }
}
