using Microsoft.Win32;
using System;
using System.IO;
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
                WinForms.Application.SetHighDpiMode(WinForms.HighDpiMode.PerMonitorV2);
                Msg.SupportURL = "https://github.com/stax76/MediaInfo.NET/issues";

                if (!File.Exists(App.SettingsFile))
                {

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
                Msg.ShowException(ex);
            }
        }

        static void Setup(bool install)
        {
            string[] extensions = App.Settings.FileTypes.Split(" \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (install)
            {
                foreach (string ext in extensions)
                {
                    string filekeyName = RegistryHelp.GetString(@"HKCR\." + ext, null);

                    if (filekeyName == "")
                    {
                        RegistryHelp.SetValue(@"HKCR\." + ext, null, ext + "file");
                        filekeyName = ext + "file";
                    }

                    RegistryHelp.SetValue(@"HKCR\" + filekeyName + @"\shell\MediaInfo.NET", null, "MediaInfo");
                    RegistryHelp.SetValue(@"HKCR\" + filekeyName + @"\shell\MediaInfo.NET\command", null, $"\"{AppHelp.ExecutablePath}\" \"%1\"");

                    string userKeyName = RegistryHelp.GetString(@$"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.{ext}\UserChoice", "ProgId");
                    
                    if (RegistryHelp.GetString(@"HKCR\" + userKeyName + @"\shell\open\command", null) != "")
                    {
                        RegistryHelp.SetValue(@"HKCR\" + userKeyName + @"\shell\MediaInfo.NET", null, "MediaInfo");
                        RegistryHelp.SetValue(@"HKCR\" + userKeyName + @"\shell\MediaInfo.NET\command", null, $"\"{AppHelp.ExecutablePath}\" \"%1\"");
                    }
                }

                Msg.Show("Install complete");
            }
            else
            {
                foreach (string name in Registry.ClassesRoot.GetSubKeyNames())
                {
                    if (!name.StartsWith("."))
                        continue;

                    RegistryHelp.RemoveKey(@"HKCR\" + RegistryHelp.GetString(@"HKCR\" + name, null) + @"\shell\MediaInfo.NET");
                }

                using RegistryKey fileExtsKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts");

                if (fileExtsKey != null)
                {
                    foreach (string name in fileExtsKey.GetSubKeyNames())
                    {
                        if (!name.StartsWith("."))
                            continue;

                        string userKeyName = RegistryHelp.GetString(@$"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{name}\UserChoice", "ProgId");
                        RegistryHelp.RemoveKey(@"HKCR\" + userKeyName + @"\shell\MediaInfo.NET");
                    }
                }

                Msg.Show("Uninstall complete");
            }
        }
    }
}