using System.Web.Mvc;

using EvernestWeb.Models;
using EvernestWeb.Application;

namespace EvernestWeb.Controllers
{
    public class AccountController : System.Web.Mvc.Controller
    {
        private Connexion IsConnected()
        {
            ViewBag.Connexion = "false";
            Connexion connexion = new Connexion();
            if (connexion.IsConnected())
            {
                ViewBag.Connexion = "true";
                ViewBag.Username = connexion.Username;
                return connexion;
            }
            return null;
        }

        // GET: Account
        public ActionResult Index(int id = -1)
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion == "true")
            {
                if(id==1)
                {
                    ViewBag.Status = "Password has been successfully changed.";
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ChangePwdModel model)
        {
            Connexion connexion = IsConnected();
            if (ViewBag.Connexion == "true")
            {
                if (ModelState.IsValid)
                {
                    EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(connexion.IdUser);
                    EvernestFront.Answers.SetPassword p = u.User.SetPassword(model.Password, model.NewPassword);
                    if(p.Success)
                    {
                        return RedirectToAction("Index", "Account", new {id=1});
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Incorrect Password.");
                        return RedirectToAction("Index", "Account", new {id=0});
                    }
                }
                return RedirectToAction("Index", "Account", new { id = 0 });
            }
            return RedirectToAction("Index", "Home");
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
            if (ViewBag.Connexion!="true")
            {
                if (ModelState.IsValid)
                {
                    Connexion connexion = new Connexion();
                    if (connexion.CheckUser(model.Username, model.Password))
                    {
                        connexion.CreateConnexionCookie(model.RememberMe);
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
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
            IsConnected();
            if (ViewBag.Connexion != "true")
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

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Connexion connexion = new Connexion();
            connexion.DeleteConnexionCookie();
            return RedirectToAction("Index","Home");
        }
    }
}