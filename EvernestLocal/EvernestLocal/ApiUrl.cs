using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class ApiUrl
    {
        public string urlBase { get; set; }
        public string apiConnexion { get; set; }
        public string apiPull { get; set; }
        public string apiPush { get; set; }

        public string GetPull()
        {
            return urlBase + apiPull + "/random";
        }

        public string GetPull(int fst)
        {
            return urlBase + apiPull + "/" + fst;
        }

        public string GetPull(int fst, int snd)
        {
            return GetPull(fst) + "/" + snd;
        }

        public string GetPush()
        {
            return urlBase + apiPush;
        }

        public string GetConnexion()
        {
            return urlBase + apiConnexion;
        }

    }
}
