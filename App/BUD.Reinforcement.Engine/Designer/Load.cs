using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer;

/// <summary>
/// 定义荷载对象
/// </summary>
public class Load
{
    /// <summary>
    /// 初始化荷载对象
    /// </summary>
    /// <param name="loadType">荷载的类型</param>
    /// <param name="unitWeigth">荷载的值</param>
    public Load(LoadType loadType, double unitWeigth)
    {
        LoadType = loadType;
        Weight = unitWeigth;
    }

    /// <summary>
    /// 荷载的值
    /// </summary>
    public double Weight { get; set; }

    /// <summary>
    /// 荷载的类型
    /// </summary>
    public LoadType LoadType { get; set; }
}
