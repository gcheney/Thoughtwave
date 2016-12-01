using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Thoughtwave.Models;

namespace Thoughtwave.Data
{
    public static class DatabaseInitializer
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
            if (!context.Articles.Any())
            {
                var Articles = new List<Article>()
                {
                    new Article()
                    {
                        Title = "How Can I Live Better",
                        Content = "Forage gochujang vape, mustache tumeric church-key master cleanse salvia godard hella hoodie everyday carry freegan. Sustainable forage aesthetic, neutra scenester lyft bespoke roof party taxidermy next level meggings coloring book. Jean shorts keffiyeh tacos migas normcore scenester. Fashion axe williamsburg lo-fi flexitarian unicorn ennui edison bulb.",
                        User = user1,
                        Comments = new List<Comment>()
                        {
                            new Comment() 
                            {  
                                Content = " Swag flannel kale chips microdosing church-key chia twee copper mug, unicorn hell of XOXO ethical letterpress fam. IPhone bitters gastropub austin, lo-fi semiotics kombucha forage coloring book ramps. Heirloom wolf trust fund flexitarian bicycle rights pop-up.",  
                                User = user2,
                            },
                            new Comment() 
                            {  
                                Content = "Snackwave 3 wolf moon tacos, fashion axe copper mug YOLO neutra disrupt hashtag vexillologist succulents. Four loko cardigan pop-up actually, salvia man braid banjo banh mi 3 wolf moon tumblr. Literally salvia neutra quinoa.", 
                                User = user3,
                            }
                        }
                    },
                    new Article()
                    {
                        Title = "What is the Comment..",
                        Content = "Vice cliche 8-bit, waistcoat tbh beard four dollar toast XOXO paleo vinyl disrupt. Stumptown YOLO celiac mlkshk, glossier hexagon schlitz four dollar toast fixie hot chicken yuccie green juice. Try-hard artisan jianbing intelligentsia trust fund gentrify prism. Typewriter hell of gochujang, brunch post-ironic DIY kogi locavore marfa prism 90's. Succulents lomo deep v, tousled whatever humblebrag bicycle rights before they sold out wayfarers skateboard selfies green juice. Photo booth cray schlitz copper mug, sartorial keffiyeh selfies letterpress offal distillery af woke. Brunch master cleanse ugh craft beer mlkshk knausgaard.",
                        User = user2,
                        Comments = new List<Comment>()
                        {
                            new Comment() 
                            {  
                                Content = "Kogi scenester iceland neutra polaroid tumeric snackwave craft beer. Authentic wolf man bun succulents messenger bag blog. Hexagon hoodie schlitz, celiac migas trust fund whatever.", 
                                User = user1,
                            },
                            new Comment() 
                            {  
                                Content = "Street art godard viral photo booth succulents hexagon. Occupy tumeric twee biodiesel, ", 
                                User = user3,
                            }
                        }
                    },
                    new Article()
                    {
                        Title = "What is the Comment part 2..",
                        Content = "Vice cliche 8-bit, waistcoat tbh beard four dollar toast XOXO paleo vinyl disrupt. Stumptown YOLO celiac mlkshk, glossier hexagon schlitz four dollar toast fixie hot chicken yuccie green juice. Try-hard artisan jianbing intelligentsia trust fund gentrify prism. Typewriter hell of gochujang, brunch post-ironic DIY kogi locavore marfa prism 90's. Succulents lomo deep v, tousled whatever humblebrag bicycle rights before they sold out wayfarers skateboard selfies green juice. Photo booth cray schlitz copper mug, sartorial keffiyeh selfies letterpress offal distillery af woke. Brunch master cleanse ugh craft beer mlkshk knausgaard.",
                        User = user2,
                        Comments = new List<Comment>()
                        {
                            new Comment() 
                            {  
                                Content = "Kogi scenester iceland neutra polaroid tumeric snackwave craft beer. Authentic wolf man bun succulents messenger bag blog. Hexagon hoodie schlitz, celiac migas trust fund whatever.", 
                                User = user1,
                            },
                            new Comment() 
                            {  
                                Content = "Street art godard viral photo booth succulents hexagon. Occupy tumeric twee biodiesel, ", 
                                User = user3,
                            }
                        }
                    },
                    new Article()
                    {
                        Title = "What is the Comment part 3..",
                        Content = "Vice cliche 8-bit, waistcoat tbh beard four dollar toast XOXO paleo vinyl disrupt. Stumptown YOLO celiac mlkshk, glossier hexagon schlitz four dollar toast fixie hot chicken yuccie green juice. Try-hard artisan jianbing intelligentsia trust fund gentrify prism. Typewriter hell of gochujang, brunch post-ironic DIY kogi locavore marfa prism 90's. Succulents lomo deep v, tousled whatever humblebrag bicycle rights before they sold out wayfarers skateboard selfies green juice. Photo booth cray schlitz copper mug, sartorial keffiyeh selfies letterpress offal distillery af woke. Brunch master cleanse ugh craft beer mlkshk knausgaard.",
                        User = user2,
                        Comments = new List<Comment>()
                        {
                            new Comment() 
                            {  
                                Content = "Kogi scenester iceland neutra polaroid tumeric snackwave craft beer. Authentic wolf man bun succulents messenger bag blog. Hexagon hoodie schlitz, celiac migas trust fund whatever.", 
                                User = user1,
                            },
                            new Comment() 
                            {  
                                Content = "Street art godard viral photo booth succulents hexagon. Occupy tumeric twee biodiesel, ", 
                                User = user3,
                            }
                        }
                    },
                    new Article()
                    {
                        Title = "What is the Comment part 4..",
                        Content = "Vice cliche 8-bit, waistcoat tbh beard four dollar toast XOXO paleo vinyl disrupt. Stumptown YOLO celiac mlkshk, glossier hexagon schlitz four dollar toast fixie hot chicken yuccie green juice. Try-hard artisan jianbing intelligentsia trust fund gentrify prism. Typewriter hell of gochujang, brunch post-ironic DIY kogi locavore marfa prism 90's. Succulents lomo deep v, tousled whatever humblebrag bicycle rights before they sold out wayfarers skateboard selfies green juice. Photo booth cray schlitz copper mug, sartorial keffiyeh selfies letterpress offal distillery af woke. Brunch master cleanse ugh craft beer mlkshk knausgaard.",
                        User = user2,
                        Comments = new List<Comment>()
                        {
                            new Comment() 
                            {  
                                Content = "Kogi scenester iceland neutra polaroid tumeric snackwave craft beer. Authentic wolf man bun succulents messenger bag blog. Hexagon hoodie schlitz, celiac migas trust fund whatever.", 
                                User = user1,
                            },
                            new Comment() 
                            {  
                                Content = "Street art godard viral photo booth succulents hexagon. Occupy tumeric twee biodiesel, ", 
                                User = user3,
                            }
                        }
                    },
                    new Article()
                    {
                        Title = "What is the Comment part 5..",
                        Content = "Stumptown iceland lumbersexual vexillologist put a bird on it. Street art organic butcher, cliche lumbersexual keffiyeh wolf neutra tumeric biodiesel asymmetrical celiac swag. Poke austin umami, pok pok meggings church-key lomo. Health goth hella pitchfork craft beer listicle celiac.",
                        User = user2,
                        Comments = new List<Comment>()
                        {
                            new Comment() 
                            {  
                                Content = "Kogi scenester iceland neutra polaroid tumeric snackwave craft beer. Authentic wolf man bun succulents messenger bag blog. Hexagon hoodie schlitz, celiac migas trust fund whatever.", 
                                User = user1,
                            },
                            new Comment() 
                            {  
                                Content = "Street art godard viral photo booth succulents hexagon. Occupy tumeric twee biodiesel, ", 
                                User = user3,
                            }
                        }
                    }
                };

                foreach (Article Article in Articles)
                {
                    context.Articles.Add(Article);
                    context.Comments.AddRange(Article.Comments);
                }

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
                    Email = email
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