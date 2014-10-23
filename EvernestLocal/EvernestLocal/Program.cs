using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
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

        private static void Main(string[] args)
        {
            ApiUrl apiUrl = loadUrl("API_Parameters.json");

            AccountResponse a = Connexion(apiUrl.GetConnexion());
            a.ToPrint();

            RequestResponse per = Pull(apiUrl.GetPull(), new RequestToken(a.new_token));
            per.ToPrint();




        }

        private static AccountResponse Connexion(string url)
        {
            HttpClient request = new HttpClient(url);
            Account account = loadAccount("API_Account.json");
            request.SendData(ObjectToJson(account));
            return JsonToObject<AccountResponse>(request.GetResponse());
        }

        private static PushEventResponse Push(string url, Request r)
        {
            HttpClient request = new HttpClient(url);
            request.SendData(ObjectToJson(r));
            return JsonToObject<PushEventResponse>(request.GetResponse());
        }

        private static RequestResponse Pull(string url, RequestToken r)
        {
            HttpClient request = new HttpClient(url);
            request.SendData(ObjectToJson(r));
            return JsonToObject<RequestResponse>(request.GetResponse());
        }

        private static Account loadAccount(string account_file)
        {
            string contentFile = System.IO.File.ReadAllText(@"..\\..\\"+account_file);   
            return JsonToObject<Account>(contentFile);
        }

        private static ApiUrl loadUrl(string param_file)
        {
            string contentFile = System.IO.File.ReadAllText(@"..\\..\\"+param_file);   
            return JsonToObject<ApiUrl>(contentFile);
        }

        private static T JsonToObject<T>(string data)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            T res = jsonSerializer.Deserialize<T>(data);
            return res;
        }

        private static string ObjectToJson(object obj)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(obj);
        }
    }
}
