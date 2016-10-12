using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Sophophile.Models;

namespace Sophophile.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
             using (var context = new ApplicationDbContext(
                    serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
             using (var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>())
             {
                    context.Database.EnsureCreated();

                    // Look for any users.
                    if (context.Users.ToList().Count > 0)
                    {
                        return;   // DB has been seeded
                    }

                    var users = new ApplicationUser[]
                    {
                        new ApplicationUser 
                        { 
                            FirstName="Carson", 
                            LastName="Alexander", 
                            SignUpDate=DateTime.Parse("2016-09-01"), 
                            Email="user@domain.com", 
                            UserName="calex"
                        }
                    };

                    foreach (ApplicationUser user in users)
                    {
                        userManager.CreateAsync(user, "pa$$word1");
                    }
             }
        }
    }
}