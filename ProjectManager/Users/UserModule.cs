using Autofac;

namespace ProjectManager.Users
{
    public class UserModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BCryptPasswordHash>().As<IPasswordHash>();
            builder.RegisterType<CompositePolicyValidator>().As<IPasswordPolicyValidator>().SingleInstance();
            builder.RegisterType<UserRegistry>().AsSelf();
            builder.RegisterType<IdentityProvider>().AsSelf();
        }
    }
}