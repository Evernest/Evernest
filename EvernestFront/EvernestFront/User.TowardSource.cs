using System;
using System.Diagnostics;
using EvernestFront.CommandHandling.Commands;
using EvernestFront.Contract;
using EvernestFront.Utilities;

namespace EvernestFront
{
    partial class User
    {
        public Response<Source> GetSource(string sourceName)
        {
            long sourceId;
            if (!SourceNameToId.TryGetValue(sourceName, out sourceId))
                return new Response<Source>(FrontError.SourceNameDoesNotExist);
            return GetSource(sourceId);
        }

        public Response<Source> GetSource(long sourceId)
        {
            string sourceKey;
            if (!SourceKeys.TryGetValue(sourceId, out sourceKey))
                return new Response<Source>(FrontError.SourceIdDoesNotExist);

            var builder = new SourcesBuilder();
            Source source;
            FrontError? error;
            if (builder.TryGetSource(sourceKey, out source, out error))
                return new Response<Source>(source);
            else
            {
                Debug.Assert(error != null, "error != null");
                return new Response<Source>((FrontError) error);
            }
        }

        public Response<Tuple<string, Guid>> CreateSource(string sourceName)
        {
            if (SourceNameToId.ContainsKey(sourceName))
                return new Response<Tuple<string, Guid>>(FrontError.SourceNameTaken);
            var keyGenerator = new KeyGenerator();
            var key = keyGenerator.NewKey();
            var command = new SourceCreationCommand(_commandHandler, Id, sourceName, key);
            command.Send();
            return new Response<Tuple<string, Guid>>(new Tuple<string, Guid>(key, command.Guid));
        }


        public Response<Guid> SetSourceRight(long sourceId, long eventStreamId, AccessRight right)
        {
            string sourceKey;
            if (!SourceKeys.TryGetValue(sourceId, out sourceKey))
                return new Response<Guid>(FrontError.SourceIdDoesNotExist);

            var command = new SourceRightSettingCommand(_commandHandler, Id, sourceId, sourceKey,
                eventStreamId, right);
            command.Send();
            return new Response<Guid>(command.Guid);
        }

        public Response<Guid> DeleteSource(long sourceId)
        {
            string sourceKey;
            if (!SourceKeys.TryGetValue(sourceId, out sourceKey))
                return new Response<Guid>(FrontError.SourceIdDoesNotExist);

            var command = new SourceDeletionCommand(_commandHandler, Id, sourceId, sourceKey);
            command.Send();
            return new Response<Guid>(command.Guid);
        } 
    }
}
