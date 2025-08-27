using DSD.Reinforcement.Engine.Components;
using DSD.Reinforcement.Engine.Designer.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer.Core;

public class WallSolution : ICalculator<CalculationResult[]>
{
    WallSolution(Wall wall, params Load[] loadings)
    {
        ShortSpan = new WallReinforcementDesigner(wall, false, loadings);
        LongSpan = new WallReinforcementDesigner(wall, true, loadings);
    }

    WallReinforcementDesigner ShortSpan { get; }

    WallReinforcementDesigner LongSpan { get; }

    public static ICalculator<CalculationResult[]> Initialize(Wall wall, params Load[] loadings)
    {
        return new WallSolution(wall, loadings);
    }

    public CalculationResult[] Calculate()
    {
        CalculationResult shortSpanResult = ShortSpan.Calculate();
        shortSpanResult.Header = "Wall Short Span";

        CalculationResult longSpanResult = LongSpan.Calculate();
        longSpanResult.Header = "Wall Long Span";

        return [shortSpanResult, longSpanResult];
    }
}
