namespace EvernestFront.Requests
{
  
        interface IRequest
        {
            string ToString();

            IAnswer Process();
        } 
    
}
