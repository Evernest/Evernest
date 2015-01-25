using System;
using System.Collections.Generic;
using EvernestFront;
using EvernestFront.Contract;
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
        /// <summary>
        /// Associate strings right names to corresponding Front objets
        /// </summary>
        public static readonly Dictionary<string, AccessRight> AccessRightsDictionary = new Dictionary<string, AccessRight>
        {
            {"ReadOnly",  AccessRight.ReadOnly },
            {"WriteOnly", AccessRight.WriteOnly},
            {"ReadWrite", AccessRight.ReadWrite},
            {"Admin",     AccessRight.Admin    },
            {"Root",      AccessRight.Root     },

        };

        public static ModelStream ModelStreamFromStream(EventStream e)
        {
            if (e != null)
                return new ModelStream
                {
                    UserRight = e.UserRight,
                    Id = e.Id,
                    Name = e.Name,
                    Count = e.Count,
                    LastEventId = e.LastEventId
                };
            return null;
        }

        public static ModelSource ModelSourceFromSource(Source s)
        {
            if (s != null)
                return new ModelSource
                {
                    Name = s.Name,
                    Id = s.Id
                };
            return null;
        }

        public static EventModel EventModelFromEvent(Event e)
        {
            if (e != null)
                return new EventModel
                {
                    Id = e.Id,
                    Message = e.Message
                };
            return null;
        }
    }
}