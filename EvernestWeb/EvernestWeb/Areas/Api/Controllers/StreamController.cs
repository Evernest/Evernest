using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EvernestWeb.Areas.Api.Controllers
{
    public class StreamController : Controller
    {
        // GET: Api/Stream
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowStream()
        {
            return View();
        }

        public ActionResult PullRandomEvent()
        {
            return View();
        }

        public ActionResult PullEvent()
        {
            return View();
        }

        public ActionResult PullEventRange()
        {
            return View();
        }
    }
}