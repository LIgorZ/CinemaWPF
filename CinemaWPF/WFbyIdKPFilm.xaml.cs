using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static CinemaWPF.Model.CinemaDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace CinemaWPF
{
    /// <summary>
    /// Логика взаимодействия для WFbyIdKPFilm.xaml
    /// </summary>
    public partial class WFbyIdKPFilm : Window
    {
        private int MyId {  get; set; } 
        private int MyIdKP { get; set; }
        private Model.FilmbyIdKP.FindFilm myFilm;
        private Model.CFilmsShortView.SVFilms myShowFilm;
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

        public WFbyIdKPFilm(int _myId, int _myIdKP)
        {
            InitializeComponent();
            MyId = _myId;
            MyIdKP = _myIdKP;
            FindFilm();
        }

        private void FindFilm()
        {
            var options = new RestClientOptions("https://api.kinopoisk.dev/v1.4/movie/" + MyIdKP);
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("X-API-KEY", APIKey1);
            var response = client.GetAsync(request);
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + Model.CinemaDB.filenameDB))
            {
                var currentFilm = db.Cinema.Where(o => o.Id == MyId).FirstOrDefault();

                try
                {
                    if (((int)response.Result.StatusCode) == 200)
                    {
                        myFilm = JsonConvert.DeserializeObject<Model.FilmbyIdKP.FindFilm>(response.Result.Content);
                        myShowFilm = new Model.CFilmsShortView.SVFilms();

                        myShowFilm.Description = myFilm.Description == null ? "Нет описания" : myFilm.Description;
                        myShowFilm.Name = myFilm.Name == null ? "" : myFilm.Name;
                        myShowFilm.AlternativeName = myFilm.AlternativeName == null ? "" : (string)myFilm.AlternativeName;
                        myShowFilm.Year = myFilm.Year == null ? "" : myFilm.Year.ToString();
                        myShowFilm.IdKP = myFilm.Id == null ? 0 : myFilm.Id;
                        myShowFilm.Id = MyId;
                        myShowFilm.PathToFilm = currentFilm.PathToCinema;

                        if (myFilm.Poster.PreviewUrl != null)
                        {
                            var imageSource = new BitmapImage();
                            imageSource.BeginInit();
                            imageSource.UriSource = new Uri(myFilm.Poster.PreviewUrl);
                            imageSource.EndInit();
                            myShowFilm.Poster = imageSource;
                        }
                        this.DataContext = myShowFilm;
                    }
                }
                catch
                {

                }
            }
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool IsExistPerson(int IdKP)
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + Model.CinemaDB.filenameDB))
            {
                //var currentFilm = db.Cinema.Where(o => o.Id == myID);
                var personsInFilm = db.Person.Where(o => o.IdKP == IdKP);
                if (personsInFilm.Count() == 1)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsExistPersonInList(int IdKP, List<Model.CinemaDB.Person> _myPersons)
        {
            foreach (var iPerson in _myPersons)
            {
                if (iPerson.IdKP == IdKP)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsExistGenre(string vgenre)
        {
            bool _isExist = false;
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + Model.CinemaDB.filenameDB))
            {
                var currentFilm = db.Cinema.Where(o => o.Id == MyId);
                var genreInCurrentFilm = currentFilm.Select(o => o.Genres);
                foreach (var genre in genreInCurrentFilm)
                {
                    foreach (var item in genre)
                    {
                        if (item.Name == vgenre)
                        {
                            _isExist = true;
                            return _isExist;
                        }
                    }
                }
            }
            return _isExist;
        }

        private Byte[] ImageToByte(BitmapImage imageSource)
        {
            Stream stream = imageSource.StreamSource;
            Byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }

        private BitmapImage ImageFromWeb(string imgFromWeb)
        {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imgFromWeb, UriKind.Absolute);
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                //bitmap.Freeze();
                return bitmap;
        }


        private static System.Drawing.Image GetImageFromPicPath(string strUrl)
        {
            try
            {
                using (WebResponse wrFileResponse = WebRequest.Create(strUrl).GetResponse())
                using (Stream objWebStream = wrFileResponse.GetResponseStream())
                {

                    MemoryStream ms = new MemoryStream();
                    objWebStream.CopyTo(ms, 8192);
                    return System.Drawing.Image.FromStream(ms);
                }
            } catch 
            {
                return null;
            }
        }

        private byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        private void CopyInfoToDB ()
        {
            List<Model.CinemaDB.Person> myPersons;
            Model.CinemaDB.Person myPerson;
            Model.CinemaDB.Person personInFilm;
            List<Model.CinemaDB.Genre> myGenres = new List<Model.CinemaDB.Genre>();
            Model.CinemaDB.Genre myGenre = new Model.CinemaDB.Genre();
            Model.CinemaDB.Rating myRating = new Model.CinemaDB.Rating();
            Model.CinemaDB.Fact myFact;
            List<Model.CinemaDB.Fact> listMyFacts = new List<Model.CinemaDB.Fact>();
            Model.CinemaDB.Country myCountry;
            List<Model.CinemaDB.Country> listMyCountry = new List<Model.CinemaDB.Country>();

            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + Model.CinemaDB.filenameDB))
            {
                var currentFilm = db.Cinema.Where(o => o.Id == MyId).Include(p => p.Poster).FirstOrDefault();
                currentFilm.Description = myFilm.Description == null ? "" : myFilm.Description;
                currentFilm.Year = myFilm.Year == null ? 0 : myFilm.Year;
                currentFilm.Name = myFilm.Name == null ? "" : myFilm.Name;
                currentFilm.ShortDescription = myFilm.ShortDescription == null ? "" : (string)myFilm.ShortDescription;
                currentFilm.AlternativeName = myFilm.AlternativeName == null ? "" : (string)myFilm.AlternativeName;
                currentFilm.EnName = myFilm.EnName == null ? "" : (string)myFilm.EnName;

                if (currentFilm.Poster == null)
                {
                    try
                    {
                        Model.CinemaDB.Poster posterFilm = new Model.CinemaDB.Poster();
                        if (myFilm.Poster.PreviewUrl != null)
                        {
                            posterFilm.Pic = ImageToByteArray(GetImageFromPicPath(myFilm.Poster.PreviewUrl));
                            currentFilm.Poster = posterFilm;
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
                myPerson = new Model.CinemaDB.Person();
                myPersons = new List<Model.CinemaDB.Person>();
                personInFilm = new Model.CinemaDB.Person();

                pBLoadInfo.Value = 0;
                pBLoadInfo.Minimum = 0;
                pBLoadInfo.Maximum = myFilm.Persons.Count();
                UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(pBLoadInfo.SetValue);
                double value = 0;

                /// передача информации об актерах
                foreach (var myFilmPerson in myFilm.Persons)
                {
                    lInfo.Content = "Обработано персон: " + pBLoadInfo.Value.ToString() + " из " + pBLoadInfo.Maximum.ToString();
                    //pBLoadInfo.Value = pBLoadInfo.Value + 1;
                    Dispatcher.Invoke(updProgress, System.Windows.Threading.DispatcherPriority.Background,
                                      new object[] { ProgressBar.ValueProperty, value });
                    ++value;  
                    if (!IsExistPerson(myFilmPerson.Id) & !IsExistPersonInList(myFilmPerson.Id, myPersons))
                    {
                        myPerson = new Model.CinemaDB.Person();
                        myPerson.IdKP = myFilmPerson.Id == null ? 0 : myFilmPerson.Id;
                        myPerson.Name = myFilmPerson.Name == null ? "" : myFilmPerson.Name;
                        myPerson.Photo = myFilmPerson.Photo == null ? "": myFilmPerson.Photo;
                        myPerson.Description = myFilmPerson.Description == null ? "" : myFilmPerson.Description;
                        myPerson.Profession = myFilmPerson.Profession == null ? "" : myFilmPerson.Profession;
                        myPerson.EnProfession = myFilmPerson.EnProfession == null ? "" : myFilmPerson.EnProfession;
                        myPerson.EnName = myFilmPerson.EnName == null ? "" : myFilmPerson.EnName;
                        //var tmpPhoto = new BitmapImage(new Uri(myFilmPerson.Photo)); //ImageFromWeb(myFilmPerson.Photo);
                       
                        var tmpPhoto = myFilmPerson.Photo == "" ? null : GetImageFromPicPath(myFilmPerson.Photo);
                        //myPerson.PhotoPic

                        myPerson.PhotoPic = myFilmPerson.Photo == null ? null : ImageToByteArray(tmpPhoto);
                        myPersons.Add(myPerson);
                    }
                    else
                    {
                        if (!IsExistPersonInList(myFilmPerson.Id, myPersons))
                        {
                            personInFilm = db.Person.Where(o => o.IdKP == myFilmPerson.Id).FirstOrDefault();
                            myPersons.Add(personInFilm);
                        }
                    }
                }

                currentFilm.Persons = myPersons;

                // Передача данных о жанрах

                foreach (var iGenres in myFilm.Genres)
                {
                    if (!IsExistGenre(iGenres.Name))
                    {
                        myGenre = new Model.CinemaDB.Genre();
                        myGenre.Name = iGenres.Name;
                        myGenres.Add(myGenre);
                    }else
                    {
                        var genreInFilm = db.Genres.Where(o => o.Name == iGenres.Name).FirstOrDefault();
                        myGenres.Add(genreInFilm);
                    }
                }

                currentFilm.Genres = myGenres;

                //добавление фактов
                if (myFilm.FactsFilm != null)
                {
                    foreach (var iFact in myFilm.FactsFilm)
                    {
                        myFact = new Model.CinemaDB.Fact();
                        myFact.Value = iFact.Value;
                        listMyFacts.Add(myFact);
                    }
                    currentFilm.FactsCinema = listMyFacts;
                }

                //добавление стран
                if (myFilm.Countries != null)
                {
                    
                    foreach (var iCountry in myFilm.Countries)
                    {
                        myCountry = new Model.CinemaDB.Country();
                        myCountry.Name = iCountry.Name;
                        listMyCountry.Add(myCountry);
                    }
                    currentFilm.Countries = listMyCountry;
                }

                // Передача данных рейтингов
                myRating.Kp = myFilm.Rating.Kp == null ? 0 : myFilm.Rating.Kp;
                myRating.Imdb = myFilm.Rating.Imdb == null ? 0 : myFilm.Rating.Imdb;
                myRating.RussianFilmCritics = myFilm.Rating.RussianFilmCritics == null ? 0 : myFilm.Rating.RussianFilmCritics;
                myRating.FilmCritics = myFilm.Rating.FilmCritics == null ? 0 : myFilm.Rating.FilmCritics;
                myRating.Await = myFilm.Rating.Await == null ? "" : myFilm.Rating.Await.ToString();
                currentFilm.Rating = myRating;

                db.SaveChanges();
            }
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            CopyInfoToDB();            
            Close();
        }
    }
}
