using System.ComponentModel.DataAnnotations;

namespace Galaxy.Auth.Models
{
    public class UserRegisterViewModel
    {
        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required] [MaxLength(256)] public string Password { get; set; }
        
        [Required]
        [MaxLength(256)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}