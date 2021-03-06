﻿using System;
using System.Net;
using System.Net.Http;
using System.Collections;
using System.Web.Http;

namespace EvernestAPI.Models
{
    internal static class Response
    {
        private static Hashtable GetBasicResponseHashtable(bool success)
        {
            var ans = new Hashtable();
            ans["Success"] = success;
            ans["UtcTime"] = DateTime.UtcNow;
            return ans;
        }
       
        public static HttpResponseMessage Error(HttpRequestMessage request, string errorMessage)
        {
            var ans = GetBasicResponseHashtable(success: false);

            var error = new Hashtable();
            error["Message"] = errorMessage;
            ans["Error"] = error;

            return request.CreateResponse(HttpStatusCode.OK, ans); // TODO: find an other Status Code
        }

        public static HttpResponseMessage MissingArgument(HttpRequestMessage request, string argument)
        {
            var ans = GetBasicResponseHashtable(success: false);

            var error = new Hashtable();
            error["Message"] = "MissingArgument";
            error["HelpMessage"] = ""; // TODO: Fill this field.
            error["Argument"] = argument;
            ans["Error"] = error;

            return request.CreateResponse(HttpStatusCode.BadRequest, ans); // TODO: find an other Status Code
        }

        public static HttpResponseMessage BadArgument(HttpRequestMessage request, string[] arguments)
        {
            var ans = GetBasicResponseHashtable(success: false);

            var error = new Hashtable();
            error["Message"] = "BadArgument";
            error["HelpMessage"] = ""; // TODO: Fill this field.
            error["Arguments"] = arguments;
            ans["Error"] = error;

            return request.CreateResponse(HttpStatusCode.BadRequest, ans); // TODO: find an other Status Code
        }

        public static HttpResponseMessage BadArgument(HttpRequestMessage request, string argument)
        {
            return BadArgument(request, new string[] {argument});
        }

        public static HttpResponseMessage BadRequest(HttpRequestMessage request)
        {
            var ans = GetBasicResponseHashtable(success: false); 
            
            var error = new Hashtable();
            error["Message"] = "BadRequest";
            error["HelpMessage"] = ""; // TODO: Fill this field.
            ans["Error"] = error;

            return request.CreateResponse(HttpStatusCode.BadRequest, ans);
        }

        public static HttpResponseMessage Success(HttpRequestMessage request, object o)
        {
            var ans = GetBasicResponseHashtable(success: true); 

            ans["Response"] = o;

            return request.CreateResponse(HttpStatusCode.OK, ans);
        }

        public static HttpResponseMessage NotImplemented(HttpRequestMessage request)
        {
            var ans = GetBasicResponseHashtable(success: false); 
            
            var error = new Hashtable();
            error["Message"] = "NotImplemented";
            error["HelpMessage"] = "";
            ans["Error"] = error;

            return request.CreateResponse(HttpStatusCode.NotImplemented, ans);
        }
    }
}