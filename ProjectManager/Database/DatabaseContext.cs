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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProject>()
                .HasKey(up => new { up.ProjectId, up.UserId });
            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(up => up.ProjectId);
            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(up => up.UserId);
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Issues)
                .WithOne(i => i.Project);
            modelBuilder.Entity<Issue>()
                .HasMany(i => i.Children)
                .WithOne(i => i.Parent);
        }
    }

    public interface IConnectionInitializer
    {
        public void Configure(DbContextOptionsBuilder optionsBuilder);
    }
}