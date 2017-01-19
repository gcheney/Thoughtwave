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

        [Fact]
        public async Task Read_ReturnsNotFound_WhenThoughtNotFound()
        {
            Console.WriteLine("Running test: Read_ReturnsNotFound_WhenThoughtNotFound");

            // Arrange
            int testThoughtId = 666;
            var mockRepo = new Mock<IThoughtwaveRepository>();
            mockRepo.Setup(repo => repo.GetThoughtAndCommentsByIdAsync(testThoughtId))
                .Returns(Task.FromResult((Thought)null));
            var controller = new ThoughtsController(mockRepo.Object, null, null, null, new LoggerFactory());

            // Act
            var result = await controller.Read(testThoughtId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Read_ReturnsViewResult_WithThoughtViewModel()
        {
            Console.WriteLine("Running test: Read_ReturnsViewResult_WithThoughtViewModel");

            // Arrange
            int testThoughtId = 1;
            var mockRepo = new Mock<IThoughtwaveRepository>();
            mockRepo.Setup(repo => repo.GetThoughtAndCommentsByIdAsync(testThoughtId))
                .Returns(Task.FromResult(GetTestThoughts().FirstOrDefault(t => t.Id == testThoughtId)));
            var mapper = GetMapper();
            var controller = new ThoughtsController(mockRepo.Object, null, null, mapper, new LoggerFactory());

            // Act
            var result = await controller.Read(testThoughtId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ThoughtViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Generic Title 1", model.Title);
            Assert.Equal(2, model.CreatedOn.Day);
            Assert.Equal(testThoughtId, model.Id);
        }


        [Fact]
        public async Task CreatePost_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            Console.WriteLine("Running test: CreatePost_ReturnsViewResult_WhenModelStateIsInvalid");
            // Arrange
            var mockRepo = new Mock<IThoughtwaveRepository>();
            mockRepo
                .Setup(repo => repo.GetAllThoughtsAsync())
                .Returns(Task.FromResult(GetTestThoughts()));
            var mapper = GetMapper();
            var controller = new ThoughtsController(null, null, null, mapper, new LoggerFactory());
            controller.ModelState.AddModelError("Title", "Required");
            var newThought = new CreateThoughtViewModel();

            // Act
            var result = await controller.Create(newThought);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CreateThoughtViewModel>(viewResult.Model);
        }

        private List<Thought> GetTestThoughts()
        {
            var thoughts = new List<Thought>();

            thoughts.Add(new Thought()
            {
                Title = "Generic Title 1",
                Id = 1,
                Content = "Generic contents.",
                CreatedOn = new DateTime(2017, 2, 2),
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
                Id = 2,
                Author = new User(),
                Category = Category.Film,
                Tags = "film,art,indie,hip,cinephile",
                Image = "http://www.victoriabid.co.uk/wp-content/uploads/2015/02/Film.jpg",
                Comments = new List<Comment>()
            });

            thoughts.Add(new Thought()
            {
                Title = "Generic Title 3",
                Id = 3,
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
