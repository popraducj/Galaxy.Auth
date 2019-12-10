using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Galaxy.Auth.Core.Models
{
    public class User: IdentityUser<int>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
    }
}