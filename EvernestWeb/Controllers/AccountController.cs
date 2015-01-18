using System.Web.Mvc;
using System.Web.Security;

using EvernestWeb.Models;

namespace EvernestWeb.Controllers
{
    public class AccountController : System.Web.Mvc.Controller
    {
        // GET: /Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Account/ChangePwd
        public ActionResult ChangePwd()
        {
            return View();
        }

        // POST: /Account/ChangePwd
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ChangePwdModel model)
        {
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

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user, string returnUrl)
        {
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

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
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

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            Session["User"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Home");
        }
    }
}