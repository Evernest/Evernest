using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace EvernestWeb.Models
{
    public class AccountModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public AccountModel()
        {
            Username = "";
            Password = "";
        }

        public AccountModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class LoginModel
    {
        [Required]
        [Display (Name = "User Name")]
        public string Username { get; set; }

        [Required]
        [Display (Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me ?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password too short, at least 6 characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}