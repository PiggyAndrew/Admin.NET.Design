using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Data.Serialization;

/// <summary>
/// Defined a class to represent a pipe
/// </summary>
public class DSD_Pipe
{
    /// <summary>
    /// Unique number of pipe
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// The pipe start center point
    /// </summary>
    public DSD_Point StartPoint { get; set; }

    /// <summary>
    /// The pipe end center point
    /// </summary>
    public DSD_Point EndPoint { get; set; }

    /// <summary>
    /// The manhole connected to the start point of pipe
    /// </summary>
    public string StartManhole { get; set; }

    /// <summary>
    /// The manhole connnected to the end point of pipe
    /// </summary>
    public string EndManhole { get; set; }

    /// <summary>
    /// The pipe diameter
    /// </summary>
    public double Diameter { get; set; }
}
