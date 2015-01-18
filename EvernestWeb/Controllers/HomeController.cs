using System.Web.Mvc;

using EvernestWeb.Models;

namespace EvernestWeb.Controllers
{
    public class HomeController : System.Web.Mvc.Controller
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

        [AllowAnonymous]
        public ActionResult Index()
        {
            IsConnected();
            

            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            IsConnected();

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            IsConnected();

            return View();
        }
    }
}