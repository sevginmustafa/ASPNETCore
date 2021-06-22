using ForExercising.CustomFilters;
using ForExercising.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ForExercising.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            this.ViewBag.ProjectName = Assembly.GetEntryAssembly().GetName().Name;

            this.ViewData["Message"] = "Current local time";
            this.ViewData["Time"] = DateTime.UtcNow.ToLocalTime().ToShortTimeString();
            return View();
        }

        public IActionResult Config()
        {
            return Content(this.configuration["ConnectionStrings:DefaultConnection"]);
        }

        public IActionResult Privacy([Required][MaxLength(10)][RegularExpression(@"[A-Za-z]+")] string name, [Range(0, 150)] int age, string[] hobbies)
        {
            if (ModelState.IsValid)
            {
                return this.Json($"My name is {name} and I am {age} years old! My hobbies are: { string.Join(", ", hobbies)}");
            }

            StringBuilder errors = new StringBuilder();

            foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
            {
                errors.AppendLine(error.ErrorMessage);
            }

            return this.Json(errors.ToString());
        }

        public IActionResult StatusCodeException(int statusCode)
        {
            return View();
        }

        public IActionResult Exception()
        {
            throw new Exception();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
