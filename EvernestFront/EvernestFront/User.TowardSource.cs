using System;
using EvernestFront.Responses;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront
{
    partial class User
    {
        //TODO: redefine sources

        public SystemCommandResponse CreateSource(string sourceName, long streamId, AccessRight rights)
        {
            throw new NotImplementedException();
        }



        public SystemCommandResponse DeleteSource(string sourceName)
        {throw new NotImplementedException();
        }
    }
}
