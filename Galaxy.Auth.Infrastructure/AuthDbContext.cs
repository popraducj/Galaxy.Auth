using Galaxy.Auth.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Galaxy.Auth.Infrastructure
{
    public class AuthDbContext : IdentityUserContext<User, int>
    {
        public AuthDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Data protection keys table used by the password hasher
        /// </summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<InvitationToken> InvitationTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
    }
}