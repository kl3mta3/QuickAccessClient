
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using ClientQuickAccess;



namespace ClientQuickAccess.Config
{
    public class Config
    {
        internal static string ConfigFilePath = "app.config";
        internal static string LogFilePath = "log.txt";

        internal static string Username { get; set; }
        internal static byte[] AdminPassword { get; set; }

        public static async Task ConfigureApplication()
        {
            if (!File.Exists(ConfigFilePath))
            {
                File.Create(ConfigFilePath).Close();
            }
            else
            {
                await LoadConfigFile();
            }

            if (!File.Exists(LogFilePath))
            { 
                File.Create(LogFilePath).Close();
            }
        }

        private static async Task LoadConfigFile()
        {
            string json = await File.ReadAllTextAsync(ConfigFilePath);

            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("Config file is empty.");

            DeviceConfig config = JsonSerializer.Deserialize<DeviceConfig>(json);

            if (config == null || string.IsNullOrWhiteSpace(config.ServerURL))
                throw new InvalidOperationException("Failed to load config. ServerURL missing.");

            MainWindow.ServerURL = config.ServerURL;
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
    }
}
