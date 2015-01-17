namespace EvernestFront.Projections
{
    class SourceDataForProjection
    {
        
        internal string SourceName { get; private set; }

        internal long UserId { get; private set; }

        internal string UserName { get; private set; }

        internal long StreamId { get; private set; }

        internal AccessRight Right { get; private set; }

        internal SourceDataForProjection(string sourceName, long userId, string userName, long streamId, AccessRight right)
        {
            SourceName = sourceName;
            UserId = userId;
            UserName = userName;
            StreamId = streamId;
            Right = right;
        }
    }
}
