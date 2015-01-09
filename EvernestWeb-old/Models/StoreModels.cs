using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EvernestWeb.Models
{
    public class StoreModels
    {
    }

    public class AddItem
    {
        [Required]
        public string Name { get; set; }
    }
}