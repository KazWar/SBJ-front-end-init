using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VueCliMiddleware;
using RMS.Web.Website.Whirlpool.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RMS.Configuration;

namespace RMS.Web.Website.Whirlpool.Startup
{
    public class Startup
    {
        private readonly string SPA_ROOT_PATH = "ClientApp";
        private readonly string SPA_SOURCE_PATH = "dist";

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _appConfiguration = hostEnvironment.GetAppConfiguration();
        }

        public IConfigurationRoot Configuration => _appConfiguration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // So that IOptions<T> could be injected wherever required.
            services.Configure<TenantConfiguration>(_appConfiguration.GetSection("Tenant"));
            services.Configure<AuthenticationConfiguration>(_appConfiguration.GetSection("Authentication"));
            services.Configure<PostCodeApi>(_appConfiguration.GetSection("PostCodeApi"));

            services.AddControllers().AddNewtonsoftJson();
            services.AddSpaStaticFiles(configuration: options => { options.RootPath = "ClientApp/dist"; });

            services.AddCors(options =>
            {
                options.AddPolicy("VueCorsPolicy", builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("https://Whirlpool.rms2.eu/");
                });
            });

            //services.AddAuthentication()
            //    .AddCookie(options =>
            //    {
            //        options.LoginPath = "/Account/Unauthorized/";
            //        options.AccessDeniedPath = "/Account/Forbidden/";
            //    })
            //    .AddJwtBearer(options =>
            //    {
            //        options.Audience = "https://Whirlpool.rms2.eu/";
            //        options.Authority = "https://Whirlpool.rms2.eu/";
            //        options.RequireHttpsMetadata = false;
            //    });

            services.AddMvc(option => option.EnableEndpointRouting = false);
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //});

            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = SPA_ROOT_PATH;
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("VueCorsPolicy");

            app.UseRouting();
            app.UseSpaStaticFiles();

            //app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });


            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = $"{SPA_ROOT_PATH}/";
                    spa.Options.StartupTimeout = TimeSpan.FromSeconds(120);
                    spa.UseVueCli(npmScript: "serve");
                }
                else
                {
                    spa.Options.SourcePath = SPA_ROOT_PATH;
                }
            });
        }
    }
}