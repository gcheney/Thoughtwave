using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Sophophile.Models;

namespace Sophophile.Data
{
    public static class SeedData
    {
        private const string adminUsername = "admin";
        private const string adminEmail = "admin@company.com";
        private const string adminPassword = "Password123$";

        public static async void Initialize(IApplicationBuilder app)
        {
            // Seed users
            UserManager<ApplicationUser> userManager = app.ApplicationServices
                .GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationUser user = await userManager.FindByNameAsync(adminUsername);

            if (user == null) {
                user = new ApplicationUser { UserName = adminUsername, Email = adminEmail };
                Console.WriteLine("Adding admin user to database");
                await userManager.CreateAsync(user, adminPassword);
            } else {
                Console.WriteLine("admin user already exist");
            }

            // Seed other data
            ApplicationDbContext context = app.ApplicationServices
                .GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureCreated();
        }
    }
}