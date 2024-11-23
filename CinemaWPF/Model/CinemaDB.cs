using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaWPF.Model
{
    public class CinemaDB
    {
        static public string? filenameDB;
        static public string? APIKey2 = "V8BDC19-8X8MG86-MS0E9ZN-RHFRA3F";
        static public string? APIKey1 = "R9PZQDP-W4C44H9-PM56Z42-NQEZ8KT";
        public class Audience
        {
            public int Id { get; set; }
            public int? Count { get; set; }
            public string? Country { get; set; }
        }

        public class Backdrop
        {
            public int Id { get; set; }
            public string? Url { get; set; }
            public string? PreviewUrl { get; set; }
        }

        public class Budget
        {
            public int Id { get; set; }
            public int? Value { get; set; }
            public string? Currency { get; set; }
        }

        public class Country
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        public class ExternalId
        {
            public int Id { get; set; }
            public string? KpHD { get; set; }
        }

        public class Genre
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        public class Item
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public Logo Logo { get; set; }
            public string? Url { get; set; }
        }

        public class Logo
        {
            public int Id { get; set; }
            public string? Url { get; set; }
        }

        public class NameFilm
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        public class Person
        {
            public int? IdKP { get; set; }
            public string? Description { get; set; }
            public string? EnProfession { get; set; }
            public string? Profession { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string? EnName { get; set; }
            public string? Photo { get; set; }
            public byte[]? PhotoPic { get; set; }
            public string? Sex { get; set; }
            public int? Growth { get; set; }
            public DateTime? Birthday { get; set; }
            public string? sBirthday { get; set; }    
            public DateTime? Death { get; set; }
            public int? Age { get; set; }
            public List<BirthPlace>? BirthPlace { get; set; }
            public List<string>? DeathPlace { get; set; }
            public List<Spouse>? Spouses { get; set; }
            public int? CountAwards { get; set; }
            public List<Fact>? Facts { get; set; }
            public List<Movie>? Movies { get; set; } 
            public List<Cinema> Cinemas { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class Poster
        {
            public int Id { get; set; }
            public string? Url { get; set; }
            public string? PreviewUrl { get; set; }

            public byte[]? Pic { get; set; }
            public byte[]? PreviewPic { get; set; }
        }

        public class Premiere
        {
            public int Id { get; set; }
            public DateTime? Russia { get; set; }
        }

        public class Rating
        {
            public int Id { get; set; }
            public double? Kp { get; set; }
            public double? Imdb { get; set; }
            public double? FilmCritics { get; set; }
            public double? RussianFilmCritics { get; set; }
            public string? Await { get; set; }
            public double? Private { get; set; }
        }

        public class Cinema
        {
            public int Id { get; set; }
            public int? IdKP { get; set; }
            public List<string>? Facts { get; set; }
            public List<NameFilm>? Names { get; set; }
            public List<Genre>? Genres { get; set; }
            public List<Country>? Countries { get; set; }
            public List<Tag>? Tags { get; set; }
            public List<Person>? Persons { get; set; }
            public List<string>? SpokenLanguages { get; set; }
            public List<string>? SeasonsInfo { get; set; }
            public List<string>? Collections { get; set; }
            public List<string>? ProductionCompanies { get; set; }
            public List<string>? SimilarMovies { get; set; }
            public List<string>? SequelsAndPrequels { get; set; }
            public List<string>? ReleaseYears { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? AlternativeName { get; set; }
            public Backdrop? Backdrop { get; set; }
            public Budget? Budget { get; set; }
            public string? Description { get; set; }
            public string? EnName { get; set; }
            public ExternalId? ExternalId { get; set; }
            public string Name { get; set; }
            public Poster? Poster { get; set; }
            public Rating? Rating { get; set; }
            public string? RatingMpaa { get; set; }
            public string? ShortDescription { get; set; }
            public string? Slogan { get; set; }
            public string? Top10 { get; set; }
            public string? Top250 { get; set; }
            public string? Type { get; set; }
            public int? TypeNumber { get; set; }
            public Votes? Votes { get; set; }
            public Watchability? Watchability { get; set; }
            public int? AgeRating { get; set; }
            public int? Year { get; set; }
            public string? Status { get; set; }
            public Premiere? Premiere { get; set; }
            public bool? TicketsOnSale { get; set; }
            public int? MovieLength { get; set; }
            public bool? IsSeries { get; set; }
            public string? SeriesLength { get; set; }
            public string? TotalSeriesLength { get; set; }
            public string? DeletedAt { get; set; }
            public string? Networks { get; set; }
            public List<string>? Lists { get; set; }
            public List<Audience>? Audience { get; set; }
            public string PathToCinema { get; set; }
            public TimeSpan? DurationCinema { get; set; }
            public string? HighCinema { get; set; }
            public string? WidthCinema { get; set; }
            public long? CinemaSize { get; set; }

            public List<Fact>? FactsCinema { get; set; }
        }

        public class Votes
        {
            public int Id { get; set; }
            public int? Kp { get; set; }
            public int? Imdb { get; set; }
            public int? FilmCritics { get; set; }
            public int? RussianFilmCritics { get; set; }
            public int? Await { get; set; }
        }

        public class Watchability
        {
            public int Id { get; set; }
            public List<Item>? Items { get; set; }
        }

        public class Spouse
        {
            public int Id { get; set; }
            public int IdKP { get; set; }
            public string? Name { get; set; }
            public bool? Divorced { get; set; }
            public string? DivorcedReason { get; set; }
            public int? Children { get; set; }
            public string? Relation { get; set; }
        }

        public class BirthPlace
        {
            public int Id { get; set; }
            public string? Value { get; set; }
        }

        public class Tag
        {
            public int Id { get; set; }
            public string? Value { get; set; }
        }

        public class Fact
        {
            public int Id { get; set; }
            public string? Value { get; set; }
        }

        public class Movie
        {
            public int Id { get; set; }
            public int IdKP { get; set; }
            public string? Name { get; set; }
            public string? AlternativeName { get; set; }
            public double? Rating { get; set; }
            public bool? General { get; set; }
            public string? Description { get; set; }
            public string? EnProfession { get; set; }
            public int? Year { get; set; }
            public string? EnName { get; set; }
        }

        public class AppContext : DbContext
        {
            public DbSet<Cinema> Cinema => Set<Cinema>();
            public DbSet<Poster> Poster => Set<Poster>();
            public DbSet<Person> Person => Set<Person>();
            public DbSet<Rating> Rating => Set<Rating>();
            public DbSet<Genre> Genres => Set<Genre>();
            public DbSet<Budget> Budget => Set<Budget>();
            public DbSet<Votes> Votes => Set<Votes>();
            public DbSet<Country> Country => Set<Country>();
            public DbSet<Logo> Logo => Set<Logo>();
            public DbSet<NameFilm> NameFilm => Set<NameFilm>();
            public DbSet<Item> Item => Set<Item>();
            public DbSet<Spouse> Spouse => Set<Spouse>();
            public DbSet<Movie> Movie => Set<Movie>();
            public DbSet<Fact> Fact => Set<Fact>();
            public DbSet<Tag> Tag => Set<Tag>();
            public DbSet<BirthPlace> BirthPlace => Set<BirthPlace>();

            public string connectionString;

            public AppContext(string connectionString)
            {
                this.connectionString = connectionString;   // получаем извне строку подключения
                Database.EnsureCreated();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite(connectionString);
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Warning);
            }
        }

    }
}
