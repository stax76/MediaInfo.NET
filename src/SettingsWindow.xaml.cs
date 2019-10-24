using System.Windows;
using System.Windows.Input;
using WinForms = System.Windows.Forms;

namespace MediaInfoNET
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = App.Settings;
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

        private void FontButton_Click(object sender, RoutedEventArgs e)
        {
            using WinForms.FontDialog dialog = new WinForms.FontDialog();
            dialog.FixedPitchOnly = true;
            dialog.Font = new System.Drawing.Font(FontTextBox.Text, float.Parse(FontSizeTextBox.Text));

            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                App.Settings.FontName = dialog.Font.FontFamily.Name;
                App.Settings.FontSize = dialog.Font.Size;
                FontTextBox.Text = dialog.Font.FontFamily.Name;
                FontSizeTextBox.Text = dialog.Font.Size.ToString();
            }
        }
    }
}