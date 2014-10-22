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
    internal class Program
    {
        private static string urlBase = "http://www.evernest.org" ;
        private static void Main(string[] args)
        {
            string response;
            string adresseConnexion = urlBase + "/api/login";
            string adressePullRandom = urlBase + "/api/pull/event/random";
            string adressePull = urlBase + "/api/pull/event/3/4";
            string adressePush = urlBase + "/api/push/event";

            /* je me connecte et je récupère un token et son timeout */

            HttpClient MyRequest = new HttpClient(adresseConnexion);


            Account connect = new Account("evernest", "zkr5XF");

            string jsonConnexion = connect.ToJsonString();
            MyRequest.SendData(jsonConnexion);
            response = MyRequest.GetResponse();

            //Console.WriteLine(response);

            ConnexionResponse cr = JsonToObject<ConnexionResponse>(response);
            cr.ToPrint();

            Console.WriteLine("\nFin Identification\n");

            /* je push un event pour voir si ça fonctionne */
            HttpClient MyRequestPush = new HttpClient(adressePush);

            Request r = new Request("blabla", cr.new_token);
            string r_string = r.ToJsonString();
            MyRequestPush.SendData(r_string);
            response = MyRequestPush.GetResponse();

            PushEventResponse per = JsonToObject<PushEventResponse>(response);

            per.ToPrint();
            //Console.WriteLine(response);

            Console.WriteLine("\nFin Push\n");

            /*
            MyRequestPush = new HttpClient(adressePush);
            Request r2 = new Request("blublo", per.new_token);
            string r2_string = r2.ToJsonString();
            MyRequest.SendData(r2_string);
            response = MyRequestPush.GetResponse();
            Console.WriteLine(response);
             */

            /* puis je récupère cet event */

            RequestToken rt = new RequestToken(per.new_token);
            string rt_string = rt.ToJsonString();
            HttpClient MyRequestPull = new HttpClient(adressePullRandom);
            MyRequestPull.SendData(rt_string);

            response = MyRequestPull.GetResponse();
            RequestResponse rr = JsonToObject<RequestResponse>(response);
            rr.ToPrint();
            //Console.WriteLine(response);

            Console.WriteLine("\nFin Pull Random\n");

            RequestToken rt2 = new RequestToken(rr.new_token);
            string rt2_string = rt2.ToJsonString();
            string adress_1 = GetPullIdUrl(3);
            Console.WriteLine(adress_1);
            HttpClient Myrt2Pull = new HttpClient(GetPullIdUrl(3,4));
            Myrt2Pull.SendData(rt2_string);

            response = Myrt2Pull.GetResponse();
            RequestResponse rr2 = JsonToObject<RequestResponse>(response);
            rr2.ToPrint();

            Console.WriteLine("Fin du pull id" );

        }

        

        public static string GetPullIdUrl(int fst)
        {
            return urlBase + "/api/pull/event/" + fst;
        }

        public static string GetPullIdUrl(int fst, int snd)
        {
            return GetPullIdUrl(fst) + "/" + snd;
        }

        public static T JsonToObject<T>(string data)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            T res = jsonSerializer.Deserialize<T>(data);
            return res;
        }
    }

}
