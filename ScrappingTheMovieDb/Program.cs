using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using Newtonsoft.Json;

namespace ScrappingTheMovieDb
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            List<GetMovieDTO> movies = new List<GetMovieDTO>();

            Parallel.For(1, 100, (i, state) =>
            {
                var movie = GetMovieProperties(context, i);

                if (movie != null)
                {
                    movies.Add(movie);
                }
            });

            var file = JsonConvert.SerializeObject(movies, Formatting.Indented);

            File.WriteAllText("../../../movies.json", file);
        }

        private static GetGenresDTO(IBrowsingContext context, int id)
        {
            var document = context.OpenAsync($"#sidebar > div:nth-child(12) > span > div > div > div > div > div > div > div:nth-child(1) > div > a").GetAwaiter().GetResult();

            foreach (var item in document)
            {

            }
        }

        private static GetMovieDTO GetMovieProperties(IBrowsingContext context, int id)
        {
            try
            {
                var document = context.OpenAsync($"https://www.themoviedb.org/movie/{id}").GetAwaiter().GetResult();

                //Get Title
                var tryTitle = document.QuerySelector("div.title.ott_false > h2 > a");

                if (tryTitle == null)
                {
                    tryTitle = document.QuerySelector("div.title.ott_true > h2 > a");
                }

                var title = tryTitle.TextContent;

                //Get ReleaseYear
                var releaseYear = short.Parse(document.QuerySelector("span.tag.release_date").TextContent
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty));

                //Get Genres
                var genres = document.QuerySelectorAll("span.genres > a")
                    .Select(x => x.TextContent)
                    .ToList();

                //Get Runtime
                var runtime = document.QuerySelector("span.runtime").TextContent.Trim();

                //Get CoverImageUrl
                var coverImageUrl = "https://www.themoviedb.org/" +
                    document.QuerySelector("div.image_content.backdrop > img.poster.lazyload")
                    .Attributes.FirstOrDefault(x => x.Name == "data-src").Value;

                //Get Description
                var description = document.QuerySelector(".overview").TextContent.Trim();

                //Get Rating
                var rating = double.Parse(document.QuerySelector(".user_score_chart").Attributes.FirstOrDefault(x => x.Name == "data-percent").Value);

                //Get TrailerUrl
                var trailerUrl = "https://www.youtube.com/watch?v=" +
                    document.QuerySelector("li.video.none > a")
                    .Attributes.FirstOrDefault(x => x.Name == "data-id").Value;

                var languageAndBudget = document.QuerySelector(".facts.left_column").TextContent
                    .Trim()
                    .Split("\n")
                    .Where(x => x.Contains("Budget") || x.Contains("Language"))
                    .ToArray();

                //Get Language
                var language = languageAndBudget[0].Split().LastOrDefault();

                //Get Budget
                var budget = double.Parse(languageAndBudget[1].Split()
                    .LastOrDefault()
                    .Replace("$", string.Empty)
                    .Replace(",", string.Empty));

                var movie = new GetMovieDTO
                {
                    Title = title,
                    CoverImageUrl = coverImageUrl,
                    TrailerUrl = trailerUrl,
                    ReleaseYear = releaseYear,
                    Runtime = runtime,
                    Description = description,
                    Rating = rating,
                    Language = language,
                    Budget = budget
                };

                return movie;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}