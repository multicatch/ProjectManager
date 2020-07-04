using Autofac;

namespace ProjectManager.Projects
{
    public class ProjectsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectsRegistry>().AsSelf();
        }
    }
}