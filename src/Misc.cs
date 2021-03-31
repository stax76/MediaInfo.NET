
using System;

[Serializable()]
public class AppSettings
{
    public bool RawView { get; set; }
    public bool CompactSummary { get; set; } = true;
    public bool WordWrap { get; set; }
    public bool CenterScreen { get; set; } = true;
    public bool FormatEncoded { get; set; }
    public bool CheckForUpdates { get; set; }

    public int WindowWidth { get; set; } = 750;
    public int WindowHeight { get; set; } = 550;
    public int ColumnPadding { get; set; } = 32;

    public double FontSize { get; set; } = 13;
    
    public string FontName { get; set; } = "Consolas";
    public string Exclude { get; set; } = "";
    public string Theme { get; set; } = "Light";
    public string FileTypes { get; set; } = "264 265 3gp aac ac3 apng ass avc avi bmp divx dts dtshd dtshr dtsma eac3 ec3 evo flac flv gif h264 h265 hevc hvc idx ivf jpe jpeg jpg m2t m2ts m2v m4a m4v mka mkv mlp mov mp2 mp3 mp4 mpa mpeg mpg mpv mts ogg ogm opus pcm png psb psd pva raw rmvb smi srt ssa ssf sup thd thd+ac3 tif truehd ts ttxt usf vdr vob w64 wav webm wmv y4m";

    public Theme LightTheme { get; set; } = new Theme() {
        Foreground = "#252525",
        Background = "#F0F0F0",
        Border = "Gray",
        TextSelection = "#535E66",
        ItemSelection = "#A7BCCC",
        ItemHover = "#B1C8D8",
        Highlight = "Gold"
    };

    public Theme DarkTheme { get; set; } = new Theme() {
        Foreground = "#E6E6E6",
        Background = "#2D2D30",
        Border = "Gray",
        TextSelection = "AliceBlue",
        ItemSelection = "DimGray",
        ItemHover = "Gray",
        Highlight = "Green"
    };
}

[Serializable()]
public class Theme
{
    public string Foreground { get; set; } = "";
    public string Background { get; set; } = "";
    public string Border { get; set; } = "";
    public string TextSelection { get; set; } = "";
    public string ItemSelection { get; set; } = "";
    public string ItemHover { get; set; } = "";
    public string Highlight { get; set; } = "";
}

public class MediaInfoParameter
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public string Group { get; set; } = "";
    public bool IsComplete { get; set; }
}