using System.Reflection;
using System.Linq;
using Microsoft.Win32;

class AppHelp
{
    public static string ProductName { get; } = (Assembly.GetEntryAssembly()?.GetCustomAttributes(
        typeof(AssemblyProductAttribute)).First() as AssemblyProductAttribute)?.Product ?? "";

    public static bool IsDarkTheme {
        get {
            object value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);

            if (value is null)
                value = 1;

            return (int)value == 0;
        }
    }
}