using System;
using System.Collections.Generic;
//using EvernestFront.Answers;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    class Stream
    {



        internal Stream()
        {
            throw new NotImplementedException();
                // TODO : appeler Martin NewStorage()
        }


        internal Event PullRandom()
        {
                    throw new NotImplementedException();
                    // TODO : appeler Back
                    // TODO : attendre réponse
        }

        internal List<Event> PullRange(int from, int to)
        {
                    throw new NotImplementedException();
                    // TODO : appeler Back
                    // TODO : attendre réponse
            
        }

        internal void Push(string message)
        {
                    throw new NotImplementedException();
                    // TODO : appeler Back
                    // TODO : attendre réponse
            
        }



        // interface de Martin :
        //
        //public interface IStorage
        //{
        //    public Message getID(int id);
        //    public List<Message> getRange(int from, int to);
        //    public int enqueue(Message message);
        //    public static IStorage NewStorage();
        //}
    }
}
