using Autofac;
using ProjectManager.Projects.Issues;

namespace ProjectManager.Projects
{
    public class ProjectsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectsRegistry>().AsSelf();
            builder.RegisterType<IssueRegistry>().AsSelf();
        }
    }
}