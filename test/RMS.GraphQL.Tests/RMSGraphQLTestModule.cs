using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RMS.Configure;
using RMS.Startup;
using RMS.Test.Base;

namespace RMS.GraphQL.Tests
{
    [DependsOn(
        typeof(RMSGraphQLModule),
        typeof(RMSTestBaseModule))]
    public class RMSGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RMSGraphQLTestModule).GetAssembly());
        }
    }
}