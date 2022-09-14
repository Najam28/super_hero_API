using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using SuperHeroAPI.Models;

namespace SuperHeroAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        { 
        }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Countries>().HasData(
                new Countries {CountryId = 1, CountryName = "Pakistan", IOSCode = "PK" },
                new Countries {CountryId = 2, CountryName = "UAE", IOSCode = "ARE" }

                );
        }

        public DbSet<SuperHero> SuperHeroes { get; set; }
        public DbSet<Student> students { get; set; }

        public DbSet<User> users { get; set; }

        public DbSet<Countries> countries { get; set; }

    }
}
