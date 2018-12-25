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

using BakalariDesktop.Login;
using BakalariDesktop.Properties;
using System.Threading;

namespace BakalariDesktop
{
    /// <summary>
    /// Interakční logika pro LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoginPersistence();
        }

        void LoginPersistence()
        {
            string date = Settings.Default.date;
            string token = Settings.Default.token;

            string currentDate = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            if (date == currentDate)
            {
                // token je stále validní
                this.Hide();
                Dashboard dashboard = new Dashboard();
                dashboard.Show();
                this.Close();
            }
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            loadingDialog.IsOpen = true;

            string username = usernameBox.Text;
            string password = passwordBox.Password;
            string url = urlBox.Text;

            await PerformLogin(username, password, url);

            loadingDialog.IsOpen = false;
        }

        private async Task PerformLogin(string username, string password, string url)
        {
            string token = null;

            await Task.Run(() =>
            {
                
            LoginManager loginMGR = new LoginManager();

            token = loginMGR.GenerateToken(username, password, url);

                Thread.Sleep(100);
            });
            if (token == null)
            {
                errorDialog.IsOpen = true;
            }
            else
            {
                // úspěšné přihlášení

                Settings.Default["token"] = token;
                Settings.Default["date"] = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                Settings.Default["url"] = url;
                Settings.Default.Save(); // uložení tokenu a data pro persistenci

                this.Hide();
                Dashboard dashboard = new Dashboard();
                dashboard.Show();
            }

                

        }

        private void usernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void closeErrorDialog(object sender, RoutedEventArgs e)
        {
            errorDialog.IsOpen = false;
        }
    }
}
