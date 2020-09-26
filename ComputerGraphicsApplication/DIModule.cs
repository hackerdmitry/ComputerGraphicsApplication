using System.Linq;
using System.Reflection;
using Autofac;
using ComputerGraphicsApplication.Interfaces;
using ComputerGraphicsApplication.Models.Common;
using Module = Autofac.Module;

namespace ComputerGraphicsApplication
{
    public class DIModule : Module
    {
        private readonly ApplicationSettings _applicationSettings;

        public DIModule(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var domainTypes = new[]
            {
                typeof(IService)
            };

            builder.RegisterAssemblyTypes(GetType().GetTypeInfo().Assembly)
                   .Where(t => t.GetTypeInfo().ImplementedInterfaces.Intersect(domainTypes).Any())
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .InstancePerLifetimeScope();

            builder.RegisterInstance(_applicationSettings)
                   .AsSelf()
                   .SingleInstance();
        }
    }
}