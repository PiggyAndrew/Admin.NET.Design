using Autodesk.Revit.UI;
using DSD.Toolkit.MVVM.DialogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Revit.Framework;

public class HostApplicationContext
{
    internal HostApplicationContext(UIControlledApplication application)
    {
        UIControlledApplication = application;
    }

    public UIControlledApplication UIControlledApplication { get; }

    /// <summary>
    /// Gets or sets the Revit application UI instance.
    /// </summary>
    public UIApplication UIApplication { get; }
}
