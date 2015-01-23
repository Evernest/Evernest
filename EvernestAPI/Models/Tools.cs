using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.Collections;

namespace EvernestAPI.Models
{
    public class Tools
    {
        public static Hashtable ParseRequest(HttpRequestMessage request) {
            /// Parses the URL and the request and raise an exception when 
            /// it gets invalid data.
            /// This method doesn't catch any exception, it should be done on the 
            /// caller side
            var nvc = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var httpContent = request.Content;
            var nvcHtbl = nvc.Cast<Hashtable>();
            var fromBody = httpContent.ReadAsStringAsync().Result; 

            var json = new Hashtable();
            var body = new Hashtable();

            // Get the body
            if (fromBody.Any()) {
                body = JsonConvert.DeserializeObject
                    <Hashtable>(fromBody);
                
            }

            // Copy the keys from the URL
            for (var i = 0; i < nvc.Count; i ++ )
            {
                json.Add(nvc.GetKey(i).ToLower(), nvc.Get(i));
            }

            // Copy the keys from the body if any
            if (body == null) return json;
            foreach (DictionaryEntry de in body)
            {
                try
                {
                    json.Add(de.Key.ToString().ToLower(), de.Value);
                }
                catch (ArgumentException)
                {
                    json[de.Key.ToString().ToLower()] = de.Value;
                }
            }
            return json;    
        }
    }
}