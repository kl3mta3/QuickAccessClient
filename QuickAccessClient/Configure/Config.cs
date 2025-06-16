
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using QuickAccessClient;



namespace QuickAccessClient.Config
{
    public class Config
    {
       // internal static string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "client.config");
        internal static string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        internal static bool useLogin= false;
      
        internal static string Username { get; set; }
        internal static byte[] AdminPassword { get; set; }

        public static async Task ConfigureApplication()
        {

                await LoadConfigFile();


            if (!File.Exists(LogFilePath))
            { 
                File.Create(LogFilePath).Close();
            }
        }

        private static async Task LoadConfigFile()
        {
            //string json = await File.ReadAllTextAsync(ConfigFilePath);

            //if (string.IsNullOrWhiteSpace(json))
            //    throw new InvalidOperationException("Config file is empty.");

            //DeviceConfig config = JsonSerializer.Deserialize<DeviceConfig>(json);

            //if (config == null || string.IsNullOrWhiteSpace(config.ServerURL))
            //    throw new InvalidOperationException("Failed to load config. ServerURL missing.");

            MainWindow.ServerURL = Properties.Settings.Default.ServerUrl;
        }

        public static string HashPassword( string password)
        {
            string hashedPass = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(password)));
            return hashedPass;
        }

        public static string HashString(string _string)
        {
            try
            {
                string hashedString = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(_string)));

                return hashedString;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    public class DeviceConfig
    {
        public string ServerURL { get; set; }
        public int ServerPort { get; set; }
        public bool UseLogin { get; set; }
    }
}
