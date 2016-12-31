using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Thoughtwave.Data;
using Thoughtwave.Models;

namespace Thoughtwave.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IThoughtwaveRepository _repository;
        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<User> _userManager;

        public UsersController(IThoughtwaveRepository repository, 
            ILogger<UsersController> logger,
            UserManager<User> userManager)
        {
            _repository = repository;
            _logger = logger;
            _userManager = userManager;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/users/admin/grant/{userId}")]
        public async Task<IActionResult> GrantAdminAccess(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogError($"Unable to locate user with id {userId}");
                return View("Error");
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    TempData["success"] = $"Successfully made user {user.UserName} an admin";
                    return RedirectToAction("Manage");
                }
                else
                {
                    TempData["error"] = $"An error occurred. User {user.UserName} was not made an admin";
                    return RedirectToAction("Manage");
                }
            }

            TempData["error"] = $"User {user.UserName} is already an admin";
            return RedirectToAction("Manage");
        }


    }
}
