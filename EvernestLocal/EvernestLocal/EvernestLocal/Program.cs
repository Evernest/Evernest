using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

namespace EvernestLocal
{
    class Program
    {
        static void Main(string[] args)
        {
            string adresseUriElie = "http://exppad.com/api/pull/event/random";

            //create the constructor with post type and few data
            HttpClient myRequest = new HttpClient(adresseUriElie, "POST");
            //show the response string on the console screen.
            Console.WriteLine(myRequest.GetResponse());



        }
    }
}
