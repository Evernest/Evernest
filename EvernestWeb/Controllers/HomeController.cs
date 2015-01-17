using System.Web.Mvc;

using EvernestWeb.Application;

namespace EvernestWeb.Controllers
{
    public class HomeController : System.Web.Mvc.Controller
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

            return View();
        }

        public ActionResult Contact()
        {
            IsConnected();

            return View();
        }
    }
}