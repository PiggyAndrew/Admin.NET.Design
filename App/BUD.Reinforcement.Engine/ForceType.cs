using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine;

/// <summary>
/// 受力类型
/// </summary>
public enum ForceType
{
    /// <summary>
    /// 单向板
    /// <para>One way slab</para>
    /// </summary>
    OneWaySlab = 1,

    /// <summary>
    /// 双向板
    /// <para>Two way slab</para>
    /// </summary>
    TwoWaySlab = 2,
}
