using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
using static ClientQuickAccess.MainWindow;

namespace ClientQuickAccess
{
    /// <summary>
    /// Interaction logic for Alert.xaml
    /// </summary>
    public partial class AlertWindow : Window
    {
        public  string SelectedOption { get; set; } = string.Empty;


        public AlertWindow(string selectedOption)
        {
            InitializeComponent();
            SelectedOption = selectedOption;
            lbl_ClientName.Content = SelectedOption;
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


        internal async Task SendAlert()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-Token", MainWindow.ApiKey);
            NewAlertRequest alertRequest = new NewAlertRequest
            {
                ClientName = SelectedOption,
                Message = txb_Description.Text,
                AddedBy = txb_Name.Text
            };

            var json = JsonSerializer.Serialize(alertRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{ServerURL}/alert", content);

            if (response.IsSuccessStatusCode)
            {
                this.Close();
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Error Sending Report: {errorMessage}");
            }
        }

        private async void btn_SendAlert_Click(object sender, RoutedEventArgs e)
        {
            await SendAlert();
        }
    }
}
