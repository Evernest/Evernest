using System;
using System.Collections.Generic;
using System.Diagnostics;
using EvernestFront.SystemCommandHandling;
using EvernestFront.Contract;
using EvernestFront.Projections;

namespace EvernestFront
{
    public class SourcesProvider
    {
        private readonly SourcesProjection _sourcesProjection;

        private readonly SystemCommandHandler _systemCommandHandler;

        public SourcesProvider()
        {
            _sourcesProjection = Injector.Instance.SourcesProjection;
            _systemCommandHandler = Injector.Instance.SystemCommandHandler;
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
            SourceRecord sourceData;
            if (!_sourcesProjection.TryGetSourceData(sourceKey, out sourceData))
            {
                source = null;
                error = FrontError.SourceKeyDoesNotExist;
                return false;
            }

            var usersBuilder = new UsersProvider();
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
