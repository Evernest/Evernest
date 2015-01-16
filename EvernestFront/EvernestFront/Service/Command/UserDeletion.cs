using System.Diagnostics.Eventing.Reader;
using System.Web.UI.WebControls;

namespace EvernestFront.Service.Command
{
    class UserDeletion : CommandBase
    {
        internal long UserId { get; private set; }

        internal string UserName { get; private set; }

        internal string Password { get; private set; }

        internal UserDeletion(CommandReceiver commandReceiver, long userId, string userName, string password)
            : base(commandReceiver)
        {
            UserId = userId;
            UserName = userName;
            Password = password;
        }
    }
}
