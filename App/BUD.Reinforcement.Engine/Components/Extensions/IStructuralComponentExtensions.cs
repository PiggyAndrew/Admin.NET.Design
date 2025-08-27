using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Components.Extensions;

/// <summary>
/// Structural extensions
/// </summary>
public static class IStructuralComponentExtensions
{
    /// <summary>
    /// Get the structual component effective depth
    /// </summary>
    /// <param name="structuralComponent"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">if input arg is null</exception>
    /// <exception cref="InvalidOperationException">if input value is invalid</exception>
    public static double GetEffectiveDepth(this IStructuralComponent structuralComponent, bool isLongSpan)
    {
        if (structuralComponent == null)
        {
            throw new ArgumentNullException("Structual component can not be null");
        }

        RebarArguments rebarArguments = isLongSpan ? structuralComponent.LongSpanRebarArguments : structuralComponent.ShortSpanRebarArguments;

        if (rebarArguments == null)
        {
            throw new ArgumentNullException("Rebar arguments", "Rebar arguments can not be null");
        }

        if (structuralComponent.ConcreteArguments == null)
        {
            throw new ArgumentNullException("Concrete arguments", "Concrete arguments can not be null");
        }

        double diameter = rebarArguments.Diameter;
        double cover = structuralComponent.ConcreteArguments.Cover;

        if (cover == 0)
        {
            throw new InvalidOperationException("cover can not be 0");
        }

        if (diameter == 0)
        {
            throw new InvalidOperationException("diameter can not be 0");
        }

        return structuralComponent.Thickness - cover - diameter - diameter / 2;
    }


}
