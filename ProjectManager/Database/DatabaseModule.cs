using Autofac;

namespace ProjectManager.Database
{
    public class DatabaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DatabaseContext>();
            builder.RegisterType<SQLiteConnectionInitializer>().As<IConnectionInitializer>();
        }
    }
}