using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using EvernestFront;
using EvernestFront.Contract;
using EvernestWeb.ViewModels;
using EvernestWeb.Models;
using EvernestWeb.Helpers;

namespace EvernestWeb.Controllers
{
    public class ManagerController : Controller
    {
        // GET: /Manager
        public ActionResult Index()
        {
            var front = new UserProvider();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return View();

            var model = new ManagerModel(userReq.Result);
            return View(model);
        }

        // ----- Streams -----

        // GET: /Manager/Stream/{Id}
        public ActionResult Stream(long id)
        {
            var front = new UserProvider();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);

            ViewBag.StreamId = id;

            var streamEventsModel = new StreamEventsModel(userReq.Result, id);
            return View(streamEventsModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stream(GetEventById model)
        {
            var front = new UserProvider();

            // Check user input
            if (!ModelState.IsValid)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);

            ViewBag.StreamId = model.StreamId;

            var streamEventsModel = new StreamEventsModel(userReq.Result, model.StreamId, model.EventId);

            return View(streamEventsModel);

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
            var front = new UserProvider();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStream(DeleteStreamModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StreamId = model.StreamId;
                return View(model);
            }

            var front = new UserProvider();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Index", "Manager");

            var delStreamReq = userReq.Result.DeleteEventStream(model.StreamId, model.DeleteConfirmPassword);
            if (!delStreamReq.Success)
            {
                ViewBag.StreamId = model.StreamId;
                ModelState.AddModelError("DeleteConfirmPassword", "Wrong Password.");
                return View(model);
            }

            return RedirectToAction("Index", "Manager");

        }

        // GET: /Manager/DeleteStream/{Id}
        public ActionResult DeleteStream(long id)
        {
            ViewBag.StreamId = id;
            return View();
        }

        // POST: /Manager/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewStreamUser(NewStreamUserModel model)
        {
            var front = new UserProvider();

            // Check user input
            if (!ModelState.IsValid)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            Models.User user = (Models.User)Session["User"];

            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            var streamReq = userReq.Result.GetEventStream(model.StreamId);
            if (!streamReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            streamReq.Result.SetUserRight(model.NewUser, model.Right);
            return RedirectToAction("Stream", "Manager", new { id = model.StreamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserRightOnStream(UpdateUserRightOnStream model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            var front = new UserProvider();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            var streamReq = userReq.Result.GetEventStream(model.StreamId);
            if (!streamReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            streamReq.Result.SetUserRight(model.UserId, model.Right);
            return RedirectToAction("Stream", "Manager", new { id = model.StreamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserRightOnStream(DeleteUserRightOnStream model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            var front = new UserProvider();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            var streamReq = userReq.Result.GetEventStream(model.StreamId);
            if (!streamReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            streamReq.Result.SetUserRight(model.UserId, EvernestFront.Contract.AccessRight.NoRight);
            return RedirectToAction("Stream", "Manager", new { id = model.StreamId });
        }

        // POST: /Manager/PushEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PushEvent(NewEventModel model)
        {
            var front = new UserProvider();

            // Check user input
            if (!ModelState.IsValid)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            var user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            var streamReq = userReq.Result.GetEventStream(model.StreamId);
            if (!streamReq.Success)
                return RedirectToAction("Stream", "Manager", new { id = model.StreamId });

            streamReq.Result.Push(model.Content);

            return RedirectToAction("Stream", "Manager", new { id = model.StreamId });
        }

        // ----- Sources -----

        // GET: /Manager/Source/{Id}
        public ActionResult Source(long id)
        {
            var front = new UserProvider();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Index", "Manager");

            var sourceModel = new SourceModel(userReq.Result, id);
            ViewBag.SourceId = id;

            return View(sourceModel);
        }

        // POST: /Manager/NewSource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewSource(NewSourceModel model)
        {
            var front = new UserProvider();

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
            var front = new UserProvider(); 
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);
            if (!userReq.Success)
                return RedirectToAction("Index", "Manager");

            userReq.Result.DeleteSource(id);

            return RedirectToAction("Index", "Manager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewStreamToSource(NewStreamToSourceModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Source", "Manager", new {id = model.SourceId});

            var front = new UserProvider();
            Models.User user = (Models.User)Session["User"];
            var userReq = front.GetUser(user.Id);

            var sourceRightReq = userReq.Result.SetSourceRight(model.SourceId, model.StreamId, model.Right);

            return RedirectToAction("Source", "Manager", new { id = model.SourceId });

        }

    }
}