using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EvernestFront;
using EvernestFront.Contract;
using EvernestWeb.Helpers;
using EvernestWeb.ViewModels;

namespace EvernestWeb.Models
{
    /// <summary>
    /// Data displayed in main manager page
    /// </summary>

    public class ModelStream
    {
        public AccessRight UserRight { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long Count { get; set; }
        public long LastEventId { get; set; }
    }

    public class ModelSource
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
    
    public class ManagerModel
    {
        public IEnumerable<ModelStream> Streams { get; set; }
        public IEnumerable<ModelSource> Sources { get; set; }

        public ManagerModel(EvernestFront.User u)
        {
            IEnumerable<EventStream> es = u.RelatedEventStreams.Select(x => u.GetEventStream(x).Result);
            IEnumerable<Source> sr = u.Sources.Select(x => u.GetSource(x).Result);
            Streams = es.Select(x => Utils.ModelStreamFromStream(x));
            Sources = sr.Select(x => Utils.ModelSourceFromSource(x));
        }
    }

    /// <summary>
    /// Data displayed in stream manager page
    /// </summary>

    public class EventModel
    {
        public long Id { get; set; }
        public string Message { get; set; }
    }
    
    public class StreamEventsModel
    {
        // Stream
        public ModelStream Stream { get; set; }

        // RelatedUsers
        public IDictionary<string, AccessRight> RelatedUsers { get; set; }

        // Pulled Event
        public EventModel PulledEvent { get; set; }

        // Last 10 Events
        public IEnumerable<EventModel> Events { get; set; }

        public StreamEventsModel(EvernestFront.User u, long streamId)
        {
            EventStream es = u.GetEventStream(streamId).Result;
            Stream = Utils.ModelStreamFromStream(es);
            RelatedUsers = es.GetRelatedUsers().Result;
            long begin = es.LastEventId - 10 < 0 ? 0 : es.LastEventId - 10;
            long end = es.LastEventId;
            List<Event> le = es.PullRange(begin, end).Result;
            if (le != null && le.Count() > 0)
                Events = le.Select(x => Utils.EventModelFromEvent(x));
        }
    }
}