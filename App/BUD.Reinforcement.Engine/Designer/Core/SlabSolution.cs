using DSD.Reinforcement.Engine.Components;
using DSD.Reinforcement.Engine.Designer.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer.Core;

public class SlabSolution : ICalculator<CalculationResult[]>
{
    public SlabSolution(Slab slab, params Load[] loadings)
    {
        SlabReinforcementDesigner = new SlabReinforcementDesigner(slab, loadings);
    }

    SlabReinforcementDesigner SlabReinforcementDesigner { get; }

    public static ICalculator<CalculationResult[]> Initialize(Slab slab, params Load[] loadings)
    {
        return new SlabSolution(slab, loadings);
    }

    public CalculationResult[] Calculate()
    {
        CalculationResult result = SlabReinforcementDesigner.Calculate();

        result.Header = "Slab Calculation";

        return [result];
    }
}
