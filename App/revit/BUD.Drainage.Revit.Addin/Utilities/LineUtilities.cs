using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Utilities;

/// <summary>
/// 直线工具
/// </summary>
internal class LineUtilities
{
    /// <summary>
    /// 通过一个点和向量创建直线线段
    /// </summary>
    /// <param name="startPoint">线段的起点</param>
    /// <param name="direction"></param>
    /// <returns>创建后的线段</returns>
    public static Line CreateLine(XYZ startPoint, XYZ direction)
    {
        return Line.CreateBound(startPoint, startPoint.Add(direction));
    }
}
