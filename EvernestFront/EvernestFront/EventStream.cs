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
        private readonly CommandReceiver _commandReceiver;

        public long Id { get; private set; }

        public string Name { get; private set; }

        public long Count { get { return BackStream.Size(); } }

        public long LastEventId { get { return Count-1; } }


        //there is a public method GetUsersRelatedToEventStream in User
        private ImmutableDictionary<string, AccessRight> RelatedUsers { get; set; }

        private IEventStream BackStream { get; set; }




        internal EventStream(CommandReceiver commandReceiver, long streamId, string name, ImmutableDictionary<string,AccessRight> users, IEventStream backStream)
        {
            _commandReceiver = commandReceiver;
            Id = streamId;
            Name = name;
            RelatedUsers = users;
            BackStream = backStream;
        }



        //public corresponding method is a User instance method
        internal RelatedUsersResponse GetRelatedUsers(string userName)
        {
            if (!ValidateActionFromUserName(userName, AccessAction.Admin))
                return new RelatedUsersResponse(FrontError.AdminAccessDenied);
            return new RelatedUsersResponse(RelatedUsers.ToList());
        }

        //public corresponding method is a User instance method
        internal SystemCommandResponse SetRight(string adminName, string targetName, AccessRight right)
        {
            if (!ValidateActionFromUserName(adminName, AccessAction.Admin))
                return new SystemCommandResponse(FrontError.AdminAccessDenied);
            if (ValidateActionFromUserName(targetName, AccessAction.Admin))
                return new SystemCommandResponse(FrontError.CannotDestituteAdmin);
            var command = new UserRightSettingByUser(_commandReceiver,
                targetName, Id, adminName, right);
            command.Send();
            return new SystemCommandResponse(command.Guid);
        }

        private bool ValidateActionFromUserName(string userName, AccessAction action)
        {
            var verifier = new AccessVerifier();
            AccessRight right;
            if (!RelatedUsers.TryGetValue(userName, out right))
                right = AccessRight.NoRight;
            return verifier.ValidateAction(right, action);
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

        //public corresponding method is a User instance method
        internal PullRandomResponse PullRandom(string userName)
        {
            if (!ValidateActionFromUserName(userName, AccessAction.Read))
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

        //public corresponding method is a User instance method
        internal PullResponse Pull(string userName, long eventId)
        {
            if (!ValidateActionFromUserName(userName, AccessAction.Read))
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
        //public corresponding method is a User instance method
        internal PullRangeResponse PullRange(string userName, long fromEventId, long toEventId)
        {
            if (!ValidateActionFromUserName(userName, AccessAction.Read))
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
                PullResponse ans = Pull(userName, id);
                if (!ans.Success)
                    throw new Exception("EventStream.PullRange");  
                    //this should never happen : both fromEventId and toEventId are valid, so id should be valid.
                Event pulledEvent = ans.EventPulled; 
                eventList.Add(pulledEvent);
            }
            return new PullRangeResponse(eventList);
        }


        //public corresponding method is a User instance method
        internal PushResponse Push(User author, string message)
        {
            long eventId = LastEventId + 1;
            var stopWaitHandle = new AutoResetEvent(false);
            bool success = false;
            var contract = new EventContract(author, DateTime.UtcNow, message);
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
