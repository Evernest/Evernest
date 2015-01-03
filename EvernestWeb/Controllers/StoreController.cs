using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EvernestWeb.Models;
using EvernestWeb.ViewModels;

namespace EvernestWeb.Controllers
{
    public class StoreController : Controller
    {
        public Connexion IsConnected()
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
        public ActionResult MyStore(string item, int nature)
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion != "true")
                return View("Index");

            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(connexion.IdUser);
            if (u.Success)
                if (item != null)
                    if (nature == 0) // a stream
                    {
                        EvernestFront.Answers.CreateEventStream stream = u.User.CreateEventStream(item);
                        if (stream.Success)
                        {
                            // update user object
                            u = EvernestFront.User.GetUser(connexion.IdUser);
                            if (u.Success)
                            {
                                StreamsSources streamsSources = getStreamsSources(u);
                                return View(streamsSources);
                            }
                        }
                    }
                    /* add a source
                    else if (nature == 1) // a source
                    {
                        
                    }
                    */
            return View();
        }
    }
}