using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaInfoNET
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public TextBox TextBox {
            get => MainTextBox;
            set => MainTextBox = value;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}