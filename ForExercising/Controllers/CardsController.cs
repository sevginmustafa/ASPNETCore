using ForExercising.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ForExercising.Controllers
{
    public class CardsController : Controller
    {
        public IActionResult Add()
        {
            var model = new AddYuGiOhCardViewModel()
            {
                Level = 1,
                Attack = 0,
                Defense = 0,
                CreatedOn = DateTime.UtcNow
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(AddYuGiOhCardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            return RedirectToAction(nameof(AddCardSuccess));
        }

        public IActionResult AddCardSuccess()
        {
            return View();
        }
    }
}
