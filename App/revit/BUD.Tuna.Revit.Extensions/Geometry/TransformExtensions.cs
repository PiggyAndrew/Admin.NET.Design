using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Geometry;

/// <summary>
/// 矩阵的扩展方法
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// 重新复制一个矩阵
    /// </summary>
    /// <param name="originalTransform">原来的矩阵</param>
    /// <returns>返回一个被复制后的新矩阵</returns>
    public static Transform Duplicate(this Transform originalTransform)
    {
        Transform transform = Transform.Identity;
        transform.BasisX = originalTransform.BasisX;
        transform.BasisY = originalTransform.BasisY;
        transform.BasisZ = originalTransform.BasisZ;
        transform.Origin = originalTransform.Origin;
        return transform;
    }
}
