using System.Web;
using EvernestFront;

[assembly: PreApplicationStartMethod(typeof(StartUp), "Start")]

namespace EvernestFront
{
    public class StartUp
    {
        public static void Start()
        {
            var injector = Injector.Instance;
            injector.Build();
            //TODO: read system event stream
            injector.CommandHandler.HandleCommands();
            injector.Dispatcher.DispatchSystemEvents();
        }
    }
}
