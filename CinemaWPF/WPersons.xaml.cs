using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using static CinemaWPF.Model.CFilmsShortView;
using static CinemaWPF.Model.CinemaDB;

namespace CinemaWPF
{
    /// <summary>
    /// Логика взаимодействия для WPersons.xaml
    /// </summary>
    public partial class WPersons : Window
    {
        private int iDActor = -1;
        private int iDKPActor = -1;
        private int MyId { get; set; } 
        private List<Model.CinemaDB.Person> listToViewPerson = new List<Model.CinemaDB.Person>();
        public WPersons(int _myId)
        {
            InitializeComponent();
            MyId = _myId;
            ShowActors();
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
        private void ShowActors()
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                Model.CinemaDB.Person vPerson = new Model.CinemaDB.Person();
                Model.CinemaDB.Person fPerson = new Model.CinemaDB.Person();
                foreach (var iPerson in db.Person)
                {
                    vPerson = new Model.CinemaDB.Person();
                    vPerson.Name = iPerson.Name;
                    vPerson.Id = iPerson.Id;
                    vPerson.PhotoPic = iPerson.PhotoPic;
                    vPerson.IdKP = iPerson.IdKP;
                    vPerson.Profession = iPerson.Profession;
                    vPerson.EnName = iPerson.EnName;
                    vPerson.Description = iPerson.Description;
                    vPerson.EnProfession = iPerson.EnProfession;
                    vPerson.Birthday = iPerson.Birthday;
                    if (iPerson.Birthday != null)
                    {
                        vPerson.sBirthday = iPerson.Birthday.Value.Day + "." + iPerson.Birthday.Value.Month + "." + iPerson.Birthday.Value.Year;
                    }
                    else vPerson.sBirthday = "";
                    if (iPerson.Id == MyId)
                    {
                        fPerson = vPerson;
                    }

                    listToViewPerson.Add(vPerson);
                }
                lBListActors.ItemsSource = null;
                lBListActors.ItemsSource = listToViewPerson;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lBListActors.ItemsSource);
                view.Filter = UserFilter;
                if (MyId != -1)
                {
                    lBListActors.SelectedIndex = lBListActors.Items.IndexOf(fPerson);
                    tBSPerson.Text = fPerson.Name;
                    iDActor = fPerson.Id;
                }
            }
        }

        private bool UserFilter(object item)
        {
            bool isAccept = false;
            isAccept = (cBActor.IsChecked.Value&((item as Model.CinemaDB.Person).EnProfession == "actor")|
                       cBDirector.IsChecked.Value& ((item as Model.CinemaDB.Person).EnProfession == "director") |
                       cBProducer.IsChecked.Value& ((item as Model.CinemaDB.Person).EnProfession == "producer")) &
                       ((item as Model.CinemaDB.Person).Name.IndexOf(tBSPerson.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            /* if (cBActor.IsChecked.Value)
                 isAccept = ((item as Model.CinemaDB.Person).EnProfession == "actor");
             if (cBDirector.IsChecked.Value)
                 isAccept = ((item as Model.CinemaDB.Person).EnProfession == "director");
             if (cBActor.IsChecked.Value && cBDirector.IsChecked.Value)
                 isAccept = (((item as Model.CinemaDB.Person).EnProfession == "actor") |
                             ((item as Model.CinemaDB.Person).EnProfession == "director"));*/
            if (!cBActor.IsChecked.Value && !cBDirector.IsChecked.Value && !cBProducer.IsChecked.Value &&
                tBSPerson.Text == "")
                return true;
            if (!cBActor.IsChecked.Value && !cBDirector.IsChecked.Value && !cBProducer.IsChecked.Value &&
                tBSPerson.Text != "")
                return ((item as Model.CinemaDB.Person).Name.IndexOf(tBSPerson.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            return isAccept;
        }

        private void b_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_FilmView_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void b_ActorView_Click(object sender, RoutedEventArgs e)
        {

        }
        private static string StripHtmlTags(string html)
        {
            return Regex.Replace(html, "<.*?>", string.Empty);
        }
        private void lBListActors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            iDActor = lBListActors.Items.IndexOf(lBListActors.SelectedItem);
            if (iDActor != -1)
            {
                iDActor = (lBListActors.Items[iDActor] as Model.CinemaDB.Person).Id;

                using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
                {
                    var currentPerson = db.Person.Where(o => o.Id == iDActor).FirstOrDefault();

                    try
                    {
                        
                        CinemaWPF.Model.CPerson.ViewInfoPerson myVPerson = new CinemaWPF.Model.CPerson.ViewInfoPerson();
                        myVPerson.Photo = currentPerson.PhotoPic == null ? null : ConvertByteArrayToBitmapImage(currentPerson.PhotoPic);

                        var currentFacts = db.Person.Where(o => o.Id == iDActor).Select(q => q.Facts).FirstOrDefault();
                        if (currentFacts.Count() > 0)
                        {
                            rTBFact.Text = "Факты: ";
                            foreach (var iFact in currentFacts)
                            {
                                myVPerson.Fact += iFact.Value + Environment.NewLine;
                            }
                            myVPerson.Fact = HttpUtility.HtmlDecode(StripHtmlTags(myVPerson.Fact));
                        } else
                        {
                            rTBFact.Text = string.Empty;
                        }
                        
                        /* foreach (var iSpouse in myPerson.Spouses)
                         {
                             tBSpouse.Text += iSpouse.Id.ToString() + " - " + iSpouse.Name + " " + iSpouse.Relation + " Детей:" + iSpouse.Children.ToString() + Environment.NewLine;
                         }

                         tBSpouse.Text += Environment.NewLine + "Возраст: " + myPerson.Age.ToString() + Environment.NewLine + "Дата рождения: " + myPerson.Birthday.ToString() +
                                          Environment.NewLine + "Пол: " + myPerson.Sex;*/
                        var currentMovies = db.Person.Where(o => o.Id == iDActor).Select(q => q.Movies).FirstOrDefault();
                        if (currentMovies.Count() > 0)
                        {
                            foreach (var iMovie in currentMovies)
                            {
                                if (iMovie.Name != null)
                                {
                                    myVPerson.Films += iMovie.Name + Environment.NewLine; // + iMovie.Description + Environment.NewLine;
                                }
                                else
                                {
                                    myVPerson.Films += iMovie.AlternativeName + Environment.NewLine; // + iMovie.Description + Environment.NewLine;
                                }
                            }
                        }
                            myVPerson.Name = currentPerson.Name == null ? "" : currentPerson.Name.ToString(); ;
                            myVPerson.EnName = currentPerson.EnName == null ? "" : currentPerson.EnName.ToString();
                            myVPerson.Profession = currentPerson.Profession == null ? "" : currentPerson.Profession.ToString();
                            myVPerson.Age = currentPerson.Age == null ? "" : currentPerson.Age.ToString();
                            myVPerson.Id = iDActor;
                            myVPerson.IdKP = iDKPActor;

                            myVPerson.Birthday = currentPerson.Birthday == null ? "" : currentPerson.Birthday.Value.Day.ToString() + "." + currentPerson.Birthday.Value.Month.ToString() + "." +
                                    currentPerson.Birthday.Value.Year.ToString();
                        if (currentPerson.Death == null)
                        {
                            rtBDeath.Text = string.Empty;
                        }
                        else 
                        {
                            rtBDeath.Text = "Дата смерти: ";
                        }
                            myVPerson.Death = currentPerson.Death == null ? "" : currentPerson.Death.Value.Day.ToString() + "." + currentPerson.Death.Value.Month.ToString() + "." +
                                                                currentPerson.Death.Value.Year.ToString();


                        /* tBoxFilmDesc.Text = myFilm.Description == null ? "Нет описания" : myFilm.Description;
                         lNameFilm.Text = myFilm.Name == null ? "" : myFilm.Name;
                         lYear.Text = myFilm.Year == null ? "" : myFilm.Year.ToString();
                         lIDKP.Text = myFilm.Id == null ? "" : myFilm.Id.ToString();

                         if (myFilm.Poster.PreviewUrl != null)
                         {
                             pBoxFilm.Image = GetImageFromPicPath(myFilm.Poster.PreviewUrl);
                         }*/
                        tBInfoPerson.DataContext = myVPerson;
                        IPhotoPerson.DataContext = myVPerson;
                    }
                    catch
                    {
                        //   lInfoLoad.Text = "Информация: не найдена";
                        // lInfoLoad.BackColor = Color.Red;
                        //bOK.Visible = false;
                    }
                }
            }
        }
        private void b_EditPerson_Click(object sender, RoutedEventArgs e)
        {
            if (iDActor != -1)
            {
                using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
                {
                    var currentPerson = db.Person.Where(o => o.Id == iDActor).FirstOrDefault();
                    WUpdatePerson mywUpdatePerson = new WUpdatePerson(iDActor, currentPerson.IdKP.Value);
                    mywUpdatePerson.ShowDialog();
                }

                using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
                {
                    var currentPerson = db.Person.Where(o => o.Id == iDActor).FirstOrDefault();
                    foreach (var iPerson in listToViewPerson)
                    {
                        if (iPerson.Id == iDActor)
                        {
                            iPerson.Birthday = currentPerson.Birthday;
                            if (iPerson.Birthday.HasValue)
                            {
                                iPerson.sBirthday = currentPerson.Birthday == null ? "" : currentPerson.Birthday.Value.Day.ToString() + "." + currentPerson.Birthday.Value.Month.ToString() + "." +
                                        currentPerson.Birthday.Value.Year.ToString();
                            }
                            else iPerson.sBirthday = "";
                            //lBListActors.SelectedIndex = lBListActors.Items.IndexOf(iPerson);
                        }
                    }
                    CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
                    lBListActors_SelectionChanged(sender, null);
                }
            }
        }

        private void tBSPerson_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
            /*  using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
              {
                  var findPerson = db.Person;
                  List<Model.CinemaDB.Person> listToViewPerson = new List<Model.CinemaDB.Person>();
                  Model.CinemaDB.Person vPerson = new Model.CinemaDB.Person();
                  foreach (var iPerson in findPerson)
                  {
                      if (iPerson.Name.ToLower().Contains(tBSPerson.Text.ToLower()))
                      {
                          vPerson = new Model.CinemaDB.Person();
                          vPerson.Name = iPerson.Name;
                          vPerson.Id = iPerson.Id;
                          vPerson.PhotoPic = iPerson.PhotoPic;
                          vPerson.IdKP = iPerson.IdKP;
                          vPerson.Profession = iPerson.Profession;
                          vPerson.EnName = iPerson.EnName;
                          vPerson.Description = iPerson.Description;
                          vPerson.EnProfession = iPerson.EnProfession;
                          listToViewPerson.Add(vPerson);
                      }
                  }
                  lBListActors.ItemsSource = null;
                  lBListActors.ItemsSource = listToViewPerson;
              }*/
        }

        private void cBActor_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
        }

        private void cBActor_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
        }

        private void cBDirector_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
        }

        private void cBDirector_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
        }

        private void cBProducer_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
        }

        private void cBProducer_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
        }

        private void cBSortPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SortFilm = cBoxSortPerson.SelectedIndex;
            if (SortFilm == 0)
            {
                lBListActors.Items.SortDescriptions.Clear();
            }
            if (SortFilm == 1)
            {
                lBListActors.Items.SortDescriptions.Clear();
                lBListActors.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            }
            if (SortFilm == 2)
            {
                lBListActors.Items.SortDescriptions.Clear();
                lBListActors.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Descending));
            }
            if (SortFilm == 3)
            {
                lBListActors.Items.SortDescriptions.Clear();
                lBListActors.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("EnName", System.ComponentModel.ListSortDirection.Ascending));
            }
            if (SortFilm == 4)
            {
                lBListActors.Items.SortDescriptions.Clear();
                lBListActors.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("EnName", System.ComponentModel.ListSortDirection.Descending));
            }
            if (SortFilm == 5)
            {
                lBListActors.Items.SortDescriptions.Clear();
                lBListActors.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Birthday", System.ComponentModel.ListSortDirection.Ascending));
            }
            if (SortFilm == 6)
            {
                lBListActors.Items.SortDescriptions.Clear();
                lBListActors.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Birthday", System.ComponentModel.ListSortDirection.Descending));
            }
            if (SortFilm == 7)
            {
                lBListActors.Items.SortDescriptions.Clear();
                lBListActors.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Profession", System.ComponentModel.ListSortDirection.Ascending));
            }
            if (SortFilm == 8)
            {
                lBListActors.Items.SortDescriptions.Clear();
                lBListActors.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Profession", System.ComponentModel.ListSortDirection.Descending));
            }


            if (lBListActors.ItemsSource != null)
            {
                CollectionViewSource.GetDefaultView(lBListActors.ItemsSource).Refresh();
            }
        }
    }
}
