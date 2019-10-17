Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.Win32

Public Class MainForm
    Inherits Form

#Region " Designer "

    Protected Overloads Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    Friend WithEvents tv As TreeView
    Friend WithEvents rtb As RichTextBox
    Friend WithEvents tbSearch As SearchTextBox
    Friend WithEvents tlpMain As TableLayoutPanel
    Friend WithEvents cms As ContextMenuStrip
    Friend WithEvents CopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents PreviousFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NextFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FileAssociationsToolStripMenuItem As ToolStripMenuItem
    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.tv = New System.Windows.Forms.TreeView()
        Me.rtb = New System.Windows.Forms.RichTextBox()
        Me.cms = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.PreviousFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NextFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.FileAssociationsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tbSearch = New MediaInfoNET.SearchTextBox()
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.cms.SuspendLayout()
        Me.tlpMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'tv
        '
        Me.tv.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tv.Location = New System.Drawing.Point(0, 100)
        Me.tv.Margin = New System.Windows.Forms.Padding(0)
        Me.tv.Name = "tv"
        Me.tv.Size = New System.Drawing.Size(341, 1080)
        Me.tv.TabIndex = 2
        '
        'rtb
        '
        Me.rtb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rtb.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtb.ContextMenuStrip = Me.cms
        Me.rtb.Location = New System.Drawing.Point(341, 0)
        Me.rtb.Margin = New System.Windows.Forms.Padding(0)
        Me.rtb.Name = "rtb"
        Me.tlpMain.SetRowSpan(Me.rtb, 2)
        Me.rtb.Size = New System.Drawing.Size(1142, 1180)
        Me.rtb.TabIndex = 4
        Me.rtb.Text = ""
        '
        'cms
        '
        Me.cms.ImageScalingSize = New System.Drawing.Size(48, 48)
        Me.cms.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyToolStripMenuItem, Me.ToolStripMenuItem1, Me.PreviousFileToolStripMenuItem, Me.NextFileToolStripMenuItem, Me.ToolStripMenuItem2, Me.FileAssociationsToolStripMenuItem, Me.SettingsToolStripMenuItem})
        Me.cms.Name = "cms"
        Me.cms.Size = New System.Drawing.Size(426, 416)
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.AutoSize = False
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+C     "
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(464, 80)
        Me.CopyToolStripMenuItem.Text = "Copy"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(422, 6)
        '
        'PreviousFileToolStripMenuItem
        '
        Me.PreviousFileToolStripMenuItem.AutoSize = False
        Me.PreviousFileToolStripMenuItem.Name = "PreviousFileToolStripMenuItem"
        Me.PreviousFileToolStripMenuItem.ShortcutKeyDisplayString = "F11     "
        Me.PreviousFileToolStripMenuItem.Size = New System.Drawing.Size(464, 80)
        Me.PreviousFileToolStripMenuItem.Text = "Previous File"
        '
        'NextFileToolStripMenuItem
        '
        Me.NextFileToolStripMenuItem.AutoSize = False
        Me.NextFileToolStripMenuItem.Name = "NextFileToolStripMenuItem"
        Me.NextFileToolStripMenuItem.ShortcutKeyDisplayString = "F12     "
        Me.NextFileToolStripMenuItem.Size = New System.Drawing.Size(464, 80)
        Me.NextFileToolStripMenuItem.Text = "Next File"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(422, 6)
        '
        'FileAssociationsToolStripMenuItem
        '
        Me.FileAssociationsToolStripMenuItem.AutoSize = False
        Me.FileAssociationsToolStripMenuItem.Name = "FileAssociationsToolStripMenuItem"
        Me.FileAssociationsToolStripMenuItem.Size = New System.Drawing.Size(464, 80)
        Me.FileAssociationsToolStripMenuItem.Text = "Install/Uninstall"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.AutoSize = False
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+S     "
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(464, 80)
        Me.SettingsToolStripMenuItem.Text = "Settings"
        '
        'tbSearch
        '
        Me.tbSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbSearch.Location = New System.Drawing.Point(0, 0)
        Me.tbSearch.Margin = New System.Windows.Forms.Padding(0)
        Me.tbSearch.Name = "tbSearch"
        Me.tbSearch.Size = New System.Drawing.Size(341, 100)
        Me.tbSearch.TabIndex = 5
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 2
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.0!))
        Me.tlpMain.Controls.Add(Me.tbSearch, 0, 0)
        Me.tlpMain.Controls.Add(Me.tv, 0, 1)
        Me.tlpMain.Controls.Add(Me.rtb, 1, 0)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 2
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.Size = New System.Drawing.Size(1483, 1180)
        Me.tlpMain.TabIndex = 6
        '
        'MainForm
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(288.0!, 288.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1483, 1180)
        Me.Controls.Add(Me.tlpMain)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MediaInfo"
        Me.cms.ResumeLayout(False)
        Me.tlpMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private SourcePath As String
    Private ActiveGroup As String
    Private Items As New List(Of Item)
    Private SettingsFolder As String
    Private SettingsFile As String

    Sub New()
        MyBase.New()

        InitializeComponent()

        AddHandler Application.ThreadException, Sub(sender As Object, e As ThreadExceptionEventArgs)
                                                    HandleException(e.Exception)
                                                End Sub
        Try
            SettingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\" + Application.ProductName + "\"
            SettingsFile = SettingsFolder + "settings.conf"

            If Not Directory.Exists(SettingsFolder) Then
                Directory.CreateDirectory(SettingsFolder)
            End If

            If Not File.Exists(SettingsFile) Then
                Dim content = "
font = Consolas
font-size = 10
window-width = 40
window-height = 35
raw-view = no
"
                File.WriteAllText(SettingsFile, content)
            End If

            ReadSettings()

            For Each item In cms.Items
                If TypeOf item Is ToolStripMenuItem Then
                    Dim menuItem = DirectCast(item, ToolStripMenuItem)
                    menuItem.Height = CInt(FontHeight * 1.7)
                End If
            Next

            ToolStripRendererEx.InitColors(Color.Blue, False, False)
            cms.Renderer = New ToolStripRendererEx()

            rtb.WordWrap = False
            rtb.ReadOnly = True
            rtb.BackColor = Color.White

            tv.ShowLines = False
            tv.HideSelection = False
            tv.FullRowSelect = True
            tv.ShowPlusMinus = False

            If Environment.GetCommandLineArgs.Length > 1 AndAlso File.Exists(Environment.GetCommandLineArgs(1)) Then
                SourcePath = Environment.GetCommandLineArgs(1)
                Text = Application.ProductName + " - " + Environment.GetCommandLineArgs(1) + " - " + Application.ProductVersion
                Parse()
            End If

            ActiveControl = tbSearch

            AddHandler tbSearch.TextChanged, Sub()
                                                 If tv.Nodes.Count = 0 Then Exit Sub

                                                 If tv.SelectedNode Is tv.Nodes(1) Then
                                                     UpdateItems()
                                                 Else
                                                     tv.SelectedNode = tv.Nodes(1)
                                                 End If
                                             End Sub
        Catch ex As Exception
            HandleException(ex)
        End Try
    End Sub

    Sub HandleException(ex As Exception)
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Sub ReadSettings()
        For Each line In File.ReadAllLines(SettingsFile)
            If line.Contains("=") Then
                Dim leftValue = LeftString(line, "=").Trim
                Dim rightValue = RightString(line, "=").Trim

                Try
                    Select Case leftValue
                        Case "font"
                            Font = New Font(rightValue, Font.Size)
                        Case "font-size"
                            Font = New Font(Font.FontFamily, Integer.Parse(rightValue))
                        Case "window-width"
                            Width = Integer.Parse(rightValue) * FontHeight
                        Case "window-height"
                            Height = Integer.Parse(rightValue) * FontHeight
                        Case "raw-view"
                            MediaInfo.RawView = rightValue = "yes"
                    End Select
                Catch ex As Exception
                    MessageBox.Show("Failed to read setting " + leftValue + ".", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        Next
    End Sub

    Sub UpdateItems()
        Dim newText As New StringBuilder
        Dim items As IEnumerable(Of Item)

        If ActiveGroup = "Advanced" Then
            items = Me.Items.Where(Function(i) i.IsComplete)
        ElseIf ActiveGroup = "Basic" Then
            items = Me.Items.Where(Function(i) Not i.IsComplete)
        Else
            Dim l As New List(Of Item)
            l.AddRange(Me.Items.Where(Function(i) Not i.IsComplete AndAlso i.Group = ActiveGroup))
            l.Add(New Item With {.Name = "", .Value = "", .Group = ActiveGroup})
            l.AddRange(Me.Items.Where(Function(i) i.IsComplete AndAlso i.Group = ActiveGroup))
            items = l
        End If

        Dim search = tbSearch.Text.ToLower

        If search <> "" Then items = items.Where(Function(i) i.Name.ToLower.Contains(search) OrElse i.Value.ToLower.Contains(search))
        Dim groups As New List(Of String)

        For Each i In items
            If i.Group <> "" AndAlso Not groups.Contains(i.Group) Then groups.Add(i.Group)
        Next

        For Each i In groups
            If newText.Length = 0 Then
                newText.Append(i + vbCrLf + vbCrLf)
            Else
                newText.Append(vbCrLf + i + vbCrLf + vbCrLf)
            End If

            Dim itemsInGroup = items.Where(Function(v) v.Group = i)

            For Each i3 In itemsInGroup
                If i3.Name <> "" Then
                    newText.Append(i3.Name.PadRight(25))
                    newText.Append(": ")
                End If

                newText.Append(i3.Value)
                newText.Append(vbCrLf)
            Next
        Next

        rtb.Text = newText.ToString
    End Sub

    Protected Overrides Sub OnDragDrop(drgevent As DragEventArgs)
        MyBase.OnDragDrop(drgevent)

        Dim files = TryCast(drgevent.Data.GetData(DataFormats.FileDrop), String())

        If Not NothingOrEmpty(files) Then
            SourcePath = files(0)
            Text = Application.ProductName + " - " + SourcePath + " - " + Application.ProductVersion
            Parse()
        End If
    End Sub

    Protected Overrides Sub OnDragEnter(e As DragEventArgs)
        MyBase.OnDragEnter(e)
        Dim files = TryCast(e.Data.GetData(DataFormats.FileDrop), String())
        If Not NothingOrEmpty(files) Then e.Effect = DragDropEffects.Copy
    End Sub

    Public Class Item
        Property Name As String
        Property Value As String
        Property Group As String
        Property IsComplete As Boolean
    End Class

    Sub Parse()
        tv.BeginUpdate()
        tv.Nodes.Clear()
        Items.Clear()

        Dim output = ""

        Using mi As New MediaInfo(SourcePath)
            output = FixBreak(mi.GetCompleteSummary())
        End Using

        Dim group As String = Nothing

        tv.Nodes.Add("Basic").Tag = "Basic"
        tv.Nodes.Add("Advanced").Tag = "Advanced"

        For Each line In SplitLinesNoEmpty(output)
            If line.Contains(":") Then
                Dim item As New Item
                item.Name = LeftString(line, ":").Trim
                item.Value = RightString(line, ":").Trim
                item.Group = group
                item.IsComplete = True

                If item.Name Is Nothing Then item.Name = ""
                If item.Value Is Nothing Then item.Value = ""

                Items.Add(item)
            Else
                group = line.Trim
                tv.Nodes.Add(line.Trim).Tag = line.Trim
            End If
        Next

        Using mi As New MediaInfo(SourcePath)
            output = FixBreak(mi.GetSummary())
        End Using

        For Each line In SplitLinesNoEmpty(output)
            If line.Contains(":") Then
                Dim item As New Item
                item.Name = LeftString(line, ":").Trim
                item.Value = RightString(line, ":").Trim
                item.Group = group

                If item.Name Is Nothing Then item.Name = ""
                If item.Value Is Nothing Then item.Value = ""

                If item.Name = "File size" AndAlso item.Value.EndsWith("GiB") Then
                    Using mi As New MediaInfo(SourcePath)
                        item.Value += " (" + CInt(CLng(mi.GetGeneral("FileSize")) / 1024 / 1024).ToString + " MB)"
                    End Using
                End If

                If item.Name = "Unique ID" Then
                    Continue For
                End If

                Items.Add(item)
            Else
                group = line.Trim
            End If
        Next

        For Each node As TreeNode In tv.Nodes
            For Each item In Items
                If item.Group = node.Text AndAlso item.Name = "Format" Then
                    node.Text += " (" + item.Value + ")"
                End If
            Next
        Next

        If tbSearch.Text = "" Then
            tv.SelectedNode = tv.Nodes(0)
        Else
            tv.SelectedNode = tv.Nodes(1)
        End If

        Dim itemHeight = CInt(ClientSize.Height / (tv.Nodes.Count + 1))

        If itemHeight > FontHeight * 2.2 Then
            tv.ItemHeight = CInt(FontHeight * 2.2)
        End If

        tv.EndUpdate()
    End Sub

    Private Sub Tv_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tv.AfterSelect
        Application.DoEvents()
        ActiveGroup = e.Node.Tag.ToString
        UpdateItems()
    End Sub

    <STAThread()>
    Shared Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim args = Environment.GetCommandLineArgs().Skip(1)

        If args.Count = 1 AndAlso args(0) = "--install" Then
            Setup(True)
        ElseIf args.Count = 1 AndAlso args(0) = "--uninstall" Then
            Setup(False)
        Else
            Application.Run(New MainForm())
        End If
    End Sub

    Shared Sub Setup(install As Boolean)
        Dim extensions As New List(Of String)
        extensions.AddRange("mpg avi vob mp4 d2v mkv avs 264 mov wmv part flv ifo h264 asf webm dgi mpeg mpv y4m avc hevc 265 h265 m2v m2ts vpy mts webm ts m4v part".Split(" "c))
        extensions.AddRange("mp2 mp3 ac3 wav w64 m4a dts dtsma dtshr dtshd eac3 thd thd+ac3 ogg mka aac opus flac mpa".Split(" "c))
        extensions.AddRange("sub sup idx ass aas srt".Split(" "c))
        extensions.AddRange("png jpg jpeg gif bmp".Split(" "c))

        Try
            If install Then
                For Each ext In extensions
                    Dim filekeyName = RegHelp.GetString("HKCR\." + ext, Nothing)

                    If filekeyName = "" Then
                        RegHelp.SetObject("HKCR\." + ext, Nothing, ext + "file")
                        filekeyName = ext + "file"
                    End If

                    RegHelp.SetObject("HKCR\" + filekeyName + "\shell\MediaInfo.NET", Nothing, "MediaInfo")
                    RegHelp.SetObject("HKCR\" + filekeyName + "\shell\MediaInfo.NET\command", Nothing, $"""{Application.ExecutablePath}"" ""%1""")
                Next

                MessageBox.Show("Install complete", "Install", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                For Each ext In extensions
                    Dim filekeyName = RegHelp.GetString("HKCR\." + ext, Nothing)
                    RegHelp.RemoveKey("HKCR\" + filekeyName + "\shell\MediaInfo.NET")
                Next

                MessageBox.Show("Uninstall complete", "Uninstall", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Shared Function FormatColumn(value As String, delimiter As String) As String
        If value = "" Then Return ""
        Dim lines = SplitKeepEmpty(value, vbCrLf)
        Dim leftSides As New List(Of String)

        For Each i In lines
            Dim pos = i.IndexOf(delimiter)

            If pos > 0 Then
                leftSides.Add(i.Substring(0, pos).Trim)
            Else
                leftSides.Add(i)
            End If
        Next

        Dim highest = Aggregate i In leftSides Into Max(i.Length)
        Dim ret As New List(Of String)

        For i = 0 To lines.Length - 1
            Dim line = lines(i)

            If line.Contains(delimiter) Then
                ret.Add(leftSides(i).PadRight(highest) + " " + delimiter + " " + line.Substring(line.IndexOf(delimiter) + 1).Trim)
            Else
                ret.Add(leftSides(i))
            End If
        Next

        Return Join(ret, vbCrLf)
    End Function

    Shared Function SplitKeepEmpty(value As String, ParamArray delimiters As String()) As String()
        Return value.Split(delimiters, StringSplitOptions.None)
    End Function

    Shared Function Join(instance As IEnumerable(Of String),
                         delimiter As String,
                         Optional removeEmpty As Boolean = False) As String

        If instance Is Nothing Then Return Nothing
        Dim containsEmpty As Boolean

        For Each item In instance
            If item = "" Then
                containsEmpty = True
                Exit For
            End If
        Next

        If containsEmpty AndAlso removeEmpty Then instance = instance.Where(Function(arg) arg <> "")
        Return String.Join(delimiter, instance)
    End Function

    Shared Function FixBreak(value As String) As String
        value = value.Replace(ChrW(13) + ChrW(10), ChrW(10))
        value = value.Replace(ChrW(13), ChrW(10))
        Return value.Replace(ChrW(10), ChrW(13) + ChrW(10))
    End Function

    Shared Function SplitLinesNoEmpty(value As String) As String()
        Return SplitNoEmpty(value, Environment.NewLine)
    End Function

    Shared Function SplitNoEmpty(value As String, ParamArray delimiters As String()) As String()
        Return value.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
    End Function

    Shared Function LeftString(value As String, index As Integer) As String
        If value = "" OrElse index < 0 Then Return ""
        If index > value.Length Then Return value
        Return value.Substring(0, index)
    End Function

    Shared Function LeftString(value As String, start As String) As String
        If value = "" OrElse start = "" Then Return ""
        If Not value.Contains(start) Then Return ""
        Return value.Substring(0, value.IndexOf(start))
    End Function

    Shared Function RightString(value As String, start As String) As String
        If value = "" OrElse start = "" Then Return ""
        If Not value.Contains(start) Then Return ""
        Return value.Substring(value.IndexOf(start) + start.Length)
    End Function

    Shared Function NothingOrEmpty(objects As IEnumerable(Of Object)) As Boolean
        If objects Is Nothing OrElse objects.Count = 0 Then Return True

        For Each i In objects
            If i Is Nothing Then Return True
        Next

        Return Nothing
    End Function

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case e.KeyData
            Case Keys.Escape
                If tbSearch.Text <> "" Then
                    tbSearch.Text = ""
                Else
                    Close()
                End If
            Case Keys.F11
                PreviousFile()
            Case Keys.F12
                NextFile()
            Case Keys.Control Or Keys.S
                ShowSettings()
        End Select
    End Sub

    Private Sub cms_Opening(sender As Object, e As CancelEventArgs) Handles cms.Opening
        CopyToolStripMenuItem.Enabled = rtb.SelectedText <> ""
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        Clipboard.SetText(rtb.SelectedText)
    End Sub

    Sub NextFile()
        Dim files = Directory.GetFiles(Path.GetDirectoryName(SourcePath))
        Dim index = Array.IndexOf(files, SourcePath)
        index += 1
        If index > files.Length - 1 Then index = 0
        SetFile(files(index))
    End Sub

    Sub PreviousFile()
        Dim files = Directory.GetFiles(Path.GetDirectoryName(SourcePath))
        Dim index = Array.IndexOf(files, SourcePath)
        index -= 1
        If index < 0 Then index = files.Length - 1
        SetFile(files(index))
    End Sub

    Sub SetFile(filepath As String)
        SourcePath = filepath
        Text = Application.ProductName + " - " + SourcePath
        Parse()
    End Sub

    Private Sub PreviousFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PreviousFileToolStripMenuItem.Click
        PreviousFile()
    End Sub

    Private Sub NextFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NextFileToolStripMenuItem.Click
        NextFile()
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        ShowSettings()
    End Sub

    Sub ShowSettings()
        Using form As New SettingsForm
            form.Font = Font
            form.tb.Text = File.ReadAllText(SettingsFile)
            form.tb.SelectionStart = 0
            form.tb.SelectionLength = 0
            form.ShowDialog()
            File.WriteAllText(SettingsFile, form.tb.Text)
            ReadSettings()
            SetFile(SourcePath)
        End Using
    End Sub

    Private Sub FileAssociationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FileAssociationsToolStripMenuItem.Click
        Dim result = MessageBox.Show("Click yes to install and no to uninstall.", "Setup", MessageBoxButtons.YesNoCancel)

        If result = DialogResult.Yes Then
            Using proc As New Process
                proc.StartInfo.FileName = Application.ExecutablePath
                proc.StartInfo.Arguments = "--install"
                proc.StartInfo.Verb = "runas"
                proc.Start()
            End Using
        ElseIf result = DialogResult.No Then
            Using proc As New Process
                proc.StartInfo.FileName = Application.ExecutablePath
                proc.StartInfo.Arguments = "--uninstall"
                proc.StartInfo.Verb = "runas"
                proc.Start()
            End Using
        End If
    End Sub

    Public Class RegHelp
        Public Shared Sub SetObject(path As String, name As String, value As Object)
            Using regKey = GetRootKey(path).CreateSubKey(path.Substring(5), RegistryKeyPermissionCheck.ReadWriteSubTree)
                regKey.SetValue(name, value)
            End Using
        End Sub

        Public Shared Function GetString(ByVal path As String, ByVal name As String, ByVal Optional defaultValue As String = "") As String
            Dim val = GetObject(path, name, defaultValue)

            If val Is Nothing OrElse Not (TypeOf val Is String) Then
                Return ""
            End If

            Return val.ToString()
        End Function

        Public Shared Function GetInt(ByVal path As String, ByVal name As String, ByVal Optional defaultValue As Integer = 0) As Integer
            Dim val = GetObject(path, name, defaultValue)

            If val Is Nothing OrElse Not (TypeOf val Is Integer) Then
                Return 0
            End If

            Return CInt(val)
        End Function

        Public Shared Function GetObject(ByVal path As String, ByVal name As String, ByVal Optional defaultValue As Object = Nothing) As Object
            Using regKey = GetRootKey(path).OpenSubKey(path.Substring(5))
                If Not regKey Is Nothing Then
                    Return regKey.GetValue(name, defaultValue)
                End If
            End Using
        End Function

        Public Shared Sub RemoveKey(ByVal path As String)
            Try
                GetRootKey(path).DeleteSubKeyTree(path.Substring(5), False)
            Catch
            End Try
        End Sub

        Private Shared Function GetRootKey(ByVal path As String) As RegistryKey
            Select Case path.Substring(0, 4)
                Case "HKLM"
                    Return Registry.LocalMachine
                Case "HKCU"
                    Return Registry.CurrentUser
                Case "HKCR"
                    Return Registry.ClassesRoot
                Case Else
                    Throw New Exception()
            End Select
        End Function
    End Class
End Class