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
        public static async void Initialize(IApplicationBuilder app)
        {
            // Seed users
            UserManager<ApplicationUser> userManager = app.ApplicationServices
                .GetRequiredService<UserManager<ApplicationUser>>();

            SeedUserData(userManager, "genericuser1", "genericuser1@email.com", "Pa$$word1");
            SeedUserData(userManager, "genericuser2", "genericuser2@email.com", "Pa$$word2");

            // Seed other data
            ApplicationDbContext context = app.ApplicationServices
                .GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureCreated();

            if (!context.Questions.Any())
            {
                var newQuestion = new Question()
                {
                    CreatedOn = DateTime.UtcNow,
                    Title = "How Can I LIve Better",
                    Content = "I am trying to lead a better life, but I am not sure how. Does anyone have any advice?",
                    User = await userManager.FindByNameAsync("genericuser1"),
                    Answers = new List<Answer>()
                    {
                        new Answer() 
                        {  
                            Content = "Look to the way of Zen, man", 
                            CreatedOn = new DateTime(2016, 12, 4), 
                            User = await userManager.FindByNameAsync("genericuser2"),
                        },
                        new Answer() 
                        {  
                            Content = "Answering my own question here, the answer is 42!", 
                            CreatedOn = new DateTime(2016, 12, 10), 
                            User = await userManager.FindByNameAsync("genericuser1"),
                        }
                    }
                };

                context.Questions.Add(newQuestion);
                context.Answers.AddRange(newQuestion.Answers);

                await context.SaveChangesAsync();
            }
        }

        public async static void SeedUserData(UserManager<ApplicationUser> userManager, string userName, 
            string email, string password)
        {
            if (await userManager.FindByNameAsync(userName) == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    FirstName = "John",
                    LastName = "Doe",
                    SignUpDate = DateTime.UtcNow,
                    Avatar = "/dist/images/generic-user.jpg"
                };

                var result = await userManager.CreateAsync(newUser, password);
                if (result.Succeeded) 
                {
                    Console.WriteLine($"User {newUser} successfully created");
                }
                else 
                {
                    Console.WriteLine($"Errors occured trying to creat {newUser}");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error);
                    }
                }
            }
        }
    }
}