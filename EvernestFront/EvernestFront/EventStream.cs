using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using EvernestFront.CommandHandling;
using EvernestFront.CommandHandling.Commands;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestBack;

//TODO : refactor callbacks and implement what to do in case of backend failure

namespace EvernestFront
{
    public class EventStream
    {
        private readonly CommandHandler _commandHandler;
        
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


        internal EventStream(CommandHandler commandHandler, User user, AccessRight userRight, 
            HashSet<AccessAction> possibleActions, long streamId, string name, 
            ImmutableDictionary<string,AccessRight> users, IEventStream backStream)
        {
            _commandHandler = commandHandler;
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

        public Response<Guid> SetUserRight(long targetId, AccessRight right)
        {
            var usersBuilder = new UsersBuilder();
            User targetUser;
            if (!usersBuilder.TryGetUser(targetId, out targetUser))
                return new Response<Guid>(FrontError.UserIdDoesNotExist);
            return SetUserRight(targetUser.Name, right);
        }

        public Response<Guid> SetUserRight(string targetName, AccessRight right)
        {
            if (!ValidateAccessAction(AccessAction.Admin))
                return new Response<Guid>(FrontError.AdminAccessDenied);
            if (TargetUserIsAdmin(targetName))
                return new Response<Guid>(FrontError.CannotDestituteAdmin);
            var command = new UserRightSettingCommand(_commandHandler,
                targetName, Id, _user.Name, right);
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
            long eventId = (long)random.Next((int)LastEventId+1);
            EventContract pulledContract=null;
            var serializer = new Serializer();
            BackStream.Pull(eventId, ( a => pulledContract = serializer.ReadContract<EventContract>(a.Message)), ((a,s)=> {}));  
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
            BackStream.Pull(eventId, (a => pulledContract = serializer.ReadContract<EventContract>(a.Message)), ((a, s) => { }));
            if (pulledContract == null)
                return new Response<Event>(FrontError.BackendError);
            return new Response<Event>(new Event(pulledContract, eventId, Name, Id));
        }

        //TODO : change this when PullRange gets implemented in back-end
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
            for (long id = fromEventId; id <= toEventId; id++)
            {
                Response<Event> ans = Pull(id);
                if (!ans.Success)
                    throw new Exception("EventStream.PullRange");  
                    //this should never happen : both fromEventId and toEventId are valid, so id should be valid.
                Event pulledEvent = ans.Result; 
                eventList.Add(pulledEvent);
            }
            return new Response<List<Event>>(eventList);
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
            var command = new EventStreamDeletionCommand(_commandHandler, Id, Name, _user.Id, password);
            command.Send();
            return new Response<Guid>(command.Guid);
        }
        
    }
}
