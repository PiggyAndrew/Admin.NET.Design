using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Components;

/// <summary>
/// Wall
/// </summary>
public class Wall : IStructuralComponent
{
    public Wall(double length, double height, double thickness)
    {
        Length = length;
        Height = height;
        Thickness = thickness;
    }

    /// <summary>
    /// The shorter side length of wall
    /// </summary>
    public double Length { get; }

    /// <summary>
    /// The Longger side length of wall
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// The wall thickness
    /// </summary>
    public double Thickness { get; set; }

    /// <summary>
    /// The rebar arguments of wall
    /// </summary>
    public RebarArguments LongSpanRebarArguments { get; set; } = default!;

    public RebarArguments ShortSpanRebarArguments { get; set; } = default!;

    /// <summary>
    /// Concrete arguments of wall
    /// </summary>
    public ConcreteArguments ConcreteArguments { get; set; } = default!;
}
