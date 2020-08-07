using Assets.Models.Configurations;
using Assets.Models.DataModels;

using Microsoft.EntityFrameworkCore;

namespace Assets.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Repair> Repair { get; set; }
        public DbSet<Repositions> Repositions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=AssetsSqlite.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AssetConfigurations());
        }
    }
}