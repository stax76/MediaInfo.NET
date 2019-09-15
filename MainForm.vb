Imports System.Text

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
    Friend WithEvents tbSearch As TextBox
    Friend WithEvents tlpMain As TableLayoutPanel
    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.tv = New System.Windows.Forms.TreeView()
        Me.rtb = New System.Windows.Forms.RichTextBox()
        Me.tbSearch = New System.Windows.Forms.TextBox()
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.tlpMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'tv
        '
        Me.tv.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tv.Location = New System.Drawing.Point(0, 61)
        Me.tv.Margin = New System.Windows.Forms.Padding(0)
        Me.tv.Name = "tv"
        Me.tv.Size = New System.Drawing.Size(350, 605)
        Me.tv.TabIndex = 2
        '
        'rtb
        '
        Me.rtb.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rtb.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtb.Location = New System.Drawing.Point(350, 0)
        Me.rtb.Margin = New System.Windows.Forms.Padding(0)
        Me.rtb.Name = "rtb"
        Me.tlpMain.SetRowSpan(Me.rtb, 2)
        Me.rtb.Size = New System.Drawing.Size(456, 666)
        Me.rtb.TabIndex = 4
        Me.rtb.Text = ""
        '
        'tbSearch
        '
        Me.tbSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbSearch.Location = New System.Drawing.Point(0, 0)
        Me.tbSearch.Margin = New System.Windows.Forms.Padding(0)
        Me.tbSearch.Name = "tbSearch"
        Me.tbSearch.Size = New System.Drawing.Size(350, 61)
        Me.tbSearch.TabIndex = 5
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 2
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
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
        Me.tlpMain.Size = New System.Drawing.Size(806, 666)
        Me.tlpMain.TabIndex = 6
        '
        'MainForm
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(288.0!, 288.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(806, 666)
        Me.Controls.Add(Me.tlpMain)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MediaInfo"
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private SourcePath As String
    Private ActiveGroup As String
    Private Items As New List(Of Item)

    Sub New(fp As String)
        MyBase.New()

        InitializeComponent()

        Width = FontHeight * 45
        Height = FontHeight * 35

        tv.ItemHeight = FontHeight * 2

        rtb.WordWrap = False
        rtb.ReadOnly = True
        rtb.BackColor = Color.White
        rtb.Font = New Font("Consolas", 10)

        tv.ShowLines = False
        tv.HideSelection = False
        tv.FullRowSelect = True
        tv.ShowPlusMinus = False

        SourcePath = fp
        Text = "MediaInfo - " + fp + " - " + Application.ProductVersion
        Parse()
        ActiveControl = tbSearch

        AddHandler tbSearch.TextChanged, Sub() If tv.SelectedNode Is tv.Nodes(1) Then UpdateItems() Else tv.SelectedNode = tv.Nodes(1)
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
            Text = "MediaInfo - " + SourcePath + " - " + Application.ProductVersion
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
        tv.Nodes.Clear()
        Items.Clear()

        Dim output = FixBreak(MediaInfo.GetCompleteSummary(SourcePath))
        Dim group = Nothing

        tv.Nodes.Add("Basic")
        tv.Nodes.Add("Advanced")

        For Each i In SplitLinesNoEmpty(output)
            If i.Contains(":") Then
                Dim item As New Item
                item.Name = LeftString(i, ":").Trim
                item.Value = RightString(i, ":").Trim
                item.Group = group
                item.IsComplete = True

                If item.Name Is Nothing Then item.Name = ""
                If item.Value Is Nothing Then item.Value = ""

                Items.Add(item)
            Else
                group = i.Trim
                tv.Nodes.Add(i.Trim)
            End If
        Next

        output = MediaInfo.GetSummary(SourcePath)

        For Each i In SplitLinesNoEmpty(output)
            If i.Contains(":") Then
                Dim item As New Item
                item.Name = LeftString(i, ":").Trim
                item.Value = RightString(i, ":").Trim
                item.Group = group

                If item.Name Is Nothing Then item.Name = ""
                If item.Value Is Nothing Then item.Value = ""

                If item.Name = "File size" AndAlso item.Value.EndsWith("GiB") Then
                    item.Value += " (" + CInt(CLng(MediaInfo.GetGeneral(SourcePath, "FileSize")) / 1024 / 1024).ToString + " MB)"
                End If

                If item.Name = "Unique ID" Then Continue For

                Items.Add(item)
            Else
                group = i.Trim
            End If
        Next

        If tbSearch.Text = "" Then
            tv.SelectedNode = tv.Nodes(0)
        Else
            tv.SelectedNode = tv.Nodes(1)
        End If
    End Sub

    Private Sub Tv_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tv.AfterSelect
        Application.DoEvents()
        ActiveGroup = e.Node.Text
        UpdateItems()
    End Sub

    <STAThread()>
    Shared Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New MainForm(Environment.GetCommandLineArgs(1)))
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
        If e.KeyData = Keys.Escape Then
            If tbSearch.Text <> "" Then
                tbSearch.Text = ""
            Else
                Close()
            End If
        End If
    End Sub
End Class