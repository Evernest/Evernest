using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EvernestWeb2.Models;

namespace EvernestWeb2.Controllers
{
    public class AccountController : Controller
    {
        public void IsConnected()
        {
            ViewBag.Connexion = "false";
            Connexion connexion = new Connexion();
            if (connexion.IsConnected())
                ViewBag.Connexion = "true";
        }

        // GET: Account
        public ActionResult Index()
        {
            IsConnected();
            // nothing for the moment, but later, user could modify password here
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            IsConnected();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            IsConnected();
            if (ModelState.IsValid)
            {
                // to modify later
                Connexion connexion = new Connexion();
                connexion.CreateConnexionCookie(1, model.Password, model.RememberMe);
                return RedirectToAction("Index", "Account");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            IsConnected();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            IsConnected();
            if (ModelState.IsValid)
            {
                // to modify later
                Connexion connexion = new Connexion();
                connexion.CreateConnexionCookie(1, model.Password, false);
                return RedirectToAction("Index", "Account");
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            Connexion connexion = new Connexion();
            connexion.DeleteConnexionCookie();
            return RedirectToAction("Index","Home");
        }
    }
}