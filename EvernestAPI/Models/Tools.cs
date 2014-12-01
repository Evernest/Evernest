using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Collections;

namespace EvernestAPI.Models
{
    public class Tools
    {
        public static Hashtable parseRequest(System.Net.Http.HttpRequestMessage Request) {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var nvcHtbl = nvc.Cast<Hashtable>();
            var httpContent = Request.Content;
            var fromBody = httpContent.ReadAsStringAsync().Result; 

            Hashtable json = new Hashtable();
            Hashtable body = new Hashtable();
            try
            {
                body = JsonConvert.DeserializeObject
                    <Hashtable>(fromBody);
            }
            catch (Newtonsoft.Json.JsonReaderException) 
            {
                body = null;
            }

            // Copy the keys from the URL to the answer
            for (int i = 0; i < nvc.Count; i ++ )
            {
                json.Add(nvc.GetKey(i), nvc.Get(i));
            }

            // Copy the body to the answer
            foreach (DictionaryEntry de in body)
            {
                try
                {
                    json.Add(de.Key, de.Value);
                }
                catch (ArgumentException)
                {
                    json[de.Key] = de.Value;
                }
            }
            return json;    
        }
    }
}