using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

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
        /// <exception cref="StreamNameTakenException"></exception>
        public static void CreateStream(string user, string streamName)
        {
            RightsTable.AddStream(user, streamName);
            var stream = new Stream();
            StreamTable.Add(streamName, stream);
            return;
        }

        /// <summary>
        /// Requests to pull a random event from stream streamName.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <returns></returns>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        public static Event PullRandom(string user, string streamName)
        {
            CheckRights.CheckCanRead(user, streamName);
            Stream stream = StreamTable.GetStream(streamName);
            return stream.PullRandom();
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from stream streamName (inclusive).
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <exception cref="AccessDeniedException"></exception>
        public static List<Event> PullRange(string user, string streamName, int from, int to)
        {
            CheckRights.CheckCanRead(user, streamName);
            Stream stream = StreamTable.GetStream(streamName);
            return stream.PullRange(from, to);
        }

        /// <summary>
        /// Requests to push eventToPush to stream streamName.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        public static void Push(string user, string streamName, string message)
        {
            CheckRights.CheckCanWrite(user, streamName);
            Stream stream = StreamTable.GetStream(streamName);
            stream.Push(message);
            return;
        }

        /// <summary>
        /// Changes access rights of targetUser over stream streamName.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <param name="targetUser"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <exception cref="UnregisteredUserException"></exception>
        public static void SetRights(string user, string streamName, string targetUser, AccessRights rights)
        {
            CheckRights.CheckCanAdmin(user, streamName);
            RightsTable.SetRights(targetUser,streamName, rights);
            return;
        }
    }
}
