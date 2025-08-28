using Autodesk.Revit.UI;
using BUD.Drainage.Revit.Addin.Commands;
using BUD.Drainage.Revit.Addin.Models;
using BUD.Drainage.Revit.Addin.Models.Arguments;
using BUD.Tuna.Revit.Extensions.ExternalEvent;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
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
    public partial class MainView : Window
    {
        private string _distFolderPath;
        private readonly IExternalEventService _externalEventService;
        public UIApplication UiApplication { get; }



        public MainView(UIApplication uiApplication)
        {
            InitializeComponent();
            UiApplication = uiApplication;
            _externalEventService = new ExternalEventService();
            // 使用Dispatcher延迟初始化WebView2，避免资源状态问题
            Dispatcher.BeginInvoke(new Action(() =>
            {
                InitializeWebviewAsync();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }


        // 在InitializeWebviewAsync方法中，添加消息接收处理
        private async void InitializeWebviewAsync()
        {
            try
            {
                // 获取dist文件夹路径
                _distFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dist");

                // 确保dist文件夹存在
                if (!Directory.Exists(_distFolderPath))
                {
                    MessageBox.Show($"Web资源文件夹不存在: {_distFolderPath}", "错误");
                    return;
                }

                try
                {
                    // 尝试使用临时文件夹作为用户数据文件夹
                    string tempFolder = Path.Combine(Path.GetTempPath(), "RevitAIBuilder_" + Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempFolder);

                    // 使用临时文件夹创建WebView2环境
                    var options = new CoreWebView2EnvironmentOptions();
                    options.AdditionalBrowserArguments = "--no-sandbox";

                    var env = await CoreWebView2Environment.CreateAsync(null, tempFolder, options);
                    await webView.EnsureCoreWebView2Async(env);

                    // 配置WebView2设置
                    webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                    webView.CoreWebView2.WebMessageReceived += Chatbot_WebMessageReceived; webView.CoreWebView2.Settings.AreHostObjectsAllowed = true;
                    webView.CoreWebView2.Settings.IsScriptEnabled = true;
                    // 使用直接文件加载
                    string indexPath = Path.Combine(_distFolderPath, "index.html");
                    if (File.Exists(indexPath))
                    {
                        // 直接使用file://协议加载
                        webView.Source = new Uri(indexPath);

                        // 隐藏加载指示器
                        this.loadingText.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageBox.Show($"找不到index.html文件: {indexPath}", "错误");
                    }

                    // 添加导航完成事件处理
                    webView.NavigationCompleted += (sender, args) =>
                    {
                        if (args.IsSuccess)
                        {
                            // 隐藏加载指示器
                            this.loadingText.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            MessageBox.Show($"页面加载失败: {args.WebErrorStatus}", "导航错误");
                            // 如果导航失败，尝试使用备用方案
                            FallbackToBrowserControl();
                        }
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"WebView2核心初始化失败: {ex.Message}", "WebView2错误");

                    // 尝试使用系统WebBrowser控件作为备用方案
                    FallbackToBrowserControl();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2初始化失败: {ex.Message}\n{ex.StackTrace}", "WebView2错误");
                FallbackToBrowserControl();
            }
        }

        // 添加一个新方法用于回退到传统WebBrowser控件
        private void FallbackToBrowserControl()
        {
            try
            {
                // 创建一个简单的浏览器控件作为备用
                System.Windows.Controls.WebBrowser browser = new System.Windows.Controls.WebBrowser();
                browser.HorizontalAlignment = HorizontalAlignment.Stretch;
                browser.VerticalAlignment = VerticalAlignment.Stretch;

                // 替换WebView2
                Grid parentGrid = webView.Parent as Grid;
                if (parentGrid != null)
                {
                    int index = parentGrid.Children.IndexOf(webView);
                    if (index >= 0)
                    {
                        parentGrid.Children.RemoveAt(index);
                        parentGrid.Children.Insert(index, browser);
                    }
                }

                // 加载HTML文件
                string indexPath = Path.Combine(_distFolderPath, "index.html");
                if (File.Exists(indexPath))
                {
                    browser.Navigate(new Uri(indexPath));
                    this.loadingText.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception fallbackEx)
            {
                MessageBox.Show($"备用浏览器也初始化失败: {fallbackEx.Message}", "严重错误");
            }
        }

       

        // 添加消息接收处理方法
        // 添加WebMessageReceived事件
        public event EventHandler<CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived;

        // 添加发送消息到WebView的方法
        public void SendWebMessage(string message)
        {
            try
            {
                if (webView != null && webView.CoreWebView2 != null)
                {
                    // 使用PostWebMessageAsString方法发送消息
                    webView.CoreWebView2.PostWebMessageAsString(message);
                    Console.WriteLine($"已发送消息到WebView: {message}");
                }
                else
                {
                    Console.WriteLine("WebView或CoreWebView2为空，无法发送消息");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送消息到WebView时出错: {ex.Message}");
            }
        }



        private void Chatbot_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonMessage = e.WebMessageAsJson;

                // 处理接收到的消息
                ProcessWebMessageWithExternalEvent(jsonMessage);
            }
            catch (Exception ex)
            {

            }
        }

        // 使用外部事件处理Web消息
        private async void ProcessWebMessageWithExternalEvent(string jsonMessage)
        {
            // 首先检查是否是双重转义的JSON字符串
            if (jsonMessage.StartsWith("\"") && jsonMessage.EndsWith("\""))
            {
                // 解析外层JSON字符串，获取内部的实际JSON
                jsonMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(jsonMessage);
            }

            // 解析命令
            RevitCommandModel command = CommandParser.ParseCommand(jsonMessage);

            try
            {

                // 处理工具调用
                if (command.ToolCalls != null && command.ToolCalls.Count > 0)
                {
                    foreach (var toolCall in command.ToolCalls)
                    {
                        // 创建参数类实例
                        var arguments = FunctionArgumentsFactory.Create(toolCall.Function);

                        // 使用外部事件执行命令
                        var result = await _externalEventService.PostCommandAsync<RevitCommandResult>(app =>
                        {
                            return arguments.Execute(app.ActiveUIDocument.Document);
                        });

                        // 检查是否有异常
                        if (result.HasException)
                        {
                            toolCall.Result = RevitCommandResult.Error($"执行命令时出错: {result.Exception.Message}");
                        }
                        else
                        {
                            toolCall.Result = result.Value;
                        }
                    }
                }

                // 序列化响应并发送回WebView
                var response = Newtonsoft.Json.JsonConvert.SerializeObject(command);
#if RVT_24_DEBUG
                //ExportCommandToJson(command);
#endif
                this.SendWebMessage(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理命令时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);

                // 发送错误响应回WebView
                var errorResponse = command;
                this.SendWebMessage(Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse));
            }
        }


        public void ProcessWebMessage(string jsonMessage)
        {
            // 首先检查是否是双重转义的JSON字符串
            if (jsonMessage.StartsWith("\"") && jsonMessage.EndsWith("\""))
            {
                // 解析外层JSON字符串，获取内部的实际JSON
                jsonMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(jsonMessage);
            }

            // 解析命令
            RevitCommandModel command = CommandParser.ParseCommand(jsonMessage);
            // 处理第一个工具调用
            if (command.ToolCalls != null && command.ToolCalls.Count > 0)
            {
                foreach (var toolCall in command.ToolCalls)
                {

                    // 直接使用FunctionModel创建参数类实例
                    var arguments = FunctionArgumentsFactory.Create(toolCall.Function);

                    // 执行命令
                    toolCall.Result = arguments.Execute(UiApplication.ActiveUIDocument.Document);
                }
            }
            var response = Newtonsoft.Json.JsonConvert.SerializeObject(command);
#if RVT_24_DEBUG
            ExportCommandToJson(command);
#endif
            this.SendWebMessage(response);
        }


        private void ExportCommandToJson(RevitCommandModel command)
        {
            try
            {
                // 获取下载文件夹路径
                string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                // 创建文件名（使用时间戳确保唯一性）
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"revit_command_{timestamp}.json";
                string filePath = Path.Combine(downloadsPath, fileName);

                // 序列化命令对象为 JSON
                var response = Newtonsoft.Json.JsonConvert.SerializeObject(command, Newtonsoft.Json.Formatting.Indented);

                // 写入文件
                File.WriteAllText(filePath, response, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出命令失败: {ex.Message}", "导出错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
