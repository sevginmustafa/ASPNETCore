using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;

namespace ScrappingTheMovieDb
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var document = await context.OpenAsync("https://www.themoviedb.org/movie/447332");

            //Get Title
            var title = document.QuerySelector(".title.ott_true > h2 > a").TextContent;

            //Get ReleaseYear
            var releaseYear = document.QuerySelector("span.tag.release_date").TextContent
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);

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
            //var rating = document.QuerySelector(".user_score_chart").Attributes.FirstOrDefault(x => x.Name == "data-percent").Value;

            //Get NumberOfVotes

            //Get TrailerUrl
            //var numberOfVotes = document.QuerySelectorAll("#video_popup");

            //Get Language
            var languageAndBudget = document.QuerySelector(".facts.left_column").TextContent
                .Trim()
                .Split("\n")
                .Where(x => x.Contains("Budget") || x.Contains("Language"))
                .ToArray();

            


            Console.WriteLine(language);


        }
    }
}