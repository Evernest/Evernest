using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Answers;
using EvernestFront.Projections;
using EvernestFront.Service;
using EvernestFront.Service.Command;

namespace EvernestFront
{
    class EventStreamsBuilder
    {
        private EventStreamsProjection _eventStreamsProjection;

        private CommandReceiver _commandReceiver;

        public EventStreamsBuilder()
        {
            _eventStreamsProjection = Injector.Instance.EventStreamsProjection;
            _commandReceiver = Injector.Instance.CommandReceiver;
        }

        //public corresponding method is a User instance method
        internal CreateEventStream CreateEventStream(string creatorName, string streamName)
        {
            if (_eventStreamsProjection.EventStreamNameExists(streamName))
                return new CreateEventStream(FrontError.EventStreamNameTaken);
            // this is supposed to be called by a user object, so creator should always exist

            var command = new EventStreamCreation(_commandReceiver, streamName, creatorName);
            command.Execute();
            return new CreateEventStream();
        }

    }
}
