using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace MediaInfoNET
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                if (!File.Exists(App.SettingsFile))
                {
                    WinForms.Application.SetHighDpiMode(WinForms.HighDpiMode.SystemAware);

                    using TaskDialog<string> td = new TaskDialog<string>();
                    td.MainInstruction = "Choose a settings directory.";
                    td.AddCommandLink("AppData");
                    td.AddCommandLink("Portable");
                    td.AddCommandLink("Custom");
                    td.AddCommandLink("Cancel");
                    td.Show();

                    if (string.IsNullOrEmpty(td.SelectedValue) || td.SelectedValue == "Cancel")
                        return;

                    switch (td.SelectedValue)
                    {
                        case "AppData":
                            App.SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                                + "\\" + AppHelp.ProductName + "\\Settings.xml";
                            break;
                        case "Portable":
                            App.SettingsFile = WinForms.Application.StartupPath + "\\Settings.xml";
                            break;
                        case "Custom":
                            using (WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog())
                            {
                                if (dialog.ShowDialog() == WinForms.DialogResult.OK)
                                    App.SettingsFile = dialog.SelectedPath + "\\Settings.xml";
                                else
                                    return;
                            }
                            break;
                    }
                }

                if (args.Length == 1 && (args[0] == "--install" || args[0] == "--uninstall"))
                    Setup(args[0] == "--install");
                else
                    App.Main();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppHelp.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        static void Setup(bool install)
        {
            string[] extensions = App.Settings.FileTypes.Split(" \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (install)
            {
                string exePath = Process.GetCurrentProcess().MainModule.FileName;

                foreach (string ext in extensions)
                {
                    string filekeyName = RegistryHelp.GetString(@"HKCR\." + ext, null);

                    if (filekeyName == "")
                    {
                        RegistryHelp.SetValue(@"HKCR\." + ext, null, ext + "file");
                        filekeyName = ext + "file";
                    }

                    RegistryHelp.SetValue(@"HKCR\" + filekeyName + @"\shell\MediaInfo.NET", null, "MediaInfo");
                    RegistryHelp.SetValue(@"HKCR\" + filekeyName + @"\shell\MediaInfo.NET\command", null, $"\"{exePath}\" \"%1\"");
                }

                MessageBox.Show("Install complete", AppHelp.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                foreach (string name in Registry.ClassesRoot.GetSubKeyNames())
                {
                    if (!name.StartsWith("."))
                        continue;

                    RegistryHelp.RemoveKey(@"HKCR\" + RegistryHelp.GetString(@"HKCR\" + name, null) + @"\shell\MediaInfo.NET");
                }

                MessageBox.Show("Uninstall complete", AppHelp.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}