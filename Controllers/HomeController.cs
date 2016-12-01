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
    public class HomeController : Controller
    {
        private IApplicationRepository _repository;
        private ILogger<HomeController> _logger;

        public HomeController(IApplicationRepository repository, 
            ILogger<HomeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomePageViewModel()
            {
                Articles = await _repository.GetRecentArticlesAsync()
            };

            return View(vm);
        }

        public IActionResult About()
        {
            return View();
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
