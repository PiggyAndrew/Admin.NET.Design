using BUD.Tuna.Revit.Extensions.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Ribbon.Abstraction;

/// <summary>
/// 定义可配置的接口
/// </summary>
/// <typeparam name="TRibbonItem"></typeparam>
public interface IRibbonButtonConfigurable<TRibbonItem> where TRibbonItem : IRibbonItem
{
    /// <summary>
    /// 配置按钮信息
    /// </summary>
    /// <param name="config"></param>
    void Configurate(Action<RibbonButtonData> config);
}
