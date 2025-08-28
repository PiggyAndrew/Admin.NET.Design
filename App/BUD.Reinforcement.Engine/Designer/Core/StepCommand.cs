using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer.Core;

internal class StepCommand : ICalculator<Step>
{
    private readonly Func<Step> _func;

    public StepCommand(Func<Step> func)
    {
        _func = func;
    }

    public Step Calculate()
    {
        Step step = _func();

        return step;
    }
}
