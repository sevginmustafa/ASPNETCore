using ForExercising.Data;
using ForExercising.ViewModels.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForExercising.ViewComponents
{
    public class GetUsersInfoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public GetUsersInfoViewComponent(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IViewComponentResult Invoke()
        {
            var model = this.db.Users
                .Select(x => new GetUsersInfoViewModel
                {
                    Id = x.Id,
                    Username = x.UserName
                })
                .ToList();

            return this.View(model);
        }
    }
}
