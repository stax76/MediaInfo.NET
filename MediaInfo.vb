Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions

Public Class MediaInfo
    Implements IDisposable

    Private Handle As IntPtr
    Private Shared Loaded As Boolean
    Shared Cache As New Dictionary(Of String, MediaInfo)

    Sub New(path As String)
        If Not Loaded Then
            Loaded = True
            LoadLibrary("MediaInfo.dll")
        End If

        Handle = MediaInfo_New()
        MediaInfo_Open(Handle, path)
    End Sub

    Shared Function GetMediaInfo(path As String) As MediaInfo
        If path = "" Then Return Nothing
        Dim key = path & File.GetLastWriteTime(path).Ticks
        If Cache.ContainsKey(key) Then Return Cache(key)
        Dim ret As New MediaInfo(path)
        Cache(key) = ret
        Return ret
    End Function

    Function GetGeneral(parameter As String) As String
        Return GetInfo(MediaInfoStreamKind.General, parameter)
    End Function

    Shared Function GetGeneral(path As String, parameter As String) As String
        Return GetInfo(path, MediaInfoStreamKind.General, parameter)
    End Function

    Function GetInfo(streamKind As MediaInfoStreamKind, parameter As String) As String
        Return Marshal.PtrToStringUni(MediaInfo_Get(Handle, streamKind, 0, parameter, MediaInfoInfoKind.Text, MediaInfoInfoKind.Name))
    End Function

    Shared Function GetInfo(path As String, streamKind As MediaInfoStreamKind, parameter As String) As String
        If path = "" Then Return ""
        Return GetMediaInfo(path).GetInfo(streamKind, parameter)
    End Function

    Shared Function GetSummary(path As String) As String
        Dim mi = GetMediaInfo(path)
        MediaInfo_Option(mi.Handle, "Complete", "0")
        Dim ret = Marshal.PtrToStringUni(MediaInfo_Inform(mi.Handle, 0))
        If ret.Contains("UniqueID/String") Then ret = Regex.Replace(ret, "UniqueID/String +: .+\n", "")
        If ret.Contains("Unique ID") Then ret = Regex.Replace(ret, "Unique ID +: .+\n", "")
        If ret.Contains("Encoded_Library_Settings") Then ret = Regex.Replace(ret, "Encoded_Library_Settings +: .+\n", "")
        If ret.Contains("Encoding settings") Then ret = Regex.Replace(ret, "Encoding settings +: .+\n", "")
        If ret.Contains("Format settings, ") Then ret = ret.Replace("Format settings, ", "Format, ")

        Return MainForm.FormatColumn(ret, ":").Trim
    End Function

    Shared Function GetCompleteSummary(path As String) As String
        Dim mi = GetMediaInfo(path)
        MediaInfo_Option(mi.Handle, "Complete", "1")
        Return Marshal.PtrToStringUni(MediaInfo_Inform(mi.Handle, 0))
    End Function

#Region "IDisposable"

    Private Disposed As Boolean

    Sub Dispose() Implements IDisposable.Dispose
        If Not Disposed Then
            Disposed = True
            MediaInfo_Close(Handle)
            MediaInfo_Delete(Handle)
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Dispose()
    End Sub

#End Region

#Region "native"
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)>
    Shared Function LoadLibrary(path As String) As IntPtr
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Function MediaInfo_New() As IntPtr
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Sub MediaInfo_Delete(Handle As IntPtr)
    End Sub

    <DllImport("MediaInfo.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function MediaInfo_Open(Handle As IntPtr, FileName As String) As Integer
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Function MediaInfo_Close(Handle As IntPtr) As Integer
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Function MediaInfo_Inform(Handle As IntPtr, Reserved As Integer) As IntPtr
    End Function

    <DllImport("MediaInfo.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function MediaInfo_Get(
        Handle As IntPtr,
        StreamKind As MediaInfoStreamKind,
        StreamNumber As Integer, Parameter As String,
        KindOfInfo As MediaInfoInfoKind,
        KindOfSearch As MediaInfoInfoKind) As IntPtr
    End Function

    <DllImport("MediaInfo.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function MediaInfo_Option(Handle As IntPtr, OptionString As String, Value As String) As IntPtr
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Function MediaInfo_State_Get(Handle As IntPtr) As Integer
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Function MediaInfo_Count_Get(Handle As IntPtr, StreamKind As MediaInfoStreamKind, StreamNumber As Integer) As Integer
    End Function
#End Region

End Class

Public Enum MediaInfoStreamKind
    General
    Video
    Audio
    Text
    Other
    Image
    Menu
    Max
End Enum

Public Enum MediaInfoInfoKind
    Name
    Text
    Measure
    Options
    NameText
    MeasureText
    Info
    HowTo
End Enum