using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using EvernestFront;

namespace EvernestWeb.ViewModels
{
    public class Stream
    {

    }

    /// <summary>
    /// Creation of new stream
    /// </summary>
    public class NewStreamModel
    {
        [Required]
        [Display(Name = "Stream name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Creation of new source
    /// </summary>
    public class NewSourceModel
    {
        [Required]
        [Display(Name = "Source name")]
        public string Name { get; set; }
    }
}