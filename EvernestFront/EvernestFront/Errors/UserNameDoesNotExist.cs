namespace EvernestFront.Errors
{
    public class UserNameDoesNotExist : FrontError
    {
        public string Name { get; private set; }

        internal UserNameDoesNotExist(string name)
        {
            Name = name;
        }
    
    }
}
