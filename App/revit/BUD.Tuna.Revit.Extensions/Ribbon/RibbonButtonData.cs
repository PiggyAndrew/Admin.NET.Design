using Autodesk.Revit.UI;
using BUD.Tuna.Revit.Extensions.Ribbon.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BUD.Tuna.Revit.Extensions.Ribbon;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class RibbonButtonData : IRibbonButtonData
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string LongDescription { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ToolTip { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object Image { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object LargeImage { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public object ToolTipImage { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ContextualHelp ContextualHelp { get; set; }
}
