using Abp.AspNetZeroCore;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RMS.Configuration;
using RMS.EntityFrameworkCore;
using RMS.Web.Authentication.JwtBearer;
using RMS.Web.Models.TokenAuth;
using RMS.Web.Mvc.Services;
using RMS.Web.Website.Whirlpool.Filters;
using RMS.Web.Website.Whirlpool.Services;

namespace RMS.Web.Website.Whirlpool.Startup
{
    [DependsOn(
        typeof(RMSWebCoreModule)
    )]
    public class RMSWebFrontEndModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IAuthenticationAppService _authenticationAppService;

        public RMSWebFrontEndModule(
            IWebHostEnvironment env, 
            RMSEntityFrameworkCoreModule abpZeroTemplateEntityFrameworkCoreModule)
        {
            _appConfiguration = env.GetAppConfiguration();
            abpZeroTemplateEntityFrameworkCoreModule.SkipDbSeed = true;
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:WebSiteRootAddress"] ?? "https://localhost:44303/";
            Configuration.Modules.AspNetZero().LicenseCode = _appConfiguration["AbpZeroLicenseCode"];

            //Changed AntiForgery token/cookie names to not conflict to the main application while redirections.
            Configuration.Modules.AbpWebCommon().AntiForgery.TokenCookieName = "Public-XSRF-TOKEN";
            Configuration.Modules.AbpWebCommon().AntiForgery.TokenHeaderName = "Public-X-XSRF-TOKEN";

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            Configuration.Navigation.Providers.Add<FrontEndNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RMSWebFrontEndModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            // Register our custom dependencies.
            IocManager.Register<IsAuthenticatedFilter>(Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.Register<IFrontEndAppService, FrontEndAppService>(Abp.Dependency.DependencyLifeStyle.Transient);
        }
    }
}
