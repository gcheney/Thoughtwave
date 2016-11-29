using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sophophile.Data;
using Sophophile.Models;

namespace Sophophile.Controllers
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

        public IActionResult Index()
        {
            var questions = _repository.GetAllQuestions();
            return View(questions);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
