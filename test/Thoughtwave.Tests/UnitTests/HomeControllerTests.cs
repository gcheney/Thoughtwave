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
    public class HomeControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfThoughts()
        {
            Console.WriteLine("Runnig test: Index_ReturnsAViewResult_WithAListOfThoughts");

            // Arrange
            var mockRepo = new Mock<IThoughtwaveRepository>();
            mockRepo.Setup(repo => repo.GetRecentThoughtsAsync())
                .Returns(Task.FromResult(GetTestThoughts()));
            var mapper = GetMapper();
            var controller = new HomeController(mockRepo.Object, mapper, new LoggerFactory());

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ThoughtViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Count());
        }

        

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

        private List<Thought> GetNullThoughts()
        {
            return null;
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
