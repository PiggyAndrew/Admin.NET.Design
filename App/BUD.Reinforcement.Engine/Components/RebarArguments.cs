using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Components;

public class RebarArguments
{
    public RebarArguments(double diameter, double spacing, double fyk)
    {
        Diameter = diameter;
        Spacing = spacing;
        Fyk = fyk;
    }

    /// <summary>
    /// 抗拉强度标准值
    /// <para>Characteristic strength of reinforcement</para>
    /// </summary>
    public double Fyk { get; set; }

    /// <summary>
    /// Diameter of rebar
    /// </summary>
    public double Diameter { get; set; }

    /// <summary>
    /// Spacing of rebar
    /// </summary>
    public double Spacing { get; set; }

    /// <summary>
    /// Calculate the cross-section area of rebar
    /// </summary>
    /// <returns>Required area of reinforcement</returns>
    public double GetRebarArea() => MathKit.Area(Diameter) * 1000 / Spacing;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns>Specification of rebar</returns>
    public override string ToString() => $"T{Diameter}-{Spacing}";
}
