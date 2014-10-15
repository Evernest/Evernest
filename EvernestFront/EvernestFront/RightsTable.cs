using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    public static class RightsTable
    {
        /// <summary>
        /// When adding a new stream, AccessRights to set for its creator
        /// </summary>
        private const AccessRights CreatorRights = AccessRights.Admin;
  

        private static readonly Dictionary<string, Dictionary<string, AccessRights>> Table 
            = new Dictionary<string, Dictionary<string, AccessRights>>();


        // TODO : historique stocké dans une stream

        // TODO : table indexée directement par les utilisateurs ? Pour vérifier si un nom est libre, donner la liste des streams qui le concernent...



        

        /// <summary>
        /// Adds a new stream to the static table, with user having rights CreatorRights.
        /// </summary>
        /// <exception cref="StreamNameTakenException"></exception>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        static internal void AddStream(string user, string stream)
        {
            if (Table.ContainsKey(stream))
                throw new StreamNameTakenException(stream);
            else
                Table[stream] = new Dictionary<string, AccessRights> {{user, CreatorRights}};
            // TODO : update la stream historique
        }


        /// <summary>
        /// Set AccessRights of user about stream to rights.
        /// (interdire de destituer un admin ?)
        /// </summary>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <param name="rights"></param>
        static internal void SetRights(string user, string stream, AccessRights rights)
        {
            if (Table.ContainsKey(stream))
            {
                //var tableAssociatedToStream = Table[stream];
                Table[stream][user] = rights;
                // TODO : interdire de destituer un admin ?
                // retirer user de la table si rights = AccessRights.NoRights ?
                // TODO : update la stream historique
            }
            else
                throw new StreamNameDoesNotExistException(stream);    
        }



        


        // Fonctions pour lire les droits

        /// <summary>
        /// Returns the AccessRights of user about stream.
        /// </summary>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static AccessRights GetRights(string user, string stream)
        {
            if (Table.ContainsKey(stream))
            {
                var tableAssociatedToStream = Table[stream];
                if (tableAssociatedToStream.ContainsKey(user))
                    return tableAssociatedToStream[user];
                else
                    return AccessRights.NoRights;
            }
            else
                throw new StreamNameDoesNotExistException(stream);
        }

        /// <summary>
        /// Returns if and only if user can read on stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanRead(string user, string stream)
        {
            var rights = GetRights(user,stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                    throw new AccessDeniedException(user, rights, AccessRights.Read);
                case (AccessRights.Read):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
                    return;
            }
        }

        /// <summary>
        /// Returns if and only if user can write on stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanWrite(string user, string stream)
        {
            var rights = GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.Read):
                    throw new AccessDeniedException(user, rights, AccessRights.ReadWrite);
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
                    return;
            }
        }

        /// <summary>
        /// Returns if and only if user can administrate stream.
        /// </summary>
        /// <exception cref="AccessDeniedException"></exception>
        /// <exception cref="StreamNameDoesNotExistException"></exception>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        static internal void CheckCanAdmin(string user, string stream)
        {
            var rights = GetRights(user, stream);
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.Read):
                case (AccessRights.ReadWrite):
                    throw new AccessDeniedException(user, rights, AccessRights.Admin);
                case (AccessRights.Admin):
                    return;
            }
        }



    }
}
