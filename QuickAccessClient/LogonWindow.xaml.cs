using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuickAccessClient.Classes;
using QuickAccessClient.Config;
using static QuickAccessClient.MainWindow;
using QuickAccessClient.Properties;

namespace QuickAccessClient
{
    /// <summary>
    /// Interaction logic for LogonWindow.xaml
    /// </summary>
    public partial class LogonWindow : Window
    {

        public static bool RememberUsername { get; set; } = false;

        public LogonWindow()
        {
            InitializeComponent();
            if (Settings.Default.RememberUsername)
            {
                txb_Username.Text = Settings.Default.SavedUsername; ;
                RememberUsername = Settings.Default.RememberUsername;
                ckb_RememberUsername.IsChecked = true;

            }
            else
            {
                txb_Username.Text = string.Empty;
                RememberUsername = false;
                ckb_RememberUsername.IsChecked = false;
            }
        }

        private async void btn_Logon_Click(object sender, RoutedEventArgs e)
        {


            if (string.IsNullOrWhiteSpace(txb_Username.Text) || string.IsNullOrWhiteSpace(txb_Password.Password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            
            HttpClient client = new HttpClient();
            string hashedUsername = Config.Config.HashString(txb_Username.Text.ToLower());
            string hashedPassword = Config.Config.HashString(txb_Password.Password);

            string apiKey = Config.Config.HashString($"{txb_Username.Text.ToLower().Trim()}|{txb_Password.Password}");

            client.DefaultRequestHeaders.Add("X-API-Token", apiKey);

            NewLogonRequest logonRequest = new NewLogonRequest
            {
                HashedUsername = hashedUsername,
                HashedPassword = hashedPassword,

            };

            var json = JsonSerializer.Serialize(logonRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{ServerURL}/logon", content);

            if (response.IsSuccessStatusCode)
            {

                var jsonResults = await response.Content.ReadAsStringAsync();
                var clientList = JsonSerializer.Deserialize<List<Client>>(jsonResults);
                MainWindow.ApiKey = apiKey;
                Clients.Clear();
                MyOptions.Clear();

                foreach (Client item in clientList)
                {
                    Clients[item.ClientName] = item;
                }

                var sortedDict = Clients.OrderBy(pair => pair.Key)
                                        .ToDictionary(pair => pair.Key, pair => pair.Value);

                foreach (var item in sortedDict)
                {
                    MyOptions.Add(item.Key);
                }

                MainWindow.LoggedIn = true;
                this.Owner.Show();
                this.Close();
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Error Sending Report: {errorMessage}");
            }

          
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void txb_Password_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter))
            {

                btn_Logon_Click(sender, e);
            }
        }

        private void ckb_RememberUsername_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"RememberUsername checkbox clicked. IsChecked: {ckb_RememberUsername.IsChecked} ");
            if (ckb_RememberUsername.IsChecked==true)
            {
                RememberUsername = true;
                Settings.Default.SavedUsername = txb_Username.Text;
                Settings.Default.RememberUsername = true;
                Console.WriteLine($"Is Checked. ");
            }
            else
            {
                RememberUsername = false;
                Settings.Default.SavedUsername = string.Empty;
                Settings.Default.RememberUsername = false;
                Console.WriteLine($"Not Checked. ");
            }
            Settings.Default.Save();
        }


    }
}
