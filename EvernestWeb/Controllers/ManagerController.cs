using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

using EvernestFront;

using EvernestWeb.Models;
using EvernestWeb.ViewModels;
using EvernestWeb.Helpers;

namespace EvernestWeb.Controllers
{
    public class ManagerController : Controller
    {
        /// <summary>
        /// Associate strings right names to corresponding Front objets
        /// </summary>
        private static readonly Dictionary<string, AccessRights> AccessRightsDictionary = new Dictionary<string, AccessRights>
        {
            {"NoRights",  AccessRights.NoRights },
            {"ReadOnly",  AccessRights.ReadOnly },
            {"WriteOnly", AccessRights.WriteOnly},
            {"ReadWrite", AccessRights.ReadWrite},
            {"Admin",     AccessRights.Admin    },
            {"Root",      AccessRights.Root     },

        };

        // GET: /Manager
        public ActionResult Index()
        {
            Models.User user = (Models.User)Session["User"];
            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(user.Id);
            if (!u.Success)
            {
                return View();
            }

            StreamsSources streamsSources = Utils.getStreamsSources(u);
            return View(streamsSources);
        }

        // POST: /Manager/AddStream
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStream(string addStream)
        {
            // Get user
            Models.User user = (Models.User)Session["User"];
            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(user.Id);
            if (!u.Success || addStream == null)
                return RedirectToAction("Index", "Manager");

            // Create stream
            EvernestFront.Answers.CreateEventStream stream = u.User.CreateEventStream(addStream);
            if (!stream.Success)
                return RedirectToAction("Index", "Manager");

            // Update user object
            u = EvernestFront.User.GetUser(user.Id);
            if (!u.Success)
                return RedirectToAction("Index", "Manager");

            // Get new stream list
            StreamsSources streamsSources = Utils.getStreamsSources(u);
            return RedirectToAction("Index", "Manager", new RouteValueDictionary(streamsSources));
        }

        // POST: /Manager/AddSource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSource(string addSource, string idStream, string accessRights)
        {
            // Get user
            Models.User user = (Models.User) Session["User"];
            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(user.Id);
            if (!u.Success || addSource == null)
                return RedirectToAction("Index", "Manager");

            // Create source
            long idStream_ = Convert.ToInt64(idStream);
            AccessRights accessRights_ = AccessRightsDictionary[accessRights];
            EvernestFront.Answers.CreateSource source = u.User.CreateSource(addSource, idStream_, accessRights_);
            if (!source.Success)
                return RedirectToAction("Index", "Manager");

            return RedirectToAction("Index", "Manager");
        }

        // GET: /Manager/Strem/{Id}
        public ActionResult Stream(long id)
        {
            Models.User user = (Models.User)Session["User"];
            StreamAndEvents streamAndEvents = Utils.getStreamsAndEvents(id, user.Id);

            List<RightModel> RightList = new List<RightModel>
            {
               new RightModel { Name = "Admin", Right = AccessRights.Admin },
               new RightModel { Name = "NoRights", Right = AccessRights.NoRights },
               new RightModel { Name = "ReadOnly", Right = AccessRights.ReadOnly },
               new RightModel { Name = "ReadWrite", Right = AccessRights.ReadWrite },
               new RightModel { Name = "WriteOnly", Right = AccessRights.WriteOnly },
            };
            ViewBag.RightList = RightList;
            ViewBag.StreamId = id;

            return View(streamAndEvents);
        }

        // POST: /Manager/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(StreamAndEvents model, int sid)
        {
            Models.User user = (Models.User)Session["User"];
            if (model.AddUserModelObject.NewUser != null)
            {
                EvernestFront.Answers.GetEventStream s = EventStream.GetStream(model.AddUserModelObject.StreamId);
                if (s.Success)
                {
                    EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(user.Id);
                    if (u.Success)
                    {
                        EvernestFront.Answers.GetUser n = EvernestFront.User.GetUser(model.AddUserModelObject.NewUser);
                        u.User.SetRights(user.Id, n.User.Id, model.AddUserModelObject.Right);
                    }
                }
            }
            return RedirectToAction("Stream", "Manager", new { id = sid });
        }

        // POST: /Manager/PushEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PushEvent(string item, int sid)
        {
            Models.User user = (Models.User)Session["User"];
            if (item != null)
            {
                EvernestFront.Answers.GetEventStream s = EventStream.GetStream(sid);
                if (s.Success)
                {
                    EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(user.Id);
                    if (u.Success)
                        u.User.Push(s.EventStream.Id, item);
                }
            }
            return RedirectToAction("Stream", "Manager", new { id = sid });
        }
    }
}