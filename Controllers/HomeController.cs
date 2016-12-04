using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thoughtwave.Data;
using Thoughtwave.Models;

namespace Thoughtwave.Controllers
{
    public class HomeController : Controller
    {
        private IThoughtwaveRepository _repository;
        private ILogger<HomeController> _logger;

        public HomeController(IThoughtwaveRepository repository, 
            ILogger<HomeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var articles = await _repository.GetRecentArticlesAsync();
            
            return View(articles);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
