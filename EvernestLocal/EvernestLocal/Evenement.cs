using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestLocal
{
    class Evenement
    {
        public int id { get; set; }
        public string content { get; set; }

        public void ToPrint()
        {
            Console.WriteLine("Id : " + id + "; Content : " + content);
        }
    }
}
