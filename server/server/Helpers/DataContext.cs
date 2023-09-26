using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using server.Models;
using server.Models.Strava;

namespace server.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        protected void OnModelCreating(ModelBuilder modelBuilder, MigrationBuilder migrationBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("uuid-ossp");
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<StravaProfile> StravaProfile { get; set; }
        public DbSet<StravaActivity> StravaActivity { get; set; }
        public DbSet<StravaActivityLap> StravaActivityLap { get; set; }
    }
}
