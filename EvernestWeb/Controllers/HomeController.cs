﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EvernestWeb.Models;

namespace EvernestWeb.Controllers
{
    public class HomeController : Controller
    {
        private void IsConnected()
        {
            ViewBag.Connexion = "false";
            Connexion connexion = new Connexion();
            if (connexion.IsConnected())
            {
                ViewBag.Connexion = "true";
                ViewBag.Username = connexion.Username;
            }
        }

        public ActionResult Index()
        {
            IsConnected();
            

            return View();
        }

        public ActionResult About()
        {
            IsConnected();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            IsConnected();
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}