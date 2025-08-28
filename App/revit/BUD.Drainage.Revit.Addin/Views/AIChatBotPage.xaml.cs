using Autodesk.Revit.UI;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace BUD.Drainage.Revit.Addin.Views
{
    /// <summary>
    /// Interaction logic for AIChatBotPage.xaml
    /// </summary>
    public partial class AIChatBotPage : Page, IDockablePaneProvider
    {
        private string _distFolderPath;

        public AIChatBotPage()
        {
            InitializeComponent();
            InitializeWebviewAsync();
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            DockablePaneProviderData d = new DockablePaneProviderData();

            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Right;
        }



        private async void InitializeWebviewAsync()
        {
            try
            {
                // 创建WebView2环境
                var options = new CoreWebView2EnvironmentOptions(language: "zh");
                var env = await CoreWebView2Environment.CreateAsync(null, null, options);
                await webView.EnsureCoreWebView2Async(env);

                // 设置进程失败事件处理
                webView.CoreWebView2.ProcessFailed +=
                    (sender, args) => MessageBox.Show("WebView2启动失败，请联系开发人员。\n" + args);

                // 获取dist文件夹路径
                _distFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dist");

                // 添加WebResourceRequested事件处理
                webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;

                // 直接加载index.html文件
                string indexPath = Path.Combine(_distFolderPath, "index.html");
                webView.CoreWebView2.Navigate(new Uri(indexPath).AbsoluteUri);

#if !DEBUG
                // 在非调试模式下禁用开发者工具和浏览器快捷键
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2初始化失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            try
            {
                // 获取请求的URI
                var uri = new Uri(e.Request.Uri);

                // 如果是file协议，尝试从dist文件夹加载资源
                if (uri.Scheme == "file")
                {
                    string filePath = uri.LocalPath;

                    // 如果路径不在dist文件夹内，尝试从相对路径解析
                    if (!filePath.StartsWith(_distFolderPath, StringComparison.OrdinalIgnoreCase))
                    {
                        string relativePath = uri.LocalPath.TrimStart('/');
                        filePath = Path.Combine(_distFolderPath, relativePath);
                    }

                    if (File.Exists(filePath))
                    {
                        // 读取文件内容
                        byte[] fileContent = File.ReadAllBytes(filePath);

                        // 创建响应流
                        var stream = new MemoryStream(fileContent);

                        // 获取MIME类型
                        string mimeType = GetMimeTypeFromFileName(filePath);

                        // 创建响应对象
                        var response = webView.CoreWebView2.Environment.CreateWebResourceResponse(
                            stream, 200, "OK", $"Content-Type: {mimeType}");

                        e.Response = response;
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录异常但不中断执行
                Console.WriteLine($"处理资源请求时出错: {ex.Message}");
            }
        }

        private string GetMimeTypeFromFileName(string fileName)
        {
            string extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".html" or ".htm" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                ".woff" => "font/woff",
                ".woff2" => "font/woff2",
                ".ttf" => "font/ttf",
                ".eot" => "application/vnd.ms-fontobject",
                _ => "application/octet-stream"
            };
        }
    }
}
