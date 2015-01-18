using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

using EvernestFront;

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
                return View();

            var model = new ManagerModel();
            return View(model);
        }

        // POST: /Manager/NewStream
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewStream(NewStreamModel model)
        {
            // Check user input
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Manager");

            // Get user
            Models.User user = (Models.User)Session["User"];
            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(user.Id);
            if (!u.Success)
                return RedirectToAction("Index", "Manager");

            // Create stream
            EvernestFront.Answers.CreateEventStream stream = u.User.CreateEventStream(model.Name);
            if (!stream.Success)
                return RedirectToAction("Index", "Manager");

            // Update user object
            u = EvernestFront.User.GetUser(user.Id);
            if (!u.Success)
                return RedirectToAction("Index", "Manager");

            // Get new stream list
            //StreamsSources streamsSources = Utils.getStreamsSources(u);
            //return RedirectToAction("Index", "Manager", new RouteValueDictionary(streamsSources));
            return RedirectToAction("Index", "Manager");
        }

        // POST: /Manager/NewSource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewSource(NewSourceModel model)
        {
            // Check user input
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Manager");

            // Get user
            Models.User user = (Models.User) Session["User"];
            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(user.Id);
            if (!u.Success)
                return RedirectToAction("Index", "Manager");

            // Create source
            //EvernestFront.Answers.CreateSource source = u.User.CreateSource(model.Name);
            //if (!source.Success)
            //    return RedirectToAction("Index", "Manager");

            return RedirectToAction("Index", "Manager");
        }

        // GET: /Manager/Stream/{Id}
        public ActionResult Stream(long id)
        {
            Models.User user = (Models.User)Session["User"];
            StreamAndEvents streamAndEvents = Utils.getStreamsAndEvents(id, user.Id);

            ViewBag.StreamId = id;

            return View(streamAndEvents);
        }

        // GET: /Manager/Source/{Id}
        public ActionResult Source(long id)
        {
            Models.User user = (Models.User)Session["User"];
            // Todo
            return View();
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
            var user = (Models.User)Session["User"];
            var s = EventStream.GetStream(sid);
            if (!s.Success || item == null)
                return RedirectToAction("Stream", "Manager", new { id = sid });
            
            var u = EvernestFront.User.GetUser(user.Id);
            if (!u.Success)
                return RedirectToAction("Stream", "Manager", new { id = sid });
            
            u.User.Push(s.EventStream.Id, item);

            return RedirectToAction("Stream", "Manager", new { id = sid });
        }

        // GET: /Manager/DeleteStream/{Id}
        public ActionResult DeleteStream(long id)
        {
            Models.User user = (Models.User)Session["User"];
            // Todo
            return RedirectToAction("Stream", "Manager");
        }

        // GET: /Manager/DeleteSource/{Id}
        public ActionResult DeleteSource(long id)
        {
            Models.User user = (Models.User)Session["User"];
            // Todo
            return RedirectToAction("Source", "Manager");
        }
    }
}