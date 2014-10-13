namespace EvernestFront.Requests
{
  
        interface IRequest
        {
            string ToString();

            /// <summary>
            /// Processes the request with a back-end call.
            /// </summary>
            /// <returns></returns>
            Answers.IAnswer Process();
        } 
    
}
