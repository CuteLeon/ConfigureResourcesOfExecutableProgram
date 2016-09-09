Imports System.IO

Public Class ParentProgram
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

    Private Declare Function BeginUpdateResource Lib "kernel32" Alias "BeginUpdateResourceA" (ByVal pFileName As String, ByVal bDeleteExistingResources As Boolean) As Integer
    Private Declare Function UpdateResource Lib "kernel32" Alias "UpdateResourceA" (ByVal hUpdate As IntPtr, ByVal lpType As Integer, ByVal lpName As Integer, ByVal wLanguage As Short, ByVal lpData As Byte(), ByVal cbData As Integer) As Boolean
    Private Declare Function EndUpdateResource Lib "kernel32" Alias "EndUpdateResourceA" (ByVal hUpdate As IntPtr, ByVal fDiscard As Boolean) As Boolean

    ''' <summary>
    ''' 配置EXE子程序
    ''' 如果要写入 文本 请使用 System.Text.Encoding.UTF8.GetBytes() 把文本转换为字节
    ''' 如果要写入 文件 请使用 ReadFileToBytes() 函数把文件转换为字节
    ''' </summary>
    ''' <param name="EXEFilePath">子程序路径</param>
    ''' <param name="FileBytes">要写入的资源的字节</param>
    ''' <param name="ResID">写入的资源在子程序里的标识</param>
    ''' <returns>是否配置成功</returns>
    Private Function WriteResource(ByVal EXEFilePath As String, ByVal FileBytes As Byte(), ByVal ResID As Integer) As Boolean
        Dim hUpdate As IntPtr = BeginUpdateResource(EXEFilePath, False)
        If hUpdate = 0 Then Return False
        If Not (UpdateResource(hUpdate, lpType.RT_RCDATA, ResID, 0, FileBytes, FileBytes.Length)) Then Return False
        If Not (EndUpdateResource(hUpdate, False)) Then Return False
        Return True
    End Function

    ''' <summary>
    ''' 把文件读取到 Byte()
    ''' </summary>
    ''' <param name="FilePath"></param>
    ''' <returns></returns>
    Private Function ReadFileToBytes(ByVal FilePath As String) As Byte()
        Dim ResourceStream As FileStream
        Dim ResourceBytes() As Byte
        Try
            ResourceStream = New FileStream(FilePath, FileMode.Open, FileAccess.Read)
            ReDim ResourceBytes(ResourceStream.Length - 1)
            ResourceStream.Read(ResourceBytes, 0, ResourceStream.Length)
        Catch ex As Exception
            Return New Byte() {}
        End Try
        ResourceStream.Dispose()
        Return ResourceBytes
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim MyDialog As OpenFileDialog = New OpenFileDialog With {
            .CheckFileExists = True,
            .CheckPathExists = True,
            .Multiselect = False,
            .Title = "请选择要写入的资源文件",
            .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            }
        If MyDialog.ShowDialog() <> DialogResult.OK Then Exit Sub
        If WriteResource(Application.StartupPath & "\子程序.exe", ReadFileToBytes(MyDialog.FileName), NumericUpDown1.Value) Then
            MsgBox("文件 " & MyDialog.FileName & " 写入成功！资源号：" & NumericUpDown1.Value)
        Else
            MsgBox("文件 " & MyDialog.FileName & " 写入失败！")
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If WriteResource(Application.StartupPath & "\子程序.exe", System.Text.Encoding.UTF8.GetBytes(TextBox1.Text), NumericUpDown1.Value) Then
            MsgBox("文本 " & TextBox1.Text & " 写入成功！")
        Else
            MsgBox("文本 " & TextBox1.Text & " 写入失败！")
        End If
    End Sub

End Class
