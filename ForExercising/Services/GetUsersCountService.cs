using ForExercising.Data;
using System.Linq;

namespace ForExercising.Services
{
    public class GetUsersCountService : IGetUsersCountService
    {
        private readonly ApplicationDbContext db;

        public GetUsersCountService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public int GetCount() =>
            this.db.Users.Count();
    }
}
