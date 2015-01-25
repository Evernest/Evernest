using System.Web.Mvc;

namespace EvernestWeb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        // GET: /Home
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Home/About
        public ActionResult About()
        {
            return View();
        }

        // GET: /Home/Documentation
        public ActionResult Documentation()
        {
            return View();
        }

        // GET: /Home/Contact
        public ActionResult Contact()
        {
            return View();
        }
    }
}