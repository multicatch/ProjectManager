using System.Data.Entity;
using SQLite.CodeFirst;

namespace ProjectManager.Database
{
    public class SQLiteConnectionInitializer : IConnectionInitializer
    {
        public IDatabaseInitializer<TContext> Create<TContext>(DbModelBuilder builder) where TContext : DbContext
        {
            return new SqliteCreateDatabaseIfNotExists<TContext>(builder);
        }
    }
}