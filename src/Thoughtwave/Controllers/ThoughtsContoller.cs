using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Thoughtwave.Data;
using Thoughtwave.Models;
using Thoughtwave.ViewModels.ThoughtViewModels;
using Thoughtwave.ExtensionMethods;
using Thoughtwave.Services;
using AutoMapper;


namespace Thoughtwave.Controllers
{
    [Authorize]
    public class ThoughtsController : Controller
    {
        private readonly IThoughtwaveRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
        private readonly ILogger<ThoughtsController> _logger;

        public ThoughtsController(IThoughtwaveRepository repository, 
            UserManager<User> userManager,
            IFileManager fileManager,
            IMapper mapper,
            ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _userManager = userManager;
            _fileManager = fileManager;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<ThoughtsController>();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/browse/all")]
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

            var model = _mapper.Map<IEnumerable<ThoughtViewModel>>(thoughts);
            return View(model);
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("/browse/{categoryId}")]
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

                var model = _mapper.Map<IEnumerable<ThoughtViewModel>>(thoughts);
                ViewBag.Content = $"Thoughts on {category.ToString()}";
                return View("Index", model);
            }

            return NotFound();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/tags/{tag}")]
        public async Task<IActionResult> TagIndex(string tag)
        {
            if (tag == null)
            {
                _logger.LogError("Invalid tag provided");
                return NotFound();
            }
                        
            var thoughts = await _repository.GetThoughtsByTagAsync(tag);

            if (thoughts == null)
            {
                _logger.LogError("Unable to retrieve thoughts for tag {tag}");
                return View("Error");
            }
            
            if (!thoughts.Any())
            {
                ViewBag.Message = $"No thoughts tagged with {tag} were found";
            }

            var model = _mapper.Map<IEnumerable<ThoughtViewModel>>(thoughts);
            ViewBag.Content = $"Thoughts tagged with {tag}";
            return View("Index", model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/search")]
        public async Task<IActionResult> Search(string q, string c = "All")
        {
            var categroy = GetCategoryFromString(c);
            q = q.ToLower();
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

            var model = _mapper.Map<IEnumerable<ThoughtViewModel>>(thoughts);
            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View("Error");
            }
            
            var userName = user.UserName;
            var thoughts = await _repository.GetThoughtsByUserNameAsync(userName);

            if (thoughts == null)
            {
                _logger.LogError($"Unable to retrieve thoughts from repository for {userName}");
                return View("Error");
            }

            if (!thoughts.Any())
            {
                ViewBag.Message = "You haven't created any thoughts yet!";
            }

            var model = _mapper.Map<IEnumerable<ThoughtViewModel>>(thoughts);
            ViewBag.Content = "Your Thoughts";
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateThoughtViewModel model)
        {
            if (ModelState.IsValid)
            {
                var thought = _mapper.Map<Thought>(model);

                // Save associated Thought author
                thought.Author = await GetCurrentUserAsync();

                // set thought image
                var imageFile = HttpContext.Request.Form.Files;
                var dest = "dist/uploads/images";
                var validFormats = new string[]{ ".jpg", ".png", ".jpeg" };
                var imagePath = await _fileManager.UploadFileAsync(imageFile, dest, validFormats);
                if (imagePath != null)
                {
                    thought.Image = imagePath;
                }

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
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{categoryId}/{id}/{slug}")]
        public async Task<IActionResult> Read(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("No thought ID was provided");
            }

            var thought = await _repository.GetThoughtAndCommentsByIdAsync(id.Value);

            if (thought == null)
            {
                _logger.LogInformation($"Unable to retrieve thought with id {id} from repository");
                return NotFound();
            }
            
            var model = _mapper.Map<ThoughtViewModel>(thought);
            return View(model);
        }

        [HttpGet]
        [Route("/thoughts/edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("No thought ID was provided");
            }

            var thought = await _repository.GetThoughtByIdAsync(id.Value);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, EditThoughtViewModel model)
        {
            if (!id.HasValue)
            {
                return BadRequest("No thought ID was provided");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var thought = _mapper.Map<Thought>(model);
            thought.Id = id.Value;

            // get new thought image
            var imageFile = HttpContext.Request.Form.Files;
            var dest = "dist/uploads/images";
            var validFormats = new string[]{ ".jpg", ".png", ".jpeg" };
            var imagePath = await _fileManager.UploadFileAsync(imageFile, dest, validFormats);
            if (imagePath != null)
            {
                // remove old image from file system
                _fileManager.DeleteFile(thought.Image);
                //set new image path
                thought.Image = imagePath;
            }

            // update thought
            _repository.UpdateThought(thought); 

            var updatedThought = await _repository.GetThoughtByIdAsync(id.Value);
            if (updatedThought == null)
            {
                _logger.LogError($"Unable to retrieve thought for id {id}");
                return View("Error");
            }

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
                var model = _mapper.Map<ThoughtViewModel>(thought);
                ViewBag.Title = $"Delete {model.Title}?";
                return View(model);
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

            if (thought == null)
            {
                _logger.LogError("Unable to retrieve thought with id {id}");
                return View("Error");
            }

            if (await UserIsThoughtAuthorAsync(thought))
            {
                // remove thought image from file system
                _fileManager.DeleteFile(thought.Image);

                // delete thought
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
