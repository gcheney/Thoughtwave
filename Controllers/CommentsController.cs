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
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly IThoughtwaveRepository _repository;
        private readonly ILogger<CommentsController> _logger;
        private readonly UserManager<User> _userManager;

        public CommentsController(IThoughtwaveRepository repository, 
            ILogger<CommentsController> logger,
            UserManager<User> userManager)
        {
            _repository = repository;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost]
        [RouteAttribute("/thoughts/{thoughtId}/comments")]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<IActionResult> Create(int thoughtId, string content,
            string returnUrl)
        {
            if (thoughtId == null)
            {
                _logger.LogError($"Invalid thoughtId provided for comment on {returnUrl}");
                return View("Error");
            }
            // no content for comment
            if (content == null)
            {
                TempData["error"] = "No comment content was provided";
                return Redirect(returnUrl);
            }

            // create new comment
            var newComment = new Comment()
            {
                Content = content,
                CreatedOn = DateTime.Now,
                User = await GetCurrentUserAsync()
            };
            
            // add comment
            _repository.AddComment(thoughtId, newComment);

            if (await _repository.CommitChangesAsync())
            {
                TempData["success"] = "Your comment has been added!";
                return Redirect(returnUrl);
            }
            else
            {
                _logger.LogError($"Unable to save comment for thought with id {thoughtId}");
                TempData["error"] = "An error occurred, please try again";
                return Redirect(returnUrl);
            }
        }


        /* ------- HELPER METHODS ---------- */

        private string GetThoughtUrl(Thought thought)
        {
            var category = thought.Category.ToString().ToLower();
            var id = thought.Id;
            var slug = thought.Slug.ToLower();
            return $"/{category}/{id}/{slug}";
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
