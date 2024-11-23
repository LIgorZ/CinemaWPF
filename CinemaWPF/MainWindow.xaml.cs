using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using static CinemaWPF.Model.CinemaDB;
using TagLib;

namespace CinemaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> files;
        private int IdFilm { get; set; }
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        private List<Model.CFilmsShortView.SVFilms> listToViewFilms = new List<Model.CFilmsShortView.SVFilms>();
        private int SortFilm { get; set; }

        public MainWindow()
        {
            //filenameDB = Directory.GetCurrentDirectory() + "\\MyCinema.sqlite";
            filenameDB = /*Directory.GetCurrentDirectory() + "\\" + */Properties.Settings.Default.FileNameDB;
            InitializeComponent();
            WStart wStart = new WStart();
            wStart.Show();

            SortFilm = 0;
            if (System.IO.File.Exists(filenameDB))
            {
                ShowFilms();
            }
            wStart.Close();
        }

        private void ShowFilms()
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(pBFilm.SetValue);
                double value = 0;
                pBFilm.Value = 1;
                pBFilm.Minimum = 1;
                pBFilm.Maximum = db.Cinema.Count();

                Model.CFilmsShortView.SVFilms viewFilm = new Model.CFilmsShortView.SVFilms();
                var allFilms = db.Cinema.Include(q => q.Poster).Include(w => w.Persons).Include(d => d.Rating).AsNoTracking();
                foreach (var iFilm in allFilms)
                {
                    Dispatcher.Invoke(updProgress, System.Windows.Threading.DispatcherPriority.Background,
                                       new object[] { ProgressBar.ValueProperty, value });
                    ++value;

                    viewFilm = new Model.CFilmsShortView.SVFilms();
                    viewFilm.PathToFilm = iFilm.PathToCinema;
                    viewFilm.Id = iFilm.Id;
                    viewFilm.Name = iFilm.Name;
                    viewFilm.Year = iFilm.Year.ToString();
                    viewFilm.AlternativeName = iFilm.AlternativeName;
                    viewFilm.dRKp = iFilm.Rating == null ? 0 : iFilm.Rating.Kp;
                    viewFilm.dRIMDb = iFilm.Rating == null ? 0 : iFilm.Rating.Imdb;

                    if (iFilm.Poster != null)
                    {
                        viewFilm.Poster = ConvertByteArrayToBitmapImage(iFilm.Poster.Pic);
                    }
                    var genreInFilm = db.Cinema.Where(q => q.Id == iFilm.Id).Select(o => o.Genres).AsNoTracking();
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
                    // var currentFilm = allFilms.Where(q => q.Id == iFilm.Id);
                    var personInFilm = iFilm.Persons.Where(s => s.EnProfession == "actor").Take(7);
                    //var personsInFilm = db.Cinema.Where(q => q.Id == iFilm.Id).Select(o => o.Persons).Where(w => w.FirstOrDefault().EnProfession == "actor").AsNoTracking().FirstOrDefault();
                    // foreach (var person in personsInFilm)
                    if (personInFilm != null)
                    {
                        foreach (var p in personInFilm)
                        {
                            if (p.EnProfession == "actor" && i < 7)
                            {
                                sActors += p.Name == "" ? p.EnName + ", " : p.Name + ", ";
                                i++;
                            }
                        }
                    }
                    if (sActors.Length > 2)
                    {
                        sActors = sActors.Remove(sActors.Length - 2, 2);
                    }
                    viewFilm.Actors = sActors;

                    listToViewFilms.Add(viewFilm);
                }
                lDir.Content = "Всего фильмов: " + db.Cinema.Count().ToString();
                lBFilms.ItemsSource = listToViewFilms;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lBFilms.ItemsSource);
                view.Filter = UserFilter;
            }
        }

        private bool UserFilter(object item)
        {
            bool isAccept = false;
            isAccept = (cBBoevik.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("боевик")) |
                       cBHistory.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("история")) |
                       cBFiction.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("фантастика")) |
                       cBWar.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("военный")) |
                       cBDrama.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("драма")) |
                       cBComedy.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("комедия")) |
                       cBBiography.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("биография")) |
                       cBAdult.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("для взрослых")) |
                       cBMelodrama.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("мелодрама")) |
                       cBVestern.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("вестерн")) |
                       cBDetective.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("детектив")) |
                       cBCriminal.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("криминал")) |
                       cBTriller.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("триллер")) |
                       cBHorror.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("ужасы")) |
                       cBSport.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("спорт")) |
                       cBFantasy.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("фэнтези")) |
                       cBFamily.IsChecked.Value & ((item as Model.CFilmsShortView.SVFilms).Genres.Contains("семейный"))) &
                       ((item as Model.CFilmsShortView.SVFilms).Name.IndexOf(tBSFilm.Text, StringComparison.OrdinalIgnoreCase) >= 0);

            if ((!cBBoevik.IsChecked.Value && !cBFiction.IsChecked.Value && !cBHistory.IsChecked.Value
                && !cBWar.IsChecked.Value && !cBDrama.IsChecked.Value && !cBComedy.IsChecked.Value
                && !cBBiography.IsChecked.Value && !cBAdult.IsChecked.Value && !cBMelodrama.IsChecked.Value
                && !cBVestern.IsChecked.Value && !cBDetective.IsChecked.Value && !cBCriminal.IsChecked.Value
                && !cBTriller.IsChecked.Value && !cBHorror.IsChecked.Value && !cBSport.IsChecked.Value
                && !cBFantasy.IsChecked.Value && !cBFamily.IsChecked.Value) && tBSFilm.Text == "")
                return true;
            if ((!cBBoevik.IsChecked.Value && !cBFiction.IsChecked.Value && !cBHistory.IsChecked.Value
                && !cBWar.IsChecked.Value && !cBDrama.IsChecked.Value && !cBComedy.IsChecked.Value
                && !cBBiography.IsChecked.Value && !cBAdult.IsChecked.Value && !cBMelodrama.IsChecked.Value
                && !cBVestern.IsChecked.Value && !cBDetective.IsChecked.Value && !cBCriminal.IsChecked.Value
                && !cBTriller.IsChecked.Value && !cBHorror.IsChecked.Value && !cBSport.IsChecked.Value
                && !cBFantasy.IsChecked.Value && !cBFamily.IsChecked.Value) && tBSFilm.Text != "")
                return ((item as Model.CFilmsShortView.SVFilms).Name.IndexOf(tBSFilm.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            return isAccept;
        }


        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static EnumerationOptions CompatibleRecursive => new EnumerationOptions
        {
            RecurseSubdirectories = true,
            MatchType = MatchType.Win32,
            AttributesToSkip = (FileAttributes.System | FileAttributes.Hidden),
            IgnoreInaccessible = false
        };

        private void bDirScan_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Выберите директорию для сканирования фильмов"
                // Set options here
            };
            UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(pBFilm.SetValue);
            double value = 0;
            // Сканировать папку
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (folderDialog.ShowDialog() == true)
            {
                string selectedFolder = folderDialog.FolderName;
                lDir.Content = "Каталог сканирования: " + selectedFolder;
                var sourceDir = new DirectoryInfo(selectedFolder);
                files = Directory.EnumerateFiles(selectedFolder,
                                                 "*.*",
                                                 CompatibleRecursive)
                                            .Where(s => s.EndsWith(".mkv") || s.EndsWith(".avi") || s.EndsWith(".ts")).ToList();
                // var nameFiles = (List<string>)files.Select(v => new String(Path.GetFileNameWithoutExtension(v)));
                List<string> nameFiles = new List<string>();
                string stmp;
                Model.CinemaDB.Poster posterFilm = new Model.CinemaDB.Poster();

                foreach (var f in files)
                {
                    stmp = System.IO.Path.GetFileNameWithoutExtension(f);
                    if (stmp.LastIndexOf(".") != -1)
                    {
                        stmp = stmp.Remove(stmp.LastIndexOf("."));
                    }
                    if (stmp.LastIndexOf(".") != -1)
                    {
                        stmp = stmp.Remove(stmp.LastIndexOf("."));
                    }
                    nameFiles.Add(stmp);
                }
                // lBoxScanFiles.DataSource = files;
                //lBoxScanNames.DataSource = nameFiles;
                lDir.Content += Environment.NewLine + "Число файлов для скана: " + files.Count.ToString();

                using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
                {
                    pBFilm.Value = 1;
                    pBFilm.Minimum = 1;
                    pBFilm.Maximum = files.Count;
                    //pBFilm. = 1;

                    foreach (var f in files)
                    {
                        if (!IsExistFilm(f))
                        {
                            Cinema film = new Cinema();
                            try
                            {
                                var tfile = TagLib.File.Create(f);
                                FileInfo myFile = new FileInfo(f);
                                string title = tfile.Tag.Title;
                                TimeSpan duration = tfile.Properties.Duration;
                                /* tBVideoInfo.Text = "___TagLib__________" + Environment.NewLine;
                                tBVideoInfo.Text += "Ширина: " + tfile.Properties.VideoWidth.ToString() + Environment.NewLine;
                                tBVideoInfo.Text += "Высота: " + tfile.Properties.VideoHeight.ToString() + Environment.NewLine;
                                tBVideoInfo.Text += "Длительность: " + tfile.Properties.Duration.Hours.ToString() + ":" + tfile.Properties.Duration.Minutes.ToString() + ":"
                                                                   + tfile.Properties.Duration.Seconds.ToString() + Environment.NewLine;
                                tBVideoInfo.Text += "Объем: " + ((decimal)myFile.Length / (1024 * 1024 * 1024)).ToString("F2") + " Гб" + Environment.NewLine;*/
                                film.PathToCinema = f;
                                film.WidthCinema = tfile.Properties.VideoWidth.ToString();
                                film.HighCinema = tfile.Properties.VideoHeight.ToString();
                                film.DurationCinema = duration;
                                film.CinemaSize = myFile.Length;

                                stmp = System.IO.Path.GetFileNameWithoutExtension(f);
                                if (stmp.LastIndexOf(".") != -1)
                                {
                                    stmp = stmp.Remove(stmp.LastIndexOf("."));
                                }
                                if (stmp.LastIndexOf(".") != -1)
                                {
                                    stmp = stmp.Remove(stmp.LastIndexOf("."));
                                }
                                film.Name = stmp;

                                try
                                {

                                    posterFilm = new Model.CinemaDB.Poster();
                                    posterFilm.Pic = tfile.Tag.Pictures[0].Data.Data;
                                    film.Poster = posterFilm;
                                }
                                catch (Exception ex)
                                {

                                }
                                db.Cinema.Add(film);
                                db.SaveChanges();
                                Dispatcher.Invoke(updProgress, System.Windows.Threading.DispatcherPriority.Background,
                                         new object[] { ProgressBar.ValueProperty, value });
                                ++value;
                                //pBFilm.Value++;
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            Dispatcher.Invoke(updProgress, System.Windows.Threading.DispatcherPriority.Background,
                                        new object[] { ProgressBar.ValueProperty, value });
                            ++value;
                        }


                        // If you've disabled multi-selection, use 'SelectedFolder'.
                        // string selectedFolder = betterFolderBrowser1.SelectedFolder;
                    }
                    stopwatch.Stop();
                    TimeSpan ts = stopwatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                                                        ts.Minutes, ts.Seconds,
                                                        ts.Milliseconds / 10);

                    lDir.Content += Environment.NewLine + "Время сканирования каталога: " + elapsedTime; //+ ts.Minutes.ToString("F2") + " мин.";

                    ShowFilms();
                }
            }
        }

        private void ClearDB()
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                lBFilms.ItemsSource = null;
            }
        }

        private void bClearDB_Click(object sender, RoutedEventArgs e)
        {
            ClearDB();
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
            if (html != null)
            {
                return Regex.Replace(html, "<.*?>", string.Empty);
            }
            return html;
        }

        private void UpdateFilm(int _IdFilm)
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                Model.CFilmsShortView.SVFilms viewFilm = new Model.CFilmsShortView.SVFilms();
                var filmForUpdate = db.Cinema.Where(o => o.Id == _IdFilm).Include(q => q.Poster).FirstOrDefault();
                foreach (var iFilm in listToViewFilms)
                {
                    if (iFilm.Id == _IdFilm)
                    {
                        //viewFilm = new Model.CFilmsShortView.SVFilms();
                        iFilm.PathToFilm = filmForUpdate.PathToCinema;
                        //iFilm.Id = filmForUpdate.Id;
                        iFilm.Name = filmForUpdate.Name;
                        iFilm.Year = filmForUpdate.Year.ToString();
                        iFilm.AlternativeName = filmForUpdate.AlternativeName;

                        if (filmForUpdate.Poster != null)
                        {
                            iFilm.Poster = ConvertByteArrayToBitmapImage(filmForUpdate.Poster.Pic);
                        }
                        var genreInFilm = db.Cinema.Where(q => q.Id == filmForUpdate.Id).Select(o => o.Genres);
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
                        iFilm.Genres = sGenres;

                        //Вывод актеров
                        string sActors = "";
                        int i = 1;
                        var personsInFilm = db.Cinema.Where(q => q.Id == filmForUpdate.Id).Select(o => o.Persons);
                        foreach (var person in personsInFilm)
                        {
                            foreach (var p in person)
                            {
                                if (p.EnProfession == "actor" && i < 9)
                                {
                                    sActors += p.Name == "" ? p.EnName + ", " : p.Name + ", ";
                                    i++;
                                }
                            }
                        }
                        if (sActors.Length > 2)
                        {
                            sActors = sActors.Remove(sActors.Length - 2, 2);
                        }
                        iFilm.Actors = sActors;
                    }
                }
                //var myUpdateFilm = listToViewFilms.Where(o => o.Id == _IdFilm).FirstOrDefault();
                //myUpdateFilm = viewFilm;
                CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
            }
        }
        private void bEditFilm_Click(object sender, RoutedEventArgs e)
        {

            if (IdFilm != -1)
            {
                WEditFilm myWEditFilm = new(IdFilm);
                myWEditFilm.ShowDialog();
                //lBFilms.ItemsSource = null;
                //ShowFilms();
                UpdateFilm(IdFilm);
                lBFilms_SelectionChanged(sender, null);
            }
        }

        private void lBFilms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IdFilm = lBFilms.Items.IndexOf(lBFilms.SelectedItem);
            if (IdFilm != -1)
            {
                IdFilm = (lBFilms.Items[IdFilm] as Model.CFilmsShortView.SVFilms).Id;

                using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
                {
                    Model.CFilmsShortView.SVFilms viewFilm = new Model.CFilmsShortView.SVFilms();
                    Model.CinemaDB.Person viewPerson = new Model.CinemaDB.Person();
                    List<Model.CinemaDB.Person> listViewPersons = new List<Model.CinemaDB.Person>();
                    Model.CinemaDB.Fact myFact;
                    var iFilm = db.Cinema.Where(q => q.Id == IdFilm).Include(r => r.Poster).Include(q => q.FactsCinema).Include(r => r.Countries)
                        .Include(t => t.Rating).FirstOrDefault();

                    viewFilm = new Model.CFilmsShortView.SVFilms();
                    viewFilm.PathToFilm = iFilm.PathToCinema;
                    viewFilm.Id = iFilm.Id;
                    viewFilm.Name = iFilm.Name;
                    viewFilm.Year = iFilm.Year.ToString();
                    viewFilm.AlternativeName = iFilm.AlternativeName;
                    viewFilm.Description = iFilm.Description;

                    if (iFilm.Poster != null)
                    {
                        viewFilm.Poster = ConvertByteArrayToBitmapImage(iFilm.Poster.Pic);
                    }
                    var genreInFilm = db.Cinema.Where(q => q.Id == iFilm.Id).Select(o => o.Genres);
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
                    var personsInFilm = db.Cinema.Where(q => q.Id == iFilm.Id).Select(o => o.Persons);
                    foreach (var person in personsInFilm)
                    {
                        foreach (var p in person)
                        {
                            if (p.EnProfession == "actor")
                            {
                                viewPerson = new Model.CinemaDB.Person();
                                viewPerson.Name = p.Name == "" ? p.EnName : p.Name;
                                viewPerson.PhotoPic = p.PhotoPic;
                                viewPerson.EnName = p.EnName;
                                viewPerson.Id = p.Id;
                                listViewPersons.Add(viewPerson);

                                sActors += p.Name == "" ? p.EnName + ", " : p.Name + ", ";
                            }

                        }
                    }
                    if (sActors.Length > 2)
                    {
                        sActors = sActors.Remove(sActors.Length - 2, 2);
                    }

                    viewFilm.Actors = sActors;

                    //Вывод Режиссеров
                    string sDirector = "";
                    foreach (var person in personsInFilm)
                    {
                        foreach (var p in person)
                        {
                            if (p.EnProfession == "director")
                            {
                                viewPerson = new Model.CinemaDB.Person();
                                viewPerson.Name = p.Name;
                                viewPerson.PhotoPic = p.PhotoPic;
                                viewPerson.EnName = p.EnName;
                                viewPerson.Id = p.Id;
                                listViewPersons.Add(viewPerson);

                                sDirector += p.Name == "" ? p.EnName + ", " : p.Name + ", ";
                            }

                        }
                    }
                    if (sDirector.Length > 2)
                    {
                        sDirector = sDirector.Remove(sDirector.Length - 2, 2);
                    }

                    viewFilm.Director = sDirector;
                    viewFilm.Duration = iFilm.DurationCinema.Value.Hours.ToString() + ":" + iFilm.DurationCinema.Value.Minutes.ToString() + ":" + iFilm.DurationCinema.Value.Seconds.ToString();
                    viewFilm.PathToFilm = iFilm.PathToCinema.ToString();
                    viewFilm.RIMDb = iFilm.Rating == null ? "" : iFilm.Rating.Imdb.ToString();
                    viewFilm.RKp = iFilm.Rating == null ? "" : iFilm.Rating.Kp.ToString();
                    viewFilm.ResFilm = iFilm.WidthCinema.ToString() + "x" + iFilm.HighCinema.ToString();
                    viewFilm.SizeFilm = ((decimal)iFilm.CinemaSize / (1024 * 1024 * 1024)).ToString("F2") + " Гб";

                    //добавление фактов
                    if (iFilm.FactsCinema != null)
                        if (iFilm.FactsCinema.Count > 0)
                        {
                            rTBFacts.Text = "Факты:";
                            foreach (var iFact in iFilm.FactsCinema)
                            {
                                viewFilm.FactsFilm += iFact.Value;
                            }
                            //viewFilm.FactsFilm = StripHtmlTags(viewFilm.FactsFilm);
                            viewFilm.FactsFilm = HttpUtility.HtmlDecode(StripHtmlTags(viewFilm.FactsFilm));
                        } else
                        {
                            rTBFacts.Text = string.Empty;
                        }

                    //добавление стран
                    if (iFilm.Countries.Count() > 0)
                    {
                        foreach (var iCountry in iFilm.Countries)
                        {
                            viewFilm.CountryFilm += iCountry.Name + ", ";
                        }
                    }

                    if (viewFilm.CountryFilm != null)
                    {
                        if (viewFilm.CountryFilm.Length > 2)
                        {
                            viewFilm.CountryFilm = viewFilm.CountryFilm.Remove(viewFilm.CountryFilm.Length - 2, 2);
                        }
                    }

                    //this.DataContext = viewFilm;
                    tbInfoFilm.DataContext = viewFilm;
                    IPoster.DataContext = viewFilm;
                    lBActors.ItemsSource = listViewPersons;
                }

            }
        }

        private void bActorView_Click(object sender, object e)
        {
            WPersons myWPersons = new WPersons(-1);
            WMainFilm.Hide();
            myWPersons.ShowDialog();
            WMainFilm.Show();
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            IdFilm = lBFilms.Items.IndexOf(lBFilms.SelectedItem);
            if (IdFilm != -1)
            {
                IdFilm = (lBFilms.Items[IdFilm] as Model.CFilmsShortView.SVFilms).Id;

                using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
                {
                    var selectedFilm = db.Cinema.Where(q => q.Id == IdFilm).FirstOrDefault();
                    db.Cinema.Remove(selectedFilm);
                    db.SaveChanges();
                    ShowFilms();
                }

            }
        }

        private bool IsExistFilm(string pathToFilm)
        {
            bool _isExist = false;
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + Model.CinemaDB.filenameDB))
            {
                //var currentFilm = db.Cinema.Where(o => o.Id == myID);
                var cinemaWithPath = db.Cinema.Where(o => o.PathToCinema == pathToFilm);
                foreach (var iCinema in cinemaWithPath)
                {
                    // foreach (var item in person)
                    {
                        if (iCinema.PathToCinema == pathToFilm)
                        {
                            _isExist = true;
                            break;
                        }
                    }
                }
            }
            return _isExist;
        }

        private void bAddFilm_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Title = "Выберите фильм для добавления"
                // Set options here
            };
            // Добавить фильм
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (fileDialog.ShowDialog() == true)
            {
                string selectedFile = fileDialog.FileName;
                lDir.Content = "Добавить файл: " + selectedFile;
                if (!IsExistFilm(selectedFile))
                {
                    string stmp;
                    Model.CinemaDB.Poster posterFilm = new Model.CinemaDB.Poster();
                    using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
                    {
                        Cinema film = new Cinema();
                        try
                        {
                            var tfile = TagLib.File.Create(selectedFile);
                            FileInfo myFile = new FileInfo(selectedFile);
                            string title = tfile.Tag.Title;
                            TimeSpan duration = tfile.Properties.Duration;
                            /* tBVideoInfo.Text = "___TagLib__________" + Environment.NewLine;
                            tBVideoInfo.Text += "Ширина: " + tfile.Properties.VideoWidth.ToString() + Environment.NewLine;
                            tBVideoInfo.Text += "Высота: " + tfile.Properties.VideoHeight.ToString() + Environment.NewLine;
                            tBVideoInfo.Text += "Длительность: " + tfile.Properties.Duration.Hours.ToString() + ":" + tfile.Properties.Duration.Minutes.ToString() + ":"
                                                               + tfile.Properties.Duration.Seconds.ToString() + Environment.NewLine;
                            tBVideoInfo.Text += "Объем: " + ((decimal)myFile.Length / (1024 * 1024 * 1024)).ToString("F2") + " Гб" + Environment.NewLine;*/
                            film.PathToCinema = selectedFile;
                            film.WidthCinema = tfile.Properties.VideoWidth.ToString();
                            film.HighCinema = tfile.Properties.VideoHeight.ToString();
                            film.DurationCinema = duration;
                            film.CinemaSize = myFile.Length;

                            stmp = System.IO.Path.GetFileNameWithoutExtension(selectedFile);
                            if (stmp.LastIndexOf(".") != -1)
                            {
                                stmp = stmp.Remove(stmp.LastIndexOf("."));
                            }
                            if (stmp.LastIndexOf(".") != -1)
                            {
                                stmp = stmp.Remove(stmp.LastIndexOf("."));
                            }
                            film.Name = stmp;

                            try
                            {
                                posterFilm = new Model.CinemaDB.Poster();
                                posterFilm.Pic = tfile.Tag.Pictures[0].Data.Data;
                                film.Poster = posterFilm;
                            }
                            catch (Exception ex)
                            {

                            }
                            db.Cinema.Add(film);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    stopwatch.Stop();
                    TimeSpan ts = stopwatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                                                        ts.Minutes, ts.Seconds,
                                                        ts.Milliseconds / 10);

                    lDir.Content += Environment.NewLine + "Время добавления фильма: " + elapsedTime; //+ ts.Minutes.ToString("F2") + " мин.";

                    ShowFilms();
                } else
                {
                    lDir.Content = "Такой фильм уже есть";
                }
            }
        }

        private void tBSFilm_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
            /* using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
             {
                 List<Model.CFilmsShortView.SVFilms> listToViewFilms = new List<Model.CFilmsShortView.SVFilms>();
                 Model.CFilmsShortView.SVFilms viewFilm = new Model.CFilmsShortView.SVFilms();
                 foreach (var iFilm in db.Cinema.Include(q => q.Poster))
                 {
                     if (iFilm.Name.ToLower().Contains(tBSFilm.Text.ToLower()))
                     {
                         viewFilm = new Model.CFilmsShortView.SVFilms();
                         viewFilm.PathToFilm = iFilm.PathToCinema;
                         viewFilm.Id = iFilm.Id;
                         viewFilm.Name = iFilm.Name;
                         viewFilm.Year = iFilm.Year.ToString();
                         viewFilm.AlternativeName = iFilm.AlternativeName;

                         if (iFilm.Poster != null)
                         {
                             viewFilm.Poster = ConvertByteArrayToBitmapImage(iFilm.Poster.Pic);
                         }

                         // добавление жанров
                         var genreInFilm = db.Cinema.Where(q => q.Id == iFilm.Id).Select(o => o.Genres);
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
                         var personsInFilm = db.Cinema.Where(q => q.Id == iFilm.Id).Select(o => o.Persons);
                         foreach (var person in personsInFilm)
                         {
                             foreach (var p in person)
                             {
                                 if (p.EnProfession == "actor")
                                 {
                                     sActors += p.Name == "" ? p.EnName + ", " : p.Name + ", ";
                                 }

                             }
                         }
                         if (sActors.Length > 2)
                         {
                             sActors = sActors.Remove(sActors.Length - 2, 2);
                         }
                         viewFilm.Actors = sActors;

                         listToViewFilms.Add(viewFilm);
                     }
                 }

                     lBFilms.ItemsSource = listToViewFilms;

             }*/
        }

        private void cBBoevik_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBBoevik_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBFiction_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBFiction_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBHistory_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBHistory_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBWar_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBWar_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBDrama_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBDrama_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBComedy_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBComedy_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBBiography_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBBiography_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBAdult_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBAdult_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBMelodrama_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBMelodrama_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBVestern_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBVestern_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBDetective_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBDetective_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBCriminal_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBCriminal_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBTriller_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBTriller_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBHorror_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBHorror_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBSport_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBSport_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBFantasy_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBFantasy_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBFamaly_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBFamaly_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBFamily_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBFamily_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
        }

        private void cBoxSortFilm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortFilm = cBoxSortFilm.SelectedIndex;
            if (SortFilm == 0)
            {
                lBFilms.Items.SortDescriptions.Clear();
            }
            if (SortFilm == 1)
            {
                lBFilms.Items.SortDescriptions.Clear();
                lBFilms.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            }
            if (SortFilm == 2)
            {
                lBFilms.Items.SortDescriptions.Clear();
                lBFilms.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Descending));
            }
            if (SortFilm == 3)
            {
                lBFilms.Items.SortDescriptions.Clear();
                lBFilms.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Year", System.ComponentModel.ListSortDirection.Ascending));
            }
            if (SortFilm == 4)
            {
                lBFilms.Items.SortDescriptions.Clear();
                lBFilms.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Year", System.ComponentModel.ListSortDirection.Descending));
            }
            if (SortFilm == 5)
            {
                lBFilms.Items.SortDescriptions.Clear();
                lBFilms.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("dRIMDb", System.ComponentModel.ListSortDirection.Descending));
            }
            if (SortFilm == 6)
            {
                lBFilms.Items.SortDescriptions.Clear();
                lBFilms.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("dRKp", System.ComponentModel.ListSortDirection.Descending));
            }

            if (lBFilms.ItemsSource != null)
            {
                CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();
            }
        }


        private void mIClearDB_Click(object sender, RoutedEventArgs e)
        {
            ClearDB();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EditGenre_Click(object sender, RoutedEventArgs e)
        {
            IdFilm = lBFilms.Items.IndexOf(lBFilms.SelectedItem);
            if (IdFilm != -1)
            {
                IdFilm = (lBFilms.Items[IdFilm] as Model.CFilmsShortView.SVFilms).Id;
                WSetGenre wSetGenre = new WSetGenre(IdFilm);
                wSetGenre.ShowDialog();
                UpdateFilm(IdFilm);
                CollectionViewSource.GetDefaultView(lBFilms.ItemsSource).Refresh();

            }
        }

        private void lBActors_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int ind = lBActors.Items.IndexOf(lBActors.SelectedItem);
            if (ind != -1)
            {
                ind = (lBActors.Items[ind] as Model.CinemaDB.Person).Id;
                lDir.Content = "Актер c Id: " + ind.ToString();

                WPersons myWPersons = new WPersons(ind);
                WMainFilm.Hide();
                myWPersons.ShowDialog();
                WMainFilm.Show();
            }
        }

        private void mSetFileNameDB_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog()
            {
                Title = "Выберите местоположение и название файла базы данных"

                // Set options here
            };
            fileDialog.Filter = "DB(*.sqlite)|*.sqlite";

            if (fileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.FileNameDB = fileDialog.FileName;
                filenameDB = Properties.Settings.Default.FileNameDB;
            }
        }
    }
    }
    