using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using AutoMapper;
using Thoughtwave.Data;
using Thoughtwave.Models;
using Thoughtwave.Services;
using Thoughtwave.Infrastructure;

namespace Thoughtwave
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; set; }
        private IHostingEnvironment Environment { get; set; }
        private MapperConfiguration MapperConfiguration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            // add AutoMapper profile
            MapperConfiguration = new MapperConfiguration(config => 
            {
                config.AddProfile(new AutoMapperProfileConfiguration());
            });

            Configuration = builder.Build();
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // setup cache and sessions
            services.AddDistributedMemoryCache();
            services.AddSession();

            // Add framework services.
            services.AddDbContext<ThoughtwaveDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                
                // Cookie settings
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(150);
                options.Cookies.ApplicationCookie.LoginPath = "/Account/LogIn";
                options.Cookies.ApplicationCookie.LogoutPath = "/Account/LogOff";
                
                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ThoughtwaveDbContext>()
                .AddDefaultTokenProviders();

            if (Environment.IsProduction())
            {
                services.AddMvc(options =>
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                });
            }
            else
            {
                services.AddMvc();
            }

            services.AddLogging();

            // add repository 
            services.AddScoped<IThoughtwaveRepository, ThoughtwaveRepository>();

            // add AutoMapper as singleton
            services.AddSingleton<IMapper>(sp => MapperConfiguration.CreateMapper());

            // use lowercase routes
            services.AddRouting(options => options.LowercaseUrls = true);

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IFileManager, FileManager>();
        }

        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Exception");
                app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
            }

            app.UseSession();

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = Configuration["Authentication:Facebook:AppId"],
                AppSecret = Configuration["Authentication:Facebook:AppSecret"]
            });

            app.UseTwitterAuthentication(new TwitterOptions()
            {
                ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"],
                ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"]
            });

            app.UseGoogleAuthentication(new GoogleOptions()
            {
                ClientId = Configuration["Authentication:Google:ClientId"],
                ClientSecret = Configuration["Authentication:Google:ClientSecret"]
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DatabaseInitializer.Initialize(app).Wait();
        }
    }
}
