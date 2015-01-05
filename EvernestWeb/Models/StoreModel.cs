using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EvernestWeb.Models
{
    public class StoreModel
    {
    }

    public class SourceModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The length of the source's name is between 4 and 100 characters.", MinimumLength = 4)]
        public string AddSource { get; set; }
    }

    public class StreamModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The length of the stream's name is between 4 and 100 characters.", MinimumLength = 4)]
        public string AddStream { get; set; }
    }

    public class EventModel
    {
        [Required]
        [StringLength(4000, ErrorMessage = "Message length is between 2 and 4000 characters.", MinimumLength = 2)]
        public string Message { get; set; }
    }
}