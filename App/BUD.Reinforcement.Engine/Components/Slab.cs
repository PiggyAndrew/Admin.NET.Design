using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Components;

public class Slab : IStructuralComponent
{
    public Slab(double length, double width, double thickness)
    {
        Thickness = thickness;

        Length = length;
        Width = width;
    }

    /// <summary>
    /// length of Slab
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Width of Slab
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Thickness of Slab
    /// </summary>
    public double Thickness { get; set; }

    /// <summary>
    /// Rebar arguments of Slab
    /// </summary>
    public RebarArguments LongSpanRebarArguments { get; set; } = default!;

    /// <summary>
    /// Concrete arguments of Slab
    /// </summary>
    public ConcreteArguments ConcreteArguments { get; set; } = default!;

    public RebarArguments ShortSpanRebarArguments { get; set; } = default!;
}
