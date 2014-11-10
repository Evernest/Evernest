using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EvernestWeb.Areas.Api.Controllers
{
    public class EventController : Controller
    {
        // GET: Api/Event
        public ActionResult Index()
        {
            return View();
        }
    }
}