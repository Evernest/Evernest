using System;
using System.Collections.Generic;
//using EvernestFront.Answers;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    class Stream
    {

        private readonly Dictionary<string, AccessRights> _rightsTable;


        internal Stream(string user)
        {
                _rightsTable = new Dictionary<string, AccessRights> {{user, AccessRights.Admin}};
                // TODO : appeler Martin NewStorage()
        }

        // TODO : ajouter une énum d'actions pour pouvoir factoriser les disjonctions de cas sur les droits
        private AccessRights GetRights(string user)
        {
            if (_rightsTable.ContainsKey(user))
                return _rightsTable[user];
            else return AccessRights.NoRights;
        }

        internal Answers.PullRandom PullRandom(string user)
        {
            AccessRights rights = GetRights(user);
            switch (rights)
            {
                case (AccessRights.NoRights):
                    var exn = new AccessDeniedException(user, rights, AccessRights.Read);
                    return new Answers.PullRandom(exn);
                default:
                    throw new NotImplementedException();
                    // TODO : appeler Back
                    // TODO : attendre réponse
            }
        }

        internal Answers.PullRange PullRange(string user, int from, int to)
        {
            AccessRights rights = GetRights(user);
            switch (rights)
            {
                case (AccessRights.NoRights):
                    var exn = new AccessDeniedException(user, rights, AccessRights.Read);
                    return new Answers.PullRange(exn);
                default:
                    throw new NotImplementedException();
                    // TODO : appeler Back
                    // TODO : attendre réponse
            }
        }

        internal Answers.Push Push(string user, Event eventToPush)
        {
            AccessRights rights = GetRights(user);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.Read):
                    var exn = new AccessDeniedException(user, rights, AccessRights.ReadWrite);
                    return new Answers.Push(exn);
                default:
                    throw new NotImplementedException();
                    // TODO : appeler Back
                    // TODO : attendre réponse
            }
        }

        internal Answers.SetRights SetRights(string user, string target, AccessRights rightsToSet)
        {
            AccessRights userRights = GetRights(user);
            switch (userRights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.Read):
                case (AccessRights.ReadWrite):
                    var exn = new AccessDeniedException(user, userRights, AccessRights.Admin);
                    return new Answers.SetRights(exn);
                default:
                    _rightsTable[target] = rightsToSet;
                    // refuser de changer les droits si target est un Admin ?
                    return new Answers.SetRights();
            }
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
