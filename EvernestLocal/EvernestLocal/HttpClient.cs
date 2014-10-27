using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

/* Added references */
using System.Net.Http;

namespace EvernestLocal
{
    class HttpClient
    {
                                                                            /* Member Variables */
        private WebRequest request;
        private Stream dataStream;
                                                                                  /* Properties */
        private string status;                                                    
        public String Status
        {
            get { return status; }
            set { status = value; }
        }
        
                                                                                /* Constructor */
        public HttpClient(string url)
        {
            request = WebRequest.Create(url);
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "text/json";
            System.Net.ServicePointManager.Expect100Continue = false;
        }

                                                                                    /* Methods */

        
        public void SendData(string data)
        {
            // convert data into byteArray
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // set parameter
            request.ContentLength = byteArray.Length;

            // create stream and send data in stream
            dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Flush();
            dataStream.Close();
        }

        public string GetResponse()
        {

            // get response

            WebResponse response; 
            try
            {
                response = request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
             
                Console.WriteLine("Message : " + e.Message);
                throw new Exception("0");
            }

            // this.Status = ((HttpWebResponse)response).StatusDescription; // optional
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            // return response
            return responseFromServer;
        }
        
    }
}
