using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thoughtwave.Data;
using Thoughtwave.Models;
using Thoughtwave.ViewModels.HomeViewModels;

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
            var vm = new HomePageViewModel()
            {
                Articles = await _repository.GetAllArticlesAsync()
            };

            return View(vm);
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

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
