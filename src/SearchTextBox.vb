Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles

Public Class SearchTextBox
    Inherits UserControl

#Region "Designer"

    Private Sub InitializeComponent()
        Me.Edit = New TextEdit()
        Me.Button = New SearchTextBox.SearchTextBoxButton()
        Me.SuspendLayout()
        '
        'Edit
        '
        Me.Edit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Edit.Location = New System.Drawing.Point(0, 0)
        Me.Edit.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Edit.Name = "Edit"
        Me.Edit.Size = New System.Drawing.Size(200, 70)
        Me.Edit.TabIndex = 3
        '
        'Button
        '
        Me.Button.Location = New System.Drawing.Point(90, 24)
        Me.Button.Name = "Button"
        Me.Button.Size = New System.Drawing.Size(27, 23)
        Me.Button.TabIndex = 2
        Me.Button.Text = "Button1"
        Me.Button.Visible = False
        '
        'SearchTextBox
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.Button)
        Me.Controls.Add(Me.Edit)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "SearchTextBox"
        Me.Size = New System.Drawing.Size(200, 70)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Edit As TextEdit
    Private Button As SearchTextBoxButton

    Public Sub New()
        InitializeComponent()
        SendMessageCue(Edit.TextBox, "Search", False)
        AddHandler Edit.TextChanged, Sub() OnTextChanged(New EventArgs)
        AddHandler Button.Click, Sub() Edit.Text = ""
    End Sub

    Sub SendMessageCue(tb As TextBox, value As String, hideWhenFocused As Boolean)
        Dim wParam = If(hideWhenFocused, 0, 1)
        Const EM_SETCUEBANNER = &H1501
        SendMessage(tb.Handle, EM_SETCUEBANNER, wParam, value)
    End Sub

    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Shared Function SendMessage(hWnd As IntPtr,
                                msg As Int32,
                                wParam As Integer,
                                lParam As String) As IntPtr
    End Function

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        Button.Visible = Edit.Text <> ""
        MyBase.OnTextChanged(e)
    End Sub

    Protected Overrides Sub OnLayout(e As LayoutEventArgs)
        MyBase.OnLayout(e)

        Button.Top = 3
        Button.Height = Height - 6
        Button.Width = Button.Height
        Button.Left = Width - Button.Width - Button.Top

        If Height <> Edit.Height Then Height = Edit.Height
        Edit.Width = Width
    End Sub

    Overrides Property Text As String
        Get
            Return Edit.Text
        End Get
        Set(value As String)
            Edit.Text = value
        End Set
    End Property

    Private Class SearchTextBoxButton
        Inherits Control

        Private MouseIsOver As Boolean

        Protected Overrides Sub OnMouseEnter(eventargs As EventArgs)
            MouseIsOver = True
            Refresh()
            MyBase.OnMouseEnter(eventargs)
        End Sub

        Protected Overrides Sub OnMouseLeave(eventargs As EventArgs)
            MouseIsOver = False
            Refresh()
            MyBase.OnMouseLeave(eventargs)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            Using p = New Pen(Color.DarkSlateGray, 5)
                Dim offset = CSng(Width / 3.3)
                e.Graphics.DrawLine(p, offset, offset, Width - offset, Height - offset)
                e.Graphics.DrawLine(p, Width - offset, offset, offset, Height - offset)
            End Using
        End Sub

        Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
            If MouseIsOver Then
                Dim r = New Rectangle(Point.Empty, Size)

                If VisualStyleInformation.IsEnabledByUser Then
                    Dim Renderer = New VisualStyleRenderer(VisualStyleElement.Button.PushButton.Hot)
                    Renderer.DrawBackground(e.Graphics, ClientRectangle)
                Else
                    Using path = ToolStripRendererEx.CreateRoundRectangle(New Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1), 3)
                        Using b As New LinearGradientBrush(New Point(0, 0), New Point(0, r.Height), Color.White, Color.LightGray)
                            e.Graphics.FillPath(b, path)
                        End Using

                        Using p As New Pen(Brushes.LightGray)
                            e.Graphics.DrawPath(p, path)
                        End Using
                    End Using
                End If
            Else
                e.Graphics.Clear(Color.White)
            End If
        End Sub
    End Class

    Public Class TextEdit
        Inherits UserControl

        Public WithEvents TextBox As New TextBoxEx
        Public Shadows Event TextChanged()
        Private BorderColor As Color = Color.CadetBlue

        Sub New()
            SetStyle(ControlStyles.Opaque Or ControlStyles.ResizeRedraw, True)
            TextBox.BorderStyle = BorderStyle.None
            Controls.Add(TextBox)
            BackColor = Color.White
            AddHandler TextBox.GotFocus, Sub() SetColor(Color.CornflowerBlue)
            AddHandler TextBox.LostFocus, Sub() SetColor(Color.CadetBlue)
            AddHandler TextBox.TextChanged, Sub() RaiseEvent TextChanged()
        End Sub

        Private Sub SetColor(c As Color)
            BorderColor = c
            Invalidate()
        End Sub

        Private TextValue As String

        <BrowsableAttribute(True)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        Overrides Property Text As String
            Get
                Return TextBox.Text
            End Get
            Set(value As String)
                TextBox.Text = value
            End Set
        End Property

        Protected Overrides Sub OnLayout(levent As LayoutEventArgs)
            If TextBox.Multiline Then
                TextBox.Top = 2
                TextBox.Left = 2
                TextBox.Width = ClientSize.Width - 4
                TextBox.Height = ClientSize.Height - 4
            Else
                TextBox.Top = (ClientSize.Height - TextBox.Height) \ 2
                TextBox.Left = 2
                TextBox.Width = ClientSize.Width - 4
                TextBox.Height = TextRenderer.MeasureText("gG", TextBox.Font).Height
            End If

            MyBase.OnLayout(levent)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            Dim r = ClientRectangle
            r.Inflate(-1, -1)

            Using brush As New SolidBrush(BackColor)
                e.Graphics.FillRectangle(If(Enabled, brush, SystemBrushes.Control), r)
            End Using

            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, BorderColor, ButtonBorderStyle.Solid)
            MyBase.OnPaint(e)
        End Sub

        Public Class TextBoxEx
            Inherits TextBox

            <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
            Shadows Property Name() As String
                Get
                    Return MyBase.Name
                End Get
                Set(value As String)
                    MyBase.Name = value
                End Set
            End Property

            <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
            Shadows Property TabIndex As Integer
                Get
                    Return MyBase.TabIndex
                End Get
                Set(value As Integer)
                    MyBase.TabIndex = value
                End Set
            End Property

            Sub SetTextWithoutTextChangedEvent(text As String)
                BlockOnTextChanged = True
                Me.Text = text
                BlockOnTextChanged = False
            End Sub

            Private BlockOnTextChanged As Boolean

            Protected Overrides Sub OnTextChanged(e As EventArgs)
                If Not BlockOnTextChanged Then MyBase.OnTextChanged(e)
            End Sub
        End Class
    End Class
End Class