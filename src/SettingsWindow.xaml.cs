
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using System.IO;

namespace MediaInfoNET
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = App.Settings;

            var fixedWidthFontNames = Fonts.SystemTypefaces
                .GroupBy(x => x.FontFamily.ToString())
                .Select(grp => grp.First())
                .Where(x => new FormattedText("Hl", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, x, 10, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip).Width == new FormattedText("HH", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, x, 10, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip).Width)
                .Select(x => x.FontFamily.ToString()).ToList();

            foreach (string name in new[] { "Segoe MDL2 Assets", "Webdings", "HoloLens MDL2 Assets", "MS Outlook" })
                if (fixedWidthFontNames.Contains(name))
                    fixedWidthFontNames.Remove(name);

            FontComboBox.ItemsSource = fixedWidthFontNames;
        }

        private void SettingsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    e.Handled = true;
                    break;
            }
        }

        private void OpenSettingsFolderHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo() {
                UseShellExecute = true,
                FileName = Path.GetDirectoryName(App.SettingsFile)
            });
        }
    }
}