using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    public static class Process
    {
        /// <summary>
        /// Requests the creation of a stream called streamName, with user as admin.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <returns></returns>
        public static Answers.CreateStream CreateStream(string user, string streamName)
        {
            var request = new Requests.CreateStream(user, streamName);
            return request.Process();
        }

        /// <summary>
        /// Requests to pull a random event from stream streamName.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <returns></returns>
        public static Answers.PullRandom PullRandom(string user, string streamName)
        {
            var request = new Requests.PullRandom(user, streamName);
            return request.Process();
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from stream streamName (inclusive).
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Answers.PullRange PullRange(string user, string streamName, int from, int to)
        {
            var request = new Requests.PullRange(user, streamName, from, to);
            return request.Process();
        }

        /// <summary>
        /// Requests to push eventToPush to stream streamName.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <param name="eventToPush"></param>
        /// <returns></returns>
        public static Answers.Push Push(string user, string streamName, string message)
        {
            var request = new Requests.Push(user, streamName, message);
            return request.Process();
        }

        /// <summary>
        /// Changes access rights of targetUser over stream streamName.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <param name="targetUser"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static Answers.SetRights SetRights(string user, string streamName, string targetUser, AccessRights rights)
        {
            var request = new Requests.SetRights(user, streamName, targetUser, rights);
            return request.Process();
        }
    }
}
