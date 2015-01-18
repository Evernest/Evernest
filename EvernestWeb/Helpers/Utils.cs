using System;
using System.Collections.Generic;
using EvernestFront;
using EvernestWeb.ViewModels;

namespace EvernestWeb.Helpers
{
    /// <summary>
    /// Misc static utility functions dealing with Front API.
    /// To be refactored.
    /// </summary>
    public class Utils
    {
        /*
        public static StreamsSources getStreamsSources(EvernestFront.Answers.GetUser u)
        {
            List<KeyValuePair<long, AccessRights>> listStreams = u.User.RelatedEventStreams;
            List<KeyValuePair<string, string>> listSources = u.User.Sources;
            StreamsSources streamsSources = new StreamsSources();
            foreach (var elt in listStreams)
            {
                EvernestFront.Answers.GetEventStream s = EventStream.GetStream(elt.Key);
                if (s.Success)
                    streamsSources.AddEventStream(s.EventStream);
            }
            foreach (var src in listSources)
            {
                EvernestFront.Answers.GetSource s = Source.GetSource(src.Value); // the second string is the Key to fetch the source
                if (s.Success)
                    streamsSources.AddSource(s.Source);
            }

            return streamsSources;
        }
        */


        public static StreamAndEvents getStreamsAndEvents(long streamId, long userId)
        {
            EvernestFront.Answers.GetEventStream s = EvernestFront.EventStream.GetStream(streamId);
            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(userId);
            if (s.Success && u.Success)
            {
                // fetch stream
                StreamAndEvents streamAndEvents = new StreamAndEvents();
                streamAndEvents.Id = s.EventStream.Id;
                streamAndEvents.Name = s.EventStream.Name;
                streamAndEvents.Count = s.EventStream.Count;
                streamAndEvents.LastEventId = s.EventStream.LastEventId;
                streamAndEvents.RelatedUsers = s.EventStream.RelatedUsers;

                // fetch stream's events
                int begin = 0;
                if (s.EventStream.LastEventId > 10)
                    begin = Convert.ToInt32(s.EventStream.LastEventId) - 10;

                EvernestFront.Answers.PullRange r = u.User.PullRange(streamId, begin, s.EventStream.LastEventId);
                streamAndEvents.Events = r.Events;

                return streamAndEvents;
            }
            return null;
        }

    }
}