using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Requests
{
    class SetRights:Request<Answers.SetRights>
    {
        private string targetUser;
        private AccessRights rights;


        /// <summary>
        /// Constructor for SetRights requests.
        /// Synopsis : user sets the rights of targetUser for stream streamName to rights.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="streamName"></param>
        /// <param name="targetUser"></param>
        /// <param name="rights"></param>
        public SetRights(string user, string streamName, string targetUser, AccessRights rights)
            : base(user, streamName)
        {
            this.targetUser = targetUser;
            this.rights = rights;
        }
        
        /// <summary>
        /// Processes a SetRights request. Request is successful if user has admin rights.
        /// </summary>
        /// <returns></returns>
        public override Answers.SetRights Process()
        {
            try
            {
                RightsTable.CheckCanAdmin(User, StreamName);
                RightsTable.SetRights(targetUser, StreamName, rights);
                return new Answers.SetRights();
            }
            catch (FrontException exn)
            {
                return new Answers.SetRights(exn);
            }

        }
    }
}
