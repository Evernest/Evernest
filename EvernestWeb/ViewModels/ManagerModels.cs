using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    /// <summary>
    /// Data displayed in main manager page
    /// </summary>
    public class ManagerModel
    {
        public List<EvernestFront.EventStream> Streams { get; set; }
        public List<EvernestFront.Source> Sources { get; set; }
    }


    // Beyond this point, a refactoring might be required

    public class AddUserModel
    {
        [Required]
        [Display(Name = "Add User")]
        public string NewUser { get; set; }

        [Required]
        public EvernestFront.AccessRights Right { get; set; }

        [Required]
        public long StreamId { get; set; }
    }

    public class StreamAndEvents
    {
        // Stream
        public long Id { get; set; }
        public string Name { get; set; }
        public long Count { get; set; }
        public long LastEventId { get; set;}
        public List<KeyValuePair<string, EvernestFront.AccessRight>> RelatedUsers { get; set; }

        // Events
        public List<EvernestFront.Event> Events;

        // AddUser
        public AddUserModel AddUserModelObject { get; set; }
    }
}