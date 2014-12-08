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
            /// Parses the URL and the request and raise an exception when 
            /// it gets invalid data.
            /// This method doesn't catch any exception, it should be done on the 
            /// caller side
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var httpContent = Request.Content;
            var nvcHtbl = nvc.Cast<Hashtable>();
            var fromBody = httpContent.ReadAsStringAsync().Result; 

            Hashtable json = new Hashtable();
            Hashtable body = new Hashtable();

            // Get the body
            if (fromBody.Count() > 0) {
                body = JsonConvert.DeserializeObject
                    <Hashtable>(fromBody);
                
            }

            // Copy the keys from the URL
            for (int i = 0; i < nvc.Count; i ++ )
            {
                json.Add(nvc.GetKey(i), nvc.Get(i));
            }

            // Copy the keys from the body if any
            if (body != null)
            { 
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
            }
            return json;    
        }
    }
}