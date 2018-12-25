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
using BakalariDesktop.Moduly;
using BakalariDesktop.Properties;
using BakalariDesktop.Moduly.Tridy;
using System.IO;
using System.Windows.Markup;
using MaterialDesignThemes.Wpf;
using System.Threading;
using System.Globalization;
using System.Windows.Threading;

namespace BakalariDesktop
{
    /// <summary>
    /// Interakční logika pro Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private string url;
        private string token;

        public Dashboard()
        {
            InitializeComponent();
            InitDashboard();
        }

        private async void InitDashboard()
        {
            loadingDialog.IsOpen = true;

            url = Settings.Default.url;
            token = Settings.Default.token;



            SetUserInfo();
            await ShowNewGrades();
            await AddSubjectsToGrades();

            loadingDialog.IsOpen = false;

            var myMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            Snackbar.MessageQueue = myMessageQueue;

            Snackbar.MessageQueue.Enqueue("Úspěšné přihlášení!");
        }

        /// <summary>
        /// Zjistí nejnovější známky a zobrazí je na Dashboardu.
        /// </summary>
        private async Task ShowNewGrades()
        {
            Znamky znamky = new Znamky();
            Znamka[] noveZnamky = null;

            await Task.Run(() =>
            {
                noveZnamky = znamky.GetNewGrades();
            });

            int counter = 0;
            TextBlock[] cards = new TextBlock[5];
            cards[0] = grade1;
            cards[1] = grade2;
            cards[2] = grade3;
            cards[3] = grade4;
            cards[4] = grade5;
            foreach (Znamka znamka in noveZnamky)
            {
                if (counter > 4)
                {
                    break;
                }
                Run run = new Run(znamka.Hodnoceni + " ");
                run.FontSize = 24;
                run.FontWeight = FontWeights.SemiBold;
                cards[counter].Inlines.Add(run);
                run = new Run(znamka.Predmet + " ");
                run.FontSize = 16;
                cards[counter].Inlines.Add(run);
                string datum = znamka.Datum.ToString("dd.MM", System.Globalization.CultureInfo.InvariantCulture);
                run = new Run(datum);
                run.FontSize = 14;
                cards[counter].Inlines.Add(run);

                counter++;
            }
        }

        /// <summary>
        /// Přidá všechny předměty do známek.
        /// </summary>
        /// 
        private async Task AddSubjectsToGrades()
        {



            dynamic response = null;

                Networking.Networking networking = new Networking.Networking();
                response = networking.ParseXMLFromURL(url + "/login.aspx?hx=" + token + "&pm=predmety");

            if (response == null)
            {
                Snackbar.MessageQueue.Enqueue("Známky nešly zobrazit.");
                return;
            }

            int counter = 0;
            foreach (dynamic predmet in response.results.predmety.predmet)
            {
                counter++;


                Znamka[] posledniZnamky = null;

                Znamky znamky = new Znamky();
                string prumerString = null;
                await Task.Run(() =>
                {
                    prumerString = znamky.GetGradeAverage(predmet.nazev).ToString("0.00", CultureInfo.GetCultureInfo("cs-CZ"));
                    posledniZnamky = znamky.GetGradesBySubject(predmet.nazev, 1);

                if (posledniZnamky == null)
                {
                    Snackbar.MessageQueue.Enqueue("Známky nešly zobrazit.");
                    return;
                }

                });




                StackPanel stackPanel = new StackPanel();
                stackPanel.Margin = new Thickness(0, 0, 0, 25);
                stackPanel.Orientation = Orientation.Vertical;

                Card predmetCard = new Card();
                predmetCard.Background = Brushes.White;
                predmetCard.Foreground = Brushes.White;
                predmetCard.Padding = new Thickness(8);
                predmetCard.Height = 84;

                Grid grid = new Grid();

                StackPanel gradePanel = new StackPanel();
                gradePanel.Orientation = Orientation.Vertical;
                gradePanel.Margin = new Thickness(0, 0, 300, 0);

                TextBlock predmetNazev = new TextBlock();
                predmetNazev.Text = predmet.nazev;
                predmetNazev.Margin = new Thickness(7);
                predmetNazev.HorizontalAlignment = HorizontalAlignment.Left;
                predmetNazev.Height = 29;
                predmetNazev.FontSize = 24;
                predmetNazev.Foreground = Brushes.Black;
                predmetNazev.VerticalAlignment = VerticalAlignment.Top;


                TextBlock posledniZnamka = new TextBlock();
                posledniZnamka.Margin = new Thickness(7, 0, 0, 0);
                posledniZnamka.HorizontalAlignment = HorizontalAlignment.Left;
                posledniZnamka.Height = 35;
                posledniZnamka.FontSize = 18;
                posledniZnamka.Foreground = Brushes.Black;
                posledniZnamka.VerticalAlignment = VerticalAlignment.Bottom;



                Run lastGrade = new Run();
                lastGrade.Text = "Poslední známka: ";

                Run tema = new Run();
                tema.FontWeight = FontWeights.Bold;
                tema.Text = posledniZnamky[0].Tema;

                Run runZnamka = new Run();
                runZnamka.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336"));
                runZnamka.FontWeight = FontWeights.Bold;
                runZnamka.FontSize = 22;
                runZnamka.Text = " " + posledniZnamky[0].Hodnoceni;

                TextBlock prumer = new TextBlock();
                prumer.Text = prumerString;
                prumer.Margin = new Thickness(7);
                prumer.HorizontalAlignment = HorizontalAlignment.Right;
                prumer.FontSize = 28;
                prumer.Height = 34;
                prumer.Foreground = Brushes.Black;
                prumer.VerticalAlignment = VerticalAlignment.Center;

                posledniZnamka.Inlines.Add(lastGrade);
                posledniZnamka.Inlines.Add(tema);
                posledniZnamka.Inlines.Add(runZnamka);

                gradePanel.Children.Add(predmetNazev);
                gradePanel.Children.Add(posledniZnamka);

                grid.Children.Add(gradePanel);
                grid.Children.Add(prumer);

                predmetCard.Content = grid;

                stackPanel.Children.Add(predmetCard);

                predmety.Children.Add(stackPanel); // KONEC

                
            }
        }

        /// <summary>
        /// Získá info uživatele a zobrazí ho v dashboardu.
        /// </summary>
        private void SetUserInfo()
        {
            Networking.Networking networking = new Networking.Networking();
            dynamic response = networking.ParseXMLFromURL(url + "/login.aspx?hx=" + token + "&pm=login");

            if (response == null)
            {
                Console.Out.WriteLine("RESPONSE JE NULL");
            }

            jmenoLabel.Text = response.results.jmeno;


        }

        private void logOutBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default["token"] = null;
            Settings.Default["date"] = null;
            Settings.Default["url"] = null;
            Settings.Default.Save();

            LoginWindow loginWindow = new LoginWindow();
            this.Hide();
            loginWindow.Show();
            this.Close();
        }
    }
}
