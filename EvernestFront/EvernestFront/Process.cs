using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using EvernestFront.Errors;
using EvernestFront.Answers;

namespace EvernestFront
{
    public static class Process
    {

        ///now in User
        ///// <summary>
        ///// Registers a new user and returns its ID.
        ///// </summary>
        ///// <param name="user"></param>
        //static public AddUser AddUser(string user)
        //{
        //    return AddUser(user, Keys.NewPassword());
        //}


        
        ///now in User
        //static public AddUser AddUser(string user, string password)
        //{
        //    if (UserTable.NameIsFree(user))
        //    {
        //        var usr = new User(user, password);
        //        UserTable.Add(usr);
        //        return new AddUser(usr.Name, usr.Id, usr.Key, password);
        //    }
        //    else
        //    {
        //        return new AddUser(new UserNameTaken(user));
        //    }
        //}

        
        ///now in Stream
        ///// <summary>
        ///// Requests the creation of a stream called streamName, with userId as admin, and returns its ID.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="streamName"></param>
        ///// <returns></returns>
        //public static CreateStream CreateStream(Int64 userId, string streamName)
        //{
        //    if (!StreamTable.NameIsFree(streamName))
        //        return new CreateStream(new StreamNameTaken(streamName));
        //    if (!UserTable.UserIdExists(userId))
        //        return new CreateStream(new UserIdDoesNotExist(userId));
        //    var usr = UserTable.GetUser(userId);
        //    var stream = new Stream(streamName);
        //    StreamTable.Add(stream);
        //    UserRight.SetRight(usr, stream, UserRight.CreatorRights);
        //    return new CreateStream(stream.Id);
        //}

        ///// <summary>
        ///// Requests to pull a random event from stream streamId.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="streamId"></param>
        ///// <returns></returns>
        //public static PullRandom PullRandom(Int64 userId, Int64 streamId)
        //{
        //    if (!UserTable.UserIdExists(userId))
        //        return new PullRandom(new UserIdDoesNotExist(userId));
        //    if (!StreamTable.StreamIdExists(streamId))
        //        return new PullRandom(new StreamIdDoesNotExist(streamId));
        //    User user = UserTable.GetUser(userId);
        //    Stream stream = StreamTable.GetStream(streamId);
        //    if (CheckRights.CheckCanRead(user, stream))
        //        return stream.PullRandom();
        //    else
        //    {
        //        return new Answers.PullRandom(new ReadAccessDenied(streamId, userId));
        //    }
        //}

        ///// <summary>
        ///// Requests to pull event with ID eventId from stream streamId.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="streamId"></param>
        ///// <param name="eventId"></param>
        ///// <returns></returns>
        //public static Pull Pull(Int64 userId, Int64 streamId, int eventId)
        //{
        //    if (!UserTable.UserIdExists(userId))
        //        return new Pull(new UserIdDoesNotExist(userId));
        //    if (!StreamTable.StreamIdExists(streamId))
        //        return new Pull(new StreamIdDoesNotExist(streamId));
        //    User user = UserTable.GetUser(userId);
        //    Stream stream = StreamTable.GetStream(streamId);
        //    if (CheckRights.CheckCanRead(user, stream))
        //        return stream.Pull(eventId);                    //eventID validity is checked here
        //    else
        //    {
        //        return new Pull(new ReadAccessDenied(streamId,userId));
        //    }
        //}

        ///// <summary>
        ///// Requests to pull events in range [from, to] from stream streamId (inclusive).
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="streamId"></param>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        ///// <returns></returns>
        //public static PullRange PullRange(Int64 userId, Int64 streamId, int from, int to)
        //{
        //    if (!UserTable.UserIdExists(userId))
        //        return new PullRange(new UserIdDoesNotExist(userId));
        //    if (!StreamTable.StreamIdExists(streamId))
        //        return new PullRange(new StreamIdDoesNotExist(streamId));
        //    User user = UserTable.GetUser(userId);
        //    Stream stream = StreamTable.GetStream(streamId);
        //    if (CheckRights.CheckCanRead(user, stream))
        //        return stream.PullRange(from, to);          //Validity of from and to is checked here
        //    else
        //    {
        //        return new PullRange(new ReadAccessDenied(streamId, userId));
        //    }
        //}

        ///// <summary>
        ///// Requests to push an event containing message to stream streamId. Returns the id of the generated event.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="streamId"></param>
        ///// <param name="message"></param>
        ///// <returns></returns>
        //public static Push Push(Int64 userId, Int64 streamId, string message)
        //{
        //    if (!UserTable.UserIdExists(userId))
        //        return new Push(new UserIdDoesNotExist(userId));
        //    if (!StreamTable.StreamIdExists(streamId))
        //        return new Push(new StreamIdDoesNotExist(streamId));
        //    User user = UserTable.GetUser(userId);
        //    Stream stream = StreamTable.GetStream(streamId);
        //    if (CheckRights.CheckCanWrite(user, stream))
        //        return stream.Push(message);
        //    else
        //    {
        //        return new Push(new WriteAccessDenied(streamId, userId));
        //    }
        //}

        ///now in user
        ///// <summary>
        ///// Changes access rights of targetUser over stream. User must have admin rights.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="streamId"></param>
        ///// <param name="targetUserId"></param>
        ///// <param name="rights"></param>
        ///// <returns></returns>
        //public static SetRights SetRights(Int64 userId, Int64 streamId, Int64 targetUserId, AccessRights rights)
        //{
        //    if (!UserTable.UserIdExists(userId))
        //        return new SetRights(new UserIdDoesNotExist(userId));
        //    if (!UserTable.UserIdExists(targetUserId))
        //        return new SetRights(new UserIdDoesNotExist(targetUserId));
        //    if (!StreamTable.StreamIdExists(streamId))
        //        return new SetRights(new StreamIdDoesNotExist(streamId));
        //    var user = UserTable.GetUser(userId);
        //    var stream = StreamTable.GetStream(streamId);
        //    var targetUser = UserTable.GetUser(targetUserId);
        //    if (!CheckRights.CheckCanAdmin(user, stream))
        //        return new SetRights(new AdminAccessDenied(streamId,userId));
        //    if (!CheckRights.CheckRightsCanBeModified(targetUser, stream))
        //        return new SetRights(new CannotDestituteAdmin(streamId,targetUserId));
        //    UserRight.SetRight(targetUser, stream, rights);
        //    // TODO : update history stream
        //    return new SetRights();
        //}


        
        /// <summary>
        /// User userId creates a source with rights right on stream streamId.
        /// Returned answer contains the key of the newly created source.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <param name="sourceName"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static CreateSource CreateSource(Int64 userId, Int64 streamId, String sourceName, AccessRights right)
        {
            if (!UserTable.UserIdExists(userId))
                return new CreateSource(new UserIdDoesNotExist(userId));
            if (!StreamTable.StreamIdExists(streamId))
                return new CreateSource(new EventStreamIdDoesNotExist(streamId));
            var user = UserTable.GetUser(userId);
            if (!user.CheckSourceNameIsFree(sourceName))
                return new CreateSource(new SourceNameTaken(userId, sourceName));
            var stream = StreamTable.GetStream(streamId);
            var source = new Source(user, stream, sourceName, right);
            user.AddSource(source);
            SourceTable.AddSource(source);
            //TODO : update history stream
            return new CreateSource(source.Key);
        }



        /// <summary>
        /// Requests to pull a random event from the stream of the source.
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <returns></returns>
        public static PullRandom PullRandom(String sourceKey)
        {   
            if (!SourceTable.SourceKeyExists(sourceKey))
                return new PullRandom(new SourceKeyDoesNotExist(sourceKey));
            Source src = SourceTable.GetSource(sourceKey);
            if (!src.CheckCanRead())
                return new PullRandom(new ReadAccessDenied(src));
            return src.EventStream.PullRandom();
        }

        /// <summary>
        /// Requests to pull event with ID eventId from the stream of the source.
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public static Pull Pull(String sourceKey, int eventId)
        {
            if (!SourceTable.SourceKeyExists(sourceKey))
                return new Pull(new SourceKeyDoesNotExist(sourceKey));
            Source src = SourceTable.GetSource(sourceKey);
            if (!src.CheckCanRead())
                return new Pull(new ReadAccessDenied(src));
            return src.EventStream.Pull(eventId);            //eventID validity is checked here
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from the stream of the source (inclusive).
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static PullRange PullRange(String sourceKey, int from, int to)
        {
            if (!SourceTable.SourceKeyExists(sourceKey))
                return new PullRange(new SourceKeyDoesNotExist(sourceKey));
            Source src = SourceTable.GetSource(sourceKey);
            if (!src.CheckCanRead())
                return new PullRange(new ReadAccessDenied(src));
            return src.EventStream.PullRange(from, to);
        }

        /// <summary>
        /// Requests to push an event containing message to the stream of the source. Answer contains the id of the generated event.
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Push Push(String sourceKey, string message)
        {
            if (!SourceTable.SourceKeyExists(sourceKey))
                return new Push(new SourceKeyDoesNotExist(sourceKey));
            Source src = SourceTable.GetSource(sourceKey);
            if (!src.CheckCanWrite())
                return new Push(new WriteAccessDenied(src));
            return src.EventStream.Push(message);
        }


        /// <summary>
        /// Changes access rights of targetUser over the stream of the source. The source and its user must have admin rights.
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="targetUserId"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static SetRights SetRights(String sourceKey, Int64 targetUserId, AccessRights rights)
        {
            if (!SourceTable.SourceKeyExists(sourceKey))
                return new SetRights(new SourceKeyDoesNotExist(sourceKey));
            if (!UserTable.UserIdExists(targetUserId))
                return new SetRights(new UserIdDoesNotExist(targetUserId));
            var targetUser = UserTable.GetUser(targetUserId);
            Source src = SourceTable.GetSource(sourceKey);
            EventStream strm = src.EventStream;
            if (!src.CheckCanAdmin())
                return new SetRights(new AdminAccessDenied(src));
            if (!CheckRights.CheckRightsCanBeModified(targetUser, strm))
                return new SetRights(new CannotDestituteAdmin(strm.Id, targetUserId));
            UserRight.SetRight(targetUser, strm, rights);
            // TODO : update history stream
            return new SetRights();
        }


        /// <summary>
        /// Returns a list of all streams on which user has rights, and the associated AccessRights.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        static public RelatedEventStreams RelatedStreams(Int64 userId)
        {
            if (!UserTable.UserIdExists(userId))
                return new RelatedEventStreams(new UserIdDoesNotExist(userId));
            User user = UserTable.GetUser(userId);
            return new RelatedEventStreams(user.RelatedStreams);

            //TODO : exclure les streams avec droits égaux à NoRights ?
        }

        /// <summary>
        /// Returns a list of all users who have rights on stream, and the associated AccessRights. User must have admin rights.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="streamId"></param>
        /// <returns></returns>
        static public RelatedUsers RelatedUsers(Int64 userId, Int64 streamId)
        {
            if (!UserTable.UserIdExists(userId))
                return new RelatedUsers(new UserIdDoesNotExist(userId));
            if (!StreamTable.StreamIdExists(streamId))
                return new RelatedUsers(new EventStreamIdDoesNotExist(streamId));
            var user = UserTable.GetUser(userId);
            var stream = StreamTable.GetStream(streamId);
            if (!CheckRights.CheckCanAdmin(user, stream))
                return new RelatedUsers(new AdminAccessDenied(streamId,userId));
            return new RelatedUsers(stream.RelatedUsers);

            //TODO : exclure les users avec droits égaux à NoRights ?
        }

        ///now in User
        //static public IdentifyUser IdentifyUser(string userName, string password)
        //{
        //    if (UserTable.NameIsFree(userName))
        //        return new IdentifyUser(new UserNameDoesNotExist(userName));
        //    var user = UserTable.GetUser(userName);                                
        //    if (user.Identify(password))
        //        return new IdentifyUser(user.Id);
        //    else
        //    {
        //        return new IdentifyUser(new WrongPassword(userName,password));
        //    }
        //}

        ///now in User
        //static public SetPassword SetPassword(Int64 userId, string newPassword)
        //{
        //    if (!UserTable.UserIdExists(userId))
        //        return new SetPassword(new UserIdDoesNotExist(userId));
        //    var user = UserTable.GetUser(userId);
        //    return user.SetPassword(newPassword);
        //}
    }
}
