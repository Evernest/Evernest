using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using EvernestFront.Responses;
using EvernestFront.Service;
using EvernestFront.Service.Command;
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

        private readonly bool _getBySource;

        private readonly Source _source;

        private readonly HashSet<AccessAction> _possibleActions;

        public long Id { get; private set; }

        public string Name { get; private set; }

        public long Count { get { return BackStream.Size(); } }

        public long LastEventId { get { return Count-1; } }

        //there is a public method GetRelatedUsers
        private ImmutableDictionary<string, AccessRight> RelatedUsers { get; set; }

        private IEventStream BackStream { get; set; }


        internal EventStream(CommandHandler commandHandler, User user, bool getBySource, Source source, 
            HashSet<AccessAction> authorizedActions, long streamId, string name, 
            ImmutableDictionary<string,AccessRight> users, IEventStream backStream)
        {
            _commandHandler = commandHandler;
            Id = streamId;
            Name = name;
            RelatedUsers = users;
            BackStream = backStream;
            _possibleActions = authorizedActions;
            _source = source;
            _getBySource = getBySource;
            _user = user;
        }


        public RelatedUsersResponse GetRelatedUsers()
        {
            if (!ValidateAccessAction(AccessAction.Admin))
                return new RelatedUsersResponse(FrontError.AdminAccessDenied);
            return new RelatedUsersResponse(RelatedUsers.ToList());
        }

        public SystemCommandResponse SetRight(string targetName, AccessRight right)
        {
            if (!ValidateAccessAction(AccessAction.Admin))
                return new SystemCommandResponse(FrontError.AdminAccessDenied);
            if (TargetUserIsAdmin(targetName))
                return new SystemCommandResponse(FrontError.CannotDestituteAdmin);
            var command = new UserRightSettingByUser(_commandHandler,
                targetName, Id, _user.Name, right);
            command.Send();
            return new SystemCommandResponse(command.Guid);
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

        public PullRandomResponse PullRandom()
        {
            if (!ValidateAccessAction(AccessAction.Read))
                return new PullRandomResponse(FrontError.ReadAccessDenied);

            var random = new Random();
            long eventId = (long)random.Next((int)LastEventId+1);
            EventContract pulledContract=null;
            var serializer = new Serializer();
            BackStream.Pull(eventId, ( a => pulledContract = serializer.ReadContract<EventContract>(a.Message)), ((a,s)=> {}));  
            if (pulledContract==null)
                return new PullRandomResponse(FrontError.BackendError);
            return new PullRandomResponse(new Event(pulledContract, eventId, Name, Id));
        }

        public PullResponse Pull(long eventId)
        {
            if (!ValidateAccessAction(AccessAction.Read))
                return new PullResponse(FrontError.ReadAccessDenied);

            eventId = ActualEventId(eventId);
            if (!IsEventIdValid(eventId))
                return new PullResponse(FrontError.InvalidEventId);
            EventContract pulledContract = null;
            var serializer = new Serializer();
            BackStream.Pull(eventId, (a => pulledContract = serializer.ReadContract<EventContract>(a.Message)), ((a, s) => { }));
            if (pulledContract == null)
                return new PullResponse(FrontError.BackendError);
            return new PullResponse(new Event(pulledContract, eventId, Name, Id));
        }

        //TODO : change this when PullRange gets implemented in back-end
        public PullRangeResponse PullRange(long fromEventId, long toEventId)
        {
            if (!ValidateAccessAction(AccessAction.Read))
                return new PullRangeResponse(FrontError.ReadAccessDenied);

            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);
            if (!IsEventIdValid(fromEventId))
                return new PullRangeResponse(FrontError.InvalidEventId);
            if (!IsEventIdValid(toEventId))
                return new PullRangeResponse(FrontError.InvalidEventId);
            var eventList = new List<Event>();
            for (long id = fromEventId; id <= toEventId; id++)
            {
                PullResponse ans = Pull(id);
                if (!ans.Success)
                    throw new Exception("EventStream.PullRange");  
                    //this should never happen : both fromEventId and toEventId are valid, so id should be valid.
                Event pulledEvent = ans.EventPulled; 
                eventList.Add(pulledEvent);
            }
            return new PullRangeResponse(eventList);
        }


        public PushResponse Push(string message)
        {
            if (!ValidateAccessAction(AccessAction.Write))
                return new PushResponse(FrontError.WriteAccessDenied);

            long eventId = LastEventId + 1;
            var stopWaitHandle = new AutoResetEvent(false);
            bool success = false;
            var contract = new EventContract(_user, DateTime.UtcNow, message);
            var serializer = new Serializer();
            BackStream.Push(serializer.WriteContract(contract), (a =>
            {
                success = true;
                stopWaitHandle.Set();
            }), 
            ((a, s) => stopWaitHandle.Set()));
            stopWaitHandle.WaitOne();
            if (success)
                return new PushResponse(eventId);
            return new PushResponse(FrontError.BackendError);
        }


        
    }
}
