using System;

namespace EvernestFront
{
    partial class User
    {
        //TODO: redefine sources

        public Response<Guid> CreateSource(string sourceName, long streamId, AccessRight rights)
        {
            throw new NotImplementedException();
        }



        public Response<Guid> DeleteSource(string sourceName)
        {throw new NotImplementedException();
        }
    }
}
