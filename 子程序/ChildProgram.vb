Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Public Class ChildProgram
    Private Enum lpType
        RT_CURSOR = 1 '由硬件支持的光标资源
        RT_BITMAP = 2 '位图资源
        RT_ICON = 3 '由硬件支持的图标资源
        RT_MENU = 4 '菜单资源
        RT_DIALOG = 5 '对话框
        RT_STRING = 6 '字符表入口
        RT_FONTDIR = 7 '字体目录资源
        RT_FONT = 8 '字体资源
        RT_ACCELERATOR = 9 '加速器表
        RT_MESSAGETABLE = 11 '消息表的入口
        RT_GROUP_CURSOR = 12 '与硬件无关的光标资源
        RT_GROUP_ICON = 14 ' 与硬件无关的目标资源
        RT_VERSION = 16 '版本资源
        RT_PLUGPLAY = 19 '即插即用资源
        RT_VXD = 20 'VXD
        RT_ANICURSOR = 21 '动态光标
        RT_ANIICON = 22 '动态图标
        RT_HTML = 23 'HTML文档
        '——< 其实我只用下面的一个> ——
        RT_RCDATA = 10 '原始数据或自定义资源
    End Enum

    Private Declare Function GetModuleHandle Lib "kernel32" Alias "GetModuleHandleA" (ByVal lpModuleName As String) As Integer
    Private Declare Function FindResource Lib "kernel32" Alias "FindResourceA" (ByVal hModule As IntPtr, ByVal lpName As Integer, ByVal lpType As Integer) As IntPtr
    Private Declare Function LoadResource Lib "kernel32" Alias "LoadResource" (ByVal hModule As IntPtr, ByVal hResInfo As IntPtr) As IntPtr
    Private Declare Function SizeofResource Lib "kernel32" Alias "SizeofResource" (ByVal hModule As IntPtr, ByVal hResInfo As IntPtr) As Integer

    ''' <summary>
    ''' 把 Byte() 转换为图像
    ''' </summary>
    ''' <param name="Bytes"></param>
    ''' <returns></returns>
    Private Function ConvertBytesToBitmap(ByVal Bytes As Byte()) As Bitmap
        Dim TempStream As MemoryStream = New MemoryStream
        Try
            TempStream.Write(Bytes, 0, Bytes.Length)
        Catch ex As Exception
            Return Nothing
        End Try
        Return Bitmap.FromStream(TempStream)
    End Function

    ''' <summary>
    ''' 读取被写入的资源字节
    ''' 如果要读取 文本 请使用 System.Text.Encoding.UTF8.GetString() 把字节转换为文本
    ''' 如果要读取 文件 请使用 SaveByteToFile() 函数把文件保存至硬盘
    ''' 如果要读取 图像 请使用 ConvertBytesToBitmap 直接使用图像
    ''' </summary>
    ''' <param name="ResID">资源标识</param>
    ''' <returns></returns>
    Private Function ReadResource(ByVal ResID As Integer) As Byte()
        Dim hModule As IntPtr
        Dim ResourceInfo As IntPtr
        Dim ResourceSize As Integer
        Dim Source As IntPtr
        Dim ResourceByte() As Byte
        Try
            hModule = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName)
            ResourceInfo = FindResource(hModule, ResID, lpType.RT_RCDATA)
            ResourceSize = SizeofResource(hModule, ResourceInfo)
            Source = LoadResource(hModule, ResourceInfo)
            ReDim ResourceByte(ResourceSize - 1)
            Marshal.Copy(Source, ResourceByte, 0, ResourceSize)
        Catch ex As Exception
            Return New Byte() {}
        End Try
        Return ResourceByte
    End Function

    ''' <summary>
    ''' 把 Byte() 储存至硬盘文件
    ''' </summary>
    ''' <param name="FileByte">传入的字节</param>
    ''' <param name="FilePath">保存的文件路径</param>
    ''' <returns></returns>
    Private Function SaveByteToFile(ByVal FileByte As Byte(), FilePath As String) As Boolean
        If FileByte.Length = 0 Then Return False
        Dim ResourceStream As FileStream
        Try
            ResourceStream = New FileStream(FilePath, FileMode.Create, FileAccess.Write)
            ResourceStream.Write(FileByte, 0, FileByte.Length)
            ResourceStream.Close()
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim TempResource As Byte() = ReadResource(NumericUpDown1.Value)
        If TempResource.Length = 0 Then
            MsgBox("读取资源失败！资源可能不存在！")
        Else
            Try
                MsgBox("文本读取成功！" & vbCrLf & vbCrLf & Encoding.UTF8.GetString(TempResource))
            Catch ex As Exception
                MsgBox("文本转换编码失败！")
            End Try
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim TempResource As Byte() = ReadResource(NumericUpDown1.Value)
        If TempResource.Length = 0 Then
            MsgBox("读取资源失败！资源可能不存在！")
        Else
            If SaveByteToFile(TempResource, NumericUpDown1.Value & ".jpg") Then
                MsgBox("资源号 " & NumericUpDown1.Value & " 保存成功！")
            Else
                MsgBox("资源号 " & NumericUpDown1.Value & " 保存失败！")
            End If
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim TempResource As Byte() = ReadResource(NumericUpDown1.Value)
        If TempResource.Length = 0 Then
            MsgBox("读取资源失败！资源可能不存在！")
        Else
            Dim TempBitmap As Bitmap = ConvertBytesToBitmap(TempResource)
            If TempBitmap Is Nothing Then
                MsgBox("图像加载失败！资源可能不是图像类型，或资源不存在！")
            Else
                Me.BackgroundImage = TempBitmap
                MsgBox("资源是图像类型！已经加载到窗体背景！")
            End If
        End If
    End Sub
End Class
