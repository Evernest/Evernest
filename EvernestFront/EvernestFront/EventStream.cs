using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using EvernestFront.Answers;
using EvernestFront.Projections;
using EvernestFront.Service;
using EvernestFront.Service.Command;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;
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


        public List<KeyValuePair<string, AccessRight>> RelatedUsers
        {
            get { return InternalRelatedUsers.ToList(); }
        }

        private ImmutableDictionary<string, AccessRight> InternalRelatedUsers { get; set; }

        private IEventStream BackStream { get; set; }




        internal EventStream(CommandReceiver commandReceiver, long streamId, string name, ImmutableDictionary<string,AccessRight> users, IEventStream backStream)
        {
            _commandReceiver = commandReceiver;
            Id = streamId;
            Name = name;
            InternalRelatedUsers = users;
            BackStream = backStream;
        }

        

        public static GetEventStream GetStream(long streamId)
        {
            var builder = new EventStreamsBuilder();
            return builder.GetEventStream(streamId);
        }






        //public corresponding method is a User instance method
        internal SetRights SetRight(string adminName, string targetName, AccessRight right)
        {
            if (!ValidateActionFromUserName(adminName, AccessAction.Admin))
                return new SetRights(FrontError.AdminAccessDenied);
            if (ValidateActionFromUserName(targetName, AccessAction.Admin))
                return new SetRights(FrontError.CannotDestituteAdmin);
            var command = new UserRightSettingByUser(_commandReceiver,
                targetName, Id, adminName, right);
            command.Execute();
            return new SetRights();
        }

        private bool ValidateActionFromUserName(string userName, AccessAction action)
        {
            var verifier = new AccessVerifier();
            AccessRight right;
            if (!InternalRelatedUsers.TryGetValue(userName, out right))
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

        internal PullRandom PullRandom()
        {
            var random = new Random();
            long eventId = (long)random.Next((int)LastEventId+1);
            EventContract pulledContract=null;
            var serializer = new Serializer();
            BackStream.Pull(eventId, ( a => pulledContract = serializer.ReadContract<EventContract>(a.Message)), ((a,s)=> {}));  
            if (pulledContract!=null)
                return new PullRandom(new Event(pulledContract, eventId, Name, Id));
            throw new NotImplementedException(); //errors to be refactored anyway
        }

        internal Pull Pull(long id)
        {
            long eventId = ActualEventId(id);


            if (IsEventIdValid(eventId))
            {
                EventContract pulledContract = null;
                var serializer = new Serializer();
                BackStream.Pull(eventId, (a => pulledContract = serializer.ReadContract<EventContract>(a.Message)), ((a,s)=>{}));
                if (pulledContract !=null)
                    return new Pull(new Event(pulledContract, eventId, Name, Id));
                return new Pull(FrontError.BackendError);
            }
            else
            {
                return new Pull(FrontError.InvalidEventId);
            }
           
        }

        internal PullRange PullRange(long fromEventId, long toEventId)
        {
            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);
            if (!IsEventIdValid(fromEventId))
                return new PullRange(FrontError.InvalidEventId);
            if (!IsEventIdValid(toEventId))
                return new PullRange(FrontError.InvalidEventId);
            var eventList = new List<Event>();
            for (long id = fromEventId; id <= toEventId; id++)
            {
                Pull ans = Pull(id);
                if (!ans.Success)
                    throw new Exception();  //this should never happen : both fromEventId and toEventId are valid, so id should be valid.
                Event pulledEvent = ans.EventPulled; //TODO : change this when PullRange gets implemented in back-end
                eventList.Add(pulledEvent);
            }
            return new PullRange(eventList);
        }




        internal Push Push(string message, User author)
        {
            long eventId = LastEventId + 1;
            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);
            bool success = false;
            var contract = new EventContract(author, DateTime.UtcNow, message);
            var serializer = new Serializer();
            BackStream.Push(serializer.WriteContract<EventContract>(contract), (a =>
            {
                success = true;
                stopWaitHandle.Set();
            }), 
            ((a, s) => stopWaitHandle.Set()));  
            stopWaitHandle.WaitOne();
            if (success)
                return new Push(eventId);
            return new Push(FrontError.BackendError);
        }


        
    }
}
