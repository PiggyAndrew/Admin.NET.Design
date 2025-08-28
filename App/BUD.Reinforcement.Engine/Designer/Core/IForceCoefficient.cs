using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer.Core;

internal interface IForceCoefficient
{
    double[] Ratio { get; }

    double[] Values_x { get; }

    double Value_y { get; }
}
