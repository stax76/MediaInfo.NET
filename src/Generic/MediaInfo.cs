using System;
using System.Runtime.InteropServices;

public class MediaInfo : IDisposable
{
    IntPtr Handle;
    static bool Loaded;

    public MediaInfo(string path)
    {
        if (!Loaded)
        {
            if (LoadLibrary("MediaInfo.dll") == IntPtr.Zero)
                throw new Exception("Error LoadLibrary MediaInfo.dll");

            Loaded = true;
        }

        Handle = MediaInfo_New();

        if (Handle == IntPtr.Zero)
            throw new Exception("Error MediaInfo_New");

        if (MediaInfo_Open(Handle, path) == 0)
            throw new Exception("Error MediaInfo_Open");
    }

    public string GetInfo(MediaInfoStreamKind streamKind, int streamNumber, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, streamKind, streamNumber, parameter, MediaInfoInfoKind.Text, MediaInfoInfoKind.Name)) ?? "";
    }

    public string GetSummary(bool complete, bool rawView)
    {
        MediaInfo_Option(Handle, "Language", rawView ? "raw" : "");
        MediaInfo_Option(Handle, "Complete", complete ? "1" : "0");
        return Marshal.PtrToStringUni(MediaInfo_Inform(Handle, 0)) ?? "";
    }

    bool Disposed;

    public void Dispose()
    {
        if (!Disposed)
        {
            if (Handle != IntPtr.Zero)
            {
                MediaInfo_Close(Handle);
                MediaInfo_Delete(Handle);
            }

            Disposed = true;
        }
    }

    ~MediaInfo() { Dispose(); }

    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string path);

    [DllImport("MediaInfo.dll")]
    static extern IntPtr MediaInfo_New();

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern int MediaInfo_Open(IntPtr handle, string path);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr MediaInfo_Option(IntPtr handle, string option, string value);

    [DllImport("MediaInfo.dll")]
    static extern IntPtr MediaInfo_Inform(IntPtr handle, int reserved);

    [DllImport("MediaInfo.dll")]
    static extern int MediaInfo_Close(IntPtr handle);

    [DllImport("MediaInfo.dll")]
    static extern void MediaInfo_Delete(IntPtr handle);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr MediaInfo_Get(IntPtr handle,
                                       MediaInfoStreamKind streamKind,
                                       int streamNumber,
                                       string parameter,
                                       MediaInfoInfoKind kindOfInfo,
                                       MediaInfoInfoKind kindOfSearch);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern int MediaInfo_Count_Get(IntPtr handle,
                                          MediaInfoStreamKind streamKind,
                                          int streamNumber);
}

public enum MediaInfoStreamKind
{
    General,
    Video,
    Audio,
    Text,
    Other,
    Image,
    Menu,
    Max,
}

public enum MediaInfoInfoKind
{
    Name,
    Text,
    Measure,
    Options,
    NameText,
    MeasureText,
    Info,
    HowTo
}