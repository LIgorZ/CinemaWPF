using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaWPF.Model
{
    public class FilmbyIdKP
    {
        public class Audience
        {
            [JsonProperty("count")]
            public int? Count { get; set; }

            [JsonProperty("country")]
            public string? Country { get; set; }
        }

        public class Backdrop
        {
            [JsonProperty("url")]
            public object? Url { get; set; }

            [JsonProperty("previewUrl")]
            public object? PreviewUrl { get; set; }
        }

        public class Budget
        {
            [JsonProperty("value")]
            public int? Value { get; set; }

            [JsonProperty("currency")]
            public string? Currency { get; set; }
        }

        public class Country
        {
            [JsonProperty("name")]
            public string? Name { get; set; }
        }

        public class ExternalId
        {
            [JsonProperty("kpHD")]
            public string? KpHD { get; set; }
        }

        public class Distributors
        {
            [JsonProperty("distributor")]
            public string? Distributor;

            [JsonProperty("distributorRelease")]
            public object? DistributorRelease;
        }

        public class Genre
        {
            [JsonProperty("name")]
            public string? Name { get; set; }
        }

        public class Item
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("logo")]
            public Logo? Logo { get; set; }

            [JsonProperty("url")]
            public string? Url { get; set; }
        }

        public class Logo
        {
            [JsonProperty("url")]
            public string? Url { get; set; }
        }

        public class NameFilm
        {
            [JsonProperty("name")]
            public string? Name { get; set; }
        }

        public class Person
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("photo")]
            public string? Photo { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("enName")]
            public string? EnName { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("profession")]
            public string? Profession { get; set; }

            [JsonProperty("enProfession")]
            public string? EnProfession { get; set; }
        }

        public class Poster
        {
            [JsonProperty("url")]
            public string? Url { get; set; }

            [JsonProperty("previewUrl")]
            public string? PreviewUrl { get; set; }
        }

        public class Premiere
        {
            [JsonProperty("russia")]
            public DateTime? Russia { get; set; }
        }

        public class ProductionCompany
        {
            [JsonProperty("name")]
            public string? Name;

            [JsonProperty("url")]
            public string? Url;

            [JsonProperty("previewUrl")]
            public string? PreviewUrl;
        }

        public class Rating
        {
            [JsonProperty("kp")]
            public double? Kp { get; set; }

            [JsonProperty("imdb")]
            public double? Imdb { get; set; }

            [JsonProperty("filmCritics")]
            public double? FilmCritics { get; set; }

            [JsonProperty("russianFilmCritics")]
            public double? RussianFilmCritics { get; set; }

            [JsonProperty("await")]
            public object? Await { get; set; }
        }

        public class FactFilm
        {
            [JsonProperty("value")]
            public string Value;

            [JsonProperty("type")]
            public string Type;

            [JsonProperty("spoiler")]
            public bool? Spoiler;
        }

        public class Networks
        {
            [JsonProperty("items")]
            public List<Item>? Items;
        }


        public class FindFilm
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("names")]
            public List<NameFilm>? Names { get; set; }

            [JsonProperty("genres")]
            public List<Genre>? Genres { get; set; }

            [JsonProperty("countries")]
            public List<Country>? Countries { get; set; }

            [JsonProperty("persons")]
            public List<Person>? Persons { get; set; }

            [JsonProperty("spokenLanguages")]
            public List<object>? SpokenLanguages { get; set; }

            [JsonProperty("seasonsInfo")]
            public List<object>? SeasonsInfo { get; set; }

            [JsonProperty("collections")]
            public List<object>? Collections { get; set; }

            [JsonProperty("similarMovies")]
            public List<object>? SimilarMovies { get; set; }

            [JsonProperty("sequelsAndPrequels")]
            public List<object>? SequelsAndPrequels { get; set; }

            [JsonProperty("releaseYears")]
            public List<object>? ReleaseYears { get; set; }

            [JsonProperty("createdAt")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("updatedAt")]
            public DateTime? UpdatedAt { get; set; }

            [JsonProperty("alternativeName")]
            public object? AlternativeName { get; set; }

            [JsonProperty("backdrop")]
            public Backdrop? Backdrop { get; set; }

            [JsonProperty("budget")]
            public Budget? Budget { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("enName")]
            public object? EnName { get; set; }

            [JsonProperty("externalId")]
            public ExternalId? ExternalId { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("poster")]
            public Poster? Poster { get; set; }

            [JsonProperty("rating")]
            public Rating? Rating { get; set; }

            [JsonProperty("ratingMpaa")]
            public object? RatingMpaa { get; set; }

            [JsonProperty("shortDescription")]
            public object? ShortDescription { get; set; }

            [JsonProperty("slogan")]
            public object? Slogan { get; set; }

            [JsonProperty("top10")]
            public object? Top10 { get; set; }

            [JsonProperty("top250")]
            public object? Top250 { get; set; }

            [JsonProperty("type")]
            public string? Type { get; set; }

            [JsonProperty("typeNumber")]
            public int? TypeNumber { get; set; }

            [JsonProperty("votes")]
            public Votes? Votes { get; set; }

            [JsonProperty("watchability")]
            public Watchability? Watchability { get; set; }

            [JsonProperty("ageRating")]
            public int? AgeRating { get; set; }

            [JsonProperty("year")]
            public int? Year { get; set; }

            [JsonProperty("status")]
            public object? Status { get; set; }

            [JsonProperty("premiere")]
            public Premiere? Premiere { get; set; }

            [JsonProperty("ticketsOnSale")]
            public bool? TicketsOnSale { get; set; }

            [JsonProperty("movieLength")]
            public int? MovieLength { get; set; }

            [JsonProperty("isSeries")]
            public bool? IsSeries { get; set; }

            [JsonProperty("seriesLength")]
            public object? SeriesLength { get; set; }

            [JsonProperty("totalSeriesLength")]
            public object? TotalSeriesLength { get; set; }

            [JsonProperty("deletedAt")]
            public object? DeletedAt { get; set; }

            [JsonProperty("networks")]
            public Networks? Networks;

            [JsonProperty("lists")]
            public List<string>? Lists { get; set; }

            [JsonProperty("audience")]
            public List<Audience>? Audience { get; set; }

            [JsonProperty("facts")]
            public List<FactFilm>? FactsFilm;

            [JsonProperty("productionCompanies")]
            public List<ProductionCompany>? ProductionCompanies;

            [JsonProperty("distributors")]
            public Distributors? Distributors;

        }

        public class Votes
        {
            [JsonProperty("kp")]
            public int Kp { get; set; }

            [JsonProperty("imdb")]
            public int? Imdb { get; set; }

            [JsonProperty("filmCritics")]
            public int? FilmCritics { get; set; }

            [JsonProperty("russianFilmCritics")]
            public int? RussianFilmCritics { get; set; }

            [JsonProperty("await")]
            public int? Await { get; set; }
        }

        public class Watchability
        {
            [JsonProperty("items")]
            public List<Item>? Items { get; set; }
        }

    }
}
