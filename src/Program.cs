using System;
using System.Diagnostics;
using System.Windows;
using MediaInfoNET;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && (args[0] == "--install" || args[0] == "--uninstall"))
                Setup(args[0] == "--install");
            else
                new App().Run(new MainWindow());
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    static void Setup(bool install)
    {
        string[] extensions = (
            "mpg avi vob mp4 mkv 264 mov wmv flv h264 asf webm mpeg mpv avc hevc 265 h265 m2v m2ts mts webm ts m4v " +
            "mp2 mp3 ac3 wav w64 m4a dts dtsma dtshr dtshd eac3 thd ogg mka aac opus flac mpa " +
            "sub sup idx ass aas srt " +
            "png jpg jpeg gif bmp").Split(' ');

        if (install)
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;

            foreach (string ext in extensions)
            {
                string filekeyName = RegHelp.GetString(@"HKCR\." + ext, null);

                if (filekeyName == "")
                {
                    RegHelp.SetValue(@"HKCR\." + ext, null, ext + "file");
                    filekeyName = ext + "file";
                }

                RegHelp.SetValue(@"HKCR\" + filekeyName + @"\shell\MediaInfo.NET", null, "MediaInfo");
                RegHelp.SetValue(@"HKCR\" + filekeyName + @"\shell\MediaInfo.NET\command", null, $"\"{exePath}\" \"%1\"");
            }

            MessageBox.Show("Install complete", "Install", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            foreach (string ext in extensions)
                RegHelp.RemoveKey(@"HKCR\" + RegHelp.GetString(@"HKCR\." + ext, null) + @"\shell\MediaInfo.NET");

            MessageBox.Show("Uninstall complete", "Uninstall", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}