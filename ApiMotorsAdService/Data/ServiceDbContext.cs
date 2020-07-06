using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MotorsAdModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMotorsAdService.Data
{
    public class ServiceDbContext : DbContext
    {
        public ServiceDbContext(DbContextOptions<ServiceDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // Table´s base
        public virtual DbSet<Ad> tb_AnuncioWebmotors { get; set; }

    }

    #region Class For Migrations to work

    /// <summary>
    /// Class For Migrations to work
    /// </summary>
    public class BloggingContextFactory : IDesignTimeDbContextFactory<ServiceDbContext>
    {
        public ServiceDbContext CreateDbContext(string[] args)
        {
            var builderConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json");
            var configuration = builderConfiguration.Build();

            // This is for Migrations works !!!!
            var connectionString = configuration.GetConnectionString("MySqlLocalConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ServiceDbContext>();
            optionsBuilder.UseMySql(connectionString);

            return new ServiceDbContext(optionsBuilder.Options);
        }
    }

    #endregion
}
