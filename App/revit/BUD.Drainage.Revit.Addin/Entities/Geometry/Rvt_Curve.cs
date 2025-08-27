using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities.Geometry;

/// <summary>
/// wrapper <see cref="Curve"/>
/// </summary>
internal class Rvt_Curve
{
    public Rvt_Curve(Curve curve)
    {
        RvtObject = curve;
        StartPoint = curve.GetEndPoint(0);
        EndPoint = curve.GetEndPoint(1);
        StartParameter = curve.GetEndParameter(0);
        EndParameter = curve.GetEndParameter(1);
        Length = curve.Length;
    }

    /// <summary>
    /// Original object
    /// </summary>
    public Curve RvtObject { get; set; }

    /// <summary>
    /// The start point
    /// </summary>
    public XYZ StartPoint { get; set; }

    /// <summary>
    /// The end point 
    /// </summary>
    public XYZ EndPoint { get; set; }

    /// <summary>
    /// The parameter of start point in the curve
    /// </summary>
    public double StartParameter { get; set; }

    /// <summary>
    /// The parameter of end point in the curve
    /// </summary>
    public double EndParameter { get; set; }

    /// <summary>
    /// The length of the curve, equal endparameter - startparameter
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// implicit <see cref="Rvt_Curve"/>
    /// </summary>
    /// <param name="line"></param>
    public static implicit operator Rvt_Curve(Curve line)
    {
        return new Rvt_Curve(line);
    }

    /// <summary>
    /// implicit <see cref="Curve"/>
    /// </summary>
    /// <param name="line"></param>
    public static implicit operator Curve(Rvt_Curve line)
    {
        return line.RvtObject;
    }
}
