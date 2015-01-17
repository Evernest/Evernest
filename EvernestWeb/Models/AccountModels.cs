using System.ComponentModel.DataAnnotations;

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
        [Display (Name = "User name")]
        public string Username { get; set; }

        [Required]
        [Display (Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password is too short. It requires at least 6 characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password must match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePwdModel
    {
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password is too short. It requires at least 6 characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "Password and confirmation password must match.")]
        public string ConfirmPassword { get; set; }
    }
}