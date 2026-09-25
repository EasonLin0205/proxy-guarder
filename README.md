# Proxy Guarder

Proxy Guarder 是一个 Windows 网络环境拦截启动器。它会在启动用户指定的代理工具前，读取当前 WiFi 名称，并根据网络拦截名单决定是否允许启动。

## 技术栈

- C#：主要开发语言
- Windows Forms：桌面图形界面
- .NET Windows Desktop：Windows 桌面应用运行环境
- System.Windows.Forms / System.Drawing：窗口、控件和界面样式
- System.Web.Extensions / JavaScriptSerializer：配置文件 JSON 读写
- Windows netsh：读取当前 WiFi SSID
- Git / GitHub Releases：源码管理与软件发布

## 工作方式

1. Proxy Guarder 启动。
2. 读取当前连接的 WiFi 名称。
3. 检查网络名称是否位于拦截名单。
4. 如果命中名单，显示红色提示并倒计时退出，不启动代理工具。
5. 如果未命中，用户确认后启动代理工具。

## 基本使用

### 1. 启动程序

运行 ProxyGuarder/ProxyGuarder.exe。

### 2. 首次配置代理工具

1. 点击“设置”。
2. 点击“浏览”。
3. 选择需要保护的代理工具 .exe 文件。
4. 点击“保存”。

代理工具路径允许为空。如果为空，程序仍可以保存网络拦截名单，但点击启动时需要先配置有效的代理工具路径。

### 3. 管理拦截名单

在设置窗口中可以添加网络名称、删除选中项、清空自定义网络名称并保存配置。

jxufe-wifi 是内置拦截项，程序会自动保留，不会被清除。

### 4. 日常启动方式

建议将原来启动代理工具的快捷方式目标改为 ProxyGuarder.exe，之后通过 Proxy Guarder 启动代理工具。

## 配置文件

配置文件位置：%APPDATA%\ProxyGuarder\config.json

配置内容包括代理工具路径和网络拦截名单。

## 项目目录

- ProxyGuarder/：主程序源码、图标和编译后的 exe
- tools/：网络名称检测辅助工具
- ProxyGuarder.csproj：项目文件
- README.md：项目说明

## 注意事项

- 程序目前主要检测 WiFi SSID，不负责识别有线网络名称。
- 运行程序需要 Windows 环境。
- 未签名 exe 可能触发 Windows SmartScreen 提示。
