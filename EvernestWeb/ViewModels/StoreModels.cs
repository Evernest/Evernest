using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EvernestWeb.ViewModels
{
    public class StoreModels
    {
    }

    public class StreamsSources
    {
        public List<EvernestFront.EventStream> Streams { get; set; }
        public List<EvernestFront.Source> Sources { get; set; }

        public StreamsSources()
        {
            Streams = new List<EvernestFront.EventStream>();
            Sources = new List<EvernestFront.Source>();
        }

        public void AddEventStream(EvernestFront.EventStream es)
        {
            Streams.Add(es);
        }

        public void AddSource(EvernestFront.Source s)
        {
            Sources.Add(s);
        }
    }

    public class StreamAndEvents
    {
        // Stream
        public long Id { get; set; }
        public string Name { get; set; }
        public long Count { get; set; }
        public long LastEventId { get; set;}
        public List<KeyValuePair<string, EvernestFront.AccessRight>> RelatedUsers { get; set; }

        // Events
        public List<EvernestFront.Event> Events;
    }
}