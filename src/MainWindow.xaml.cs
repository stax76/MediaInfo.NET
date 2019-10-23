using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MediaInfoNET
{
    public partial class MainWindow : Window
    {
        bool Wrap;
        string SettingsFolder = "";
        string SettingsFile = "";
        string SourcePath = "";
        String ActiveGroup = "";
        List<Item> Items = new List<Item>();
        string[] Exclude = {};

        public MainWindow()
        {
            InitializeComponent();

            ContentRichTextBox.SelectionChanged += ContentTextBox_SelectionChanged;
         
            SettingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" +
                FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()?.Location).ProductName + @"\";
            
            SettingsFile = SettingsFolder + "settings.conf";

            if (!Directory.Exists(SettingsFolder))
                Directory.CreateDirectory(SettingsFolder);

            if (!File.Exists(SettingsFile))
            {
                string content = @"font = consolas
font-size = 14
window-width = 750
window-height = 550
center-screen = yes
raw-view = yes
word-wrap = no
exclude = UniqueID/String";
                File.WriteAllText(SettingsFile, content);
            }

            ReadSettings();

            if (Environment.GetCommandLineArgs().Length > 1)
                LoadFile(Environment.GetCommandLineArgs()[1]);
            else
                SetText("Drag files here or right-click.");
        }

        void ReadSettings()
        {
            foreach (string line in File.ReadAllLines(SettingsFile))
            {
                if (!line.Contains("="))
                    continue;

                string left = line.Substring(0, line.IndexOf("=")).Trim();
                string right = line.Substring(line.IndexOf("=") + 1).Trim();

                try
                {
                    switch (left)
                    {
                        case "font": FontFamily = new FontFamily(right); break;
                        case "font-size": FontSize = int.Parse(right); break;
                        case "window-width": Width = int.Parse(right); break;
                        case "window-height": Height = int.Parse(right); break;
                        case "raw-view": MediaInfo.RawView = right == "yes"; break;
                        case "word-wrap": Wrap = right == "yes"; break;
                        case "center-screen": WindowStartupLocation = right == "yes" ? WindowStartupLocation.CenterScreen : WindowStartupLocation.Manual; break;
                        case "exclude": Exclude = right.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries); break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to read setting " + left + "." + "\n\n" + ex.Message,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void LoadFile(string file)
        {
            if (!File.Exists(file))
                return;

            PreviousMenuItem.IsEnabled = Directory.GetFiles(Path.GetDirectoryName(file)).Length > 1;
            NextMenuItem.IsEnabled = PreviousMenuItem.IsEnabled;
            SourcePath = file;
            Title = file + " - " + FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()?.Location).ProductName +
                " " + FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()?.Location).FileVersion;
            List<TabItem> tabItems = new List<TabItem>();
            tabItems.Clear();
            HashSet<string> captionNames = new HashSet<string>();
            captionNames.Add("Basic");
            captionNames.Add("Advanced");
            Items = GetItems();

            foreach (Item item in Items)
                captionNames.Add(item.Group);

            foreach (string name in captionNames)
                tabItems.Add(new TabItem { Name = name, Value = name });

            foreach (TabItem tabItem in tabItems)
                foreach (Item item in Items)
                    if (item.Group == tabItem.Name && item.Name == "Format")
                        tabItem.Name += " (" + item.Value + ")";

            TabListBox.ItemsSource = tabItems;

            if (tabItems.Count > 1 && SearchTextBox.Text != "")
                TabListBox.SelectedIndex = 1;
            else if (tabItems.Count > 0)
                TabListBox.SelectedIndex = 0;
        }

        public class TabItem
        {
            public string Name { get; set; } = "";
            public string Value { get; set; } = "";
        }

        List<Item> GetItems()
        {
            List<Item> items = new List<Item>();
            using MediaInfo mediaInfo = new MediaInfo(SourcePath);
            string summary = mediaInfo.GetSummary(true);
            string group = "";

            foreach (string line in summary.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(":"))
                {
                    Item item = new Item();
                    item.Name = line.Substring(0, line.IndexOf(":")).Trim();

                    if (Exclude.Contains(item.Name))
                        continue;

                    item.Value = line.Substring(line.IndexOf(":") + 1).Trim();
                    item.Group = group;
                    item.IsComplete = true;
                    Fix(item);
                    items.Add(item);
                }
                else
                    group = line.Trim();
            }

            summary = mediaInfo.GetSummary(false);

            foreach (string line in summary.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(":"))
                {
                    Item item = new Item();
                    item.Name = line.Substring(0, line.IndexOf(":")).Trim();
                    item.Value = line.Substring(line.IndexOf(":") + 1).Trim();
                    item.Group = group;
                    Fix(item);
                    items.Add(item);
                }
                else
                    group = line.Trim();
            }

            return items;
        }

        void Fix(Item item)
        {
            if (item.Name.StartsWith("FrameRate/"))
            {
                if (item.Value.Contains("fps1"))
                    item.Value = item.Value.Replace("fps1", "FPS");
                else if (item.Value.Contains("fps2"))
                    item.Value = item.Value.Replace("fps2", "FPS");
                else if (item.Value.Contains("fps3"))
                    item.Value = item.Value.Replace("fps3", "FPS");
            }
            else if ((item.Name.StartsWith("Width/") || item.Name.StartsWith("Height/")) &&
                item.Value.Contains("pixel3"))

                item.Value = item.Value.Replace("pixel3", "pixels");
            else if (item.Name.Contains("Channel"))
            {
                if (item.Value.Contains("channel1"))
                    item.Value = item.Value.Replace("channel1", "channels");
                else if (item.Value.Contains("channel2"))
                    item.Value = item.Value.Replace("channel2", "channels");
                else if (item.Value.Contains("channel3"))
                    item.Value = item.Value.Replace("channel3", "channels");
            }
            else if (item.Name.StartsWith("BitDepth/") && item.Value.Contains("bit3"))
                item.Value = item.Value.Replace("bit3", "bits");
            else if (item.Name == "Encoded_Library_Settings")
                Format_Encoded_Library_Settings(item);
        }

        string GetValue(string group, string name)
        {
            foreach (Item item in Items)
                if (item.Group == group && item.Name == name)
                    return item.Value;
            return "";
        }

        string Join(List<string> list)
        {
            List<string> newList = new List<string>();

            foreach (string i in list)
                if (!string.IsNullOrEmpty(i))
                    newList.Add(i);

            return string.Join(", ", newList.ToArray());
        }

        void UpdateItems()
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<Item> items;

            if (ActiveGroup == "Basic")
            {
                List<string> values = new List<string>();

                values.Add(GetValue("General", "Format"));
                values.Add(GetValue("General", "FileSize/String"));
                values.Add(GetValue("General", "Duration/String"));
                values.Add(GetValue("General", "OverallBitRate/String"));

                sb.AppendLine("G: " + Join(values) + "\r\n");

                if (GetValue("Video", "Format") != "")
                {
                    values.Clear();
                    
                    values.Add(GetValue("Video", "Format"));
                    values.Add(GetValue("Video", "Format_Profile"));
                    values.Add(GetValue("Video", "Width") + "x" + GetValue("Video", "Height"));
                    values.Add(GetValue("Video", "FrameRate").Replace(".000", "") + " FPS");
                    values.Add(GetValue("Video", "BitRate/String"));
    
                    sb.AppendLine("V: " + Join(values) + "\r\n");
                }

                var audioGroups = Items.Where(item => item.Group.StartsWith("Audio"))
                                       .Select(item => item.Group)
                                       .Distinct();

                if (audioGroups.Count() > 0)
                {
                    foreach (string group in audioGroups)
                    {
                        values.Clear();

                        string lang = GetValue(group, "Language/String");

                        if (lang.Length == 2)
                            lang = new CultureInfo(lang).EnglishName;

                        values.Add(lang);

                        values.Add(GetValue(group, "Format"));
                        values.Add(GetValue(group, "BitRate/String"));
                        values.Add(GetValue(group, "Channel(s)/String").Replace(" channels", "ch"));
                        values.Add(GetValue(group, "SamplingRate/String"));
                        values.Add(GetValue(group, "Default/String") == "Yes" ? "Default" : "");
                        values.Add(GetValue(group, "Forced/String") == "Yes" ? "Forced" : "");
     
                        sb.AppendLine("A: " + Join(values));
                    }

                    sb.AppendLine();
                }

                var textGroups = Items.Where(item => item.Group.StartsWith("Text"))
                                      .Select(item => item.Group)
                                      .Distinct();

                if (textGroups.Count() > 0)
                {
                    foreach (string group in textGroups)
                    {
                        values.Clear();
                        string lang = GetValue(group, "Language/String");

                        if (lang.Length == 2)
                            lang = new CultureInfo(lang).EnglishName;

                        values.Add(lang);
                        values.Add(GetValue(group, "Format"));
                        values.Add(GetValue(group, "Default/String") == "Yes" ? "Default" : "");
                        values.Add(GetValue(group, "Forced/String") == "Yes" ? "Forced" : "");

                        sb.AppendLine("S: " + Join(values));
                    }

                    sb.AppendLine();
                }
            }

            if (ActiveGroup == "Advanced")
                items = Items.Where(i => i.IsComplete);
            else if (ActiveGroup == "Basic")
                items = Items.Where(i => !i.IsComplete);
            else
            {
                var newItems = new List<Item>();
                newItems.AddRange(Items.Where(i => !i.IsComplete && i.Group == ActiveGroup));
                newItems.Add(new Item { Name = "", Value = "", Group = ActiveGroup });
                newItems.AddRange(Items.Where(i => i.IsComplete && i.Group == ActiveGroup));
                items = newItems;
            }

            string search = SearchTextBox.Text.ToLower();
            
            if (search != "")
                items = items.Where(i => i.Name.ToLower().Contains(search) || i.Value.ToLower().Contains(search));

            List<string> groups = new List<string>();

            foreach (Item item in items)
                if (item.Group != "" && !groups.Contains(item.Group))
                    groups.Add(item.Group);

            foreach (string group in groups)
            {
                if (sb.Length == 0)
                    sb.Append(group + "\r\n\r\n");
                else
                    sb.Append("\r\n" + group + "\r\n\r\n");

                var itemsInGroup = items.Where(i => i.Group == group);

                foreach (Item item in itemsInGroup)
                {
                    if (item.Name != "")
                    {
                        sb.Append(item.Name.PadRight(30));
                        sb.Append(": ");
                    }

                    sb.Append(item.Value);
                    sb.Append("\r\n");
                }
            }

            string text = sb.ToString();
            SetText(text);

            if (Wrap)
                ContentRichTextBox.Document.PageWidth = ContentRichTextBox.Width;
            else
            {          
                FormattedText formatted = new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                    FontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                ContentRichTextBox.Document.PageWidth = formatted.Width + 50;
            }

            if (SearchTextBox.Text.Length > 1)
                Highlight(ContentRichTextBox.Document.ContentStart,
                          ContentRichTextBox.Document.ContentEnd,
                          SearchTextBox.Text);
        }

        void SetText(string value)
        {
            ContentRichTextBox.Document.Blocks.Clear();
            ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(value)));
        }

        void Highlight(TextPointer startPos, TextPointer endPos, string find)
        {
            TextRange? findRange = FindTextInRange(new TextRange(startPos, endPos), find);

            if (findRange != null)
            {
                findRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Green);
                Highlight(findRange.End, endPos, find);
            }
        }

        TextRange? FindTextInRange(TextRange searchRange, string searchText)
        {
            int offset = searchRange.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);

            if (offset < 0)
                return null;

            TextPointer start = searchRange.Start.GetPositionAtOffset(offset);
            return new TextRange(start, start.GetPositionAtOffset(searchText.Length));
        }

        void Previous()
        {
            if (!File.Exists(SourcePath))
                return;

            string[] files = Directory.GetFiles(Path.GetDirectoryName(SourcePath));

            if (files.Length < 2)
                return;

            int index = Array.IndexOf(files, SourcePath);

            if (--index < 0)
                index = files.Length - 1;

            LoadFile(files[index]);
        }

        void Next()
        {
            if (!File.Exists(SourcePath))
                return;

            string[] files = Directory.GetFiles(Path.GetDirectoryName(SourcePath));

            if (files.Length < 2)
                return;

            int index = Array.IndexOf(files, SourcePath);

            if (++index > files.Length - 1)
                index = 0;

            LoadFile(files[index]);
        }

        void ShowSettings()
        {
            SettingsWindow window = new SettingsWindow();
            window.ShowInTaskbar = false;
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.FontFamily = FontFamily;
            window.FontSize = 20;
            window.TextBox.Background = ContentRichTextBox.Background;
            window.TextBox.Foreground = ContentRichTextBox.Foreground;
            window.TextBox.Text = File.ReadAllText(SettingsFile);
            window.ShowDialog();
            File.WriteAllText(SettingsFile, window.TextBox.Text);
            ReadSettings();
            LoadFile(SourcePath);
        }

        void TabListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabListBox.SelectedItem != null)
                ActiveGroup = (TabListBox.SelectedItem as TabItem)?.Value ?? "";

            UpdateItems();
            ContentRichTextBox.ScrollToHome();
        }

        void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ContentRichTextBox.Selection.Text);
        }

        void ContentTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            CopyMenuItem.IsEnabled = ContentRichTextBox.Selection.Text.Length > 0;
        }

        void PreviousMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Previous();
        }

        void NextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (SearchTextBox.Text == "")
                        Close();
                    else
                        SearchTextBox.Text = "";
                    break;
                case Key.F11: Previous(); break;
                case Key.F12: Next(); break;
                case Key.O when Keyboard.IsKeyDown(Key.LeftCtrl):
                    OpenFile();
                    break;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                e.Handled = true;
        }

        void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchTextBox.Text;
            HintTextBlock.Text = text == "" ? "Search" : "";
            ClearButton.Visibility = text == "" ? Visibility.Hidden : Visibility.Visible;

            if (TabListBox.Items.Count > 1)
            {
                TabListBox.SelectedIndex = 1;
                UpdateItems();
            }
        }

        void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            Keyboard.Focus(SearchTextBox);
        }

        void Window_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(e);
        }

        void ContentTextBox_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(e);
        }

        void HandleDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                LoadFile(files[0]);
            }
        }

        void ContentTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        void SetupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Click yes to install and no to uninstall.",
                "Setup", MessageBoxButton.YesNoCancel);

            string args = result switch {
                MessageBoxResult.Yes => "--install",
                MessageBoxResult.No => "--uninstall",
                _ => ""
            };

            if (args != "")
            {
                try
                {
                    using Process proc = new Process();
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                    proc.StartInfo.Arguments = args;
                    proc.StartInfo.Verb = "runas";
                    proc.Start();
                } catch {}
            }
        }

        void Format_Encoded_Library_Settings(Item item)
        {
            Dictionary<string, string> switches = new Dictionary<string, string>();

            switches["allow-non-conformance"] = "Other";
            switches["amp"] = "Analysis";
            switches["analysis-load"] = "Analysis";
            switches["analysis-reuse-file"] = "Analysis";
            switches["analysis-reuse-level"] = "Analysis";
            switches["analysis-save"] = "Analysis";
            switches["analyze-src-pics"] = "Motion Search";
            switches["aq-mode"] = "Rate Control";
            switches["aq-motion"] = "Rate Control";
            switches["aq-strength"] = "Rate Control";
            switches["asm avx512"] = "Performance";
            switches["asm"] = "Performance";
            switches["atc-sei"] = "VUI";
            switches["aud"] = "Bitstream";
            switches["b-adapt"] = "Slice Decision";
            switches["bframe-bias"] = "Slice Decision";
            switches["bframes"] = "Slice Decision";
            switches["b-intra"] = "Analysis";
            switches["b-pyramid"] = "Slice Decision";
            switches["cbqpoffs"] = "Rate Control";
            switches["chromaloc"] = "VUI";
            switches["chunk-end"] = "Input/Output";
            switches["chunk-start"] = "Input/Output";
            switches["cip"] = "Other";
            switches["cll"] = "VUI";
            switches["colormatrix"] = "VUI";
            switches["colorprim"] = "VUI";
            switches["constrained-intra"] = "Other";
            switches["const-vbv"] = "Rate Control";
            switches["copy-pic"] = "Performance";
            switches["cplxblur"] = "Rate Control";
            switches["crf-max"] = "Rate Control";
            switches["crf-min"] = "Rate Control";
            switches["crqpoffs"] = "Rate Control";
            switches["csv"] = "Statistic";
            switches["csv-log-level"] = "Statistic";
            switches["ctu"] = "Analysis";
            switches["ctu-info"] = "Slice Decision";
            switches["cu-lossless"] = "Analysis";
            switches["cu-stats"] = "Analysis";
            switches["cutree"] = "Rate Control";
            switches["deblock"] = "Loop Filter";
            switches["dhdr10-info"] = "VUI";
            switches["dhdr10-opt"] = "VUI";
            switches["display-window"] = "VUI";
            switches["dither"] = "Input/Output";
            switches["dolby-vision-profile"] = "Bitstream";
            switches["dolby-vision-rpu"] = "Bitstream";
            switches["dynamic-rd"] = "Analysis";
            switches["dynamic-refine"] = "Analysis";
            switches["early-skip"] = "Analysis";
            switches["fades"] = "Slice Decision";
            switches["fast-intra"] = "Analysis";
            switches["field"] = "Input/Output";
            switches["force-flush"] = "Slice Decision";
            switches["fps"] = "Input/Output";
            switches["frames"] = "Input/Output";
            switches["frame-threads"] = "Performance";
            switches["gop-lookahead"] = "Slice Decision";
            switches["hash"] = "Bitstream";
            switches["hdr"] = "VUI";
            switches["hdr-opt"] = "VUI";
            switches["hevc-aq"] = "Analysis";
            switches["high-tier"] = "Other";
            switches["hme"] = "Motion Search";
            switches["hme-search"] = "Motion Search";
            switches["hrd"] = "Bitstream";
            switches["hrd-concat"] = "Bitstream";
            switches["idr-recovery-sei"] = "Bitstream";
            switches["info"] = "Bitstream";
            switches["input-csp"] = "Input/Output";
            switches["input-depth"] = "Input/Output";
            switches["interlace"] = "Input/Output";
            switches["intra-refresh"] = "Slice Decision";
            switches["ip-factor"] = "Rate Control";
            switches["ipratio"] = "Rate Control";
            switches["keyint"] = "Slice Decision";
            switches["lambda-file"] = "Other";
            switches["limit-modes"] = "Analysis";
            switches["limit-refs"] = "Analysis";
            switches["limit-sao"] = "Loop Filter";
            switches["limit-tu"] = "Analysis";
            switches["log"] = "Statistic";
            switches["log2-max-poc-lsb"] = "Bitstream";
            switches["log-level"] = "Statistic";
            switches["lookahead-slices"] = "Slice Decision";
            switches["lookahead-threads"] = "Slice Decision";
            switches["lossless"] = "Rate Control";
            switches["lowpass-dct"] = "Other";
            switches["master-display"] = "VUI";
            switches["max-ausize-factor"] = "Other";
            switches["max-cll"] = "VUI";
            switches["max-luma"] = "VUI";
            switches["max-merge"] = "Motion Search";
            switches["max-tu-size"] = "Analysis";
            switches["me"] = "Motion Search";
            switches["merange"] = "Motion Search";
            switches["min-cu-size"] = "Analysis";
            switches["min-keyint"] = "Slice Decision";
            switches["min-luma"] = "VUI";
            switches["multi-pass-opt-analysis"] = "Rate Control";
            switches["multi-pass-opt-distortion"] = "Rate Control";
            switches["multi-pass-opt-rps"] = "Bitstream";
            switches["nalu-file"] = "VUI";
            switches["no-amp"] = "Analysis";
            switches["no-analyze-src-pics"] = "Motion Search";
            switches["no-asm"] = "Performance";
            switches["no-b-intra"] = "Analysis";
            switches["no-b-pyramid"] = "Slice Decision";
            switches["no-cll"] = "VUI";
            switches["no-constrained-intra"] = "Other";
            switches["no-const-vbv"] = "Rate Control";
            switches["no-copy-pic"] = "Performance";
            switches["no-cutree"] = "Rate Control";
            switches["no-early-skip"] = "Analysis";
            switches["no-fast-intra"] = "Analysis";
            switches["no-field"] = "Input/Output";
            switches["no-hme"] = "Motion Search";
            switches["no-info"] = "Bitstream";
            switches["no-open-gop"] = "Slice Decision";
            switches["no-pme"] = "Performance";
            switches["no-pmode"] = "Performance";
            switches["no-rc-grain"] = "Rate Control";
            switches["no-rect"] = "Analysis";
            switches["no-rskip"] = "Analysis";
            switches["no-sao"] = "Loop Filter";
            switches["no-signhide"] = "Other";
            switches["no-slow-firstpass"] = "Performance";
            switches["no-strong-intra-smoothing"] = "Other";
            switches["no-temporal-mvp"] = "Motion Search";
            switches["no-weightb"] = "Motion Search";
            switches["no-weightp"] = "Motion Search";
            switches["no-wpp"] = "Performance";
            switches["nr-inter"] = "Rate Control";
            switches["nr-intra"] = "Rate Control";
            switches["numa-pools"] = "Performance";
            switches["open-gop"] = "Slice Decision";
            switches["opt-cu-delta-qp"] = "Bitstream";
            switches["opt-qp-pps"] = "Bitstream";
            switches["opt-ref-list-length-pps"] = "Bitstream";
            switches["overscan"] = "VUI";
            switches["pb-factor"] = "Rate Control";
            switches["pbratio"] = "Rate Control";
            switches["pic-struct"] = "VUI";
            switches["pme"] = "Performance";
            switches["pmode"] = "Performance";
            switches["pools"] = "Performance";
            switches["psnr"] = "Statistic";
            switches["psy-rd"] = "Other";
            switches["psy-rdoq"] = "Analysis";
            switches["qblur"] = "Rate Control";
            switches["qcomp"] = "Rate Control";
            switches["qg-size"] = "Rate Control";
            switches["qp-adaptation-range"] = "Analysis";
            switches["qpfile"] = "Other";
            switches["qpmax"] = "Rate Control";
            switches["qpmin"] = "Rate Control";
            switches["qpstep"] = "Rate Control";
            switches["radl"] = "Slice Decision";
            switches["range"] = "VUI";
            switches["rc-grain"] = "Rate Control";
            switches["rc-lookahead"] = "Slice Decision";
            switches["rd"] = "Analysis";
            switches["rdoq"] = "Analysis";
            switches["rdoq-level"] = "Analysis";
            switches["rdpenalty"] = "Other";
            switches["rd-refine"] = "Analysis";
            switches["recon"] = "Other";
            switches["recon-depth"] = "Other";
            switches["rect"] = "Analysis";
            switches["ref"] = "Slice Decision";
            switches["refine-analysis-type"] = "Slice Decision";
            switches["refine-ctu-distortion"] = "Analysis";
            switches["refine-inter"] = "Analysis";
            switches["refine-intra"] = "Analysis";
            switches["refine-mv"] = "Analysis";
            switches["repeat-headers"] = "Bitstream";
            switches["rskip"] = "Analysis";
            switches["sao"] = "Loop Filter";
            switches["sao-non-deblock"] = "Loop Filter";
            switches["sar"] = "VUI";
            switches["scale-factor"] = "Analysis";
            switches["scaling-list"] = "Other";
            switches["scenecut"] = "Slice Decision";
            switches["scenecut-bias"] = "Slice Decision";
            switches["seek"] = "Input/Output";
            switches["selective-sao"] = "Loop Filter";
            switches["signhide"] = "Other";
            switches["single-sei"] = "Bitstream";
            switches["slices"] = "Performance";
            switches["slow-firstpass"] = "Performance";
            switches["splitrd-skip"] = "Analysis";
            switches["ssim"] = "Statistic";
            switches["ssim-rd"] = "Analysis";
            switches["strict-cbr"] = "Rate Control";
            switches["strong-intra-smoothing"] = "Other";
            switches["subme"] = "Motion Search";
            switches["temporal-layers"] = "Bitstream";
            switches["temporal-mvp"] = "Motion Search";
            switches["transfer"] = "VUI";
            switches["tskip"] = "Analysis";
            switches["tskip-fast"] = "Analysis";
            switches["tu-inter-depth"] = "Analysis";
            switches["tu-intra-depth"] = "Analysis";
            switches["uhd-bd"] = "Other";
            switches["vbv-bufsize"] = "Rate Control";
            switches["vbv-end"] = "Rate Control";
            switches["vbv-end-fr-adj"] = "Rate Control";
            switches["vbv-init"] = "Rate Control";
            switches["vbv-maxrate"] = "Rate Control";
            switches["videoformat"] = "VUI";
            switches["vui-hrd-info"] = "Bitstream";
            switches["vui-timing-info"] = "Bitstream";
            switches["weightb"] = "Motion Search";
            switches["weightp"] = "Motion Search";
            switches["wpp"] = "Performance";
            switches["zonefile"] = "Rate Control";
            switches["zones"] = "Rate Control";

            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();

            foreach (var pair in switches)
            {
                if (!groups.ContainsKey(pair.Value))
                    groups[pair.Value] = new List<string>();

                if (!groups[pair.Value].Contains(pair.Key))
                    groups[pair.Value].Add(pair.Key);
            }

            List<KeyValuePair<string, List<string>>> list = new List<KeyValuePair<string, List<string>>>();

            list.Add(new KeyValuePair<string, List<string>>("Analysis", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Rate Control", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Slice Decision", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Motion Search", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Performance", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Bitstream", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("VUI", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Statistic", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Loop Filter", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Input/Output", new List<string>()));
            list.Add(new KeyValuePair<string, List<string>>("Other", new List<string>()));

            foreach (string _switch in item.Value.Split(" / "))
            {
                string name = _switch;

                if (name.Contains("="))
                    name = name.Substring(0, name.IndexOf("=")).Trim();

                string group = "Other";

                foreach (var groupPair in groups)
                {
                    if (groupPair.Value.Contains(name))
                    {
                        group = groupPair.Key;
                        break;
                    }
                }

                foreach (var i in list)
                {
                    if (i.Key == group)
                    {
                        i.Value.Add(_switch);
                        break;
                    }
                }
            }

            string text = "\r\n";

            foreach (var i in list)
            {
                if (i.Value.Count == 0)
                    continue;

                text += "\r\n    " + i.Key + "\r\n";

                string temp = "";
                
                foreach (var i2 in i.Value)
                {
                    temp += " / " + i2;

                    if (temp.Length > 40)
                    {
                        text += "        " + temp.Trim(" /".ToCharArray()) + "\r\n";
                        temp = "";
                    }
                }

                if (temp != "")
                    text += "        " + temp.Trim(" /".ToCharArray()) + "\r\n";
            }

            item.Value = text;
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
                LoadFile(dialog.FileName);
        }
    }
}