using CinemaWPF.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TagLib;
using static CinemaWPF.Model.CFilmsShortView;
using static CinemaWPF.Model.CinemaDB;

namespace CinemaWPF
{
    /// <summary>
    /// Логика взаимодействия для WSetGenre.xaml
    /// </summary>
    public partial class WSetGenre : Window
    {
        private CGenre myGenre;
        private List<CGenre> myGenreList;
        private CinemaWPF.Model.CFilmsShortView.SVFilms myFilm;
        private int MyId {  get; set; } 
        public WSetGenre(int _MyId)
        {
            InitializeComponent();
            MyId = _MyId;
            showAllGenre();
        }

        private void showAllGenre()
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                var allGenre = db.Genres.Select(q => q.Name).Distinct();
                var currentFilmGenre = db.Cinema.Where(s => s.Id == MyId).Include(q => q.Genres).Select(d=>d.Genres);
                var currentFilm = db.Cinema.Where(s => s.Id == MyId).FirstOrDefault();
                myFilm = new CFilmsShortView.SVFilms();
                myGenreList = new List<CGenre>();
                myFilm.Name = currentFilm.Name;
                myFilm.AlternativeName = currentFilm.AlternativeName;
                myFilm.PathToFilm = currentFilm.PathToCinema;

                foreach (var iGenre in allGenre)
                {
                    myGenre = new CGenre();
                    myGenre.Name = iGenre.ToString();
                    foreach (var d in currentFilmGenre)
                    {
                        myGenre.isChecked = false;
                        foreach (var p in d)
                        {
                            if (myGenre.Name == p.Name)
                            {
                                myGenre.isChecked = true;
                            } 
                        }
                    }
                            
                    //CheckBox checkBox = new CheckBox() { Content = iGenre.ToString(), IsChecked = false };
                    //ShowGenre.Children.Add(checkBox);
                    myGenreList.Add(myGenre);
                }
                lBoxGenre.ItemsSource = myGenreList;
                SpInfoFilm.DataContext = myFilm;
            }
        }
        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void bAddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tBoxInputGenre.Text))
            {
                var isExistGenre = false;
                myGenre = new CGenre();
                foreach (var iGenre in myGenreList)
                {
                    if (iGenre.Name == tBoxInputGenre.Text)
                    {
                        isExistGenre = true;
                    }
                }
                if (!isExistGenre)
                {
                    myGenre.Name = tBoxInputGenre.Text;
                    myGenre.isChecked = false;
                    myGenreList.Add(myGenre);
                    CollectionViewSource.GetDefaultView(lBoxGenre.ItemsSource).Refresh();
                    sInfo.Text = "Жанр "+ tBoxInputGenre.Text + " добавлен.";
                    sInfo.Foreground = Brushes.Black;
                }
                else
                {
                    sInfo.Text = "Такой жанр уже присутсвует";
                    sInfo.Foreground = Brushes.Red;
                }
            }
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                var currentFilm = db.Cinema.Where(s => s.Id == MyId).Include(q =>q.Genres).FirstOrDefault();
                List<CinemaWPF.Model.CinemaDB.Genre> addGenres = new List<Genre>();
                Genre genreToAdd = new Genre();

                foreach (var iGenre in myGenreList)
                {
                    if (iGenre.isChecked.Value)
                    {
                        genreToAdd = new Genre();
                        genreToAdd.Name = iGenre.Name;
                        addGenres.Add(genreToAdd);
                    }
                }
                currentFilm.Genres = addGenres; 
                db.SaveChanges();
            }
            Close();
        }
    }
}
