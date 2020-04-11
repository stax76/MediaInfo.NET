
using Microsoft.Win32;
using System;

public class RegistryHelp
{
    public static string ApplicationKey { get; } = @"HKCU\Software\" + AppHelp.ProductName;

    public static void SetValue(string path, string name, object value)
    {
        using (RegistryKey regKey = GetRootKey(path).CreateSubKey(path.Substring(5), RegistryKeyPermissionCheck.ReadWriteSubTree))
            regKey?.SetValue(name, value);
    }

    public static string GetString(string path, string name, string defaultValue = "")
    {
        object value = GetValue(path, name, defaultValue);
        return value is string ? (string)value : defaultValue;
    }

    public static bool GetBool(string path, string name, bool defaultValue = false)
    {
        object val = GetValue(path, name, defaultValue);
        return val is bool ? (bool)val : defaultValue;
    }

    public static int GetInt(string path, string name, int defaultValue = 0)
    {
        object val = GetValue(path, name, defaultValue);
        return val is int ? (int)val : defaultValue;
    }

    public static object GetValue(string path, string name, object defaultValue = null)
    {
        using (RegistryKey regKey = GetRootKey(path).OpenSubKey(path.Substring(5)))
            return regKey == null ? defaultValue : regKey.GetValue(name, defaultValue);
    }

    public static void RemoveKey(string path)
    {
        GetRootKey(path).DeleteSubKeyTree(path.Substring(5), false);
    }

    public static void RemoveValue(string path, string name)
    {
        using (RegistryKey regKey = GetRootKey(path).OpenSubKey(path.Substring(5), true))
            regKey?.DeleteValue(name, false);
    }

    static RegistryKey GetRootKey(string path)
    {
        switch (path.Substring(0, 4))
        {
            case "HKLM": return Registry.LocalMachine;
            case "HKCU": return Registry.CurrentUser;
            case "HKCR": return Registry.ClassesRoot;
            default: throw new Exception("unknown reg root");
        }
    }
}
