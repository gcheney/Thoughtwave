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
using Thoughtwave.ViewModels.ThoughtViewModels;
using Thoughtwave.ExtensionMethods;
using AutoMapper;


namespace Thoughtwave.Controllers
{
    [Authorize]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        [Route("/{categoryId}")]
        public async Task<IActionResult> CategoryIndex(string categoryId)
        {
            if (categoryId == null)
            {
                _logger.LogError("Invalid category provided");
                return NotFound();
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
                    ViewBag.Message = $"No thoughts found for {category.ToString()}";
                }

                ViewBag.Content = $"Thoughts on {category.ToString()}";
                return View("Index", thoughts);
            }
            else
            {
                return Redirect("/all");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/search")]
        public async Task<IActionResult> Search(string q, string c = "All")
        {
            Category categroy = GetCategoryFromString(c);
            IEnumerable<Thought> thoughts = null;

            if (categroy != Category.None) 
            {
                thoughts = await _repository.GetThoughtsByQueryAsync(q, categroy);
                ViewBag.Content = $"Search Results in {categroy.ToString()}";
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateThoughtViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var thought = Mapper.Map<Thought>(viewModel);

                // Save associated Thought author
                thought.Author = await GetCurrentUserAsync();

                // Save to the database
                _repository.AddThought(thought);

                if (await _repository.CommitChangesAsync())
                {
                    var thoughtUrl = GetThoughtUrl(thought);
                    TempData["success"] = "Your new Thought has been created";
                    return Redirect(thoughtUrl);
                }
                else 
                {
                    _logger.LogError($"Issue saving thought: {thought.Title} by {thought.Author.UserName}");
                    TempData["error"] = "There was an issue creating your new Thought";
                    return RedirectToAction("Manage");
                }
            }

            // issue with model state
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{categoryId}/{id}/{slug}")]
        public async Task<IActionResult> Read(int id)
        {
            var thought = await _repository.GetThoughtAndCommentsByIdAsync(id);

            if (thought == null)
            {
                _logger.LogInformation($"Unable to retrieve thought with id {id} from repository");
                return NotFound();
            }
            
            return View(thought);
        }

        [HttpGet]
        [Route("/thoughts/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var thought = await _repository.GetThoughtByIdAsync(id);

            if (thought == null)
            {
                _logger.LogInformation($"Unable to retrieve thought with id {id} for editing");
                return NotFound();
            }

            // block non-authors from viewing page
            if (await UserIsThoughtAuthorAsync(thought))
            {
                // current user is author
                var viewModel = Mapper.Map<EditThoughtViewModel>(thought);
                ViewBag.Title = $"Editing {thought.Title}";
                return View(viewModel);
            }

            // current user is not author
            return Forbid();
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<IActionResult> Edit(int id, EditThoughtViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var thought = Mapper.Map<Thought>(viewModel);
                thought.Id = id;
                _repository.UpdateThought(thought); 

                var updatedThought = await _repository.GetThoughtByIdAsync(id);
                if (await UserIsThoughtAuthorAsync(updatedThought))
                {
                    if (await _repository.CommitChangesAsync())
                    {
                        var thoughtUrl = GetThoughtUrl(thought);
                        TempData["success"] = "Thought successfully saved";
                        return Redirect(thoughtUrl);
                    }
                    else 
                    {
                        _logger.LogError($"Issue saving changes for thought with id: {thought.Id}");
                        TempData["error"] = "There was an issue saving your changes";
                        return RedirectToAction("Manage");
                    }
                }

                // user is not thought author
                return Forbid();
            }

            // issue with model state 
            return View(viewModel);
        }

        [HttpGet]
        [Route("/thoughts/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var thought = await _repository.GetThoughtByIdAsync(id);

            if (thought == null)
            {
                _logger.LogInformation($"Unable to retrieve thought with id {id}");
                return NotFound();
            }

            // block non-authors from viewing page
            if (await UserIsThoughtAuthorAsync(thought))
            {
                // current user is author
                ViewBag.Title = $"Delete {thought.Title}?";
                return View(thought);
            }

            // current user is not author
            return Forbid();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thought = await _repository.GetThoughtByIdAsync(id);

            if (await UserIsThoughtAuthorAsync(thought))
            {
                _repository.DeleteThought(thought);
                
                if (await _repository.CommitChangesAsync())
                {
                    TempData["success"] = "Thought successfully deleted";
                    return RedirectToAction("Manage");
                }

                // an error occured saving changes
                _logger.LogError($"Unable to commit changes for deleting thought with id: {id}");
                TempData["error"] = "Something went wrong. Thought not deleted";
                return RedirectToAction("Manage");
            }

            // current user is not author
            return Forbid();
        }

        /* ------- HELPER METHODS ---------- */

        private async Task<bool> UserIsThoughtAuthorAsync(Thought thought)
        {
            var currentUser = await GetCurrentUserAsync();
            return currentUser.Id == thought.Author.Id;
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private string GetThoughtUrl(Thought thought)
        {
            var category = thought.Category.ToString().ToLower();
            var id = thought.Id;
            var slug = thought.Slug.ToLower();
            return $"/{category}/{id}/{slug}";
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
