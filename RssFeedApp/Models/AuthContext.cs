using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using RssFeedApp.Entities;

namespace RssFeedApp.Models
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("AuthContext") { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}