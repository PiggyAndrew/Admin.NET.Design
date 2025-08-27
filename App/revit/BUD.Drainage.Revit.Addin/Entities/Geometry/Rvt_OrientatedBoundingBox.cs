using Autodesk.Revit.DB;
using BUD.Tuna.Revit.Extensions.Geometry;

namespace BUD.Drainage.Revit.Addin.Entities.Geometry;

/// <summary>
/// 有向包围盒
/// </summary>
internal class Rvt_OrientatedBoundingBox
{
    /// <summary>
    /// 创建一个有向包围盒
    /// </summary>
    /// <param name="transform">包围盒的矩阵</param>
    /// <param name="max">包围盒的顶点最大值</param>
    /// <param name="min">包围盒的顶点最小值</param>
    public Rvt_OrientatedBoundingBox(Transform transform, XYZ max, XYZ min)
    {
        Transform = transform;
        Max = max;
        Min = min;

        Transform.Origin = (min + max) / 2;

        Initialize();
    }


    /// <summary>
    /// 包围盒得矩阵，矩阵原点位于包围盒中心
    /// </summary>
    public Transform Transform { get; set; }

    /// <summary>
    /// 包围盒得最大值
    /// </summary>
    public XYZ Max { get; set; }

    /// <summary>
    /// 包围盒的最小值
    /// </summary>
    public XYZ Min { get; set; }

    /// <summary>
    /// 包围盒X方向的向量
    /// </summary>
    public XYZ XVector { get; set; }

    /// <summary>
    /// 包围盒Y方向的向量
    /// </summary>
    public XYZ YVector { get; set; }

    /// <summary>
    /// 包围盒Z方向的向量
    /// </summary>
    public XYZ ZVector { get; set; }

    /// <summary>
    /// 等比例进行缩放
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public Rvt_OrientatedBoundingBox Scale(double length)
    {
        var maxVector = Max - Min;
        var minVector = Min - Max;
        return new Rvt_OrientatedBoundingBox(Transform, Max.Translate(maxVector, length), Min.Translate(minVector, length));
    }

    /// <summary>
    /// 向外进行扩展
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public Rvt_OrientatedBoundingBox Extend(double length)
    {
        var max = Max.Translate(XVector, length).Translate(YVector, length).Translate(ZVector, length);
        var min = Min.Translate(-XVector, length).Translate(-YVector, length).Translate(-ZVector, length);
        return new Rvt_OrientatedBoundingBox(Transform, max, min);
    }

    /// <summary>
    /// 初始化点位向量
    /// </summary>
    public void Initialize()
    {
        XVector = (Max - Min).DotProduct(Transform.BasisX) * Transform.BasisX;
        YVector = (Max - Min).DotProduct(Transform.BasisY) * Transform.BasisY;
        ZVector = (Max - Min).DotProduct(Transform.BasisZ) * Transform.BasisZ;
    }
}
