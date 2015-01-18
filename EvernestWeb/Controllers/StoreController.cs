using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EvernestFront;
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

        private StreamsSources getStreamsSources(EvernestFront.Response<User> u)
        {
            List<KeyValuePair<long, EvernestFront.AccessRight>> listStreams = u.Result.RelatedEventStreams;
            List<KeyValuePair<string, string>> listSources = u.Result.Sources;
            StreamsSources streamsSources = new StreamsSources();
            foreach (KeyValuePair<long, EvernestFront.AccessRight> elt in listStreams)
            {
                EvernestFront.Response<EventStream> s = EvernestFront.EventStream.GetStream(elt.Key);
                if (s.Success)
                    streamsSources.AddEventStream(s.Result);
            }
            foreach (KeyValuePair<string, string> src in listSources)
            {
                EvernestFront.Response<Source> s = EvernestFront.Source.GetSource(src.Value); // the second string is the Key to fetch the source
                if (s.Success)
                    streamsSources.AddSource(s.Result);
            }

            return streamsSources;
        }

        public ActionResult MyStore()
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion != "true")
                return View("Index");

            EvernestFront.Response<User> u = EvernestFront.User.GetUser(connexion.IdUser);
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

            EvernestFront.Response<User> u = EvernestFront.User.GetUser(connexion.IdUser);
            if (u.Success)
                if (addStream != null)
                {
                    EvernestFront.Response<Guid> stream = u.Result.CreateEventStream(addStream);
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

        public EvernestFront.AccessRight StringToAccessRights(string accessRights)
        {
            EvernestFront.AccessRight accessRights_ = EvernestFront.AccessRight.NoRight; // something by default
            switch (accessRights)
            {
                case "NoRights": accessRights_ = EvernestFront.AccessRight.NoRight; break;
                case "ReadOnly": accessRights_ = EvernestFront.AccessRight.ReadOnly; break;
                case "WriteOnly": accessRights_ = EvernestFront.AccessRight.WriteOnly; break;
                case "ReadWrite": accessRights_ = EvernestFront.AccessRight.ReadWrite; break;
                case "Admin": accessRights_ = EvernestFront.AccessRight.Admin; break;
                case "Root": accessRights_ = EvernestFront.AccessRight.Root; break;
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
            EvernestFront.Response<User> u = EvernestFront.User.GetUser(connexion.IdUser);
            if (u.Success)
                if (addSource != null)
                {
                    long idStream_ = Convert.ToInt64(idStream);
                    EvernestFront.AccessRight accessRights_ = StringToAccessRights(accessRights);
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
            var usb = new UsersBuilder();
            EvernestFront.Response<User> u = usb.GetUser(userId);
            if (!u.Success)
                return null;
            EvernestFront.Response<EventStream> s = u.Result.GetEventStream(streamId);
            if (s.Success)
            {
                // fetch stream
                StreamAndEvents streamAndEvents = new StreamAndEvents();
                streamAndEvents.Id = s.Result.Id;
                streamAndEvents.Name = s.Result.Name;
                streamAndEvents.Count = s.Result.Count;
                streamAndEvents.LastEventId = s.Result.LastEventId;
                streamAndEvents.RelatedUsers = s.Result.GetRelatedUsers().Result;

                // fetch stream's events
                int begin = 0;
                if (s.Result.LastEventId > 10)
                    begin = Convert.ToInt32(s.Result.LastEventId) - 10;

                EvernestFront.Response<List<Event>> r = s.Result.PullRange(begin, s.Result.LastEventId);
                streamAndEvents.Events = r.Result;

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
                var usb = new UsersBuilder();
                EvernestFront.Response<User> u = usb.GetUser(connexion.IdUser);
                if (u.Success)
                {

                    EvernestFront.Response<EventStream> s = u.Result.GetEventStream(sid);
                    if (s.Success)
                       s.Result.Push(item);
                }
            }
            return RedirectToAction("Stream", "Store", new { id = sid });
        }
    }
}