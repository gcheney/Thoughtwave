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
        public static async Task Initialize(IApplicationBuilder app)
        {
            // Get database context
            ApplicationDbContext context = app.ApplicationServices
                .GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureCreated();

            // Get user manager
            UserManager<ApplicationUser> userManager = app.ApplicationServices
                .GetRequiredService<UserManager<ApplicationUser>>();

            // seed user data
            var user1 = await SeedUserDataAsync(userManager, "genericuser1", "genericuser1@email.com", "Pa$$word1");
            var user2 = await SeedUserDataAsync(userManager, "genericuser2", "genericuser2@email.com", "Pa$$word2");
            var user3 = await SeedUserDataAsync(userManager, "genericuser3", "genericuser3@email.com", "Pa$$word3");

            // Seed other data
            if (!context.Questions.Any())
            {
                var question1 = new Question()
                {
                    CreatedOn = DateTime.UtcNow,
                    Title = "How Can I LIve Better",
                    Content = "I am trying to lead a better life, but I am not sure how. Does anyone have any advice?",
                    User = user1,
                    Answers = new List<Answer>()
                    {
                        new Answer() 
                        {  
                            Content = "Look to the way of Zen, man", 
                            CreatedOn = new DateTime(2016, 12, 4), 
                            User = user2,
                        },
                        new Answer() 
                        {  
                            Content = "Learning is the answer", 
                            CreatedOn = new DateTime(2016, 12, 10), 
                            User = user3,
                        }
                    }
                };

                context.Questions.Add(question1);
                context.Answers.AddRange(question1.Answers);

                var question2 = new Question()
                {
                    CreatedOn = DateTime.UtcNow,
                    Title = "What is the answer..",
                    Content = "...to life, the universe, and everything?",
                    User = user2,
                    Answers = new List<Answer>()
                    {
                        new Answer() 
                        {  
                            Content = "I had learned once that it was 24?", 
                            CreatedOn = new DateTime(2016, 12, 4), 
                            User = user1,
                        },
                        new Answer() 
                        {  
                            Content = "The answer is 42!", 
                            CreatedOn = new DateTime(2016, 12, 10), 
                            User = user3,
                        }
                    }
                };

                context.Questions.Add(question2);
                context.Answers.AddRange(question2.Answers);

                await context.SaveChangesAsync();
            }
        }

        public async static Task<ApplicationUser> SeedUserDataAsync(UserManager<ApplicationUser> userManager, 
            string userName, string email, string password)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
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

                IdentityResult result = await userManager.CreateAsync(newUser, password);
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

                return newUser;
            }

            return user;
        }
    }
}