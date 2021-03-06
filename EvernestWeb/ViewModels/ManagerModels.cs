﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EvernestFront.Contract;
using EvernestWeb.Helpers;

namespace EvernestWeb.ViewModels
{
    /// <summary>
    ///     Creation of new stream
    /// </summary>
    public class NewStreamModel
    {
        [Required]
        [Display(Name = "Stream name")]
        [StringLength(100, ErrorMessage = "The stream name length must be between 4 and 100 characters.",
            MinimumLength = 4)]
        public string Name { get; set; }
    }

    /// <summary>
    ///     Creation of new source
    /// </summary>
    public class NewSourceModel
    {
        [Required]
        [Display(Name = "Source name")]
        [StringLength(100, ErrorMessage = "The source name length must be between 4 and 100 characters.",
            MinimumLength = 4)]
        public string Name { get; set; }
    }

    /// <summary>
    ///     Creation of new event
    /// </summary>
    public class NewEventModel
    {
        [Required]
        [Display(Name = "Event content")]
        [StringLength(100, ErrorMessage = "The event name length must be between 4 and 100 characters.",
            MinimumLength = 4)]
        public string Content { get; set; }

        [Required]
        public long StreamId { get; set; }
    }

    public class NewStreamUserModel
    {
        public Dictionary<string, AccessRight> AccessRightsDictionary = Utils.AccessRightsDictionary;

        [Required]
        [Display(Name = "New Stream User")]
        public string NewUser { get; set; }

        [Required]
        public AccessRight Right { get; set; }

        [Required]
        public long StreamId { get; set; }
    }

    public class NewStreamUserModelExtended
    {
        [Required]
        public string NewUser { get; set; }

        [Required]
        public long StreamId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The password length must be between 4 and 100 characters.", MinimumLength = 4
            )]
        public string Password { get; set; }
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
        [StringLength(100, ErrorMessage = "Password is too short. It requires at least 6 characters.", MinimumLength = 6
            )]
        [Display(Name = "Confirm with your password")]
        public string DeleteConfirmPassword { get; set; }

        [Required]
        public long StreamId { get; set; }
    }

    public class DeleteSourceModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Password is too short. It requires at least 6 characters.", MinimumLength = 6
            )]
        [Display(Name = "Confirm with your password")]
        public string DeleteConfirmPassword { get; set; }

        [Required]
        public long SourceId { get; set; }
    }

    public class NewStreamToSourceModel
    {
        public Dictionary<string, AccessRight> AccessRightsDictionary = Utils.AccessRightsDictionary;

        [Required]
        public long StreamId { get; set; }

        [Required]
        public AccessRight Right { get; set; }

        [Required]
        public long SourceId { get; set; }

        public Dictionary<string, long> StreamsDictionary { get; set; }
    }

    public class UpdateUserRightOnStream
    {
        public Dictionary<string, AccessRight> AccessRightsDictionary = Utils.AccessRightsDictionary;

        [Required]
        public long StreamId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public AccessRight Right { get; set; }
    }

    public class DeleteUserRightOnStream
    {
        [Required]
        public long StreamId { get; set; }

        [Required]
        public string UserId { get; set; }
    }

    public class UpdateSourceRight
    {
        public Dictionary<string, AccessRight> AccessRightsDictionary = Utils.AccessRightsDictionary;

        [Required]
        public long SourceId { get; set; }

        [Required]
        public long StreamId { get; set; }

        [Required]
        public AccessRight Right { get; set; }
    }

    public class DeleteSourceRight
    {
        [Required]
        public long SourceId { get; set; }

        [Required]
        public long StreamId { get; set; }
    }
}