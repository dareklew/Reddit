using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reddit.Contracts.Entities;
using System.Collections.Generic;

namespace Reddit.DAL
{
    public class RedditDbContext : DbContext
    {
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging();
            optionsBuilder.UseInMemoryDatabase(databaseName: "RedditDb");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Post> Posts { get; set; }
        
    }
}
