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
        /// <exception cref="UserIdDoesNotExistException"></exception>
        public static Int64 CreateStream(Int64 user, string streamName)
        {
            StreamTable.CheckNameIsFree(streamName);
            var usr = UserTable.GetUser(user);

            var stream = new Stream(streamName);

            StreamTable.Add(stream);
            UserRight.SetRight(usr, stream, Users.CreatorRights);
            return stream.Id;
        }

        /// <summary>
        /// Requests to pull a random event from stream streamId.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <returns></returns>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="UserIdDoesNotExistException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        public static Event PullRandom(Int64 userId, Int64 streamId)
        {
            User user = UserTable.GetUser(userId);
            Stream stream = StreamTable.GetStream(streamId);
            CheckRights.CheckCanRead(user, stream);
            return stream.PullRandom();
        }

        /// <summary>
        /// Requests to pull event with ID eventId from stream streamId.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        /// <exception cref="UserIdDoesNotExistException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="ReadAccessDeniedException"></exception>
        /// <exception cref="InvalidEventIdException"></exception>
        public static Event Pull(Int64 userId, Int64 streamId, int eventId)
        {
            User user = UserTable.GetUser(userId);
            Stream stream = StreamTable.GetStream(streamId);
            CheckRights.CheckCanRead(user, stream);
            return stream.Pull(eventId);
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from stream streamId (inclusive).
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        /// <exception cref="UserIdDoesNotExistException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="ReadAccessDeniedException"></exception>
        /// <exception cref="InvalidEventIdException"></exception>
        public static List<Event> PullRange(Int64 userId, Int64 streamId, int from, int to)
        {
            User user = UserTable.GetUser(userId);
            Stream stream = StreamTable.GetStream(streamId);
            CheckRights.CheckCanRead(user, stream);
            return stream.PullRange(from, to);
        }

        /// <summary>
        /// Requests to push an event containing message to stream streamId. Returns the id of the generated event.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="WriteAccessDeniedException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="UserIdDoesNotExistException"></exception>
        public static int Push(Int64 userId, Int64 streamId, string message)
        {
            User user = UserTable.GetUser(userId);
            Stream stream = StreamTable.GetStream(streamId);
            CheckRights.CheckCanWrite(user, stream);
            return stream.Push(message);
        }

        /// <summary>
        /// Changes access rights of targetUser over stream. User must have admin rights.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <param name="targetUserId"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        /// <exception cref="AdminAccessDeniedException"></exception>
        /// <exception cref="CannotDestituteAdminException"></exception>
        /// <exception cref="StreamIdDoesNotExistException"></exception>
        /// <exception cref="UserIdDoesNotExistException"></exception>
        public static void SetRights(Int64 userId, Int64 streamId, Int64 targetUserId, AccessRights rights)
        {
            var user = UserTable.GetUser(userId);
            var stream = StreamTable.GetStream(streamId);
            var targetUser = UserTable.GetUser(targetUserId);
            CheckRights.CheckCanAdmin(user, stream);
            CheckRights.CheckRightsCanBeModified(targetUser, stream);
            UserRight.SetRight(targetUser, stream, rights);
            // TODO : update la stream historique
        }

        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated AccessRights.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UserIdDoesNotExistException"></exception>
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
        /// <exception cref="UserIdDoesNotExistException"></exception>
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
