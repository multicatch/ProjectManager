using System.Data.Entity;

namespace ProjectManager.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly IConnectionInitializer _connectionInitializer;

        public DatabaseContext(IConnectionInitializer connectionInitializer)
        {
            this._connectionInitializer = connectionInitializer;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var databaseInitializer = _connectionInitializer.Create<DatabaseContext>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(databaseInitializer);
        }
    }

    public interface IConnectionInitializer
    {
        public IDatabaseInitializer<TContext> Create<TContext>(DbModelBuilder builder) where TContext : DbContext;
    }
}