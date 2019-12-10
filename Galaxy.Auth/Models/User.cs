using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Models
{
    public class User: IdentityUser<int>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
    }
}