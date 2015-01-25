using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using EvernestFront.SystemCommandHandling;
using EvernestFront.SystemCommandHandling.Commands;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestBack;

//TODO : refactor callbacks and implement what to do in case of backend failure

namespace EvernestFront
{
    /// <summary>
    /// Represents an event stream from a given user's point of view: 
    /// some methods return an error if this user does not have the appropriate right over the event stream.
    /// </summary>
    public class EventStream
    {
        private readonly SystemCommandHandler _systemCommandHandler;
        
        private readonly User _user;

        public AccessRight UserRight { get; private set; }

        private readonly HashSet<AccessAction> _possibleActions;

        public long Id { get; private set; }

        public string Name { get; private set; }

        public long Count { get { return BackStream.Size(); } }

        public long LastEventId { get { return Count-1; } }

        //there is a public method GetRelatedUsers
        private ImmutableDictionary<string, AccessRight> RelatedUsers { get; set; }

        private IEventStream BackStream { get; set; }


        internal EventStream(SystemCommandHandler systemCommandHandler, User user, AccessRight userRight, 
            HashSet<AccessAction> possibleActions, long streamId, string name, 
            ImmutableDictionary<string,AccessRight> users, IEventStream backStream)
        {
            _systemCommandHandler = systemCommandHandler;
            Id = streamId;
            Name = name;
            RelatedUsers = users;
            BackStream = backStream;
            _possibleActions = possibleActions;
            _user = user;
            UserRight = userRight;
        }


        public Response<IDictionary<string,AccessRight>> GetRelatedUsers()
        {
            if (!ValidateAccessAction(AccessAction.Admin))
                return new Response<IDictionary<string, AccessRight>>(FrontError.AdminAccessDenied);
            return new Response<IDictionary<string, AccessRight>>(RelatedUsers);
        }

        /// <summary>
        /// Give user with id targetId right about this, 
        /// provided the user or source which was used to create this instance has Admin right on this.
        /// Right to give cannot be Admin (use method SetUserRightToAdmin in that case).
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Response<Guid> SetUserRight(long targetId, AccessRight right)
        {
            var usersBuilder = new UserProvider();
            User targetUser;
            if (!usersBuilder.TryGetUser(targetId, out targetUser))
                return new Response<Guid>(FrontError.UserIdDoesNotExist);
            return SetUserRight(targetUser.Name, right);
        }

        //Password is required to give someone Admin right because this operation cannot be reverted.
        public Response<Guid> SetUserRightToAdmin(long targetId, string password)
        {
            var usersBuilder = new UserProvider();
            User targetUser;
            if (!usersBuilder.TryGetUser(targetId, out targetUser))
                return new Response<Guid>(FrontError.UserIdDoesNotExist);
            return SetUserRightToAdmin(targetUser.Name, password);
        }

        /// <summary>
        /// Give user called targetName right about this, 
        /// provided the user or source which was used to create this instance has Admin right on this.
        /// Right to give cannot be Admin (use method SetUserRightToAdmin in that case).
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public Response<Guid> SetUserRight(string targetName, AccessRight right)
        {
            if (right==AccessRight.Admin)
                return new Response<Guid>(FrontError.CannotSetAdminWithoutPassword);
            return SetUserRightToAny(targetName, right);
        }

        //Password is required to give someone Admin right because this operation cannot be reverted.
        public Response<Guid> SetUserRightToAdmin(string targetName, string password)
        {
            if (!_user.VerifyPassword(password))
                return new Response<Guid>(FrontError.WrongPassword);
            return SetUserRightToAny(targetName, AccessRight.Admin);
        }

        private Response<Guid> SetUserRightToAny(string targetName, AccessRight right)
        {
            if (!ValidateAccessAction(AccessAction.Admin))
                return new Response<Guid>(FrontError.AdminAccessDenied);
            if (TargetUserIsAdmin(targetName))
                return new Response<Guid>(FrontError.CannotDestituteAdmin);
            var command = new UserRightSettingCommand(_systemCommandHandler, targetName, Id, _user.Name, _user.Id, right);
            command.Send();
            return new Response<Guid>(command.Guid);
        }

        private bool ValidateAccessAction(AccessAction action)
        {
            return _possibleActions.Contains(action);
        }

        private bool TargetUserIsAdmin(string targetUserName)
        {
            var verifier = new AccessVerifier();
            AccessRight right;
            if (!RelatedUsers.TryGetValue(targetUserName, out right))
                right = AccessRight.NoRight;
            return verifier.ValidateAction(right, AccessAction.Admin);
        }

        private long ActualEventId(long eventId)
        {
            var id = eventId;
            if (id < 0)
                id = id + (LastEventId + 1);
            return id;
        }

        private bool IsEventIdValid(long id)
        {
            return ((id >= 0) && (id <= LastEventId));
        }

        public Response<Event> PullRandom()
        {
            if (!ValidateAccessAction(AccessAction.Read))
                return new Response<Event>(FrontError.ReadAccessDenied);

            var random = new Random();
            long eventId = random.Next((int)LastEventId+1);
            EventContract pulledContract=null;
            var serializer = new Serializer();
            var stopWaitHandle = new AutoResetEvent(false);
            BackStream.Pull
            (
                eventId,
                ev =>
                {
                    pulledContract = serializer.ReadContract<EventContract>(ev.Message);
                    stopWaitHandle.Set();
                },
                (requestedId, errorMessage) => { stopWaitHandle.Set(); }
            );
            stopWaitHandle.WaitOne();
            if (pulledContract==null)
                return new Response<Event>(FrontError.BackendError);
            return new Response<Event>(new Event(pulledContract, eventId, Name, Id));
        }

        public Response<Event> Pull(long eventId)
        {
            if (!ValidateAccessAction(AccessAction.Read))
                return new Response<Event>(FrontError.ReadAccessDenied);

            eventId = ActualEventId(eventId);
            if (!IsEventIdValid(eventId))
                return new Response<Event>(FrontError.InvalidEventId);
            EventContract pulledContract = null;
            var serializer = new Serializer();
            var stopWaitHandle = new AutoResetEvent(false);
            BackStream.Pull(eventId, (ev => {pulledContract = serializer.ReadContract<EventContract>(ev.Message); stopWaitHandle.Set(); }), ((a, s) => { stopWaitHandle.Set(); }));
            stopWaitHandle.WaitOne();
            if (pulledContract == null)
                return new Response<Event>(FrontError.BackendError);
            return new Response<Event>(new Event(pulledContract, eventId, Name, Id));
        }

        public Response<List<Event>> PullRange(long fromEventId, long toEventId)
        {
            if (!ValidateAccessAction(AccessAction.Read))
                return new Response<List<Event>>(FrontError.ReadAccessDenied);

            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);
            if (!IsEventIdValid(fromEventId))
                return new Response<List<Event>>(FrontError.InvalidEventId);
            if (!IsEventIdValid(toEventId))
                return new Response<List<Event>>(FrontError.InvalidEventId);
            var eventList = new List<Event>();
            bool success = false;
            var stopWaitHandle = new AutoResetEvent(false);
            var serializer = new Serializer();
            BackStream.PullRange
            (
                fromEventId,
                toEventId,
                range =>
                {
                    var eventId = fromEventId;
                    success = true;
                    foreach(LowLevelEvent ev in range)
                    {
                        var pulledContract = serializer.ReadContract<EventContract>(ev.Message);
                        eventList.Add(new Event(pulledContract, eventId, Name, Id));
                        eventId++;
                    }
                    stopWaitHandle.Set();
                },
                (firstId, lastId, errorMessage) => stopWaitHandle.Set());
            stopWaitHandle.WaitOne();
            if (success)
                return new Response<List<Event>>(eventList);
            return new Response<List<Event>>(FrontError.BackendError); //TODO : handle error message ?
        }


        public Response<long> Push(string message)
        {
            if (!ValidateAccessAction(AccessAction.Write))
                return new Response<long>(FrontError.WriteAccessDenied);

            long eventId = LastEventId + 1;
            var stopWaitHandle = new AutoResetEvent(false);
            bool success = false;
            var contract = new EventContract(_user.Name, _user.Id, DateTime.UtcNow, message);
            var serializer = new Serializer();
            BackStream.Push(serializer.WriteContract(contract), (a =>
            {
                success = true;
                stopWaitHandle.Set();
            }), 
            ((a, s) => stopWaitHandle.Set()));
            stopWaitHandle.WaitOne();
            if (success)
                return new Response<long>(eventId);
            return new Response<long>(FrontError.BackendError);
        }

        public Response<Guid> Delete(string password)
        {
            if (!ValidateAccessAction(AccessAction.Admin))
                return new Response<Guid>(FrontError.AdminAccessDenied);
            if (!_user.VerifyPassword(password))
                return new Response<Guid>(FrontError.WrongPassword);
            var command = new EventStreamDeletionCommand(_systemCommandHandler, Id, Name, _user.Name, _user.Id, password);
            command.Send();
            return new Response<Guid>(command.Guid);
        }
        
    }
}
