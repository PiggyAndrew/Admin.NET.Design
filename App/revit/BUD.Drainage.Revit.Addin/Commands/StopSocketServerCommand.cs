using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using BUD.Revit.Framework;
using BUD.Tuna.Revit.Extensions.Ribbon.Attributes;
using System;

namespace BUD.Drainage.Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [CommandButton(Title = "停止Revit Socket服务")]
    public class StopSocketServerCommand : CommandBase
    {
        public override CommandResult Execute()
        {
            try
            {
                // 调用静态方法停止服务
                RevitSocketServerCommand.StopServer();
                
                TaskDialog.Show("Revit Socket服务", "Revit Socket服务已停止");
                return Succeeded();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("错误", $"停止Socket服务失败：{ex.Message}");
                return Failed();
            }
        }
    }
} 