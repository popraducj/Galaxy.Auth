using System;
using System.ComponentModel.DataAnnotations;

namespace Galaxy.Auth.Core.Models
{
    public class InvitationToken
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Token { get; set; }
        
        public string Email { get; set; }
    }
}
