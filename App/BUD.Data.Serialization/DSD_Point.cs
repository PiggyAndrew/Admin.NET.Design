using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Data.Serialization;

/// <summary>
/// Defined a class to represent 3D point
/// </summary>
public class DSD_Point
{
    /// <summary>
    /// Create a zero point 
    /// </summary>
    public DSD_Point()
    {
        X = 0; Y = 0; Z = 0;
    }

    /// <summary>
    /// Create a point
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public DSD_Point(double x, double y, double z)
    {
        X = x; Y = y; Z = z;
    }

    /// <summary>
    /// X value of the point
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Y value of the point
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Z value of the point
    /// </summary>
    public double Z { get; set; }

    /// <summary>
    /// Original point
    /// </summary>
    public static DSD_Point Zero => new DSD_Point(0, 0, 0);
}
