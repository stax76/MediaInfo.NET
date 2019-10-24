using System.Reflection;
using System.Linq;

class AppHelp
{
    public static string ProductName { get; } = (Assembly.GetEntryAssembly()?.GetCustomAttributes(
        typeof(AssemblyProductAttribute)).First() as AssemblyProductAttribute)?.Product ?? "";
}