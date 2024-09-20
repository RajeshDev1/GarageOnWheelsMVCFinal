using GarageOnWheelsMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GarageOnWheelsMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Index1()
        {
            return View();
        }


        public IActionResult Index2()
        {
            return View();
        }


    }
}
