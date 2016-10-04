# ConfigureResourcesOfExecutableProgram
配置EXE可执行程序的资源


### 母程序： ###

> __配置子程序的资源：__
    *WriteResource(ByVal EXEFilePath As String, ByVal FileBytes As Byte(), ByVal ResID As Integer) As Boolean*

  > __读取文件到字节数组：__
    *ReadFileToBytes(ByVal FilePath As String) As Byte()*

### 子程序： ###

  > __读取子程序的资源：__
    *ReadResource(ByVal ResID As Integer) As Byte()*

  > __把字节数组转换为图像：__
    *ConvertBytesToBitmap(ByVal Bytes As Byte()) As Bitmap*

  > __把字节数组储存至硬盘文件：__
    *SaveByteToFile(ByVal FileByte As Byte(), FilePath As String) As Boolean*

### 截图
![image](./生成/截图.png)