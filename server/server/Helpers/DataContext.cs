using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using server.Models;
using server.Models.Profile;
using server.Models.Profile.Summary;
using server.Models.Activity;

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
        public DbSet<StravaProfileStats> StravaProfileStats { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<ActivityLap> ActivityLap { get; set; }
        public DbSet<ProfileHeartRate> ProfileHeartRate { get; set; }
        public DbSet<ProfilePower> ProfilePower { get; set; }
        public DbSet<ProfileWeeklySummary> ProfileWeeklySummary { get; set; }
        public DbSet<ProfileMonthlySummary> ProfileMonthlySummary { get; set; }
        public DbSet<ProfileYearlySummary> ProfileYearlySummary { get; set; }
        public DbSet<TrainingLoads> TrainingLoads { get; set; }
    }
}
