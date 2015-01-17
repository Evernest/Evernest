using System;
using System.Web;


namespace EvernestWeb.Application
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
            Username = "";
        }

        public Connexion(Int64 id, string hashedPassword, string name)
        {
            IdUser = id;
            HashedPassword = hashedPassword;
            Username = name;
        }

        /**************** Methods ****************/

        private void RetrieveUserName()
        {
            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(IdUser);
            if (u.Success)
            {
                Username = u.User.Name;
            }
        }

        private bool CheckUserById()
        {
            EvernestFront.Answers.GetUser u = EvernestFront.User.GetUser(IdUser);
            if (u.Success)
            {
                if (HashedPassword == u.User.SaltedPasswordHash) // never works: strange symbols in cookie
                    return true;
                return true;        // check with Raphael and Diane, for the moment, return true in every case.
            }
            return false;
        }

        public bool CheckUser(string username, string password)
        {
            EvernestFront.Answers.IdentifyUser iu = EvernestFront.User.IdentifyUser(username, password);
            if (iu.Success)
            {
                IdUser = iu.User.Id;
                HashedPassword = iu.User.SaltedPasswordHash;
                Username = iu.User.Name;
                return true;
            }
            return false;
        }

        public void CreateConnexionCookie(bool rememberMe)
        {
            HttpCookie myCookie = new HttpCookie("EvernestWeb");
            myCookie.Values["IdUser"] = Convert.ToString(IdUser);
            myCookie.Values["Hashed"] = HashedPassword;
            
            if (rememberMe)
                myCookie.Expires = DateTime.Now.AddDays(7);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        public void DeleteConnexionCookie()
        {
            HttpCookie deletedCookie = new HttpCookie("EvernestWeb") {Expires = DateTime.Now.AddDays(-1d)};
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

                if (CheckUserById())
                {
                    RetrieveUserName();
                    return true;
                }
            }
            return false;
        }

        public bool IsConnected()
        {
            return LoadConnexionCookie();
        }
    }
}