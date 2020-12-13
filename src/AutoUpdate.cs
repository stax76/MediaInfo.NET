
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MediaInfoNET
{
    class AutoUpdate
    {
        public static event Action Updating;

        public static void Check()
        {
            if (App.Settings.CheckForUpdates && RegistryHelp.GetInt(RegistryHelp.ApplicationKey, "UpdateCheckLast")
                != DateTime.Now.DayOfYear)

                CheckOnline();
        }

        public static async void CheckOnline(bool showUpToDateMessage = false)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    RegistryHelp.SetValue(RegistryHelp.ApplicationKey, "UpdateCheckLast", DateTime.Now.DayOfYear);
                    client.DefaultRequestHeaders.Add("User-Agent", "MediaInfo.NET");
                    var response = await client.GetAsync("https://api.github.com/repos/stax76/MediaInfo.NET/releases/latest");
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();
                    Match match = Regex.Match(content, @"""MediaInfo\.NET-([\d\.]+)\.zip""");
                    Version onlineVersion = Version.Parse(match.Groups[1].Value);
                    Version currentVersion = Assembly.GetEntryAssembly()?.GetName().Version;

                    if (onlineVersion <= currentVersion)
                    {
                        if (showUpToDateMessage)
                            Msg.Show($"{Application.ProductName} is up to date.");

                        return;
                    }

                    if ((RegistryHelp.GetString(RegistryHelp.ApplicationKey, "UpdateCheckVersion")
                        != onlineVersion.ToString() || showUpToDateMessage) && Msg.ShowQuestion(
                            $"New version {onlineVersion} is available, update now?") == MsgResult.OK)
                    {
                        string url = $"https://github.com/stax76/MediaInfo.NET/releases/download/{onlineVersion}/MediaInfo.NET-{onlineVersion}.zip";

                        using (Process proc = new Process())
                        {
                            proc.StartInfo.UseShellExecute = true;
                            proc.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            proc.StartInfo.FileName = "powershell.exe";
                            proc.StartInfo.Arguments = $"-NoLogo -NoExit -NoProfile -ExecutionPolicy Bypass -File \"{Application.StartupPath + @"\Update.ps1"}\" \"{url}\" \"{Application.StartupPath.TrimEnd('\\')}\"";

                            if (Application.StartupPath.Contains("Program Files"))
                                proc.StartInfo.Verb = "runas";

                            proc.Start();
                        }

                        Updating?.Invoke();
                    }

                    RegistryHelp.SetValue(RegistryHelp.ApplicationKey, "UpdateCheckVersion", onlineVersion.ToString());
                }
            }
            catch (Exception ex)
            {
                if (showUpToDateMessage)
                    Msg.ShowException(ex);
            }
        }
    }
}
