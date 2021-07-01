using System;
using System.Collections.Generic;
using System.Text;

namespace ScrappingTheMovieDb
{
    public class PersonDTO
    {
        public string Name { get; set; }

        public string Biography { get; set; }

        public DateTime Birthday { get; set; }

        public string Gender { get; set; }

        public string BirthCity { get; set; }

        public string CoverImageUrl { get; set; }
    }
}
