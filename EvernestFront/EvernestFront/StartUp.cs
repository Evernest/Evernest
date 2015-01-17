using System.Web;

[assembly: PreApplicationStartMethod(typeof(EvernestFront.StartUp), "Start")]

namespace EvernestFront
{
    public class StartUp
    {
        public void Start()
        {
            Injector.Instance.Build();
            //TODO: read system event stream
        }
    }
}
