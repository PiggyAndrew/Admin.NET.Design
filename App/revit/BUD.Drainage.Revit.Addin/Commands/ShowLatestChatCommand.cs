using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BUD.Drainage.Revit.Addin.Models;
using BUD.Drainage.Revit.Addin.Views;
using BUD.Revit.Framework;
using BUD.Tuna.Revit.Extensions.Ribbon.Attributes;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;

namespace BUD.Drainage.Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [CommandButton(Title = "AI代码生成器")]
    public class ShowLatestChatCommand : CommandBase
    {
        // 静态变量保存DockablePane和事件处理器
        private static DockablePaneId _dockablePaneId = new DockablePaneId(new Guid("B6F6F66F-4B4E-56D4-C287-8B6C31EC26F3"));
        private static ChatbotPage _dockablePane;

        public override CommandResult Execute()
        {
            try
            {
                UIApplication uiApp = CommandContext.UIApplication;

                // 显示DockablePane
                ShowDockablePane(uiApp);

                return Succeeded();
            }
            catch (Exception ex)
            {
                return Cancelled();
            }
        }


        // 修改注册停靠面板方法
        public static void RegisterDockablePane(UIControlledApplication uiApplication)
        {
            try
            {

                var flag = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod;

                var uiApp = (Autodesk.Revit.UI.UIApplication)uiApplication.GetType().InvokeMember("getUIApplication", flag, Type.DefaultBinder, uiApplication, null);
                _dockablePane = new ChatbotPage(uiApp);

                // 创建停靠面板

                // 设置停靠面板属性
                DockablePaneProviderData data = new DockablePaneProviderData();
                data.FrameworkElement = _dockablePane;
                data.InitialState = new DockablePaneState
                {
                    DockPosition = DockPosition.Right,
                    MinimumWidth = 300,
                    MinimumHeight = 400
                };

                // 注册停靠面板
                uiApplication.RegisterDockablePane(_dockablePaneId, "AI代码生成器", (IDockablePaneProvider)_dockablePane);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"注册面板失败：{ex.Message}", "错误");
                throw;
            }
        }

        // 检查面板是否已注册
        public static bool IsPaneRegistered(UIControlledApplication uiApplication)
        {
            try
            {
                DockablePane pane = uiApplication.GetDockablePane(_dockablePaneId);
                return pane != null;
            }
            catch
            {
                return false;
            }
        }

        // 显示停靠面板
        private static void ShowDockablePane(UIApplication uiApplication)
        {
            try
            {
                DockablePane dockablePane = uiApplication.GetDockablePane(_dockablePaneId);
                dockablePane.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"显示面板失败：{ex.Message}", "错误");
                throw;
            }
        }

      
    }
}