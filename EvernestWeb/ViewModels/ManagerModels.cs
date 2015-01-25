using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EvernestFront.Contract;
using EvernestWeb.Helpers;

namespace EvernestWeb.ViewModels
{
    /// <summary>
    /// Creation of new stream
    /// </summary>
    public class NewStreamModel
    {
        [Required]
        [Display(Name = "Stream name")]
        [StringLength(100, ErrorMessage = "The stream name length must be between 4 and 100 characters.", MinimumLength = 4)]
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
        [StringLength(100, ErrorMessage = "The event name length must be between 4 and 100 characters.", MinimumLength = 4)]
        public string Content { get; set; }

        [Required]
        public long StreamId { get; set; }
    }
    
    public class NewStreamUserModel
    {
        [Required]
        [Display(Name = "New Stream User")]
        public long NewUser { get; set; }

        [Required]
        public AccessRight Right { get; set; }

        [Required]
        public long StreamId { get; set; }

        public Dictionary<string, AccessRight> AccessRightsDictionary = Utils.AccessRightsDictionary;
    }

    public class GetEventById
    {
        [Required]
        [Display(Name = "Get event by Id")]
        public long EventId { get; set; }

        [Required]
        public long StreamId { get; set; }
    }

    public class DeleteStreamModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Password is too short. It requires at least 6 characters.", MinimumLength = 6)]
        [Display(Name = "Confirm with your password")]
        public string DeleteConfirmPassword { get; set; }

        [Required]
        public long StreamId { get; set; }
    }

    public class DeleteSourceModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Password is too short. It requires at least 6 characters.", MinimumLength = 6)]
        [Display(Name = "Confirm with your password")]
        public string DeleteConfirmPassword { get; set; }

        [Required]
        public long SourceId { get; set; }
    }

    public class NewStreamToSourceModel
    {
        [Required]
        [Display(Name = "Stream Id")]
        public long StreamId { get; set; }

        [Required]
        public AccessRight Right { get; set; }

        [Required]
        public long SourceId { get; set; }

        public Dictionary<string, AccessRight> AccessRightsDictionary = Utils.AccessRightsDictionary;
    }

    public class UpdateUserRightOnStream
    {
        [Required]
        public long StreamId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public AccessRight Right { get; set; }

        public AccessRight SelectedRight { get; set; }
    
        public Dictionary<string, AccessRight> AccessRightsDictionary = Utils.AccessRightsDictionary;

        public UpdateUserRightOnStream(AccessRight r)
        {
            SelectedRight = r;
        }
    }

    public class DeleteUserRightOnStream
    {
        [Required]
        public long StreamId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}