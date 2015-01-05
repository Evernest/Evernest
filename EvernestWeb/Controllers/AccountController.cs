using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EvernestWeb.Models;

namespace EvernestWeb.Controllers
{
    public class AccountController : Controller
    {
        private bool IsConnected()
        {
            ViewBag.Connexion = "false";
            Connexion connexion = new Connexion();
            if (connexion.IsConnected())
            {
                ViewBag.Connexion = "true";
                ViewBag.Username = connexion.Username;
                return true;
            }
            return false;
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
            if (!IsConnected())
            {
                if (ModelState.IsValid)
                {
                    Connexion connexion = new Connexion();
                    if (connexion.CheckUser(model.Username, model.Password))
                    {
                        connexion.CreateConnexionCookie(model.RememberMe);
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("Username", "Login failed.");
                        return View(model);
                    }
                }
                return View(model);
            }
            return RedirectToAction("Index", "Home");
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
            if (!IsConnected())
            {
                if (ModelState.IsValid)
                {
                    // add user in blob
                    EvernestFront.Answers.AddUser u = EvernestFront.User.AddUser(model.Username, model.Password);
                    if (u.Success)
                    {
                        // if it is ok, then create a cookie for the session
                        EvernestFront.Answers.GetUser g = EvernestFront.User.GetUser(u.UserId);
                        if (g.Success)
                        {
                            Connexion connexion = new Connexion(g.User.Id, g.User.SaltedPasswordHash, g.User.Name);
                            connexion.CreateConnexionCookie(false);
                            return RedirectToAction("Index", "Account");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Username", "User Name already taken.");
                        return View(model);
                    }
                }
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff()
        {
            Connexion connexion = new Connexion();
            connexion.DeleteConnexionCookie();
            return RedirectToAction("Index","Home");
        }
    }
}