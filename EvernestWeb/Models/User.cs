using System.ComponentModel.DataAnnotations;

namespace EvernestWeb.Models
{
    public class User
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}