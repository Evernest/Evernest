using System.Web.Mvc;

using EvernestWeb.Models;

namespace EvernestWeb.Controllers
{
    [AllowAnonymous]
    public class HomeController : System.Web.Mvc.Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}