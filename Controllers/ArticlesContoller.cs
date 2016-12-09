using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thoughtwave.Data;
using Thoughtwave.Models;
using Thoughtwave.ExtensionMethods;

namespace Thoughtwave.Controllers
{
    public class ArticlesController : Controller
    {
        private IThoughtwaveRepository _repository;
        private ILogger<ArticlesController> _logger;

        public ArticlesController(IThoughtwaveRepository repository, ILogger<ArticlesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [Route("/all")]
        public async Task<IActionResult> Index()
        {
            var articles = await _repository.GetAllArticlesAsync();
            ViewBag.Content = "All Thoughts";

            if (articles == null || !articles.Any()) 
            {
                ViewBag.Message = "No articles currently available";
            }

            return View(articles);
        }

        [HttpGet]
        [Route("{categoryId}/{id}/{slug}")]
        public async Task<IActionResult> Read(int id)
        {
            var article = await _repository.GetArticleByIdAsync(id);

            if (article == null)
            {
                return RedirectToAction("Index");
            }
            
            return View(article);
        }
        
        [HttpGet]
        [Route("/{categoryId}")]
        public async Task<IActionResult> CategoryIndex(string categoryId)
        {
            Category category = GetCategoryFromString(categoryId);
            if (category != Category.None) 
            {
                var articles = await _repository.GetArticlesByCategoryAsync(category);

                if (articles == null || !articles.Any())
                {
                    ViewBag.Message = "No articles found for this category";
                }

                ViewBag.Content = "Thoughts on " + category.ToString();
                return View("Index", articles);
            }
            else
            {
                return RedirectToAction("Index", "Articles");
            }
        }

        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> Search(string q, string c = "All")
        {
            Category categroy = GetCategoryFromString(c);
            IEnumerable<Article> articles = null;

            if (categroy != Category.None) 
            {
                articles = await _repository.GetArticlesByQueryAsync(q, categroy);
                ViewBag.Content = "Search Results in " + categroy.ToString();
            }

            if (articles == null || !articles.Any())
            {
                ViewBag.Content = "Search Results";
                ViewBag.Message = "No articles found for this Search";
            }

            return View("Index", articles);
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
