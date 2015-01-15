using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
