using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using static CinemaWPF.Model.CinemaDB;

namespace CinemaWPF
{
    /// <summary>
    /// Логика взаимодействия для WUpdatePerson.xaml
    /// </summary>
    public partial class WUpdatePerson : Window
    {
        private int myID;
        private int myIDKP;
        private CinemaWPF.Model.PersonbyIdKP.Person myPerson;
        public WUpdatePerson(int _iD, int _iDKP)
        {
            InitializeComponent();
            myID = _iD;
            myIDKP = _iDKP;
            FindPerson();
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
        private static string StripHtmlTags(string html)
        {
            return Regex.Replace(html, "<.*?>", string.Empty);
        }
        private void FindPerson ()
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                var currentPerson = db.Person.Where(o => o.Id == myID).FirstOrDefault();
                var options = new RestClientOptions("https://api.kinopoisk.dev/v1.4/person/" + currentPerson.IdKP.ToString());
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.AddHeader("accept", "application/json");
                request.AddHeader("X-API-KEY", APIKey1);
                var response = client.GetAsync(request);

                //pBPersonPhoto.Image = currentPerson.PhotoPic == null ? null : ConvertByteArrayToBitmapImage(currentPerson.PhotoPic);

                try
                {
                    if (((int)response.Result.StatusCode) == 200)
                    {
                        //lInfoLoad.Text = "Информация: получена";
                        //lInfoLoad.BackColor = Color.Green;
                        //bOK.Enabled = true;
                        myPerson = JsonConvert.DeserializeObject<CinemaWPF.Model.PersonbyIdKP.Person>(response.Result.Content);
                        CinemaWPF.Model.CPerson.ViewInfoPerson myVPerson = new CinemaWPF.Model.CPerson.ViewInfoPerson();

                        myVPerson.Photo = currentPerson.PhotoPic == null ? null : ConvertByteArrayToBitmapImage (currentPerson.PhotoPic);

                        if (myPerson.Facts != null)
                        {
                            if (myPerson.Facts.Count > 0)
                            {
                                foreach (var iFact in myPerson.Facts)
                                {
                                    myVPerson.Fact += iFact.Value + Environment.NewLine;
                                }
                                myVPerson.Fact = StripHtmlTags(myVPerson.Fact);
                            }
                        }

                        /* foreach (var iSpouse in myPerson.Spouses)
                         {
                             tBSpouse.Text += iSpouse.Id.ToString() + " - " + iSpouse.Name + " " + iSpouse.Relation + " Детей:" + iSpouse.Children.ToString() + Environment.NewLine;
                         }

                         tBSpouse.Text += Environment.NewLine + "Возраст: " + myPerson.Age.ToString() + Environment.NewLine + "Дата рождения: " + myPerson.Birthday.ToString() +
                                          Environment.NewLine + "Пол: " + myPerson.Sex;*/

                        if (myPerson.Movies.Count > 0)
                        {
                            foreach (var iMovie in myPerson.Movies)
                            {
                                if (iMovie.Name != null)
                                {
                                    myVPerson.Films += iMovie.Id.ToString() + " - " + iMovie.Name + " " + iMovie.EnProfession + " " + Environment.NewLine + iMovie.Description + Environment.NewLine;
                                }
                                else
                                {
                                    myVPerson.Films += iMovie.Id.ToString() + " - " + iMovie.AlternativeName + " " + iMovie.EnProfession + " " + Environment.NewLine + iMovie.Description + Environment.NewLine;
                                }
                            }
                        }
                        myVPerson.Name = myPerson.Name;
                        myVPerson.EnName = myPerson.EnName == null ? "" : myPerson.EnName.ToString();
                        myVPerson.Profession = myPerson.Profession.ToString();
                        myVPerson.Id = myID;
                        myVPerson.IdKP = myPerson.Id;

                        /* tBoxFilmDesc.Text = myFilm.Description == null ? "Нет описания" : myFilm.Description;
                         lNameFilm.Text = myFilm.Name == null ? "" : myFilm.Name;
                         lYear.Text = myFilm.Year == null ? "" : myFilm.Year.ToString();
                         lIDKP.Text = myFilm.Id == null ? "" : myFilm.Id.ToString();

                         if (myFilm.Poster.PreviewUrl != null)
                         {
                             pBoxFilm.Image = GetImageFromPicPath(myFilm.Poster.PreviewUrl);
                         }*/
                        this.DataContext = myVPerson;
                    }
                }
                catch
                {
                 //   lInfoLoad.Text = "Информация: не найдена";
                   // lInfoLoad.BackColor = Color.Red;
                    //bOK.Visible = false;
                }
            }
        }
        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close ();
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            if (myPerson == null)
            {
                Close();
            }
            List<Model.CinemaDB.Fact> listMyFact = new List<Model.CinemaDB.Fact>();
            Model.CinemaDB.Fact myFact = new Model.CinemaDB.Fact();
            List<Model.CinemaDB.Spouse> listMySpouse = new List<Model.CinemaDB.Spouse>();
            Model.CinemaDB.Spouse mySpouse = new Model.CinemaDB.Spouse();
            List<Model.CinemaDB.Movie> listMyMovie = new List<Model.CinemaDB.Movie>();
            Model.CinemaDB.Movie myMovie = new Model.CinemaDB.Movie();

            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                var currentPerson = db.Person.Where(o => o.Id == myID).FirstOrDefault();
                currentPerson.Birthday = myPerson.Birthday == null ? null : myPerson.Birthday;
                currentPerson.Sex = myPerson.Sex == null ? "" : myPerson.Sex;
                currentPerson.Age = myPerson.Age == null ? null : myPerson.Age;
                currentPerson.Death = myPerson.Death == null ? null : (DateTime)myPerson.Death;
                currentPerson.Growth = myPerson.Growth == null ? null : myPerson.Growth;

                //добавление фактов
                if (myPerson.Facts != null)
                {
                    foreach (var iFact in myPerson.Facts)
                    {
                        myFact = new Model.CinemaDB.Fact();
                        myFact.Value = iFact.Value;
                        listMyFact.Add(myFact);
                    }
                    currentPerson.Facts = listMyFact;
                }

                //добавление супругов
                foreach (var iSpouse in myPerson.Spouses)
                {
                    mySpouse = new Model.CinemaDB.Spouse();
                    mySpouse.IdKP = iSpouse.Id == null ? 0 : iSpouse.Id;
                    mySpouse.Name = iSpouse.Name == null ? "" : iSpouse.Name.ToString();
                    mySpouse.Children = iSpouse.Children == null ? null : iSpouse.Children;
                    mySpouse.Divorced = iSpouse.Divorced == null ? null : iSpouse.Divorced;
                    mySpouse.DivorcedReason = iSpouse.DivorcedReason == null ? null : iSpouse.DivorcedReason;
                    mySpouse.Relation = iSpouse.Relation == null ? null : iSpouse.Relation;
                    listMySpouse.Add(mySpouse);
                }
                currentPerson.Spouses = listMySpouse;

                //Загрузка фильмографии
                foreach (var iMovie in myPerson.Movies)
                {
                    myMovie = new Model.CinemaDB.Movie();
                    myMovie.IdKP = iMovie.Id == null ? 0 : iMovie.Id;
                    myMovie.Description = iMovie.Description == null ? null : iMovie.Description.ToString();
                    myMovie.AlternativeName = iMovie.AlternativeName == null ? null : iMovie.AlternativeName.ToString();
                    myMovie.EnProfession = iMovie.EnProfession == null ? null : iMovie.EnProfession.ToString();
                    myMovie.Name = iMovie.Name == null ? null : iMovie.Name.ToString();
                    myMovie.Rating = iMovie.Rating == null ? null : (int)iMovie.Rating;
                    myMovie.General = iMovie.General == null ? null : iMovie.General;
                    listMyMovie.Add(myMovie);
                }
                currentPerson.Movies = listMyMovie;

                db.SaveChanges();
            }

            Close();
        }
    }
}
