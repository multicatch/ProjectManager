using Autofac;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Database
{
    public class DatabaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DatabaseContext>().AsSelf().SingleInstance();
            builder.RegisterType<SQLiteConnectionInitializer>().As<IConnectionInitializer>();
            builder.RegisterBuildCallback(PrepareContext);
        }

        private static void PrepareContext(ILifetimeScope scope)
        {
            var databaseContext = scope.Resolve<DatabaseContext>();
            databaseContext.Database.EnsureCreated();
            databaseContext.Database.Migrate();
        }
    }
}