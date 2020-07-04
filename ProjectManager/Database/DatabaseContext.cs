using Microsoft.EntityFrameworkCore;
using ProjectManager.Database.Models;

namespace ProjectManager.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly IConnectionInitializer _connectionInitializer;
        
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Issue> Issues { get; set; }

        public DatabaseContext(IConnectionInitializer connectionInitializer)
        {
            this._connectionInitializer = connectionInitializer;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _connectionInitializer.Configure(optionsBuilder);
        }
    }

    public interface IConnectionInitializer
    {
        public void Configure(DbContextOptionsBuilder optionsBuilder);
    }
}