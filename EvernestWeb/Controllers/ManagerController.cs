using System.Collections.Generic;
using System.Web.Mvc;

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
        private static readonly Dictionary<string, AccessRight> AccessRightsDictionary = new Dictionary<string, AccessRight>
        {
            {"ReadOnly",  AccessRight.ReadOnly },
            {"WriteOnly", AccessRight.WriteOnly},
            {"ReadWrite", AccessRight.ReadWrite},
            {"Admin",     AccessRight.Admin    },
            {"Root",      AccessRight.Root     },

        };

        // GET: /Manager
        public ActionResult Index()
        {
            var front = new UsersBuilder();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return View();

            var model = new ManagerModel();
            model.Streams = new List<EventStream>();
            model.Sources = new List<Source>();
            return View(model);
        }

        // ----- Streams -----

        // GET: /Manager/Stream/{Id}
        public ActionResult Stream(long id)
        {
            Models.User user = (Models.User)Session["User"];
            StreamAndEvents streamAndEvents = Utils.getStreamsAndEvents(id, user.Id);

            ViewBag.StreamId = id;

            return View(streamAndEvents);
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
            var front = new UsersBuilder();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Index", "Manager");

            // Create stream
            var streamReq = userReq.Result.CreateEventStream(model.Name);
            if (!streamReq.Success)
                return RedirectToAction("Index", "Manager");

            return RedirectToAction("Index", "Manager");
        }

        // GET: /Manager/DeleteStream/{Id}
        public ActionResult DeleteStream(long id)
        {
            Models.User user = (Models.User)Session["User"];
            // Todo
            return RedirectToAction("Stream", "Manager");
        }


        // ----- Sources -----

        // GET: /Manager/Source/{Id}
        public ActionResult Source(long id)
        {
            Models.User user = (Models.User)Session["User"];
            // Todo
            return View();
        }

        // POST: /Manager/NewSource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewSource(NewSourceModel model)
        {
            var front = new UsersBuilder();

            // Check user input
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Manager");

            // Get user
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Index", "Manager");

            // Create source
            var sourceReq = userReq.Result.CreateSource(model.Name);
            if (!sourceReq.Success)
                return RedirectToAction("Index", "Manager");

            return RedirectToAction("Index", "Manager");
        }

        // GET: /Manager/DeleteSource/{Id}
        public ActionResult DeleteSource(long id)
        {
            Models.User user = (Models.User)Session["User"];
            // Todo
            return RedirectToAction("Source", "Manager");
        }


        // ----- To refactor -----

        // POST: /Manager/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(StreamAndEvents model, int sid)
        {
            var front = new UsersBuilder();

            Models.User user = (Models.User)Session["User"];
            if (model.AddUserModelObject.NewUser == null)
                return RedirectToAction("Stream", "Manager");
            
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Stream", "Manager");

            var streamReq = userReq.Result.GetEventStream(model.AddUserModelObject.StreamId);
            if (!streamReq.Success)
                return RedirectToAction("Stream", "Manager");

            var newUserReq = front.GetUser(model.AddUserModelObject.NewUser);
            if (!newUserReq.Success)
                return RedirectToAction("Stream", "Manager");

            streamReq.Result.SetRight(newUserReq.Result.Name, model.AddUserModelObject.Right);
            return RedirectToAction("Stream", "Manager");
        }

        // POST: /Manager/PushEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PushEvent(string item, int sid)
        {
            var front = new UsersBuilder();

            var user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success || item == null)
                return RedirectToAction("Stream", "Manager");

            var streamReq = userReq.Result.GetEventStream(sid);
            if (!streamReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = sid });
            
            streamReq.Result.Push(item);

            return RedirectToAction("Stream", "Manager", new { id = sid });
        }


    }
}