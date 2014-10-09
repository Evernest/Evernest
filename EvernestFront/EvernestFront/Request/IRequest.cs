namespace EvernestFront.Request
{
  
        interface IRequest
        {
            string ToString();

            IAnswer Process();
        } 
    
}
