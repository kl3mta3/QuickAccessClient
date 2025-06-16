using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickAccessClient.Config;
using QuickAccessClient.Classes;

namespace QuickAccessClient
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static ObservableCollection<string> MyOptions { get; set; } = new();
        public static string ServerURL { get; set; }
        public static string ApiKey { get; set; }

        public static Dictionary<string, Client> Clients { get; set; } = new Dictionary<string, Client>();

        private string _selectedOption;
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (_selectedOption != value)
                {
                    _selectedOption = value;
                    OnPropertyChanged();
                }
            }
        }
        public static bool LoggedIn { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Config.Config.ConfigureApplication();

                if(Config.Config.useLogin)
                {
                    LogonWindow loginWindow = new LogonWindow();
                    loginWindow.Owner = this;
                    loginWindow.Show();
                    this.Hide();
                    while (!LoggedIn)
                    {
                        await Task.Delay(100);
                    }
                }
                else
                {
                    await UpdateComboBox();
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to load client list.\n\n{ex.Message}", "Contact your STM", MessageBoxButton.OK, MessageBoxImage.Warning);

                MyOptions.Clear();

            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_Kba_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectedOption))
            {
                MessageBox.Show("Please select a client first.");
                return;
            }
            OpenSelectedClientInBrowser(SelectedOption);
        }

        private void btn_Rdp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectedOption))
            {
                MessageBox.Show("Please select a client first.");
                return;
            }
            OpenRemoteLocation(SelectedOption);
        }

        private void btn_Alert_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(SelectedOption))
            {
                MessageBox.Show("Please select a client first.");
                return;
            }
            var alertWindow = new AlertWindow(SelectedOption);
            alertWindow.SelectedOption = SelectedOption;
            alertWindow.Owner = this;
            alertWindow.Show();
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {

            await UpdateComboBox();


        }

        internal static async Task UpdateComboBox()
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("X-API-Token", MainWindow.ApiKey);
            var response = await client.PostAsync($"{ServerURL}/update", null);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var clientList = JsonSerializer.Deserialize<List<Client>>(json);

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
            }
            else
            {

                MyOptions.Add("Failed to Load Clients");
            }
        }

        public static void OpenSelectedClientInBrowser(string selectedClientName)
        {
            if (Clients.TryGetValue(selectedClientName, out Client client) &&
                !string.IsNullOrWhiteSpace(client.KbaUrl))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = client.KbaUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error launching browser: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Client not found or URL is empty.");
            }
        }

        public static void OpenRemoteLocation(string selectedClientName)
        {
            if (!Clients.TryGetValue(selectedClientName, out Client client) ||
                string.IsNullOrWhiteSpace(client.RemoteLocation))
            {
                MessageBox.Show("No remote location specified for this client.");
                return;
            }

            try
            {
                var raw = client.RemoteLocation.Trim();

                var parts = raw.Split(' ', 2);
                var exe = parts[0];
                var args = parts.Length > 1 ? parts[1] : "";

                if (exe.Contains("chrome", StringComparison.OrdinalIgnoreCase) ||
                    exe.Contains("edge", StringComparison.OrdinalIgnoreCase))
                {
                    args = " --incognito " + args;
                }

                if (!File.Exists(exe))
                {
                    MessageBox.Show($"Executable not found: {exe}");
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = args,
                    UseShellExecute = true
                });

                Console.WriteLine($"Opening remote location: {exe} {args}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening remote location:\n{ex.Message}");
            }
        }

        public class NewAlertRequest
        {
            [JsonPropertyName("clientName")]
            public string ClientName { get; set; }

            [JsonPropertyName("addedBy")]
            public string AddedBy { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }
        }


    }
}