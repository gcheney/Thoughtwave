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
        public async Task<IActionResult> Index()
        {
            var articles = await _repository.GetAllArticlesAsync();
            ViewBag.Category = "All";

            if (articles == null) 
            {
                ViewBag.Message = "No articles currently available";
            }

            return View(articles);
        }

        [HttpGet]
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
        [Route("/articles/category/{categoryId}")]
        public async Task<IActionResult> Category(string categoryId)
        {
            Category categroy;
            if (Enum.TryParse(categoryId.Capitalize(), true, out categroy) 
                && Enum.IsDefined(typeof(Category), categroy)) 
            {
                var articles = await _repository.GetArticlesByCategoryAsync(categroy);

                if (articles == null)
                {
                    ViewBag.Message = "No articles found for this category";
                }

                ViewBag.Category = categroy.ToString();
                return View("Index", articles);
            }
            else
            {
                return RedirectToAction("Index", "Articles");
            }
        }
    }
}
