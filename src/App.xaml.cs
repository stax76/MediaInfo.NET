using System.Windows;
using System.Windows.Threading;

namespace MediaInfoNET
{
    public partial class App : Application
    {
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