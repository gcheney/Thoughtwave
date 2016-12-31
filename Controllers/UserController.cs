using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Thoughtwave.Data;
using Thoughtwave.Models;

namespace Thoughtwave.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly IThoughtwaveRepository _repository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IThoughtwaveRepository repository, 
            ILogger<UsersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/users")]
        public async Task<IActionResult> Index()
        {
            var users = await _repository.GetAllUsersAsync();

            if (users == null)
            {
                _logger.LogError("Unable to retrieve users from repository");
                return View("Error");
            }
            
            if (!users.Any())
            {
                ViewBag.Message = "No users found";
            }

            ViewBag.Content = "Thoughtwave Users";
            
            return View(users);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/users/{username}")]
        public async Task<IActionResult> Details(string username)
        {
            if (username == null)
            {
                _logger.LogError("Invalid username provided");
                return NotFound();
            }

            var user = await _repository.GetUserActivityByUserNameAsync(username);

            if (user == null)
            {
                _logger.LogError($"No user found for username {username}");
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        [Route("/users/manage")]
        public async Task<IActionResult> Manage()
        {
            var users = await _repository.GetAllUsersAsync();

            if (users == null)
            {
                _logger.LogError("Unable to retrieve users from repository");
                return View("Error");
            }
            
            if (!users.Any())
            {
                ViewBag.Message = "No users found";
            }

            ViewBag.Content = "Thoughtwave Users - Admin Panel";
            
            return View(users);
        }
    }
}
