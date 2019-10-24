using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace MediaInfoNET
{
    public partial class App : Application
    {
        static AppSettings? _Settings;

        public static AppSettings Settings {
            get {
                if (_Settings == null)
                {
                    if (File.Exists(App.SettingsFile))
                        _Settings = XmlSerializerHelp.LoadXML<AppSettings>(App.SettingsFile);
                    else
                    {
                        _Settings = new AppSettings();
                        SaveSettings();
                    }
                }

                return _Settings;
            }
        }

        public static string SettingsFile {
            get => RegistryHelp.GetString(RegistryHelp.ApplicationKey, "SettingsFile");
            set => RegistryHelp.SetValue(RegistryHelp.ApplicationKey, "SettingsFile", value);
        }

        public static void SaveSettings()
        {
            if (!Directory.Exists(Path.GetDirectoryName(App.SettingsFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(App.SettingsFile));

            XmlSerializerHelp.SaveXML(App.SettingsFile, Settings);
        }

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}