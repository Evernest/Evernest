using System.Web;

[assembly: PreApplicationStartMethod(typeof(EvernestFront.StartUp), "Start")]

namespace EvernestFront
{
    public class StartUp
    {
        public void Start()
        {
            var injector = Injector.Instance;
            injector.Build();
            //TODO: read system event stream
            injector.CommandHandler.HandleCommands();
            injector.Dispatcher.DispatchSystemEvents();
        }
    }
}
