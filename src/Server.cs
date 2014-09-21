using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloud14
{
	class Server
	{
        private LinkedList<Message> messagesList = new LinkedList<Message>();
        private int id = 0;

        public void enqueue(String newMessage)
        {
            Message message = new Message(newMessage, id);
            messagesList.AddLast(message);
            id++;
        }

        /**
         * Return the message with given id
         */
        public void getID(int id)
        {
            Message message;
            try
            {
                message = System.Linq.Enumerable.ElementAt<Message>(messagesList, id);
            } catch (ArgumentNullException e)
            {
                Console.Error.Write("Try to get from message a null messagesList.");
                Console.Error.Write(e.StackTrace);
                return;
            } catch (ArgumentOutOfRangeException e)
            {
                Console.Error.Write("Index out of range when getting a message from messagesList.");
                Console.Error.Write(e.StackTrace);
                return;
            }
            transmitMessage(message);
            return;
        }

        /**
         * Get a range of messages between from and to (both included)
         */
        public void getRange(int from, int to)
        {
            if(from < 0 || to >= messagesList.Count)
            {
                Console.Error.WriteLine("Incorrect bounds when getting a range of values, aborting.");
                return;
            }
            LinkedListNode<Message> first = messagesList.First;
            int index = 0;
            while(from > index)
            { 
                first = first.Next;
                index++;
            }
            while(index <= to )
            {
                transmitMessage(first.Value);
                first = first.Next;
                index++;
            }

        }

        /**
         * Method called to transmit a message, just prompt it for the moment. Should pass it to the network layer
         */
        private void transmitMessage(Message message)
        {
            Console.WriteLine(message);
        }
	}
}
