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
    public class ShowLatestChatWindowCommand : CommandBase
    {
        // 静态变量保存DockablePane和事件处理器
        private static MainView _chatbotView;

        public override CommandResult Execute()
        {
            try
            {
                UIApplication uiApp = CommandContext.UIApplication;

                // 显示DockablePane
                ShowWindow(uiApp);

                return Succeeded();
            }
            catch (Exception ex)
            {
                return Cancelled();
            }
        }

        // 显示停靠面板
        private static void ShowWindow(UIApplication uiApplication)
        {
            try
            {
                MainView chatbotView = new MainView(uiApplication);
                chatbotView.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"显示面板失败：{ex.Message}", "错误");
                throw;
            }
        }

      
    }
}