using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EvernestWeb.Models;
using EvernestWeb.ViewModels;

namespace EvernestWeb.Controllers 
{
    public class StoreController : Controller
    {
        //
        // GET: /Store/

        public ActionResult Index()
        {
            string idString = "";
            Int64 id = -1;
            if (Request.Cookies["EvernestWeb"] != null)
            {
                if (Request.Cookies["EvernestWeb"]["Id"] != null)
                {
                    idString = Request.Cookies["EvernestWeb"]["Id"];
                    id = Int64.Parse(idString);
                }
            }
            EvernestFront.Answers.GetUser gU = EvernestFront.User.GetUser(id);
            if (gU.Success)
            {
                List<KeyValuePair<long, EvernestFront.AccessRights>> listStreams = gU.User.RelatedEventStreams;
                List<KeyValuePair<string, string>> listSources = gU.User.Sources;
                StreamsSources ss = new StreamsSources();
                foreach (KeyValuePair<long, EvernestFront.AccessRights> elt in listStreams)
                {
                    EvernestFront.Answers.GetEventStream s = EvernestFront.EventStream.GetStream(elt.Key);
                    if (s.Success)
                        ss.AddEventStream(s.EventStream);
                }
                foreach (KeyValuePair<string, string> src in listSources)
                {
                    EvernestFront.Answers.GetSource s = EvernestFront.Source.GetSource(src.Key);
                    if (s.Success)
                        ss.AddSource(s.Source);
                }

                return View(ss);    
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string addStream)
        {
            string idString = "";
            Int64 id = -1;
            if (Request.Cookies["EvernestWeb"] != null)
            {
                if (Request.Cookies["EvernestWeb"]["Id"] != null)
                {
                    idString = Request.Cookies["EvernestWeb"]["Id"];
                    id = Int64.Parse(idString);
                }
            }
            EvernestFront.Answers.GetUser gU = EvernestFront.User.GetUser(id);
            if (gU.Success)
            {
                // add stream
                if (addStream != null)
                {
                    
                    EvernestFront.Answers.CreateEventStream stream = gU.User.CreateEventStream(addStream);
                    if (stream.Success)
                    {
                        // update user object
                        gU = EvernestFront.User.GetUser(id);

                        // seek data to print

                        
                        List<KeyValuePair<long, EvernestFront.AccessRights>> listStreams = gU.User.RelatedEventStreams;
                        List<KeyValuePair<string, string>> listSources = gU.User.Sources;
                        StreamsSources ss = new StreamsSources();
                        foreach (KeyValuePair<long, EvernestFront.AccessRights> elt in listStreams)
                        {
                            EvernestFront.Answers.GetEventStream s = EvernestFront.EventStream.GetStream(elt.Key);
                            if (s.Success)
                                ss.AddEventStream(s.EventStream);
                        }
                        foreach (KeyValuePair<string, string> src in listSources)
                        {
                            EvernestFront.Answers.GetSource s = EvernestFront.Source.GetSource(src.Key);
                            if (s.Success)
                                ss.AddSource(s.Source);
                        }
                        
                        return View(ss);
                    }
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSource(string addSource)
        {
            return View();
        }

        public ActionResult Stream()
        {
            return View();
        }

        public ActionResult Source()
        {
            return View();
        }

        public ActionResult Coworker()
        {
            return View();
        }
    }
}
