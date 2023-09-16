using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using server.Models;

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

            // migrationBuilder.AlterColumn<Guid>(
            //     name: "ID",
            //     table: "Users",
            //     type: "uuid using \"ID\"::uuid",
            //     nullable: false,
            //     oldClrType: typeof(long),
            //     oldType: "integer"
            //     );
        }
        
        public DbSet<User> Users { get; set; }
    }
}
