using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using BUD.Drainage.Revit.Addin.Models;
using BUD.Drainage.Revit.Addin.Models.Arguments;
using BUD.Revit.Framework;
using BUD.Tuna.Revit.Extensions.Collection;
using BUD.Tuna.Revit.Extensions.Extensions;
using BUD.Tuna.Revit.Extensions.ExternalEvent;
using BUD.Tuna.Revit.Extensions.Ribbon.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [CommandButton(Title = "启动Revit Socket服务")]
    public class RevitSocketServerCommand : CommandBase
    {
        private static TcpListener _server;
        private static bool _isRunning = false;
        private static Thread _serverThread;
        private static UIApplication _uiApp;
        private IExternalEventService _externalEventService;

        public override CommandResult Execute()
        {
            try
            {
                _uiApp = CommandContext.UIApplication;

                _externalEventService = new ExternalEventService();

                if (!_isRunning)
                {
                    // 启动服务器
                    _isRunning = true;
                    _serverThread = new Thread(StartSocketServer);
                    _serverThread.IsBackground = true;
                    _serverThread.Start();
                    
                    TaskDialog.Show("Revit Socket服务", "Revit Socket服务已启动，监听端口：8080");
                    return Succeeded();
                }
                else
                {
                    TaskDialog.Show("Revit Socket服务", "服务已在运行中");
                    return Succeeded();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("错误", $"启动Socket服务失败：{ex.Message}");
                return Failed();
            }
        }

        private void StartSocketServer()
        {
            try
            {
                _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
                _server.Start();

                while (_isRunning)
                {
                    try
                    {
                        // 等待客户端连接
                        TcpClient client = _server.AcceptTcpClient();
                        
                        // 处理请求
                        Task.Run(() => HandleClientRequest(client));
                    }
                    catch (Exception ex)
                    {
                        // 记录异常但继续运行
                        System.Diagnostics.Debug.WriteLine($"Socket服务异常：{ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Socket服务严重错误：{ex.Message}");
                _isRunning = false;
            }
            finally
            {
                if (_server != null)
                {
                    _server.Stop();
                }
            }
        }

        private async void HandleClientRequest(TcpClient client)
        {
            using (client)
            {
                try
                {
                    // 获取网络流
                    NetworkStream stream = client.GetStream();
                    
                    // 读取客户端发送的数据
                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string requestString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    // 解析请求
                    JObject request = JObject.Parse(requestString);
                    string command = request["command"].ToString();
                    JObject args = (JObject)request["args"];
                    
                    // 处理命令并获取响应
                    object response =await ProcessCommandAsync(command, args);
                    
                    // 将响应转换为JSON并发送回客户端
                    string responseString = JsonConvert.SerializeObject(response);
                    byte[] responseData = Encoding.UTF8.GetBytes(responseString);
                    stream.Write(responseData, 0, responseData.Length);
                }
                catch (Exception ex)
                {
                    // 处理异常
                    string errorResponse = JsonConvert.SerializeObject(new { error = ex.Message });
                    byte[] errorData = Encoding.UTF8.GetBytes(errorResponse);
                    client.GetStream().Write(errorData, 0, errorData.Length);
                }
            }
        }

         private async Task<object> ProcessCommandAsync(string command, JObject args)
        {
            try
            {
                var arguments = FunctionArgumentsFactory.Create(command,args);

                RevitCommandResult result = arguments.Execute(CommandContext.Document);

                return result.Content;
            }
            catch (Exception ex)
            {
                return new { error = $"执行命令出错: {ex.Message}" };
            }
        }


        // 提供一个停止服务的方法
        public static void StopServer()
        {
            if (_isRunning)
            {
                _isRunning = false;
                if (_server != null)
                {
                    _server.Stop();
                }
            }
        }
    }
} 