using System;
using System.Collections.Generic;
using EvernestFront.Errors;

namespace EvernestFront
{
    public static class Process
    {

        /// <summary>
        /// Registers a new user and returns its ID.
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="UserNameTaken"></exception>
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
        /// <exception cref="StreamNameTaken"></exception>
        /// <exception cref="UserIdDoesNotExist"></exception>
        public static Int64 CreateStream(Int64 user, string streamName)
        {
            StreamTable.CheckNameIsFree(streamName);
            var usr = UserTable.GetUser(user);

            var stream = new Stream(streamName);

            StreamTable.Add(stream);
            UserRight.SetRight(usr, stream, UserRight.CreatorRights);
            return stream.Id;
        }

        /// <summary>
        /// Requests to pull a random event from stream streamId.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <returns></returns>
        /// <exception cref="AccessDenied"></exception>
        /// <exception cref="UserIdDoesNotExist"></exception>
        /// <exception cref="StreamIdDoesNotExist"></exception>
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
        /// <exception cref="UserIdDoesNotExist"></exception>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        /// <exception cref="ReadAccessDenied"></exception>
        /// <exception cref="InvalidEventId"></exception>
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
        /// <exception cref="UserIdDoesNotExist"></exception>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        /// <exception cref="ReadAccessDenied"></exception>
        /// <exception cref="InvalidEventId"></exception>
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
        /// <exception cref="WriteAccessDenied"></exception>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        /// <exception cref="UserIdDoesNotExist"></exception>
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
        /// <exception cref="AdminAccessDenied"></exception>
        /// <exception cref="CannotDestituteAdmin"></exception>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        /// <exception cref="UserIdDoesNotExist"></exception>
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
        /// User userId creates a source with rights right on stream streamId.
        /// Returns the key of the newly created source.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <param name="sourceName"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="SourceNameTaken"></exception>
        /// <exception cref="UserIdDoesNotExist"></exception>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        public static String CreateSource(Int64 userId, Int64 streamId, String sourceName, AccessRights right)
        {
            var user = UserTable.GetUser(userId);
            user.CheckSourceNameIsFree(sourceName);
            var stream = StreamTable.GetStream(streamId);
            var source = new Source(user, stream, sourceName, right);
            user.AddSource(source);
            SourceTable.AddSource(source);
            return source.Key;
        }



        /// <summary>
        /// Requests to pull a random event from the stream of the source.
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <returns></returns>
        /// <exception cref="AccessDenied"></exception>
        public static Event PullRandom(String sourceKey)
        {
            Source src = SourceTable.GetSource(sourceKey);
            src.CheckCanRead();
            return src.Stream.PullRandom();
        }

        /// <summary>
        /// Requests to pull event with ID eventId from the stream of the source.
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        /// <exception cref="UserIdDoesNotExist"></exception>
        /// <exception cref="InvalidEventId"></exception>
        public static Event Pull(String sourceKey, int eventId)
        {
            Source src = SourceTable.GetSource(sourceKey);
            src.CheckCanRead();
            return src.Stream.Pull(eventId);
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from the stream of the source (inclusive).
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        /// <exception cref="ReadAccessDenied"></exception>
        /// <exception cref="InvalidEventId"></exception>
        public static List<Event> PullRange(String sourceKey, int from, int to)
        {
            Source src = SourceTable.GetSource(sourceKey);
            src.CheckCanRead();
            return src.Stream.PullRange(from, to);
        }

        /// <summary>
        /// Requests to push an event containing message to the stream of the source. Returns the id of the generated event.
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="WriteAccessDenied"></exception>
        public static int Push(String sourceKey, string message)
        {
            Source src = SourceTable.GetSource(sourceKey);
            src.CheckCanWrite();
            return src.Stream.Push(message);
        }

        /// <summary>
        /// Changes access rights of targetUser over the stream of the source. The source and its user must have admin rights.
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="targetUserId"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        /// <exception cref="AdminAccessDenied"></exception>
        /// <exception cref="CannotDestituteAdmin"></exception>
        /// <exception cref="UserIdDoesNotExist"></exception>
        public static void SetRights(String sourceKey, Int64 targetUserId, AccessRights rights)
        {
            Source src = SourceTable.GetSource(sourceKey);
            var targetUser = UserTable.GetUser(targetUserId);
            src.CheckCanAdmin();
            CheckRights.CheckRightsCanBeModified(targetUser, src.Stream);
            UserRight.SetRight(targetUser, src.Stream, rights);
            // TODO : update la stream historique
        }


        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated AccessRights.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="UserIdDoesNotExist"></exception>
        static public List<KeyValuePair<Int64, AccessRights>> RelatedStreams(Int64 userId)
        {
            User user = UserTable.GetUser(userId);
            return user.RelatedStreams;

            //TODO : exclure les streams avec droits égaux à NoRights ?
        }

        /// <summary>
        /// Returns a list of all users who have rights on stream, and the associated AccessRights. User must have admin rights.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <returns></returns>
        /// <exception cref="UserIdDoesNotExist"></exception>
        /// <exception cref="StreamIdDoesNotExist"></exception>
        /// <exception cref="AdminAccessDenied"></exception>
        static public List<KeyValuePair<Int64, AccessRights>> RelatedUsers(Int64 userId, Int64 streamId)
        {
            var user = UserTable.GetUser(userId);
            var stream = StreamTable.GetStream(streamId);
            CheckRights.CheckCanAdmin(user, stream);
            return stream.RelatedUsers;

            //TODO : exclure les users avec droits égaux à NoRights ?
        }
    }
}
