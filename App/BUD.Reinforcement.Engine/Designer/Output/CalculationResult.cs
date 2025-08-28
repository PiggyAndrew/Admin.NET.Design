using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSD.Reinforcement.Engine.Designer.Core;

namespace DSD.Reinforcement.Engine.Designer.Output;

public class CalculationResult : INode
{
    public CalculationResult() { }

    public Step[] Steps { get; internal set; } = default!;

    public string Header { get; internal set; }

    public NodeStatus NodeStatus { get; internal set; }

    public string ToDebugString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var step in Steps)
        {
            stringBuilder.AppendLine(step.Header);
            foreach (var item in step.Log)
            {
                stringBuilder.AppendLine(item);
            }
        }
        return stringBuilder.ToString();
    }
}
