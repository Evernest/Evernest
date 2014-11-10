using System;
using System.Collections.Generic;
using EvernestFront.Exceptions;
using KeyType=System.String; //base64 encoded int

namespace EvernestFront
{
    public static class Process
    {

        /// <summary>
        /// Registers a new user and returns its ID.
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="UserNameTakenException"></exception>
        static public Int64 AddUser(string user)
        {
            UserTable.CheckNameIsFree(user);
            var usr = new User(user);
            UserTable.Add(usr);
            return usr.Id;
        }

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

        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated AccessRights.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnregisteredUserException"></exception>
        static public List<KeyValuePair<string, AccessRights>> StreamsOfUser(Int64 user)
        {
            throw new NotImplementedException();
            //if (!RightsTableByUser.ContainsUser(user))
            //    throw new UnregisteredUserException(user);
            //return RightsTableByUser.StreamsOfUser(user);

            //TODO : exclure les streams avec droits égaux à NoRights ?
        }

        /// <summary>
        /// Returns a list of all users who have rights on stream, and the associated AccessRights. User must have admin rights.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="AccessDeniedException"></exception>
        static public List<KeyValuePair<string, AccessRights>> UsersOfStream(Int64 user, Int64 stream)
        {
            throw new NotImplementedException();

            //if (!RightsTableByStream.ContainsStream(stream))
            //    throw new StreamIdDoesNotExistException(stream);
            //CheckRights.CheckCanAdmin(user, stream);
            //return RightsTableByStream.UsersOfStream(stream);

            //TODO : exclure les users avec droits égaux à NoRights ?
        }
    }
}
