using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer.Core;

internal abstract class ForceCoefficient : IForceCoefficient
{
    public abstract double[] Ratio { get; }

    public abstract double[] Values_x { get; }

    public abstract double Value_y { get; }

    /// <summary>
    /// 弯矩系数
    /// </summary>
    public class BendingMoment
    {
        /// <summary>
        /// UDL
        /// </summary>
        public sealed class UniformlyDistributedLoad : ForceCoefficient
        {
            public override double[] Ratio { get; } = [1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.75, 2];

            public override double[] Values_x { get; } = [0.039, 0.044, 0.048, 0.052, 0.055, 0.058, 0.063, 0.067];

            public override double Value_y { get; } = 0.037;
        }


        /// <summary>
        /// Tri
        /// </summary>
        public sealed class TriangularLoad : ForceCoefficient
        {
            private TriangularLoad(bool isLongSpan)
            {
                Values_x = isLongSpan ? [0.011, 0.023, 0.035, 0.048, 0.061, 0.086, 0.109, 0.127] : [0.012, 0.022, 0.03, 0.037, 0.044, 0.066, 0.082, 0.091];
                Value_y = isLongSpan ? 0.149 : 0.099;
            }

            public override double[] Ratio { get; } = [0.5, 0.75, 1, 1.25, 1.5, 2, 2.5, 3];

            public override double[] Values_x { get; } = default!;

            public override double Value_y { get; }

            public static IForceCoefficient GetLongSpanTriangularLoad()
            {
                return new TriangularLoad(true);
            }

            public static IForceCoefficient GetShortSpanTriangularLoad()
            {
                return new TriangularLoad(false);
            }
        }
    }

    public class Shear
    {
        /// <summary>
        /// UDL
        /// </summary>
        public sealed class UniformlyDistributedLoad : ForceCoefficient
        {
            public override double[] Ratio { get; } = [1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.75, 2];

            public override double[] Values_x { get; } = [0.36, 0.39, 0.41, 0.43, 0.45, 0.48, 0.5, 0.52];

            public override double Value_y { get; } = 0.036;
        }


        /// <summary>
        /// Tri
        /// </summary>
        public sealed class TriangularLoad : ForceCoefficient
        {
            private TriangularLoad(bool isLongSpan)
            {
                Values_x = isLongSpan ? [0.19, 0.26, 0.32, 0.36, 0.4, 0.45, 0.48, 0.5] : [0.17, 0.22, 0.24, 0.25, 0.26, 0.27, 0.33, 0.37];
                Value_y = isLongSpan ? 0.5 : 0.38;
            }

            public override double[] Ratio { get; } = [0.5, 0.75, 1, 1.25, 1.5, 2, 2.5, 3];

            public override double[] Values_x { get; } = default!;

            public override double Value_y { get; }

            public static IForceCoefficient GetLongSpanTriangularLoad()
            {
                return new TriangularLoad(true);
            }

            public static IForceCoefficient GetShortSpanTriangularLoad()
            {
                return new TriangularLoad(false);
            }
        }
    }
}
