﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EvernestWeb.Models;
using EvernestWeb.ViewModels;

namespace EvernestWeb.Controllers
{
    public class StoreController : Controller
    {
        private Connexion IsConnected()
        {
            ViewBag.Connexion = "false";
            Connexion connexion = new Connexion();
            if (connexion.IsConnected())
            {
                ViewBag.Connexion = "true";
                ViewBag.Username = connexion.Username;
            }
            return connexion;
        }

        // GET: Store
        public ActionResult Index()
        {
            IsConnected();

            return View();
        }

        private StreamsSources getStreamsSources(EvernestFront.Responses.GetUserResponse u)
        {
            List<KeyValuePair<long, EvernestFront.AccessRights>> listStreams = u.User.RelatedEventStreams;
            List<KeyValuePair<string, string>> listSources = u.User.Sources;
            StreamsSources streamsSources = new StreamsSources();
            foreach (KeyValuePair<long, EvernestFront.AccessRights> elt in listStreams)
            {
                EvernestFront.Responses.GetEventStreamResponse s = EvernestFront.EventStream.GetStream(elt.Key);
                if (s.Success)
                    streamsSources.AddEventStream(s.EventStream);
            }
            foreach (KeyValuePair<string, string> src in listSources)
            {
                EvernestFront.Responses.GetSourceResponse s = EvernestFront.Source.GetSource(src.Value); // the second string is the Key to fetch the source
                if (s.Success)
                    streamsSources.AddSource(s.Source);
            }

            return streamsSources;
        }

        public ActionResult MyStore()
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion != "true")
                return View("Index");

            EvernestFront.Responses.GetUserResponse u = EvernestFront.User.GetUser(connexion.IdUser);
            if (u.Success)
            {
                StreamsSources streamsSources = getStreamsSources(u);
                return View(streamsSources);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStream(string addStream) // add stream
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion != "true")
                return View("Index");

            EvernestFront.Responses.GetUserResponse u = EvernestFront.User.GetUser(connexion.IdUser);
            if (u.Success)
                if (addStream != null)
                {
                    EvernestFront.Responses.CreateEventStream stream = u.User.CreateEventStream(addStream);
                    if (stream.Success)
                    {
                        // update user object
                        u = EvernestFront.User.GetUser(connexion.IdUser);
                        if (u.Success)
                        {
                            StreamsSources streamsSources = getStreamsSources(u);
                            return RedirectToAction("MyStore", "Store", new RouteValueDictionary(streamsSources));
                        }
                    }
                }
            return View("Index");
        }

        public EvernestFront.AccessRights StringToAccessRights(string accessRights)
        {
            EvernestFront.AccessRights accessRights_ = EvernestFront.AccessRights.NoRights; // something by default
            switch (accessRights)
            {
                case "NoRights": accessRights_ = EvernestFront.AccessRights.NoRights; break;
                case "ReadOnly": accessRights_ = EvernestFront.AccessRights.ReadOnly; break;
                case "WriteOnly": accessRights_ = EvernestFront.AccessRights.WriteOnly; break;
                case "ReadWrite": accessRights_ = EvernestFront.AccessRights.ReadWrite; break;
                case "Admin": accessRights_ = EvernestFront.AccessRights.Admin; break;
                case "Root": accessRights_ = EvernestFront.AccessRights.Root; break;
            }
            return accessRights_;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSource(string addSource, string idStream, string accessRights) // add source
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion != "true")
                return View("Index");

            EvernestFront.Responses.GetUserResponse u = EvernestFront.User.GetUser(connexion.IdUser);
            if (u.Success)
                if (addSource != null)
                {
                    long idStream_ = Convert.ToInt64(idStream);
                    EvernestFront.AccessRights accessRights_ = StringToAccessRights(accessRights);
                    EvernestFront.Responses.CreateSource source = u.User.CreateSource(addSource, idStream_, accessRights_);
                    if (source.Success)
                    {
                        // update user object
                        u = EvernestFront.User.GetUser(connexion.IdUser);
                        if (u.Success)
                            return RedirectToAction("MyStore", "Store");
                    }
                }
            return View("Index");
        }

        private StreamAndEvents getStreamsAndEvents(long streamId, long userId)
        {
            EvernestFront.Responses.GetEventStreamResponse s = EvernestFront.EventStream.GetStream(streamId);
            EvernestFront.Responses.GetUserResponse u = EvernestFront.User.GetUser(userId);
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

                EvernestFront.Responses.PullRangeResponse r = u.User.PullRange(streamId, begin, s.EventStream.LastEventId);
                streamAndEvents.Events = r.Events;

                return streamAndEvents;
            }
            return null;
        }

        public ActionResult Stream(long id)
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion != "true")
                return View("Index");
            StreamAndEvents streamAndEvents = getStreamsAndEvents(id, connexion.IdUser);
            return View(streamAndEvents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PushEvent(string item, int sid)
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion != "true")
                return View("Index");

            if (item != null)
            {
                EvernestFront.Responses.GetEventStreamResponse s = EvernestFront.EventStream.GetStream(sid);
                if (s.Success)
                {
                    EvernestFront.Responses.GetUserResponse u = EvernestFront.User.GetUser(connexion.IdUser);
                    if (u.Success)
                        u.User.Push(s.EventStream.Id, item);
                }
            }
            return RedirectToAction("Stream", "Store", new { id = sid });
        }
    }
}