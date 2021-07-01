using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
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

            Parallel.For(447332, 447333, (i, state) =>
            {
                var movie = GetMovieProperties(context, i);

                if (movie != null)
                {
                    movies.Add(movie);
                }
            });

            //var file = JsonConvert.SerializeObject(movies, Formatting.Indented);

            //File.WriteAllText("../../../movies.json", file);

            //var genres = JsonConvert.SerializeObject(GetAllGenres(context), Formatting.Indented);

            //File.WriteAllText("../../../genres.json", genres);
        }

        private static IEnumerable<GenreDTO> GetAllGenres(IBrowsingContext context)
        {
            List<GenreDTO> genres = new List<GenreDTO>();

            var document = context.OpenAsync("https://www.imdb.com/feature/genre/").GetAwaiter().GetResult();

            var elements = document.QuerySelector($"#sidebar > div:nth-child(12) > span > div > div > div > div > div > div")
                .TextContent.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in elements)
            {
                genres.Add(new GenreDTO { Name = item });
            }

            return genres;
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

                //Get Actors
                var characters = document.QuerySelectorAll(".people.scroller > .card > .character");
                var elements = document.QuerySelectorAll(".people.scroller > .card > p > a");

                foreach (var item in elements)
                {
                    foreach (var att in item.Attributes)
                    {
                        Console.WriteLine(att.Value);
                    }
                }

                //IDocument personInfo = GetPersonProperties(context, document);

                var movie = new GetMovieDTO
                {
                    Title = title,
                    CoverImageUrl = coverImageUrl,
                    TrailerUrl = trailerUrl,
                    ReleaseYear = releaseYear,
                    Runtime = runtime,
                    Description = description,
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

        private static IDocument GetPersonProperties(IBrowsingContext context, IDocument document)
        {
            //Get Directors
            var directorLink = "https://www.themoviedb.org" +
                document.QuerySelectorAll(".people.no_image > li > p > a")
                .FirstOrDefault()
                .Attributes.FirstOrDefault().Value;

            var directorInfo = context.OpenAsync(directorLink).GetAwaiter().GetResult();

            //Get Name
            var name = directorInfo.QuerySelector(".title > h2 > a").TextContent;

            //Get Bio
            var bio = directorInfo.QuerySelectorAll(".text.initial > p").FirstOrDefault().TextContent;

            //Get Birthday
            var birthdayAndBirthPlace = directorInfo.QuerySelectorAll(".full").Select(x => x.TextContent.Replace("  ", string.Empty)).ToArray();

            var birthday = Regex.Match(birthdayAndBirthPlace[0], @"[0-9]{4}-[0-9]{2}-[0-9]{2}").ToString();

            var cityAndCountry = birthdayAndBirthPlace[1].Replace("Place of Birth ", string.Empty).Split(", ").ToArray();

            var city = cityAndCountry.FirstOrDefault();
            var country = cityAndCountry.LastOrDefault();

            //Get Gender
            var gender = directorInfo.QuerySelector("#media_v4 > div > div > div.grey_column > div > section > section > p:nth-child(3)")
                .TextContent.Replace("Gender ", string.Empty);

            //Get CoverImageUrl
            var coverImageUrl = "https://www.themoviedb.org" +
                directorInfo.QuerySelector("#original_header > div > div.image_content > img")
                .Attributes.FirstOrDefault(x => x.Name == "data-src").Value;

            return directorInfo;
        }
    }
}