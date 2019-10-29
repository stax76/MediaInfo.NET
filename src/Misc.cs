using System;

[Serializable()]
public class AppSettings
{
    public bool RawView { get; set; }
    public bool CompactSummary { get; set; } = true;
    public bool WordWrap { get; set; }
    public bool CenterScreen { get; set; } = true;
    public bool FormatEncoded { get; set; }
    
    public int WindowWidth { get; set; } = 750;
    public int WindowHeight { get; set; } = 550;
    public int ColumnPadding { get; set; } = 32;

    public double FontSize { get; set; } = 14;
    
    public string FontName { get; set; } = "Consolas";
    public string Exclude { get; set; } = "";
    public string Theme { get; set; } = "Light";
    public string FileTypes { get; set; } = "avi mp4 mkv mov webm mpg vob ts m2ts wmv flv mp2 mp3 mpa ogg opus mka dts dtshd ac3 eac3 thd wav flac sup ass png jpg gif bmp";

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
        Background = "#1E1E1E",
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