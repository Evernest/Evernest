using System;
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

        private StreamsSources getStreamsSources(EvernestFront.Answers.GetUser u)
        {
            List<KeyValuePair<long, EvernestFront.AccessRights>> listStreams = u.User.RelatedEventStreams;
            List<KeyValuePair<string, string>> listSources = u.User.Sources;
            StreamsSources streamsSources = new StreamsSources();
            foreach (KeyValuePair<long, EvernestFront.AccessRights> elt in listStreams)
            {
                EvernestFront.Answers.GetEventStream s = EvernestFront.EventStream.GetStream(elt.Key);
                if (s.Success)
                    streamsSources.AddEventStream(s.EventStream);
            }
            foreach (KeyValuePair<string, string> src in listSources)
            {
                EvernestFront.Answers.GetSource s = EvernestFront.Source.GetSource(src.Key);
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

            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(connexion.IdUser);
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

            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(connexion.IdUser);
            if (u.Success)
                if (addStream != null)
                {
                    EvernestFront.Answers.CreateEventStream stream = u.User.CreateEventStream(addStream);
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
                case "NoRights" : accessRights_ = EvernestFront.AccessRights.NoRights;break;
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

            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(connexion.IdUser);
            if (u.Success)
                if (addSource != null)
                {
                    long idStream_ = Convert.ToInt64(idStream);
                    EvernestFront.AccessRights accessRights_ = StringToAccessRights(accessRights);
                    EvernestFront.Answers.CreateSource source = u.User.CreateSource(addSource, idStream_, accessRights_);
                    if (source.Success)
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
    }
}