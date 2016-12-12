using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thoughtwave.Data;
using Thoughtwave.Models;
using Thoughtwave.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Thoughtwave.Controllers
{
    public class ThoughtsController : Controller
    {
        private readonly IThoughtwaveRepository _repository;
        private readonly ILogger<ThoughtsController> _logger;
        private readonly UserManager<User> _userManager;

        public ThoughtsController(IThoughtwaveRepository repository, 
            UserManager<User> userManager,
            ILogger<ThoughtsController> logger)
        {
            _repository = repository;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("/all")]
        public async Task<IActionResult> Index()
        {
            var thoughts = await _repository.GetAllThoughtsAsync();
            ViewBag.Content = "All Thoughts";

            if (thoughts == null)
            {
                _logger.LogError("Unable to retrieve all thoughts from repository");
                return View("Error");
            }
            
            if (!thoughts.Any())
            {
                ViewBag.Message = "No thoughts found";
            }

            return View(thoughts);
        }

        [HttpGet]
        [Route("{categoryId}/{id}/{slug}")]
        public async Task<IActionResult> Read(int id)
        {
            var thought = await _repository.GetThoughtByIdAsync(id);

            if (thought == null)
            {
                _logger.LogError($"Unable to retrieve thought with id {id} from repository");
                return View("Error");
            }
            
            return View(thought);
        }
        
        [HttpGet]
        [Route("/{categoryId}")]
        public async Task<IActionResult> CategoryIndex(string categoryId)
        {
            if (categoryId == null)
            {
                _logger.LogError("Invalid category provided");
                return RedirectToAction("Index");
            }
            
            Category category = GetCategoryFromString(categoryId);
            if (category != Category.None) 
            {
                var thoughts = await _repository.GetThoughtsByCategoryAsync(category);

                if (thoughts == null)
                {
                    _logger.LogError("Unable to retrieve thoughts for category {categoryId}");
                    return View("Error");
                }
                
                if (!thoughts.Any())
                {
                    ViewBag.Message = "No thoughts found for this search";
                }

                ViewBag.Content = "Thoughts on " + category.ToString();
                return View("Index", thoughts);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> Search(string q, string c = "All")
        {
            Category categroy = GetCategoryFromString(c);
            IEnumerable<Thought> thoughts = null;

            if (categroy != Category.None) 
            {
                thoughts = await _repository.GetThoughtsByQueryAsync(q, categroy);
                ViewBag.Content = "Search Results in " + categroy.ToString();
            }
            else 
            {
                thoughts = await _repository.GetThoughtsByQueryAsync(q);
                ViewBag.Content = "Search Results";
            }

            if (thoughts == null)
            {
                _logger.LogError("Unable to retrieve thoughts from repository");
                return View("Error");
            }

            if (!thoughts.Any())
            {
                ViewBag.Message = "No thoughts found for this search";
            }

            return View("Index", thoughts);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }
            
            var thoughts = await _repository.GetThoughtsByUserNameAsync(user.UserName);

            if (thoughts == null)
            {
                _logger.LogError($"Unable to retrieve thoughts from repository for {user.UserName}");
                return View("Error");
            }

            if (!thoughts.Any())
            {
                ViewBag.Message = "You haven't created any thoughts yet!";
            }

            ViewBag.Content = "Your Thoughts";
            return View(thoughts);
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        
        private Category GetCategoryFromString(string categoryStr)
        {
            Category category;

            if (Enum.TryParse(categoryStr.Capitalize(), true, out category) 
                && Enum.IsDefined(typeof(Category), category)) 
            {
                return category;
            }
            else 
            {
                return Category.None;
            }
        }
    }
}
