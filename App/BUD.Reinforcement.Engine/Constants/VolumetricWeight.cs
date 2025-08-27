using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Constants;

/// <summary>
/// Constants of volimetric weight which unit is kg/m³ 
/// </summary>
/// <remarks>
/// 容重的常量
/// </remarks>
public static class VolumetricWeight
{
    /// <summary>
    /// Volumetric weight of water
    /// </summary>
    /// <remarks>
    /// γw
    /// </remarks>
    public const double Water = 9.8;

    /// <summary>
    /// Volumetric weight of soil (saturated unit weight)
    /// </summary>
    /// <remarks>
    /// γs
    /// </remarks>
    public const double Soil = 19;
}
