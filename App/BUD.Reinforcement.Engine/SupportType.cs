using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine;

/// <summary>
/// Type C-Cantilever/ S-Simply Supported/ Ct-Continous
/// </summary>
public enum SupportType
{
    /// <summary>
    /// 简支S
    /// </summary>
    SimplySupported,

    /// <summary>
    /// 连续CT
    /// </summary>
    Continous,

    /// <summary>
    /// 悬臂C
    /// </summary>
    Cantilever,
}
