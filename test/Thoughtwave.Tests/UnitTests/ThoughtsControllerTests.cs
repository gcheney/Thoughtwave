using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using Thoughtwave.Infrastructure;
using Thoughtwave.Controllers;
using Thoughtwave.Data;
using Thoughtwave.Models;
using Thoughtwave.ViewModels.ThoughtViewModels;
using Xunit;

namespace Thoughtwave.Tests.UnitTests
{
    public class ThoughtsControllerTests
    {
        [Fact]
        public async Task Read_ReturnsBadRequest_WhenIdIsNull()
        {
            Console.WriteLine("Running test: Read_ReturnsBadRequest_WhenIdIsNull");
            // Arrange
            var controller = new ThoughtsController(null, null, null, null, new LoggerFactory());

            // Act
            var result = await controller.Read(id: null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<String>(badRequestResult.Value);
        }



        /*
        
        */

        private List<Thought> GetTestThoughts()
        {
            var thoughts = new List<Thought>();

            thoughts.Add(new Thought()
            {
                Title = "Generic Title 1",
                Content = "Generic contents.",
                Author = new User(),
                Category = Category.Film,
                Tags = "film,art,indie,hip,cinephile",
                Image = "http://www.victoriabid.co.uk/wp-content/uploads/2015/02/Film.jpg",
                Comments = new List<Comment>()
            });

            thoughts.Add(new Thought()
            {   
                Title = "Generic Title 2",
                Content = "Generic contents.",
                Author = new User(),
                Category = Category.Film,
                Tags = "film,art,indie,hip,cinephile",
                Image = "http://www.victoriabid.co.uk/wp-content/uploads/2015/02/Film.jpg",
                Comments = new List<Comment>()
            });

            thoughts.Add(new Thought()
            {
                Title = "Generic Title 3",
                Content = "Generic contents.",
                Author = new User(),
                Category = Category.Film,
                Tags = "film,art,indie,hip,cinephile",
                Image = "http://www.victoriabid.co.uk/wp-content/uploads/2015/02/Film.jpg",
                Comments = new List<Comment>()
            });

            return thoughts;
        }

        private IMapper GetMapper()
        {
            return new MapperConfiguration(config => 
            {
                config.AddProfile(new AutoMapperProfileConfiguration());
            }).CreateMapper();
        }
    }
}
