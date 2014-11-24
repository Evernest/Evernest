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
        public static Hashtable parseBody(System.Net.Http.HttpRequestMessage Request) {
            var httpContent = Request.Content;
            var asyncContent = httpContent.ReadAsStringAsync().Result;

            Hashtable json;
            try
            {
                json = JsonConvert.DeserializeObject
                    <Hashtable>(asyncContent);
            }
            catch (Newtonsoft.Json.JsonReaderException) 
            {
                json = null;
            }
            return json;
        }
    }
}