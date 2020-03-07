
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;
using WinForms = System.Windows.Forms;

namespace MediaInfoNET
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        string SourcePath = "";
        String ActiveGroup = "";

        List<MediaInfoParameter> Items = new List<MediaInfoParameter>();
        List<MediaInfoParameter> ItemsRaw = new List<MediaInfoParameter>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ApplySettings();
            WriteShellRegistryKey();
            UpdateCheck.Updating += () => Dispatcher.Invoke(() => Close());

            if (Environment.GetCommandLineArgs().Length > 1)
                LoadFile(Environment.GetCommandLineArgs()[1]);
            else
                SetText("Drag files here or right-click.");
        }

        void WriteShellRegistryKey()
        {
            string keyPath = @"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" +
                Path.GetFileName(AppHelp.ExecutablePath);
            
            if (!File.Exists(RegistryHelp.GetString(keyPath, null)))
                RegistryHelp.SetValue(keyPath, null, AppHelp.ExecutablePath);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string _TextSelectionColor = "";

        public string TextSelectionColor {
            get => _TextSelectionColor;
            set {
                _TextSelectionColor = value;
                NotifyPropertyChanged();
            }
        }

        string _BorderColor = "";

        public string BorderColor {
            get => _BorderColor;
            set {
                _BorderColor = value;
                NotifyPropertyChanged();
            }
        }

        string _ItemSelectionColor = "";

        public string ItemSelectionColor {
            get => _ItemSelectionColor;
            set {
                _ItemSelectionColor = value;
                NotifyPropertyChanged();
            }
        }

        string _ItemHoverColor = "";

        public string ItemHoverColor {
            get => _ItemHoverColor;
            set {
                _ItemHoverColor = value;
                NotifyPropertyChanged();
            }
        }

        string _HighlightColor = "";

        public string HighlightColor {
            get => _HighlightColor;
            set {
                _HighlightColor = value;
                NotifyPropertyChanged();
            }
        }

        void ApplySettings()
        {
            FontFamily = new FontFamily(App.Settings?.FontName);
            FontSize = App.Settings.FontSize;
            Width = App.Settings.WindowWidth;
            Height = App.Settings.WindowHeight;
            WindowStartupLocation = App.Settings.CenterScreen ? WindowStartupLocation.CenterScreen : WindowStartupLocation.Manual;

            Theme theme = App.Settings.Theme switch
            {
                "Light" => App.Settings.LightTheme,
                "Dark" => App.Settings.DarkTheme,
                _ => AppHelp.IsDarkTheme ? App.Settings.DarkTheme : App.Settings.LightTheme
            };

            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(theme.Background));
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(theme.Foreground));
            TextSelectionColor = theme.TextSelection;
            ItemSelectionColor = theme.ItemSelection;
            ItemHoverColor = theme.ItemHover;
            BorderColor = theme.Border;
            HighlightColor = theme.Highlight;
        }

        void LoadFile(string file)
        {
            if (!File.Exists(file))
                return;

            SaveMenuItem.IsEnabled = true;
            PreviousMenuItem.IsEnabled = Directory.GetFiles(Path.GetDirectoryName(file)).Length > 1;
            NextMenuItem.IsEnabled = PreviousMenuItem.IsEnabled;
            SourcePath = file;
            Title = file + " - " + AppHelp.ProductName + " " + WinForms.Application.ProductVersion;
            List<TabItem> tabItems = new List<TabItem>();
            tabItems.Clear();
            HashSet<string> captionNames = new HashSet<string>();
            captionNames.Add("Basic");
            captionNames.Add("Advanced");

            ItemsRaw = GetItems(true);
            Items = App.Settings.RawView ? ItemsRaw : GetItems(false);

            foreach (MediaInfoParameter item in Items)
                captionNames.Add(item.Group);

            foreach (string name in captionNames)
                tabItems.Add(new TabItem { Name = name, Value = name });

            foreach (TabItem tabItem in tabItems)
                foreach (MediaInfoParameter item in ItemsRaw)
                    if (item.Group == tabItem.Name && item.Name == "Format/String")
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

        List<MediaInfoParameter> GetItems(bool rawView)
        {
            List<MediaInfoParameter> items = new List<MediaInfoParameter>();
            using MediaInfo mediaInfo = new MediaInfo(SourcePath);
            string summary = mediaInfo.GetSummary(true, rawView);
            string group = "";
            string[] exclude = App.Settings.Exclude.Split(new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < exclude.Length; i++)
                exclude[i] = exclude[i].Trim();

            List<string> added = new List<string>();

            foreach (string line in summary.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(":"))
                {
                    MediaInfoParameter item = new MediaInfoParameter();
                    item.Name = line.Substring(0, line.IndexOf(":")).Trim();

                    if (exclude.Contains(item.Name))
                        continue;

                    item.Value = line.Substring(line.IndexOf(":") + 1).Trim();
                    item.Group = group;
                    item.IsComplete = true;
                    Fix(item, rawView);
                    string addedKey = item.Name + item.Value + item.Group;

                    if (!added.Contains(addedKey))
                        items.Add(item);

                    added.Add(addedKey);
                }
                else
                    group = line.Trim();
            }

            summary = mediaInfo.GetSummary(false, rawView);

            foreach (string line in summary.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(":"))
                {
                    MediaInfoParameter item = new MediaInfoParameter();
                    item.Name = line.Substring(0, line.IndexOf(":")).Trim();

                    if (exclude.Contains(item.Name))
                        continue;

                    item.Value = line.Substring(line.IndexOf(":") + 1).Trim();
                    item.Group = group;
                    Fix(item, rawView);
                    items.Add(item);
                }
                else
                    group = line.Trim();
            }

            return items;
        }

        void Fix(MediaInfoParameter item, bool rawView)
        {
            void FixS(MediaInfoParameter item, string prefix, string replace = "s")
            {
                for (int i = 0; i < 4; i++)
                    if (item.Value.Contains(prefix + i))
                        item.Value = item.Value.Replace(prefix + i, prefix + replace);
            }

            if (item.Name.StartsWith("FrameRate/") || item.Name.StartsWith("Frame rate"))
                FixS(item, "fps", "");
            else if (item.Name.StartsWith("Width") || item.Name.StartsWith("Height"))
                FixS(item, "pixel");
            else if (item.Name.Contains("Channel"))
                FixS(item, "channel");
            else if (item.Name.StartsWith("Bit"))
                FixS(item, "bit");
            else if (item.Name.Contains("Size", StringComparison.OrdinalIgnoreCase))
                FixS(item, "Byte");
            else if ((item.Name == "Encoded_Library_Settings" || item.Name == "Encoding settings")
                && App.Settings.FormatEncoded)

                Format_Encoded_Library_Settings(item);
            else if (!rawView && item.Name == "Language" && item.Value.Length == 2)
                item.Value = GetLanguageName(item.Value);
            else if (item.Name.StartsWith("Format settings") || item.Name.StartsWith("Format_Settings"))
            {
                if (item.Name.Contains("Reference frames"))
                    item.Name = item.Name.Replace("Reference frames", "ref frames");

                FixS(item, "frame");
            }
            else if (item.Group == "Menu" && item.Name == "00" &&
                item.Value.Contains("                     : "))
            {
                Match match = Regex.Match(item.Value, @"(\d\d:\d\d\.\d\d\d) +: +(.+)");

                if (match.Success)
                {
                    item.Name = match.Groups[1].Value;
                    item.Value = match.Groups[2].Value;
                }

                match = Regex.Match(item.Value, @"(\w\w:Chapter \d\d) / \w\w:Chapter \d\d");

                if (match.Success)
                    item.Value = item.Value.Replace(match.Value, match.Groups[1].Value);
            }
        }

        string GetLanguageName(string id)
        {
            try
            {
                return new CultureInfo(id).EnglishName;
            }
            catch
            {
                return id;
            }
        }

        string GetValue(string group, string name)
        {
            foreach (var item in ItemsRaw)
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

        void UpdateContentRichTextBox()
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<MediaInfoParameter> items;

            if (ActiveGroup == "Basic" && App.Settings.CompactSummary)
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
                            lang = GetLanguageName(lang);

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

                        sb.AppendLine("T: " + Join(values));
                    }

                    sb.AppendLine();
                }

                if (Items.Any(item => item.Group == "Menu"))
                    sb.AppendLine("M: Menu\r\n");
            }

            if (ActiveGroup == "Advanced")
                items = Items.Where(i => i.IsComplete);
            else if (ActiveGroup == "Basic")
                items = Items.Where(i => !i.IsComplete);
            else
            {
                var newItems = new List<MediaInfoParameter>();
                newItems.AddRange(Items.Where(i => !i.IsComplete && i.Group == ActiveGroup));
                newItems.Add(new MediaInfoParameter { Name = "", Value = "", Group = ActiveGroup });
                newItems.AddRange(Items.Where(i => i.IsComplete && i.Group == ActiveGroup));
                items = newItems;
            }

            string search = SearchTextBox.Text.ToLower();
            
            if (search != "")
                items = items.Where(i => i.Name.ToLower().Contains(search) || i.Value.ToLower().Contains(search));

            List<string> groups = new List<string>();

            foreach (MediaInfoParameter item in items)
                if (item.Group != "" && !groups.Contains(item.Group))
                    groups.Add(item.Group);

            foreach (string group in groups)
            {
                if (sb.Length == 0)
                    sb.Append(group + "\r\n\r\n");
                else
                    sb.Append("\r\n" + group + "\r\n\r\n");

                var itemsInGroup = items.Where(i => i.Group == group);

                foreach (MediaInfoParameter item in itemsInGroup)
                {
                    if (item.Name != "")
                    {
                        sb.Append(item.Name.PadRight(App.Settings.ColumnPadding));
                        sb.Append(": ");
                    }

                    sb.Append(item.Value);
                    sb.Append("\r\n");
                }
            }

            string text = sb.ToString();
            SetText(text);

            if (App.Settings.WordWrap)
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
                findRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString(HighlightColor)));
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
            window.Owner = this;
            window.ShowDialog();
            ApplySettings();
            App.SaveSettings();
            LoadFile(SourcePath);
        }

        void TabListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabListBox.SelectedItem != null)
                ActiveGroup = (TabListBox.SelectedItem as TabItem)?.Value ?? "";

            UpdateContentRichTextBox();
            ContentRichTextBox.ScrollToHome();
        }

        void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ContentRichTextBox.Selection.Text);
        }

        void ContentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
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
                    e.Handled = true;
                    OpenFile();
                    break;
                case Key.S when Keyboard.IsKeyDown(Key.LeftCtrl):
                    e.Handled = true;
                    SaveFile();
                    break;
            }
        }

        void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = SearchTextBox.Text;
            HintTextBlock.Text = text == "" ? "Search" : "";
            ClearButton.Visibility = text == "" ? Visibility.Hidden : Visibility.Visible;

            if (TabListBox.Items.Count > 1)
            {
                TabListBox.SelectedIndex = 1;
                UpdateContentRichTextBox();
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

        void ContentRichTextBox_Drop(object sender, DragEventArgs e)
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

        void ContentRichTextBox_PreviewDragOver(object sender, DragEventArgs e)
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
            using TaskDialog<string> td = new TaskDialog<string>();
            td.MainInstruction = "Register or unregister file associations?";
            td.Content = "Add/remove MediaInfo.NET in File Explorer context menu.";
            td.AddCommand("Register file associations", "", "--install", true);
            td.AddCommand("Unregister file associations", "", "--uninstall", true);
            td.AddCommand("Cancel");

            if ((td.Show() ?? "").StartsWith("--"))
            {
                try
                {
                    using Process proc = new Process();
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.FileName = AppHelp.ExecutablePath;
                    proc.StartInfo.Arguments = td.SelectedValue;
                    proc.StartInfo.Verb = "runas";
                    proc.Start();
                } catch {}
            }
        }

        void Format_Encoded_Library_Settings(MediaInfoParameter item)
        {
            OrderedDictionary switches = new OrderedDictionary();

            switches["aq-mode"] = "Rate Control";
            switches["aq-motion"] = "Rate Control";
            switches["aq-strength"] = "Rate Control";
            switches["bitrate"] = "Rate Control";
            switches["cbqpoffs"] = "Rate Control";
            switches["const-vbv"] = "Rate Control";
            switches["cplxblur"] = "Rate Control";
            switches["crf"] = "Rate Control";
            switches["crf-max"] = "Rate Control";
            switches["crf-min"] = "Rate Control";
            switches["crqpoffs"] = "Rate Control";
            switches["cutree"] = "Rate Control";
            switches["ip-factor"] = "Rate Control";
            switches["ipratio"] = "Rate Control";
            switches["lossless"] = "Rate Control";
            switches["multi-pass-opt-analysis"] = "Rate Control";
            switches["multi-pass-opt-distortion"] = "Rate Control";
            switches["nr-inter"] = "Rate Control";
            switches["nr-intra"] = "Rate Control";
            switches["pb-factor"] = "Rate Control";
            switches["pbratio"] = "Rate Control";
            switches["qblur"] = "Rate Control";
            switches["qcomp"] = "Rate Control";
            switches["qg-size"] = "Rate Control";
            switches["qpmax"] = "Rate Control";
            switches["qpmin"] = "Rate Control";
            switches["qpstep"] = "Rate Control";
            switches["rc"] = "Rate Control";
            switches["rc-grain"] = "Rate Control";
            switches["strict-cbr"] = "Rate Control";
            switches["vbv-bufsize"] = "Rate Control";
            switches["vbv-end"] = "Rate Control";
            switches["vbv-end-fr-adj"] = "Rate Control";
            switches["vbv-init"] = "Rate Control";
            switches["vbv-maxrate"] = "Rate Control";
            switches["zone-count"] = "Rate Control";
            switches["zonefile"] = "Rate Control";
            switches["zones"] = "Rate Control";

            switches["amp"] = "Analysis";
            switches["analysis-load"] = "Analysis";
            switches["analysis-reuse-file"] = "Analysis";
            switches["analysis-reuse-level"] = "Analysis";
            switches["analysis-save"] = "Analysis";
            switches["b-intra"] = "Analysis";
            switches["ctu"] = "Analysis";
            switches["cu-lossless"] = "Analysis";
            switches["cu-stats"] = "Analysis";
            switches["dynamic-rd"] = "Analysis";
            switches["dynamic-refine"] = "Analysis";
            switches["early-skip"] = "Analysis";
            switches["fast-intra"] = "Analysis";
            switches["hevc-aq"] = "Analysis";
            switches["limit-modes"] = "Analysis";
            switches["limit-refs"] = "Analysis";
            switches["limit-tu"] = "Analysis";
            switches["max-tu-size"] = "Analysis";
            switches["min-cu-size"] = "Analysis";
            switches["psy-rdoq"] = "Analysis";
            switches["qp-adaptation-range"] = "Analysis";
            switches["rd"] = "Analysis";
            switches["rdoq"] = "Analysis";
            switches["rdoq-level"] = "Analysis";
            switches["rdpenalty"] = "Analysis";
            switches["rd-refine"] = "Analysis";
            switches["rect"] = "Analysis";
            switches["refine-ctu-distortion"] = "Analysis";
            switches["refine-inter"] = "Analysis";
            switches["refine-intra"] = "Analysis";
            switches["refine-mv"] = "Analysis";
            switches["rskip"] = "Analysis";
            switches["scale-factor"] = "Analysis";
            switches["splitrd-skip"] = "Analysis";
            switches["ssim-rd"] = "Analysis";
            switches["tskip"] = "Analysis";
            switches["tskip-fast"] = "Analysis";
            switches["tu-inter-depth"] = "Analysis";
            switches["tu-intra-depth"] = "Analysis";

            switches["b-adapt"] = "Slice Decision";
            switches["bframe-bias"] = "Slice Decision";
            switches["bframes"] = "Slice Decision";
            switches["b-pyramid"] = "Slice Decision";
            switches["ctu-info"] = "Slice Decision";
            switches["fades"] = "Slice Decision";
            switches["force-flush"] = "Slice Decision";
            switches["gop-lookahead"] = "Slice Decision";
            switches["intra-refresh"] = "Slice Decision";
            switches["keyint"] = "Slice Decision";
            switches["lookahead-slices"] = "Slice Decision";
            switches["lookahead-threads"] = "Slice Decision";
            switches["min-keyint"] = "Slice Decision";
            switches["open-gop"] = "Slice Decision";
            switches["radl"] = "Slice Decision";
            switches["rc-lookahead"] = "Slice Decision";
            switches["ref"] = "Slice Decision";
            switches["refine-analysis-type"] = "Slice Decision";
            switches["scenecut"] = "Slice Decision";
            switches["scenecut-bias"] = "Slice Decision";

            switches["analyze-src-pics"] = "Motion Search";
            switches["hme"] = "Motion Search";
            switches["hme-search"] = "Motion Search";
            switches["max-merge"] = "Motion Search";
            switches["me"] = "Motion Search";
            switches["merange"] = "Motion Search";
            switches["subme"] = "Motion Search";
            switches["temporal-mvp"] = "Motion Search";
            switches["weightb"] = "Motion Search";
            switches["weightp"] = "Motion Search";

            switches["deblock"] = "Loop Filter";
            switches["limit-sao"] = "Loop Filter";
            switches["sao"] = "Loop Filter";
            switches["sao-non-deblock"] = "Loop Filter";
            switches["selective-sao"] = "Loop Filter";

            switches["atc-sei"] = "VUI";
            switches["chromaloc"] = "VUI";
            switches["cll"] = "VUI";
            switches["colormatrix"] = "VUI";
            switches["colorprim"] = "VUI";
            switches["dhdr10-info"] = "VUI";
            switches["dhdr10-opt"] = "VUI";
            switches["display-window"] = "VUI";
            switches["hdr"] = "VUI";
            switches["hdr-opt"] = "VUI";
            switches["master-display"] = "VUI";
            switches["max-cll"] = "VUI";
            switches["max-luma"] = "VUI";
            switches["min-luma"] = "VUI";
            switches["nalu-file"] = "VUI";
            switches["overscan"] = "VUI";
            switches["pic-struct"] = "VUI";
            switches["range"] = "VUI";
            switches["sar"] = "VUI";
            switches["transfer"] = "VUI";
            switches["videoformat"] = "VUI";

            switches["annexb"] = "Bitstream";
            switches["aud"] = "Bitstream";
            switches["dolby-vision-profile"] = "Bitstream";
            switches["dolby-vision-rpu"] = "Bitstream";
            switches["hash"] = "Bitstream";
            switches["hrd"] = "Bitstream";
            switches["hrd-concat"] = "Bitstream";
            switches["idr-recovery-sei"] = "Bitstream";
            switches["info"] = "Bitstream";
            switches["log2-max-poc-lsb"] = "Bitstream";
            switches["multi-pass-opt-rps"] = "Bitstream";
            switches["opt-cu-delta-qp"] = "Bitstream";
            switches["opt-qp-pps"] = "Bitstream";
            switches["opt-ref-list-length-pps"] = "Bitstream";
            switches["repeat-headers"] = "Bitstream";
            switches["single-sei"] = "Bitstream";
            switches["temporal-layers"] = "Bitstream";
            switches["vui-hrd-info"] = "Bitstream";
            switches["vui-timing-info"] = "Bitstream";

            switches["chunk-end"] = "Input/Output";
            switches["chunk-start"] = "Input/Output";
            switches["dither"] = "Input/Output";
            switches["field"] = "Input/Output";
            switches["fps"] = "Input/Output";
            switches["frames"] = "Input/Output";
            switches["input-csp"] = "Input/Output";
            switches["input-depth"] = "Input/Output";
            switches["input-res"] = "Input/Output";
            switches["interlace"] = "Input/Output";
            switches["seek"] = "Input/Output";
            switches["total-frames"] = "Input/Output";

            switches["asm"] = "Performance";
            switches["copy-pic"] = "Performance";
            switches["frame-threads"] = "Performance";
            switches["numa-pools"] = "Performance";
            switches["pme"] = "Performance";
            switches["pmode"] = "Performance";
            switches["pools"] = "Performance";
            switches["slices"] = "Performance";
            switches["slow-firstpass"] = "Performance";
            switches["wpp"] = "Performance";

            switches["csv"] = "Statistic";
            switches["csv-log-level"] = "Statistic";
            switches["log"] = "Statistic";
            switches["log-level"] = "Statistic";
            switches["psnr"] = "Statistic";
            switches["ssim"] = "Statistic";
            switches["stats-read"] = "Statistic";
            switches["stats-write"] = "Statistic";

            switches["allow-non-conformance"] = "Other";
            switches["cip"] = "Other";
            switches["constrained-intra"] = "Other";
            switches["high-tier"] = "Other";
            switches["lambda-file"] = "Other";
            switches["lowpass-dct"] = "Other";
            switches["max-ausize-factor"] = "Other";
            switches["psy-rd"] = "Other";
            switches["qpfile"] = "Other";
            switches["recon"] = "Other";
            switches["recon-depth"] = "Other";
            switches["scaling-list"] = "Other";
            switches["signhide"] = "Other";
            switches["strong-intra-smoothing"] = "Other";
            switches["uhd-bd"] = "Other";

            OrderedDictionary targetDictionary = new OrderedDictionary();

            foreach (string value in switches.Values)
            {
                if (!targetDictionary.Contains(value))
                    targetDictionary[value] = new List<string>();
            }

            Dictionary<string, string> sourceDictionary = new Dictionary<string, string>();
            
            foreach (string value in item.Value.Split(" / "))
            {
                if (value.Contains("="))
                {
                    string left = value.Substring(0, value.IndexOf("="));
                    sourceDictionary[left] = value;
                }
                else
                {
                    if (value.StartsWith("no-"))
                        sourceDictionary[value.Substring(3)] = value;
                    else
                        sourceDictionary[value] = value;
                }
            }

            List<string> added = new List<string>();

            foreach (string key in switches.Keys)
            {
                if (sourceDictionary.ContainsKey(key))
                {
                    ((List<string>)targetDictionary[switches[key]]).Add(sourceDictionary[key]);
                    added.Add(key);
                }
            }

            foreach (string key in sourceDictionary.Keys)
                if (!added.Contains(key))
                    ((List<string>)(targetDictionary["Other"])).Add(sourceDictionary[key]);

            ((List<string>)(targetDictionary["Other"])).Sort();
            string text = "\r\n";

            foreach (string? key in targetDictionary.Keys)
            {
                List<string> list = (List<string>)targetDictionary[key];

                if (list?.Count == 0)
                    continue;

                text += "\r\n    " + key + "\r\n";
                string temp = "";

                foreach (string value in list)
                {
                    temp += " / " + value;

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

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == true)
            {
                if (!dialog.FileName.EndsWith(".txt"))
                    dialog.FileName += ".txt";

                File.WriteAllText(dialog.FileName, GetText());
            }
        }

        string GetText()
        {
            return new TextRange(ContentRichTextBox.Document.ContentStart,
                ContentRichTextBox.Document.ContentEnd).Text;
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Msg.Show(AppHelp.ProductName + " " + WinForms.Application.ProductVersion,
                "MediaInfo " + FileVersionInfo.GetVersionInfo(WinForms.Application
                .StartupPath + @"\MediaInfo.dll").ProductVersion +
                "\n\nCopyright (C) 2019 Frank Skare (stax76)\n\nMIT License");
        }

        bool WasActivated;

        private void Window_Activated(object sender, EventArgs e)
        {
            if (!WasActivated)
            {
                ActivateWindow();
                WasActivated = true;
            }
        }

        async void ActivateWindow()
        {
            await Task.Run(new Action(() => Thread.Sleep(500)));
            Activate();
            UpdateCheck.DailyCheck();
        }

        private void WebsiteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo() {
                UseShellExecute = true,
                FileName = "https://github.com/stax76/MediaInfo.NET"
            });
        }

        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateCheck.CheckOnline(true);
        }
    }
}
