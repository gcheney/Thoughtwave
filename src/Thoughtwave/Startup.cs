using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Thoughtwave.Data;
using Thoughtwave.Models;
using Thoughtwave.ViewModels.ThoughtViewModels;
using Thoughtwave.ViewModels.ManageViewModels;
using Thoughtwave.ExtensionMethods;
using Thoughtwave.Services;
using AutoMapper;

namespace Thoughtwave
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

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
                    options.SslPort = 44321;
                    options.Filters.Add(new RequireHttpsAttribute());
                });
            }
            else
            {
                services.AddMvc();
            }

            services.AddLogging();

            services.AddScoped<IThoughtwaveRepository, ThoughtwaveRepository>();

            // use lowercase routes
            services.AddRouting(options => options.LowercaseUrls = true);

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory)
        {
            // Automapper configuration 
            Mapper.Initialize(config =>
            {
                config.CreateMap<CreateThoughtViewModel, Thought>()
                    .BeforeMap((src, dest) => src.Content = WebUtility.HtmlEncode(src.Content))
                    .BeforeMap((src, dest) => src.Tags = src.Tags.RemoveWhiteSpaces());

                config.CreateMap<EditThoughtViewModel, Thought>()
                    .BeforeMap((src, dest) => src.Content = WebUtility.HtmlEncode(src.Content))
                    .BeforeMap((src, dest) => src.Tags = src.Tags.RemoveWhiteSpaces())
                    .ReverseMap()
                    .AfterMap((src, dest) => dest.Content = WebUtility.HtmlDecode(dest.Content));

                config.CreateMap<EditProfileViewModel, User>().ReverseMap();
            });

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
