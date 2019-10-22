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
word-wrap = no";
                File.WriteAllText(SettingsFile, content);
            }

            ReadSettings();

            if (Environment.GetCommandLineArgs().Length > 1)
                LoadFile(Environment.GetCommandLineArgs()[1]);
        }

        private void ReadSettings()
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
                        case "font":
                            FontFamily = new FontFamily(right); break;
                        case "font-size":
                            FontSize = int.Parse(right); break;
                        case "window-width":
                            Width = int.Parse(right); break;
                        case "window-height":
                            Height = int.Parse(right); break;
                        case "raw-view":
                            MediaInfo.RawView = right == "yes"; break;
                        case "word-wrap":
                            Wrap = right == "yes"; break;
                        case "center-screen":
                            WindowStartupLocation = right == "yes" ? WindowStartupLocation.CenterScreen :
                                                                     WindowStartupLocation.Manual; break;
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
            Title = file + " - " + FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()?.Location).ProductName + " " + FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()?.Location).FileVersion;
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

            ContentRichTextBox.Document.Blocks.Clear();
            ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));

            if (Wrap)
            {
                ContentRichTextBox.Document.PageWidth = ContentRichTextBox.Width;
            }
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

        void Highlight(TextPointer startPos, TextPointer endPos, string find)
        {
            TextRange? findRange = FindTextInRange(new TextRange(startPos, endPos), find);

            if (findRange != null)
            {
                findRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Green);
                Highlight(findRange.End, endPos, find);
            }
        }

        public TextRange? FindTextInRange(TextRange searchRange, string searchText)
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

        private void TabListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabListBox.SelectedItem != null)
                ActiveGroup = (TabListBox.SelectedItem as TabItem)?.Value ?? "";

            UpdateItems();
            ContentRichTextBox.ScrollToHome();
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ContentRichTextBox.Selection.Text);
        }

        private void ContentTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            CopyMenuItem.IsEnabled = ContentRichTextBox.Selection.Text.Length > 0;
        }

        private void PreviousMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Previous();
        }

        private void NextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (SearchTextBox.Text == "")
                        Close();
                    else
                        SearchTextBox.Text = "";
                    break;
                case Key.F11:
                    Previous(); break;
                case Key.F12:
                    Next(); break;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            Keyboard.Focus(SearchTextBox);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(e);
        }

        private void ContentTextBox_Drop(object sender, DragEventArgs e)
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

        private void ContentTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void SetupMenuItem_Click(object sender, RoutedEventArgs e)
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
    }
}