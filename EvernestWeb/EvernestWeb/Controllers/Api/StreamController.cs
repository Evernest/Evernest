using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using EvernestFront;

namespace EvernestWeb.Controllers.Api
{
    public class StreamController : Controller
    {
        // GET: Api/Stream
        public ActionResult Index()
        {
            return Json("Get related stream list", JsonRequestBehavior.AllowGet);
        }

        // GET: Api/Stream/{streamId}
        public ActionResult Show(int streamId)
        {
            return Json("Get stream info", JsonRequestBehavior.AllowGet);
        }

        // GET: Api/Stream/{streamId}/Pull
        public ActionResult PullRandom(string streamId)
        {
            Event ans = Process.PullRandom("testing", streamId);
            return Json(ans, JsonRequestBehavior.AllowGet);
        }

        // GET: Api/Stream/{streamId}/Pull/{firstEventId}/{lastEventId}
        public ActionResult PullRange(string streamId, int firstEventId, int lastEventId)
        {
            List<Event> ans = EvernestFront.Process.PullRange("testing", streamId, firstEventId, lastEventId);
            return Json(ans, JsonRequestBehavior.AllowGet);
        }

        // POST: Api/Stream/{streamId}/Push
        public ActionResult Push(string streamId, [FromBody]string content)
        {
            Process.Push("testing", streamId, content);
            return Json("Push new event", JsonRequestBehavior.AllowGet);
        }
    }
}