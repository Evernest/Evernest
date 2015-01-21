using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EvernestFront.Contract;

namespace EvernestWeb.ViewModels
{
    /// <summary>
    /// Creation of new stream
    /// </summary>
    public class NewStreamModel
    {
        [Required]
        [Display(Name = "Stream name")]
        [StringLength(100, ErrorMessage = "The source name length must be between 4 and 100 characters.", MinimumLength = 4)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Creation of new source
    /// </summary>
    public class NewSourceModel
    {
        [Required]
        [Display(Name = "Source name")]
        [StringLength(100, ErrorMessage = "The source name length must be between 4 and 100 characters.", MinimumLength = 4)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Creation of new event
    /// </summary>
    public class NewEventModel
    {
        [Required]
        [Display(Name = "Event content")]
        [StringLength(100, ErrorMessage = "The source name length must be between 4 and 100 characters.", MinimumLength = 4)]
        public string Content { get; set; }
    }
    
    // Beyond this point, a refactoring might be required

    public class AddUserModel
    {
        [Required]
        [Display(Name = "Add User")]
        public string NewUser { get; set; }

        [Required]
        public AccessRight Right { get; set; }

        [Required]
        public long StreamId { get; set; }
    }


}