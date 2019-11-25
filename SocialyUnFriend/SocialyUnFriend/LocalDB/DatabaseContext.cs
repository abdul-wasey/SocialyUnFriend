using Microsoft.EntityFrameworkCore;
using SocialyUnFriend.Common;
using SocialyUnFriend.Model;

namespace SocialyUnFriend.LocalDB
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(string.Format("Filename={0}", Constants.dbPath));
        }

        
    }
}
