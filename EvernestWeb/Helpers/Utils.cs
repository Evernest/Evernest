using System;
using System.Collections.Generic;
using EvernestFront;
using EvernestWeb.Models;
using EvernestWeb.ViewModels;

namespace EvernestWeb.Helpers
{
    /// <summary>
    /// Misc static utility functions dealing with Front API.
    /// To be refactored.
    /// </summary>
    public class Utils
    {
        public static ModelStream ModelStreamFromStream(EventStream e)
        {
            return new ModelStream
            {
                UserRight = e.UserRight,
                Id = e.Id,
                Name = e.Name,
                Count = e.Count,
                LastEventId = e.LastEventId
            };
        }

        public static ModelSource ModelSourceFromSource(Source s)
        {
            return new ModelSource
            {
                Name = s.Name,
                Id = s.Id
            };
        }

        public static EventModel EventModelFromEvent(Event e)
        {
            return new EventModel
            {
                Id = e.Id,
                Message = e.Message
            };
        }
    }
}