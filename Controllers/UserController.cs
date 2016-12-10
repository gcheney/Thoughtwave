using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thoughtwave.Data;
using Thoughtwave.Models;

namespace Thoughtwave.Controllers
{
    public class UsersController : Controller
    {
        private IThoughtwaveRepository _repository;
        private ILogger<UsersController> _logger;

        public UsersController(IThoughtwaveRepository repository, 
            ILogger<UsersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [Route("/users")]
        public async Task<IActionResult> Index()
        {
            var users = await _repository.GetAllUsersAsync();

            if (users == null)
            {
                _logger.LogError("Unable to retrieve users from repository");
                users = new List<User>();
            }
            
            if (!users.Any())
            {
                ViewBag.Message = "No users found";
            }

            ViewBag.Content = "Thoughtwave Users";
            
            return View(users);
        }
    }
}
