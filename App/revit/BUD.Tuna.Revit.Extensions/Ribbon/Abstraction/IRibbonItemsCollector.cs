using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Ribbon.Abstraction;

/// <summary>
/// 定义具有元素集合的接口
/// </summary>
public interface IRibbonItemsCollector
{
    /// <summary>
    /// 获取对象下面的集合
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IRibbonItem> GetItems();
}
