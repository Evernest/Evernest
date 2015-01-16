namespace EvernestFront.Errors
{
    public class UserIdDoesNotExist : FrontError
    {
        public long Id { get; private set; }

        internal UserIdDoesNotExist(long id)
        {
            Id = id;
        }

    }
}
