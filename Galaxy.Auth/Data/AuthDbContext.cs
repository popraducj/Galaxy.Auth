using Galaxy.Auth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Galaxy.Auth.Data
{
    public class AuthDbContext : IdentityUserContext<User, int>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Options for EF DataContext</param>
        public AuthDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Data protection keys table used by the password hasher
        /// </summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
    }
}