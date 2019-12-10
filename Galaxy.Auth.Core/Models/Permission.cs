using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Galaxy.Auth.Core.Enums;

namespace Galaxy.Auth.Core.Models
{
    [Table("UserPermissions")]
    public class Permission
    {
        [Key]
        public int UserId { get; set; }
        public UserPermission UserPermission { get; set; }
        public User User { get; set; }
    }
}