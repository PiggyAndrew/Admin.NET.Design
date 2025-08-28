using DSD.Reinforcement.Engine.Designer.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer.Core;

internal class CrackChecker : ICalculator<CalculationResult>
{
    public CrackChecker()
    {
        
    }



    public CalculationResult Calculate()
    {
        return new CalculationResult();
    }
}
