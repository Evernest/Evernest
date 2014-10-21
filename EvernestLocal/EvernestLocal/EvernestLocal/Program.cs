using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Added references */
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace EvernestLocal
{
    class Program
    {
        static void Main(string[] args)
        {
            string response;
            string urlBase = "http://www.evernest.org";
            string adresseConnexion = urlBase + "/api/login";
            string adressePullRandom = urlBase + "/api/pull/event/random";
            string adressePull = urlBase + "/api/pull/event/3/4";
            string adressePush = urlBase + "/api/push/event";

            /* je me connecte et je récupère un token et son timeout */

            HttpClient MyRequest = new HttpClient(adresseConnexion);
            

            Account connect = new Account("evernest", "zkr5XF");

            string jsonConnexion = connect.ToJsonString();
            response = MyRequest.SendDataGetResponse(jsonConnexion);

            Console.WriteLine(response);

            ConnexionResponse cr = JsonToConnexionResponse(response);
            cr.ToPrint();



            /* je push un event pour voir si ça fonctionne */
            HttpClient MyRequestPush = new HttpClient(adressePush);
            
            Request r = new Request("blabla", cr.new_token);
            string r_string = r.ToJsonString();
            response = MyRequestPush.SendDataGetResponse(r_string);
            PushEventResponse per = JsonToPushEventResponse(response);
            per.ToPrint();
            Console.WriteLine(response);

            MyRequestPush = new HttpClient(adressePush);
            Request r2 = new Request("blublo", per.new_token);
            string r2_string = r2.ToJsonString();
            response = MyRequestPush.SendDataGetResponse(r2_string);
            Console.WriteLine(response);

            /* puis je récupère cet event */
            //MyRequestPush = new HttpClient(adressePullRandom);
            //Request r3 = new Request("", "");
            //string r3_string = r3.ToJsonString();
            //response = MyRequestPush.SendDataGetResponse(r3_string);
            //Console.WriteLine(response);

        }

        static ConnexionResponse JsonToConnexionResponse(string data)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            ConnexionResponse res = jsonSerializer.Deserialize<ConnexionResponse>(data);
            return res;
        }

        static PushEventResponse JsonToPushEventResponse(string data)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            PushEventResponse res = jsonSerializer.Deserialize<PushEventResponse>(data);
            return res;
        }
    }


}
