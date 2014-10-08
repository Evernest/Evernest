namespace EvernestFront.Request
{
  
        public abstract class Request : IRequest
        {
            private string user;

            private string streamName;

            protected Request(string user, string streamName)
            {
                this.user = user;
                this.streamName = streamName;
            }

            public abstract IAnswer Process();
           
            
        } 
    
}
