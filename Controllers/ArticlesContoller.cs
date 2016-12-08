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
        public async Task<IActionResult> Category(string categoryId)
        {
            Category articleCategory;
            if (Enum.TryParse(categoryId.Capitalize(), true, out articleCategory) 
                && Enum.IsDefined(typeof(Category), articleCategory)) 
            {
                var articles = await _repository.GetArticlesByCategoryAsync(articleCategory);

                if (articles == null || !articles.Any())
                {
                    ViewBag.Message = "No articles found for this category";
                }

                ViewBag.Content = "Thoughts on " + articleCategory.ToString();
                return View("Index", articles);
            }
            else
            {
                return RedirectToAction("Index", "Articles");
            }
        }

        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> Search(string q, string category = "All")
        {
            Category articleCategory;
            IEnumerable<Article> articles;

            if (Enum.TryParse(category, true, out articleCategory) 
                && Enum.IsDefined(typeof(Category), articleCategory)) 
            {
                articles = await _repository.GetArticlesByQueryAsync(q, articleCategory);
                ViewBag.Content = "Search Results in " + articleCategory.ToString();
            }
            else 
            {
                articles = await _repository.GetArticlesByQueryAsync(q);
                ViewBag.Content = "Search Results";
            }

            if (articles == null || !articles.Any())
            {
                ViewBag.Message = "No articles found for this Search";
            }

            return View("Index", articles);
        }
    }
}
