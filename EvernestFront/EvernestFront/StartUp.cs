using System.Web;

[assembly: PreApplicationStartMethod(typeof(EvernestFront.StartUp), "Start")]

namespace EvernestFront
{
    public class StartUp
    {
        public void Start()
        {
            var unused = Injector.Instance;
            //TODO: read system event stream
        }
    }
}
