using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Database
{
    public class SQLiteConnectionInitializer : IConnectionInitializer
    {

        public void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "projectManager.sqlite",
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }
    }
}