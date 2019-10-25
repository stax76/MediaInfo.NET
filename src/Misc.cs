using System;

[Serializable()]
public class AppSettings
{
    public bool RawView { get; set; } = true;
    public bool ShowCompactSummary { get; set; } = true;
    public bool WordWrap { get; set; }
    public bool CenterScreen { get; set; } = true;
    public bool FormatEncoded { get; set; } = true;
    
    public double FontSize { get; set; } = 14;
    public int WindowWidth { get; set; } = 750;
    public int WindowHeight { get; set; } = 550;
    public int ColumnPadding { get; set; } = 30;
    
    public string FontName { get; set; } = "Consolas";
    public string Exclude { get; set; } = "";
    public string FileTypes { get; set; } = "avi mp4 mkv mov webm mpg vob ts m2ts wmv flv mp2 mp3 mpa ogg opus mka dts dtshd ac3 eac3 thd wav flac sup ass png jpg gif bmp";
}

public class MediaInfoParameter
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public string Group { get; set; } = "";
    public bool IsComplete { get; set; }
}