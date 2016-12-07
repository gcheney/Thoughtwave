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

        public ArticlesController(IThoughtwaveRepository repository, 
            ILogger<ArticlesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [Route("/articles")]
        public async Task<IActionResult> Index()
        {
            var articles = await _repository.GetAllArticlesAsync();
            ViewBag.Category = "All";

            if (articles == null || articles.Count == 0) 
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

                if (articles == null || articles.Count == 0)
                {
                    ViewBag.Message = "No articles found for this category";
                }

                ViewBag.Category = articleCategory.ToString();
                return View("Index", articles);
            }
            else
            {
                return RedirectToAction("Index", "Articles");
            }
        }

        /*
        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> Search(string q)
        {
            // TODO: Implement search feature
        }
        */
    }
}
