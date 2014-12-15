using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace EvernestWeb2.Controllers
{
    public class Connexion
    {
        /**************** Properties ****************/

        public Int64 IdUser { get; set; }
        public string HashedPassword { get; set; }
        public string Username { get; set; }

        /**************** Constructors ****************/

        public Connexion()
        {
            IdUser = -1;
            HashedPassword = "";
            Username = "toto";
        }

        public Connexion(Int64 idUser, string hashedPassword)
        {
            IdUser = idUser;
            HashedPassword = hashedPassword;
        }

        /**************** Methods ****************/
        
        public void CreateConnexionCookie(bool rememberMe)
        {
            HttpCookie myCookie = new HttpCookie("EvernestWeb");
            myCookie.Values["IdUser"] = Convert.ToString(IdUser);
            myCookie.Values["Hashed"] = HashedPassword;
            
            if (rememberMe)
                myCookie.Expires = DateTime.Now.AddDays(7);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        public void CreateConnexionCookie(Int64 idUser, string hashedPassword, bool rememberMe)
        {
            IdUser = idUser;
            HashedPassword = hashedPassword;
            CreateConnexionCookie(rememberMe);
        }

        public void DeleteConnexionCookie()
        {
            HttpCookie deletedCookie = new HttpCookie("EvernestWeb");
            deletedCookie.Expires = DateTime.Now.AddDays(-1d);
            HttpContext.Current.Response.Cookies.Add(deletedCookie);
        }

        public bool LoadConnexionCookie()
        {
            if (HttpContext.Current.Request.Cookies["EvernestWeb"] != null)
            {
                if (HttpContext.Current.Request.Cookies["EvernestWeb"]["IdUser"] != null)
                    IdUser = Convert.ToInt64(HttpContext.Current.Request.Cookies["EvernestWeb"]["IdUser"]);
                else
                    return false;
                if (HttpContext.Current.Request.Cookies["EvernestWeb"]["Hashed"] != null)
                    HashedPassword = HttpContext.Current.Request.Cookies["EvernestWeb"]["Hashed"];
                else
                    return false;
                
                return true;
                // later confirm hashedPassword and IdUser with EvernestFront
            }
            return false;
        }

        public bool IsConnected()
        {
            return LoadConnexionCookie();
        }
    }
}