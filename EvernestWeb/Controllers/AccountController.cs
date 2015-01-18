using System.Web.Mvc;
using System.Web.Security;

using EvernestWeb.Models;

namespace EvernestWeb.Controllers
{
    public class AccountController : System.Web.Mvc.Controller
    {
        private void IsConnected()
        {
            ViewBag.SessionAvailable = "false";
            if (Session["User"] != null)
            {
                ViewBag.SessionAvailable = "true";
                ViewBag.User = (User)Session["User"];
            }
        }

        // GET: Account
        public ActionResult Index()
        {
            IsConnected();
            return View();
        }

        public ActionResult ChangePwd()
        {
            IsConnected();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ChangePwdModel model)
        {
            IsConnected();
            if (ModelState.IsValid)
            {
                User user = (User)Session["User"];
                EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(user.Id);
                EvernestFront.Answers.SetPassword p = u.User.SetPassword(model.Password, model.NewPassword);
                if(p.Success)
                {
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect Password.");
                    return View(model);
                }
            }
            return View(model);
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
        public ActionResult Login(User user, string returnUrl)
        {
            IsConnected();
            if (ModelState.IsValid)
            {
                EvernestFront.Answers.IdentifyUser iu = EvernestFront.User.IdentifyUser(user.Username, user.Password);
                if (iu.Success)
                {
                    user.Id = iu.User.Id;
                    Session["User"] = user;
                    FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Account");
                }

                ModelState.AddModelError("Username", "Invalid credentials.");
                return View(user);
            }
            return View(user);
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
                // add user in blob
                EvernestFront.Answers.AddUser u = EvernestFront.User.AddUser(model.Username, model.Password);
                if (u.Success)
                {
                    // if it is ok, then create the session
                    EvernestFront.Answers.GetUser g = EvernestFront.User.GetUser(u.UserId);
                    if (g.Success)
                    {
                        User user = new User(g.User.Id, model.Username, model.Password);
                        Session["User"] = user;
                        FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
                        return RedirectToAction("Index", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("Username", "This user name has already been taken.");
                    return View(model);
                }
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            Session["User"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Home");
        }
    }
}