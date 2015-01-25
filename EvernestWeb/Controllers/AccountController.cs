using System.Web.Mvc;
using System.Web.Security;

﻿using EvernestFront;
using EvernestWeb.ViewModels;
﻿
namespace EvernestWeb.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account
        public ActionResult Index()
        {
            Models.User user = (Models.User)Session["User"];
            return View(user);
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
                var front = new UserProvider();
                Models.User user = (Models.User) Session["User"];
                var userReq = front.GetUser(user.Id);
                if (!userReq.Success)
                {
                    ModelState.AddModelError("Password", "Incorrect Password.");
                    return View(model);
                }
                var setPasswordReq = userReq.Result.SetPassword(model.Password, model.NewPassword);
                if(!setPasswordReq.Success)
                {
                    ModelState.AddModelError("Password", "Incorrect Password.");
                    return View(model);
                }
                ViewBag.Status = "Password modified.";
                return View(model);
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
        public ActionResult Login(Models.User user, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Ask front for identification
                var front = new UserProvider();
                var userReq = front.IdentifyUser(user.Username, user.Password);
                if (!userReq.Success)
                {
                    ModelState.AddModelError("Username", "Invalid credentials.");
                    return View(user);
                }

                // Save session
                user.Id = userReq.Result.Id;
                Session["User"] = user;
                FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);

                // Redirect
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Account");
            }
            return View(user);
        }

        // GET: /Account/SignUp
        [AllowAnonymous]
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: /Account/SignUp
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(RegisterModel model)
        {
            // Check user input
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Add user in front
            var front = new UserProvider();
            var addUserReq = front.AddUser(model.Username, model.Password);
            if (!addUserReq.Success)
            {
                ModelState.AddModelError("Username", "This user name has already been taken.");
                return View(model);
            }

            ViewBag.Message = "User as succesfully been added.";
            return View();
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