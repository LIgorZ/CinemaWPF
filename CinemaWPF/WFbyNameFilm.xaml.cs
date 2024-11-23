using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using static CinemaWPF.Model.AllFilmsbyName;
using static CinemaWPF.Model.CFilmsShortView;
using static CinemaWPF.Model.CinemaDB;

namespace CinemaWPF
{
    /// <summary>
    /// Логика взаимодействия для WFbyNameFilm.xaml
    /// </summary>
    public partial class WFbyNameFilm : Window
    {
        private string NameFilm {  get; set; }
        private int MyID { get; set; }  
        private int currentFilm = 0;
        private int maxFilms = 0;
        private FindFilms myFilms;
        private IEnumerable<FilmItem>? myFilmsQ;
        private Model.CFilmsShortView.SVFilms ShowFilm;
        public WFbyNameFilm(string _nameFilm, int _myId)
        {
            InitializeComponent();
            NameFilm = _nameFilm;
            MyID = _myId;
            FindFilm();
        }

        private static BitmapImage GetImageFromPicPath(string strUrl)
        {
            using (WebResponse wrFileResponse = WebRequest.Create(strUrl).GetResponse())
            using (Stream objWebStream = wrFileResponse.GetResponseStream())
            {
                MemoryStream ms = new MemoryStream();
                objWebStream.CopyTo(ms, 8192);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        private void FindFilm()
        {
            var options = new RestClientOptions("https://api.kinopoisk.dev/v1.4/movie/search?page=1&limit=20&query=" + NameFilm);
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("X-API-KEY", APIKey1);
            var response = client.GetAsync(request);

            currentFilm = 0;
           // lWarrning.Text = "";
            try
            {
                if (((int)response.Result.StatusCode) == 200)
                {
                    myFilms = JsonConvert.DeserializeObject<FindFilms>(response.Result.Content);

                    ShowFilm = new Model.CFilmsShortView.SVFilms();

                    myFilmsQ =
                        (from cfilm in myFilms.Films
                         where cfilm.Name.Contains(NameFilm) == true
                         select cfilm);
                    tBInfo.Text = "Отфильтрованных фильмов: " + myFilmsQ.Count().ToString();
                    if (myFilmsQ.Count() == 0)
                    {
                        bOK.Visibility = Visibility.Collapsed;
                    }

                    if (myFilms.Total > 0)
                    {
                        //bPrev = true;
                        //bNext.Visible = true;
                        if (myFilms.Total < myFilms.Limit)
                        {
                            maxFilms = myFilms.Total.Value;
                        }
                        else
                        {
                            maxFilms = myFilms.Limit.Value;
                        }

                        maxFilms = myFilmsQ.Count();

                        if (maxFilms > 0)
                        {
                            ShowFilm.Description = myFilmsQ.ElementAt(currentFilm).Description == null ? "Нет описания" : myFilmsQ.ElementAt(currentFilm).Description;
                            ShowFilm.Name = myFilmsQ.ElementAt(currentFilm).Name;
                            ShowFilm.Year = myFilmsQ.ElementAt(currentFilm).Year.ToString();
                            ShowFilm.IdKP = myFilmsQ.ElementAt(currentFilm).Id;

                            
                            if (myFilmsQ.ElementAt(currentFilm).Poster.PreviewUrl != null)
                            {
                                var imageSource = new BitmapImage();
                                imageSource.BeginInit();
                                imageSource.UriSource = new Uri(myFilmsQ.ElementAt(currentFilm).Poster.PreviewUrl);
                                imageSource.EndInit();
                                ShowFilm.Poster = imageSource;
                            }
                        }
                        else
                        {

                            ShowFilm.Description = myFilmsQ.ElementAt(currentFilm).Description == null ? "Нет описания" : myFilmsQ.ElementAt(currentFilm).Description;
                            ShowFilm.Name = myFilmsQ.ElementAt(currentFilm).Name;
                            ShowFilm.Year = myFilmsQ.ElementAt(currentFilm).Year.ToString();
                        }

                        //lNumAllFilm.Text = "Всего найдено фильмов: " + myFilms.Total.ToString();
                        //lNumFilminQ.Text = "Фильмов в запросе: " + myFilms.Limit.ToString();
                        tBCurFilm.Text = (currentFilm + 1).ToString();
                        this.DataContext = ShowFilm;
                    }
                    else
                    {
                        //lNumAllFilm.Text = "Всего найдено фильмов: 0";
                        //lNumFilminQ.Text = "Фильмов в запросе: 0";
                    }
                }
            }
            catch
            {
                //lNumAllFilm.Text = "Всего найдено фильмов: 0. Ошибка запроса " + response.Status.ToString();
                ///lNumFilminQ.Text = "Фильмов в запросе: 0. Ошибка запроса";
            }
            
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void bPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentFilm > 0)
            {
                ShowFilm = new SVFilms();
                currentFilm--;
                ShowFilm.Description = myFilmsQ.ElementAt(currentFilm).Description == null ? "Нет описания" : myFilmsQ.ElementAt(currentFilm).Description;
                ShowFilm.Name = myFilmsQ.ElementAt(currentFilm).Name;
                ShowFilm.Year = myFilmsQ.ElementAt(currentFilm).Year.ToString();
                ShowFilm.IdKP = myFilmsQ.ElementAt(currentFilm).Id;

                if (myFilmsQ.ElementAt(currentFilm).Poster.PreviewUrl != null)
                {
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.UriSource = new Uri(myFilmsQ.ElementAt(currentFilm).Poster.PreviewUrl);
                    imageSource.EndInit();
                    ShowFilm.Poster = imageSource;
                }
                tBCurFilm.Text = (currentFilm + 1).ToString();
                this.DataContext = ShowFilm;
            }
        }

        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentFilm < (maxFilms - 1))
            {
                ShowFilm = new SVFilms();
                currentFilm++;
                ShowFilm.Description = myFilmsQ.ElementAt(currentFilm).Description == null ? "Нет описания" : myFilmsQ.ElementAt(currentFilm).Description;
                ShowFilm.Name = myFilmsQ.ElementAt(currentFilm).Name;
                ShowFilm.Year = myFilmsQ.ElementAt(currentFilm).Year.ToString();
                ShowFilm.IdKP = myFilmsQ.ElementAt(currentFilm).Id;

                if (myFilmsQ.ElementAt(currentFilm).Poster.PreviewUrl != null)
                {
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.UriSource = new Uri(myFilmsQ.ElementAt(currentFilm).Poster.PreviewUrl);
                    imageSource.EndInit();
                    ShowFilm.Poster = imageSource;
                }
                tBCurFilm.Text = (currentFilm + 1).ToString();
                this.DataContext = ShowFilm;
            }
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + Model.CinemaDB.filenameDB))
            {
                var cFilm = db.Cinema.Where(o => o.Id == MyID).FirstOrDefault();
                cFilm.IdKP = myFilmsQ.ElementAt(currentFilm).Id;
                db.SaveChanges();
            }

            Close();
        }
    }
}
