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
            var thoughts = await _repository.GetRecentThoughtsAsync();

            if (thoughts == null)
            {
                _logger.LogError("Unable to retrieve recent thoughts from repository");
                thoughts = new List<Thought>();
            }
            
            if (!thoughts.Any())
            {
                ViewBag.Message = "No recent thoughts found";
            }
            
            return View(thoughts);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
