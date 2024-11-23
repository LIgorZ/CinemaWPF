using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CinemaWPF
{
    /// <summary>
    /// Логика взаимодействия для WEditFilm.xaml
    /// </summary>
    public partial class WEditFilm : Window
    {
        private int fID;
        public WEditFilm(int _fId)
        {
            InitializeComponent();
            lNumofFilm.Text = "ID Фильма: " + _fId.ToString();
            fID = _fId;
            bGetInfoFilmbyIdKP.Visibility = Visibility.Collapsed;
            ShowFilm();
        }

        private void ShowFilm()
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + Model.CinemaDB.filenameDB))
            {
                //List<Model.CFilmsShortView.SVFilms> listToViewFilms = new List<Model.CFilmsShortView.SVFilms>();
                Model.CFilmsShortView.SVFilms viewFilm = new Model.CFilmsShortView.SVFilms();
                var myFilm = db.Cinema.Where(q => q.Id == fID).Include(o => o.Poster).FirstOrDefault();

                    viewFilm = new Model.CFilmsShortView.SVFilms();
                    viewFilm.PathToFilm = myFilm.PathToCinema;
                    viewFilm.Id = myFilm.Id;
                    viewFilm.Name = myFilm.Name;
                    viewFilm.IdKP = myFilm.IdKP;
                    viewFilm.Description = myFilm.Description;
                    viewFilm.Year = myFilm.Year.ToString();

                if (myFilm.Poster != null)
                    {
                        viewFilm.Poster = ConvertByteArrayToBitmapImage(myFilm.Poster.Pic);
                    }
                
                // Вывод жанров
                var genreInFilm = db.Cinema.Where(q => q.Id == fID).Select(o => o.Genres);
                //lGenre.Text = "";
                string sGenres = "";
                foreach (var genre in genreInFilm)
                {
                    foreach (var g in genre)
                    {
                        sGenres += g.Name + ", ";
                    }
                }
                if (sGenres.Length > 2)
                {
                    sGenres = sGenres.Remove(sGenres.Length - 2, 2);
                }
                viewFilm.Genres = sGenres;

                //Вывод актеров
                string sActors = "";
                int i = 1;
                var personsInFilm = db.Cinema.Where(q => q.Id == fID).Select(o => o.Persons);
                foreach (var person in personsInFilm)
                {
                    foreach (var p in person)
                    {
                        if (p.EnProfession == "actor")
                        {
                            sActors += p.Name + ", ";
                        }
                        
                    }
                }
                if (sActors.Length > 2)
                {
                    sActors = sActors.Remove(sActors.Length - 2, 2);
                }
                viewFilm.Actors = sActors;

                this.DataContext = viewFilm;
            }

        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private static BitmapImage ConvertByteArrayToBitmapImage(Byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        private void bGetIdKPbyNameFilm_Click(object sender, RoutedEventArgs e)
        {
            if ( !string.IsNullOrWhiteSpace(tBNameFilm.Text))
            {
                WFbyNameFilm myFbyNameFilm = new(tBNameFilm.Text, fID);
                myFbyNameFilm.ShowDialog();
                ShowFilm(); 
                bGetInfoFilmbyIdKP.Visibility = Visibility.Visible;
            }
        }

        private void bGetInfoFilmbyIdKP_Click(object sender, RoutedEventArgs e)
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + Model.CinemaDB.filenameDB))
            {
                var myFilm = db.Cinema.Where(q => q.Id == fID).Include(o => o.Poster).FirstOrDefault();
                if (myFilm.IdKP != null | myFilm.IdKP != 0)
                {
                    WFbyIdKPFilm myFbyIdKPFilm = new(fID, myFilm.IdKP.Value);
                    myFbyIdKPFilm.ShowDialog();
                    ShowFilm();
                }
            }
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
