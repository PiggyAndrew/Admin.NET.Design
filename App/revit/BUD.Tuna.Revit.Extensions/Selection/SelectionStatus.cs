using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Selection;

/// <summary>
/// 用户选择的状态
/// </summary>
public enum SelectionStatus
{
    /// <summary>
    /// 成功
    /// </summary>
    Succeeded,

    /// <summary>
    /// 失败
    /// </summary>
    Failed,

    /// <summary>
    /// 取消
    /// </summary>
    Cancelled
}
