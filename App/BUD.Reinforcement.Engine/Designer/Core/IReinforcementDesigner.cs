using DSD.Reinforcement.Engine.Components;
using DSD.Reinforcement.Engine.Designer.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer.Core;

internal interface IReinforcementDesigner
{
    Step Dimensions();

    Step Loading();

    Step DesignMoment();

    Step DesignShear();

    Step BendingReinforcement();

    Step ShearReinforcement();
}
