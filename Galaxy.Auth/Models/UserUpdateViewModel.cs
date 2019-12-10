using System.ComponentModel.DataAnnotations;

namespace Galaxy.Auth.Models
{
    public class UserUpdateViewModel
    {
        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}