using Autofac;

namespace ProjectManager.Users
{
    public class UserModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRegistry>().AsSelf();
            builder.RegisterType<CompositePolicyValidator>().As<IPasswordPolicyValidator>().SingleInstance();
        }
    }
}