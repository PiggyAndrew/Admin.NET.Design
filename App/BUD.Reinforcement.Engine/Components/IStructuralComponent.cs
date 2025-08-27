using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Components;

/// <summary>
/// Define a structural component
/// </summary>
public interface IStructuralComponent
{
    /// <summary>
    /// Section thickness of component
    /// </summary>
    public double Thickness { get; set; }

    /// <summary>
    /// Concrete arguments of component
    /// </summary>
    public ConcreteArguments ConcreteArguments { get; set; }

    /// <summary>
    /// Rebar arguments of component
    /// </summary>
    public RebarArguments LongSpanRebarArguments { get; set; }

    /// <summary>
    /// Rebar arguments of component
    /// </summary>
    public RebarArguments ShortSpanRebarArguments { get; set; }
}
