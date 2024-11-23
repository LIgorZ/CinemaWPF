using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CinemaWPF.Model
{
    public class CFilmsShortView
    {
        public class SVFilms
        {
            public int Id { get; set; }
            public int? IdKP { get; set; }
            public string? Name { get; set; }
            public string? PathToFilm { get; set; }
            public BitmapImage? Poster { get; set; }
            public string? Genres { get; set; }
            public string? Actors { get; set; }
            public string? Description { get; set; }
            public string? Year { get; set; }
            public string? AlternativeName { get; set; }
            public string? Director {  get; set; }
            public string? Duration {  get; set; }
            public string? FactsFilm { get; set; }
            public string? CountryFilm { get; set; }
            public string? RIMDb { get;set; }
            public double? dRIMDb { get; set; }
            public string? RKp { get; set; }
            public double? dRKp { get; set; } 
            public string? ResFilm { get; set; }
            public string? SizeFilm { get; set; }
        }

    }
}
