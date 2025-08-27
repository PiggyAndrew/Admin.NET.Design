using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Components;

/// <summary>
/// Arguments of concrete
/// </summary>
public class ConcreteArguments
{
    public ConcreteArguments(double cover, double fck, double breadth)
    {
        Cover = cover;
        Fck = fck;
        Breadth = breadth;
    }

    /// <summary>
    /// Cover of concrete
    /// <para>混凝土保护层厚度</para>
    /// </summary>
    public double Cover { get; set; }

    /// <summary>
    /// Characteristic strength of concrete 
    /// <para>混凝土轴心抗压强度等级</para>
    /// </summary>
    public double Fck { get; set; }

    /// <summary>
    /// Symbol "b"
    /// </summary>
    public double Breadth { get; set; }
}
