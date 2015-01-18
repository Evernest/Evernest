using System;
using System.Collections.Generic;
using System.Diagnostics;
using EvernestFront.CommandHandling;
using EvernestFront.Projections;

namespace EvernestFront
{
    class SourcesBuilder
    {
        private readonly SourcesProjection _sourcesProjection;

        private readonly CommandHandler _commandHandler;

        public SourcesBuilder()
        {
            _sourcesProjection = Injector.Instance.SourcesProjection;
            _commandHandler = Injector.Instance.CommandHandler;
        }

        public Response<Source> GetSource(string sourceKey)
        {
            Source source;
            FrontError? error;
            if (TryGetSource(sourceKey, out source, out error))
                return new Response<Source>(source);
            else
            {
                Debug.Assert(error != null, "error != null");
                return new Response<Source>((FrontError)error);
            }
        }

        internal bool TryGetSource(string sourceKey, out Source source, out FrontError? error)
        {
            SourceDataForProjection sourceData;
            if (!_sourcesProjection.TryGetSourceData(sourceKey, out sourceData))
            {
                source = null;
                error = FrontError.SourceKeyDoesNotExist;
                return false;
            }

            var usersBuilder = new UsersBuilder();
            User user;
            if (!usersBuilder.TryGetUser(sourceData.UserId, out user))
            {
                source = null;
                error = FrontError.UserOwningSourceDoesNotExist;
                return false;
            }

            source = new Source(sourceKey, sourceData.SourceName, sourceData.SourceId, user, sourceData.RelatedEventStreams);
            error = null;
            return true;
        }
    }
}
