using System;
using EvernestFront.CommandHandling.Commands;
using EvernestFront.Utilities;

namespace EvernestFront
{
    partial class User
    {
        public Response<Source> GetSource(string sourceName)
        {
            throw new NotImplementedException();
        }

        public Response<Source> GetSource(long sourceId)
        {
            throw new NotImplementedException();
        }

        public Response<Tuple<string, Guid>> CreateSource(string sourceName)
        {
            if (InternalSources.ContainsKey(sourceName))
                return new Response<Tuple<string, Guid>>(FrontError.SourceNameTaken);
            var keyGenerator = new KeyGenerator();
            var key = keyGenerator.NewKey();
            var command = new SourceCreation(_commandHandler, Id, sourceName, key);
            command.Send();
            return new Response<Tuple<string, Guid>>(new Tuple<string, Guid>(key, command.Guid));
        }


        public Response<Guid> SetSourceRight(long sourceId, long streamId)
        {
            throw new NotImplementedException();
        }

        public Response<Guid> DeleteSource(long sourceId)
        {
            throw new NotImplementedException();
        } 
    }
}
