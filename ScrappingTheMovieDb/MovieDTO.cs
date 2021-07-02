using System.Collections.Generic;

namespace ScrappingTheMovieDb
{
    public class MovieDTO
    {
        public string Title { get; set; }

        public short ReleaseYear { get; set; }

        public string Runtime { get; set; }

        public string Description { get; set; }

        public string Language { get; set; }

        public double Budget { get; set; }

        public double Revenue { get; set; }

        public string CoverImageUrl { get; set; }

        public string TrailerUrl { get; set; }

        public PersonDTO Director { get; set; }

        public ICollection<PersonDTO> Actors { get; set; }

        public ICollection<GenreDTO> Genres { get; set; }
    }
}
