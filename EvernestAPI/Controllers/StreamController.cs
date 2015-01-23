using System.Collections;
using System.Linq;
using System.Web.Http;
using EvernestFront;
using EvernestAPI.Models;
using System.Net;
using System.Net.Http;

namespace EvernestAPI.Controllers
{
    public class StreamController : ApiController
    {

        // Default controller.
        //     /Stream/{id}
        // Return informations about the Stream.
        // Requires Read right.
        [HttpGet]
        [HttpPost]
        [ActionName("Default")]
        public HttpResponseMessage Default(long id)
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("key"))
                return Response.MissingArgument(Request, "Key");

            var sourceProvider = new SourceProvider();
            var sourceRequest = sourceProvider.GetSource((string) body["key"]);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "Key");

            var source = sourceRequest.Result;

            var eventStreamRequest = source.GetEventStream(id);

            if (!eventStreamRequest.Success)
                return Response.BadArgument(Request, "StreamId");

            var eventStream = eventStreamRequest.Result;

            var ans = new Hashtable();
            ans["Stream"] = eventStream; // TODO: Change this.
            return Response.Success(Request, ans);
        }


        // Controller for pull of one event.
        //     /Stream/{id}/Pull/{arg0}
        // Requires the good rights on the stream.
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public HttpResponseMessage PullOne(long id, long arg0)
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("key"))
                return Response.MissingArgument(Request, "Key");

            var sourceProvider = new SourceProvider();
            var sourceRequest = sourceProvider.GetSource((string)body["key"]);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "Key");

            var source = sourceRequest.Result;

            var eventStreamRequest = source.GetEventStream(id);

            if (!eventStreamRequest.Success)
                return Response.BadArgument(Request, "StreamId");

            var eventStream = eventStreamRequest.Result;

            var eventRequest = eventStream.Pull(arg0);

            if (!eventRequest.Success)
                return Response.BadArgument(Request, "EventId");

            var evnt = eventRequest.Result; // Note the missing e in evnt.

            var ans = new Hashtable();
            ans["Event"] = evnt.Message;
            return Response.Success(Request, ans);
        }


        // Controller for pull of a range.
        //     /Stream/{id}/Pull/{arg0}/{arg1}
        // Requires the good rights on the stream.
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public HttpResponseMessage PullRange(long id, long arg0, long arg1)
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("key"))
                return Response.MissingArgument(Request, "Key");

            var sourceProvider = new SourceProvider();
            var sourceRequest = sourceProvider.GetSource((string)body["key"]);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "Key");

            var source = sourceRequest.Result;

            var eventStreamRequest = source.GetEventStream(id);

            if (!eventStreamRequest.Success)
                return Response.BadArgument(Request, "StreamId");

            var eventStream = eventStreamRequest.Result;

            var eventRequest = eventStream.PullRange(arg0, arg1);

            if (!eventRequest.Success)
                return Response.BadArgument(Request, new string[] {"FromEventId", "ToEventId"});

            var evntList = eventRequest.Result; // Note the missing e in evnt.
            var evntMessageList = evntList.Select(evnt => evnt.Message).ToList();

            var ans = new Hashtable();
            ans["EventList"] = evntMessageList;
            return Response.Success(Request, ans);
        }


        // Controller for random pull.
        //     /Stream/{id}/Pull/Random
        // Requires good rights.
        [HttpGet]
        [HttpPost]
        [ActionName("Pull")]
        public HttpResponseMessage PullRandom(int id)
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("key"))
                return Response.MissingArgument(Request, "Key");

            var sourceProvider = new SourceProvider();
            var sourceRequest = sourceProvider.GetSource((string)body["key"]);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "Key");

            var source = sourceRequest.Result;

            var eventStreamRequest = source.GetEventStream(id);

            if (!eventStreamRequest.Success)
                return Response.BadArgument(Request, "StreamId");

            var eventStream = eventStreamRequest.Result;

            var eventRequest = eventStream.PullRandom();

            if (!eventRequest.Success)
                return Response.Error(Request, "Empty stream");

            var evnt = eventRequest.Result; // Note the missing e in evnt.

            var ans = new Hashtable();
            ans["Event"] = evnt.Message;
            return Response.Success(Request, ans);
        }


        // Controller for pushing.
        //     /Stream/{id}/Push
        // Requires good right.
        [HttpGet]
        [HttpPost]
        [ActionName("Push")]
        public HttpResponseMessage Push(int id)
        {
            Hashtable body;
            try { body = Tools.ParseRequest(Request); }
            catch { return Response.BadRequest(Request); }

            if (!body.ContainsKey("key"))
                return Response.MissingArgument(Request, "Key");

            var sourceProvider = new SourceProvider();
            var sourceRequest = sourceProvider.GetSource((string)body["key"]);

            if (!sourceRequest.Success)
                return Response.BadArgument(Request, "Key");

            var source = sourceRequest.Result;

            var eventStreamRequest = source.GetEventStream(id);

            if (!eventStreamRequest.Success)
                return Response.BadArgument(Request, "StreamId");

            var eventStream = eventStreamRequest.Result;

            var pushRequest = eventStream.Push((string) body["Message"]);

            if (!pushRequest.Success)
                return Response.Error(Request, "Error while pushing"); // TODO: Change this

            return Response.Success(Request, null); // TODO: Change this
        }
    }
}
