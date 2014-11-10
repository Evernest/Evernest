using System;
using System.Collections.Generic;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    public static class Process
    {
        /// <summary>
        /// Requests the creation of a stream called streamName, with user as admin, and returns its ID if successful.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <returns></returns>
        /// <exception cref="StreamNameTakenException"></exception>
        public static Int64 CreateStream(Int64 user, string streamName)
        {
            StreamTable.CheckNameIsFree(streamName);
            var stream = new Stream(streamName);
            var id = stream.Id;
            StreamTable.Add(stream);
            // ajouter droits de User
            return id;
        }

        /// <summary>
        /// Requests to pull a random event from stream streamId.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamId"></param>
        /// <returns></returns>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        public static Event PullRandom(Int64 user, Int64 streamId)
        {
            CheckRights.CheckCanRead(user, streamId);
            Stream stream = StreamTable.GetStream(streamId);
            return stream.PullRandom();
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from stream streamId (inclusive).
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="AccessDeniedException"></exception>
        public static List<Event> PullRange(Int64 user, Int64 streamId, int from, int to)
        {
            CheckRights.CheckCanRead(user, streamId);
            Stream stream = StreamTable.GetStream(streamId);
            return stream.PullRange(from, to);
        }

        /// <summary>
        /// Requests to push eventToPush to stream streamId.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        public static void Push(Int64 user, Int64 streamId, string message)
        {
            CheckRights.CheckCanWrite(user, streamId);
            Stream stream = StreamTable.GetStream(streamId);
            stream.Push(message);
            return;
        }

        /// <summary>
        /// Changes access rights of targetUser over stream streamId.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamId"></param>
        /// <param name="targetUser"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="UnregisteredUserException"></exception>
        public static void SetRights(Int64 user, Int64 streamId, Int64 targetUser, AccessRights rights)
        {
            CheckRights.CheckCanAdmin(user, streamId);
            Users.SetRights(user,streamId, targetUser, rights);
            return;
        }
    }
}
