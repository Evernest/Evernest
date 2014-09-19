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
            Console.WriteLine(message);
            return;
        }
	}


}
