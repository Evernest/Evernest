namespace EvernestFront.Requests
{
  
        public abstract class Request : IRequest
        {
            protected string User;

            protected string StreamName;

            protected Request(string user, string streamName)
            {
                this.User = user;
                this.StreamName = streamName;
            }

            public abstract IAnswer Process();
           
            
        } 
    
}
