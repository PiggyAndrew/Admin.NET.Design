using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Data.Serialization;

/// <summary>
/// Defind a class to represent a manhole
/// </summary>
public class DSD_Manhole
{
    /// <summary>
    /// Unique number of manhole
    /// </summary>
    public string Number { get; set; } = default!;

    /// <summary>
    /// The manhole placement location
    /// </summary>
    public DSD_Point Location { get; set; } = default!;

    /// <summary>
    /// The manhole type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The manhole width size
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// The manhole length size
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// The mahole depth size
    /// </summary>
    public double Depth { get; set; }

    /// <summary>
    /// The rotation of the manhole
    /// </summary>
    public double Rotation { get; set; }
}
