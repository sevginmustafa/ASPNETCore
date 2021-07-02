using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp;
using Newtonsoft.Json;

namespace ScrappingTheMovieDb
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            List<MovieDTO> movies = new List<MovieDTO>();

            //Parallel.For(1, 101, (i, state) =>
            //{
            //    var movie = GetMovieProperties(context, i);

            //    if (movie != null)
            //    {
            //        movies.Add(movie);
            //    }

            //    Console.WriteLine(i);
            //});

            for (int i = 539; i < 539 + 1; i++)
            {
                var movie = GetMovieProperties(context, i);

                if (movie != null)
                {
                    movies.Add(movie);
                }

                Console.WriteLine(i);
            }

            var file = JsonConvert.SerializeObject(movies, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            File.WriteAllText("../../../movies.json", file);

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

        private static MovieDTO GetMovieProperties(IBrowsingContext context, int id)
        {
            try
            {
                var document = context.OpenAsync($"https://www.themoviedb.org/movie/{id}")
                    .GetAwaiter()
                    .GetResult();

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
                    .ToArray();


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

                var languageBudgetRevenue = document.QuerySelector(".facts.left_column").TextContent
                    .Trim()
                    .Split("\n")
                    .Where(x => x.Contains("Budget") || x.Contains("Language") || x.Contains("Revenue"))
                    .ToArray();


                //Get Language
                var language = languageBudgetRevenue[0].Split().LastOrDefault();


                //Get Budget
                var budget = double.Parse(languageBudgetRevenue[1].Split()
                    .LastOrDefault()
                    .Replace("$", string.Empty)
                    .Replace(",", string.Empty));

                var revenue = double.Parse(languageBudgetRevenue[2].Split()
                    .LastOrDefault()
                    .Replace("$", string.Empty)
                    .Replace(",", string.Empty));


                //Get Actors
                List<PersonDTO> actors = new List<PersonDTO>();

                var links = document.QuerySelectorAll(".people.scroller > .card > p > a");
                var characters = document.QuerySelectorAll(".people.scroller > .card > .character");

                for (int i = 0; i < links.Length; i++)
                {
                    var actorLink = links[i].Attributes.FirstOrDefault().Value;

                    try
                    {
                        var actor = GetPersonProperties(context, actorLink);
                        var characterName = characters[i].TextContent;
                        actor.CharacterName = characterName;
                        actors.Add(actor);
                    }
                    catch (Exception)
                    {
                    }
                }


                //Get Dirctor
                var directorLink = document.QuerySelectorAll(".people.no_image > li > p > a")
                    .FirstOrDefault()
                    .Attributes.FirstOrDefault().Value;

                var director = GetPersonProperties(context, directorLink);

                var movie = new MovieDTO
                {
                    Title = title,
                    ReleaseYear = releaseYear,
                    Runtime = runtime,
                    Description = description,
                    Language = language,
                    Budget = budget,
                    Revenue = revenue,
                    CoverImageUrl = coverImageUrl,
                    TrailerUrl = trailerUrl,
                    Director = director,
                    Actors = actors,
                    Genres = genres.Select(x => new GenreDTO { Name = x }).ToArray(),
                };

                return movie;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static PersonDTO GetPersonProperties(IBrowsingContext context, string personLink)
        {
            //Get Directors
            var link = "https://www.themoviedb.org" + personLink;

            var personInfo = context.OpenAsync(link)
                .GetAwaiter()
                .GetResult();


            //Get Name
            var name = personInfo.QuerySelector(".title > h2 > a").TextContent;


            //Get Biography
            var biography = personInfo.QuerySelectorAll(".text.initial > p")
                .FirstOrDefault().TextContent;


            //Get Birthday and Birthplace
            var birthdayAndBirthplace = personInfo.QuerySelectorAll(".full")
                .Select(x => x.TextContent.Replace("  ", string.Empty))
                .ToArray();

            var birthday = Regex.Match(birthdayAndBirthplace[0], @"[0-9]{4}-[0-9]{2}-[0-9]{2}").ToString();

            string deathday = null;
            string[] placeAndCountry = null;

            if (birthdayAndBirthplace[1].Contains("Day of Death"))
            {
                deathday = Regex.Match(birthdayAndBirthplace[1], @"[0-9]{4}-[0-9]{2}-[0-9]{2}").ToString();
                placeAndCountry = birthdayAndBirthplace[2].Replace("Place of Birth ", string.Empty)
                    .Split(", ")
                    .ToArray();
            }
            else
            {
                placeAndCountry = birthdayAndBirthplace[1].Replace("Place of Birth ", string.Empty)
                    .Split(", ")
                    .ToArray();
            }

            var birthplace = placeAndCountry.FirstOrDefault();
            var country = placeAndCountry.LastOrDefault();


            //Get Gender
            var gender = personInfo.QuerySelector("#media_v4 > div > div > div.grey_column > div > section > section > p:nth-child(3)")
                .TextContent.Replace("Gender ", string.Empty);


            //Get CoverImageUrl
            var coverImageUrl = "https://www.themoviedb.org" +
                personInfo.QuerySelector("#original_header > div > div.image_content > img")
                .Attributes.FirstOrDefault(x => x.Name == "data-src").Value;

            var person = new PersonDTO
            {
                Name = name,
                Biography = biography,
                Gender = gender,
                Birthday = birthday,
                Deathday = deathday,
                Birthplace = birthplace,
                Country = country,
                CoverImageUrl = coverImageUrl
            };

            return person;
        }
    }
}