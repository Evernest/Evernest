using System;
using System.Collections.Generic;
using System.Diagnostics;
using EvernestFront.SystemCommandHandling;
using EvernestFront.Contract;
using EvernestFront.Projections;

namespace EvernestFront
{
    public class SourceProvider
    {
        internal readonly SourcesProjection SourcesProjection;

        internal readonly EventStreamProvider EventStreamProvider;

        public SourceProvider()
        {
            var sourceProvider = Injector.Instance.SourceProvider;
            SourcesProjection = sourceProvider.SourcesProjection;
            EventStreamProvider = sourceProvider.EventStreamProvider;
        }

        internal SourceProvider(SourcesProjection sourcesProjection, EventStreamProvider eventStreamProvider)
        {
            SourcesProjection = sourcesProjection;
            EventStreamProvider = eventStreamProvider;
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
            if (!SourcesProjection.TryGetSourceData(sourceKey, out sourceData))
            {
                source = null;
                error = FrontError.SourceKeyDoesNotExist;
                return false;
            }

            var usersBuilder = new UserProvider();
            User user;
            if (!usersBuilder.TryGetUser(sourceData.UserId, out user))
            {
                source = null;
                error = FrontError.UserOwningSourceDoesNotExist;
                return false;
            }

            source = new Source(EventStreamProvider, sourceKey, sourceData.SourceName, sourceData.SourceId, user, sourceData.RelatedEventStreams);
            error = null;
            return true;
        }
    }
}
